package socks

import (
	"io"
	"net"
	"testing"
	"time"

	"rixcloud.moe/aiocloud/tun2socks/structs"
)

// fakeServer is a minimal SOCKS5 server: method negotiation, optional user/pass, one CONNECT.
type fakeServer struct {
	l        net.Listener
	method   byte   // method chosen in the reply (0 or 2, anything else is "unknown")
	user     string // expected credentials when method == 2
	pass     string
	authFail bool
}

func newFakeServer(t *testing.T, method byte) *fakeServer {
	l, err := net.Listen("tcp", "127.0.0.1:0")
	if err != nil {
		t.Fatal(err)
	}
	t.Cleanup(func() { l.Close() })

	s := &fakeServer{l: l, method: method, user: "user", pass: "pass"}
	go s.serve()
	return s
}

func (s *fakeServer) info() *structs.ServerInfo {
	return &structs.ServerInfo{Type: "Socks5", Hostname: s.l.Addr().String(), Username: "user", Password: "pass"}
}

func (s *fakeServer) serve() {
	for {
		c, err := s.l.Accept()
		if err != nil {
			return
		}
		go s.handle(c)
	}
}

func (s *fakeServer) handle(c net.Conn) {
	defer c.Close()

	greeting := make([]byte, 4)
	if _, err := io.ReadFull(c, greeting); err != nil || greeting[0] != 5 || greeting[1] != 2 || greeting[2] != 0 || greeting[3] != 2 {
		return
	}

	c.Write([]byte{5, s.method})
	if s.method == 2 {
		head := make([]byte, 2)
		io.ReadFull(c, head[:2])
		user := make([]byte, head[1])
		io.ReadFull(c, user)
		plen := make([]byte, 1)
		io.ReadFull(c, plen)
		pass := make([]byte, plen[0])
		io.ReadFull(c, pass)

		if head[0] != 1 || string(user) != s.user || string(pass) != s.pass || s.authFail {
			c.Write([]byte{1, 1})
			return
		}
		c.Write([]byte{1, 0})
	}
	if s.method != 0 && s.method != 2 {
		return
	}

	// CONNECT 5 1 0 atyp addr port
	req := make([]byte, 4)
	if _, err := io.ReadFull(c, req); err != nil || req[1] != 1 {
		return
	}
	switch req[3] {
	case 1:
		io.ReadFull(c, make([]byte, 6))
	case 3:
		n := make([]byte, 1)
		io.ReadFull(c, n)
		io.ReadFull(c, make([]byte, int(n[0])+2))
	case 4:
		io.ReadFull(c, make([]byte, 18))
	}

	c.Write([]byte{5, 0, 0, 1, 0, 0, 0, 0, 0, 0})

	// echo
	io.Copy(c, c)
}

func TestHandshakeNoAuth(t *testing.T) {
	s := newFakeServer(t, 0)
	c, err := handshake(s.info(), make([]byte, 0x103))
	if err != nil {
		t.Fatal(err)
	}
	c.Close()
}

func TestHandshakeUserPass(t *testing.T) {
	s := newFakeServer(t, 2)
	c, err := handshake(s.info(), make([]byte, 0x103))
	if err != nil {
		t.Fatal(err)
	}
	c.Close()
}

func TestHandshakeAuthFailed(t *testing.T) {
	s := newFakeServer(t, 2)
	s.authFail = true
	if _, err := handshake(s.info(), make([]byte, 0x103)); err == nil || err.Error() != "authentication failed" {
		t.Fatalf("got %v", err)
	}
}

func TestHandshakeUnknownMethod(t *testing.T) {
	s := newFakeServer(t, 0xff)
	if _, err := handshake(s.info(), make([]byte, 0x103)); err == nil || err.Error() != "unknown method" {
		t.Fatalf("got %v", err)
	}
}

func TestTCPHandleRelaysThroughServer(t *testing.T) {
	s := newFakeServer(t, 2)
	h := NewTCPHandler(s.info())

	client, local := net.Pipe()
	t.Cleanup(func() { client.Close(); local.Close() })

	if err := h.Handle(local, &net.TCPAddr{IP: net.ParseIP("93.184.216.34"), Port: 80}); err != nil {
		t.Fatal(err)
	}

	if _, err := client.Write([]byte("ping")); err != nil {
		t.Fatal(err)
	}

	client.SetReadDeadline(time.Now().Add(3 * time.Second))
	got := make([]byte, 4)
	if _, err := io.ReadFull(client, got); err != nil || string(got) != "ping" {
		t.Fatalf("got %q, %v", got, err)
	}
}

func TestTCPHandleServerDown(t *testing.T) {
	l, _ := net.Listen("tcp", "127.0.0.1:0")
	addr := l.Addr().String()
	l.Close()

	h := NewTCPHandler(&structs.ServerInfo{Hostname: addr})
	if err := h.Handle(nil, &net.TCPAddr{IP: net.ParseIP("1.2.3.4"), Port: 80}); err == nil {
		t.Fatal("expected an error")
	}
}
