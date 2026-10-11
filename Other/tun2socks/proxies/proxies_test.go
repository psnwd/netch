package proxies

import (
	"io"
	"net"
	"testing"
	"time"

	"github.com/yl2chen/cidranger"

	"rixcloud.moe/aiocloud/tun2socks/core"
)

type recordTCP struct{ called bool }

func (r *recordTCP) Handle(conn net.Conn, target *net.TCPAddr) error {
	r.called = true
	return nil
}

type recordUDP struct{ connect, receive int }

func (r *recordUDP) Connect(conn core.UDPConn, target *net.UDPAddr) error {
	r.connect++
	return nil
}

func (r *recordUDP) ReceiveTo(conn core.UDPConn, data []byte, addr *net.UDPAddr) error {
	r.receive++
	return nil
}

func echoServer(t *testing.T) *net.TCPAddr {
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
			go func() { defer c.Close(); io.Copy(c, c) }()
		}
	}()

	return l.Addr().(*net.TCPAddr)
}

func rangerOf(t *testing.T, cidrs ...string) cidranger.Ranger {
	r := cidranger.NewPCTrieRanger()
	for _, c := range cidrs {
		_, n, err := net.ParseCIDR(c)
		if err != nil {
			t.Fatal(err)
		}
		r.Insert(cidranger.NewBasicRangerEntry(*n))
	}
	return r
}

func TestTCPBypassDialsDirect(t *testing.T) {
	target := echoServer(t)
	proxy := &recordTCP{}
	h := NewTCPHandler(proxy, &net.TCPAddr{IP: net.ParseIP("127.0.0.1")}, rangerOf(t, "127.0.0.0/8"))

	client, local := net.Pipe()
	t.Cleanup(func() { client.Close(); local.Close() })

	if err := h.Handle(local, target); err != nil {
		t.Fatal(err)
	}
	if proxy.called {
		t.Fatal("bypassed address went to the proxy")
	}

	client.Write([]byte("hi"))
	client.SetReadDeadline(time.Now().Add(3 * time.Second))
	got := make([]byte, 2)
	if _, err := io.ReadFull(client, got); err != nil || string(got) != "hi" {
		t.Fatalf("got %q, %v", got, err)
	}
}

func TestTCPNotInListGoesToProxy(t *testing.T) {
	proxy := &recordTCP{}
	h := NewTCPHandler(proxy, nil, rangerOf(t, "10.0.0.0/8"))

	h.Handle(nil, &net.TCPAddr{IP: net.ParseIP("8.8.8.8"), Port: 443})
	if !proxy.called {
		t.Fatal("expected the proxy handler")
	}
}

func TestTCPNoRangerGoesToProxy(t *testing.T) {
	proxy := &recordTCP{}
	h := NewTCPHandler(proxy, nil, nil)

	h.Handle(nil, &net.TCPAddr{IP: net.ParseIP("8.8.8.8"), Port: 443})
	if !proxy.called {
		t.Fatal("expected the proxy handler")
	}
}

func TestTCPIPv6GoesToProxy(t *testing.T) {
	proxy := &recordTCP{}
	h := NewTCPHandler(proxy, nil, rangerOf(t, "0.0.0.0/0"))

	h.Handle(nil, &net.TCPAddr{IP: net.ParseIP("2001:db8::1"), Port: 443})
	if !proxy.called {
		t.Fatal("IPv6 targets are never matched against the IPv4 list")
	}
}

func TestUDPNonDNSGoesToProxy(t *testing.T) {
	proxy := &recordUDP{}
	h := NewUDPHandler(proxy)

	h.Connect(nil, nil)
	h.ReceiveTo(nil, []byte{1}, &net.UDPAddr{IP: net.ParseIP("1.1.1.1"), Port: 443})

	if proxy.connect != 1 || proxy.receive != 1 {
		t.Fatalf("connect=%d receive=%d", proxy.connect, proxy.receive)
	}
}
