package main

import (
	"net"
	"sync/atomic"
	"testing"

	"github.com/miekg/dns"
)

func TestExchangeCachesAnswers(t *testing.T) {
	var hits int32
	ln, err := net.Listen("tcp", "127.0.0.1:0")
	if err != nil {
		t.Fatal(err)
	}
	srv := &dns.Server{Listener: ln, Handler: dns.HandlerFunc(func(w dns.ResponseWriter, m *dns.Msg) {
		atomic.AddInt32(&hits, 1)
		r := new(dns.Msg)
		r.SetReply(m)
		r.Answer = append(r.Answer, &dns.A{Hdr: dns.RR_Header{Name: m.Question[0].Name, Rrtype: dns.TypeA, Class: dns.ClassINET, Ttl: 30}, A: net.IPv4(1, 2, 3, 4)})
		_ = w.WriteMsg(r)
	})}
	go srv.ActivateAndServe()
	defer srv.Shutdown()

	c := &dns.Client{Net: "tcp"}
	for id := uint16(1); id <= 2; id++ {
		m := new(dns.Msg)
		m.SetQuestion("cache.test.", dns.TypeA)
		m.Id = id
		r, err := exchange(c, m, ln.Addr().String())
		if err != nil || len(r.Answer) != 1 || r.Id != id {
			t.Fatalf("id %d: reply %v err %v", id, r, err)
		}
	}
	if hits != 1 {
		t.Fatalf("upstream hit %d times, want 1", hits)
	}
}
