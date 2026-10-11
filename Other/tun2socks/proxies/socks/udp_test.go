package socks

import (
	"io"
	"net"
	"testing"
	"time"

	"github.com/shadowsocks/go-shadowsocks2/socks"

	"rixcloud.moe/aiocloud/tun2socks/structs"
)

type fakeConn struct {
	got    chan []byte
	from   chan *net.UDPAddr
	closed chan struct{}
}

func newFakeConn() *fakeConn {
	return &fakeConn{got: make(chan []byte, 4), from: make(chan *net.UDPAddr, 4), closed: make(chan struct{}, 1)}
}

func (f *fakeConn) LocalAddr() *net.UDPAddr                        { return nil }
func (f *fakeConn) ReceiveTo(data []byte, addr *net.UDPAddr) error { return nil }
func (f *fakeConn) Close() error                                   { f.closed <- struct{}{}; return nil }
func (f *fakeConn) WriteFrom(data []byte, addr *net.UDPAddr) (int, error) {
	f.got <- append([]byte(nil), data...)
	f.from <- addr
	return len(data), nil
}

// udpAssociateServer answers UDP ASSOCIATE and echoes every relayed datagram back unchanged.
func udpAssociateServer(t *testing.T) *structs.ServerInfo {
	relay, err := net.ListenPacket("udp", "127.0.0.1:0")
	if err != nil {
		t.Fatal(err)
	}
	t.Cleanup(func() { relay.Close() })

	go func() {
		buf := make([]byte, 2048)
		for {
			n, from, err := relay.ReadFrom(buf)
			if err != nil {
				return
			}
			relay.WriteTo(buf[:n], from)
		}
	}()

	l, err := net.Listen("tcp", "127.0.0.1:0")
	if err != nil {
		t.Fatal(err)
	}
	t.Cleanup(func() { l.Close() })

	go func() {
		for {
			c, err := l.Accept()
			if err != nil {
				return
			}
			go func() {
				defer c.Close()
				io.ReadFull(c, make([]byte, 4))
				c.Write([]byte{5, 0})
				io.ReadFull(c, make([]byte, 10)) // UDP ASSOCIATE request
				bind := relay.LocalAddr().(*net.UDPAddr)
				reply := append([]byte{5, 0, 0}, socks.ParseAddr(bind.String())...)
				c.Write(reply)
				io.Copy(io.Discard, c) // keep the association open
			}()
		}
	}()

	return &structs.ServerInfo{Type: "Socks5", Hostname: l.Addr().String()}
}

func TestUDPAssociateRoundTrip(t *testing.T) {
	h := NewUDPHandler(udpAssociateServer(t))
	conn := newFakeConn()

	if err := h.Connect(conn, nil); err != nil {
		t.Fatal(err)
	}

	dest := &net.UDPAddr{IP: net.ParseIP("8.8.4.4"), Port: 5353}
	if err := h.ReceiveTo(conn, []byte("hello"), dest); err != nil {
		t.Fatal(err)
	}

	select {
	case data := <-conn.got:
		if string(data) != "hello" {
			t.Fatalf("payload %q", data)
		}
		if from := <-conn.from; !from.IP.Equal(dest.IP) || from.Port != dest.Port {
			t.Fatalf("source %v, want %v", from, dest)
		}
	case <-time.After(3 * time.Second):
		t.Fatal("no reply written back to the TUN")
	}

	h.(*udpHandler).Close(conn)
	select {
	case <-conn.closed:
	case <-time.After(time.Second):
		t.Fatal("Close did not close the core connection")
	}
}

func TestUDPReceiveToWithoutConnectFails(t *testing.T) {
	h := NewUDPHandler(&structs.ServerInfo{})
	conn := newFakeConn()

	if err := h.ReceiveTo(conn, []byte("x"), &net.UDPAddr{IP: net.ParseIP("1.1.1.1"), Port: 1}); err != io.EOF {
		t.Fatalf("got %v, want io.EOF", err)
	}
}

func TestUDPConnectRefused(t *testing.T) {
	l, _ := net.Listen("tcp", "127.0.0.1:0")
	addr := l.Addr().String()
	l.Close()

	h := NewUDPHandler(&structs.ServerInfo{Hostname: addr})
	if err := h.Connect(newFakeConn(), nil); err == nil {
		t.Fatal("expected an error")
	}
}
