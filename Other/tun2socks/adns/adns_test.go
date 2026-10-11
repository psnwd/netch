package adns

import (
	"net"
	"testing"

	"github.com/miekg/dns"
)

type fakeUDPConn struct {
	written []byte
	to      *net.UDPAddr
}

func (f *fakeUDPConn) LocalAddr() *net.UDPAddr                        { return nil }
func (f *fakeUDPConn) ReceiveTo(data []byte, addr *net.UDPAddr) error { return nil }
func (f *fakeUDPConn) Close() error                                   { return nil }
func (f *fakeUDPConn) WriteFrom(data []byte, addr *net.UDPAddr) (int, error) {
	f.written, f.to = data, addr
	return len(data), nil
}

func TestPTRQueryAnsweredLocally(t *testing.T) {
	query := new(dns.Msg)
	query.SetQuestion("4.3.2.1.in-addr.arpa.", dns.TypePTR)
	query.Id = 0x1234
	data, _ := query.Pack()

	conn := &fakeUDPConn{}
	from := &net.UDPAddr{IP: net.ParseIP("1.1.1.1"), Port: 53}
	Handle(conn, data, from)

	var reply dns.Msg
	if err := reply.Unpack(conn.written); err != nil {
		t.Fatal(err)
	}
	if !reply.Response || reply.Id != 0x1234 || reply.Rcode != dns.RcodeSuccess {
		t.Fatalf("bad header: %+v", reply.MsgHdr)
	}
	if len(reply.Answer) != 1 {
		t.Fatalf("answers: %v", reply.Answer)
	}
	ptr, ok := reply.Answer[0].(*dns.PTR)
	if !ok || ptr.Ptr != "aioCloud." {
		t.Fatalf("answer: %v", reply.Answer[0])
	}
	if conn.to != from {
		t.Fatal("reply not addressed to the query's destination")
	}
}

func TestGarbageIsDropped(t *testing.T) {
	conn := &fakeUDPConn{}
	Handle(conn, []byte{1, 2, 3}, &net.UDPAddr{})
	if conn.written != nil {
		t.Fatal("nothing should be written for an undecodable query")
	}
}

func TestOtherQueriesGoUpstreamOverTCP(t *testing.T) {
	// upstream DNS over TCP that always answers 9.9.9.9
	l, err := net.Listen("tcp", "127.0.0.1:0")
	if err != nil {
		t.Fatal(err)
	}
	defer l.Close()

	srv := &dns.Server{Listener: l, Handler: dns.HandlerFunc(func(w dns.ResponseWriter, r *dns.Msg) {
		m := new(dns.Msg)
		m.SetReply(r)
		rr, _ := dns.NewRR(r.Question[0].Name + " 60 IN A 9.9.9.9")
		m.Answer = append(m.Answer, rr)
		w.WriteMsg(m)
	})}
	started := make(chan struct{})
	srv.NotifyStartedFunc = func() { close(started) }
	go srv.ActivateAndServe()
	<-started
	defer srv.Shutdown()

	old := Address
	Address = l.Addr().String()
	defer func() { Address = old }()

	query := new(dns.Msg)
	query.SetQuestion("example.com.", dns.TypeA)
	data, _ := query.Pack()

	conn := &fakeUDPConn{}
	Handle(conn, data, &net.UDPAddr{})

	var reply dns.Msg
	if err := reply.Unpack(conn.written); err != nil {
		t.Fatal(err)
	}
	if a, ok := reply.Answer[0].(*dns.A); !ok || a.A.String() != "9.9.9.9" {
		t.Fatalf("answer: %v", reply.Answer)
	}
}
