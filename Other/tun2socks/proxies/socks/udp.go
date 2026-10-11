package socks

import (
	"fmt"
	"io"
	"log"
	"net"
	"sync"
	"time"

	"github.com/shadowsocks/go-shadowsocks2/socks"

	"rixcloud.moe/aiocloud/tun2socks/core"
	"rixcloud.moe/aiocloud/tun2socks/structs"
)

const (
	// SOCKS5 UDP header: RSV(2) + FRAG(1)
	headerSize = 3

	readBufferSize = 0x5dc
	readTimeout    = 30 * time.Second
)

// udpData is the state of one UDP association: the TCP control connection, the local
// socket used to talk to the relay, and the relay address.
type udpData struct {
	StreamConn net.Conn
	PacketConn net.PacketConn
	Target     *net.UDPAddr
}

type udpHandler struct {
	sync.Mutex

	t     *structs.ServerInfo
	conns map[core.UDPConn]*udpData
}

// NewUDPHandler relays UDP packets through the SOCKS5 server described by t (UDP ASSOCIATE).
func NewUDPHandler(t *structs.ServerInfo) core.UDPConnHandler {
	return &udpHandler{t: t, conns: make(map[core.UDPConn]*udpData)}
}

// handle holds the association open: it returns when the TCP control connection ends.
func (h *udpHandler) handle(c net.Conn) {
	defer c.Close()

	buf := make([]byte, 1)
	for {
		if _, err := c.Read(buf); err != nil {
			return
		}
	}
}

// fetchInput forwards relay replies back to the TUN until the relay stays silent for readTimeout.
func (h *udpHandler) fetchInput(conn core.UDPConn, pc net.PacketConn) {
	defer h.Close(conn)

	buf := make([]byte, readBufferSize)
	for {
		pc.SetReadDeadline(time.Now().Add(readTimeout))

		n, _, err := pc.ReadFrom(buf)
		if err != nil {
			return
		}

		if n < headerSize {
			log.Println("[tun2socks][UDP][socks][fetchInput] Short packet")
			continue
		}

		addr := socks.SplitAddr(buf[headerSize:n])
		if addr == nil {
			log.Println("[tun2socks][UDP][socks][fetchInput] Wrong destination address")
			continue
		}

		source, err := net.ResolveUDPAddr("udp", addr.String())
		if err != nil {
			log.Printf("[tun2socks][UDP][socks][fetchInput] Failed to resolve target address: %v", err)
			continue
		}

		if _, err = conn.WriteFrom(buf[headerSize+len(addr):n], source); err != nil {
			log.Printf("[tun2socks][UDP][socks][fetchInput] Unable to write data packet: %v", err)
			return
		}
	}
}

func (h *udpHandler) Connect(conn core.UDPConn, target *net.UDPAddr) error {
	buf := make([]byte, socks.MaxAddrLen)

	c, err := handshake(h.t, buf)
	if err != nil {
		log.Printf("[tun2socks][UDP][socks][Connect] Handshake failed: %v", err)
		return err
	}

	// UDP ASSOCIATE request: version 5, command 3, reserved 0, IPv4 0.0.0.0:0
	if _, err = c.Write([]byte{5, 3, 0, 1, 0, 0, 0, 0, 0, 0}); err != nil {
		log.Printf("[tun2socks][UDP][socks][Connect] Failed to write handshake data: %v", err)
		c.Close()
		return err
	}

	if _, err = io.ReadAtLeast(c, buf[:3], 3); err != nil {
		log.Printf("[tun2socks][UDP][socks][Connect] Failed to read handshake data: %v", err)
		c.Close()
		return err
	}

	if buf[1] != 0 {
		log.Printf("[tun2socks][UDP][socks][Connect] The server refused the connection: %x", buf[1])
		c.Close()
		return fmt.Errorf("[tun2socks][UDP][socks][Connect] The server refused the connection: %x", buf[1])
	}

	relay, err := socks.ReadAddr(c)
	if err != nil {
		log.Printf("[tun2socks][UDP][socks][Connect] Failed to read remote address: %v", err)
		c.Close()
		return err
	}

	relayAddr, err := net.ResolveUDPAddr("udp", relay.String())
	if err != nil {
		log.Printf("[tun2socks][UDP][socks][Connect] Failed to resolve remote address: %v", err)
		c.Close()
		return err
	}

	go h.handle(c)

	pc, err := net.ListenPacket("udp", "")
	if err != nil {
		log.Printf("[tun2socks][UDP][socks][Connect]Failed to bind socket: %v", err)
		c.Close()
		return err
	}

	h.Lock()
	h.conns[conn] = &udpData{StreamConn: c, PacketConn: pc, Target: relayAddr}
	h.Unlock()

	go h.fetchInput(conn, pc)

	return nil
}

func (h *udpHandler) ReceiveTo(conn core.UDPConn, data []byte, addr *net.UDPAddr) error {
	h.Lock()
	d, ok := h.conns[conn]
	h.Unlock()

	if !ok {
		h.Close(conn)
		log.Println("[tun2socks][UDP][socks][ReceiveTo] Non-existent connection")
		return io.EOF
	}

	// SOCKS5 UDP header: RSV(2) + FRAG(1) are zero, then the destination address and the payload
	destination := socks.ParseAddr(addr.String())

	packet := make([]byte, headerSize+len(destination)+len(data))
	copy(packet[headerSize:], destination)
	copy(packet[headerSize+len(destination):], data)

	if _, err := d.PacketConn.WriteTo(packet, d.Target); err != nil {
		h.Close(conn)
		log.Printf("[tun2socks][UDP][socks][ReceiveTo] Failed to send data: %v", err)
		return io.EOF
	}

	return nil
}

// Close ends the association: control connection, relay socket and the core connection.
func (h *udpHandler) Close(conn core.UDPConn) {
	h.Lock()
	defer h.Unlock()

	if d, ok := h.conns[conn]; ok {
		if d.StreamConn != nil {
			d.StreamConn.Close()
		}
		if d.PacketConn != nil {
			d.PacketConn.Close()
		}

		delete(h.conns, conn)
		conn.Close()
	}
}
