package socks

import (
	"io"
	"log"
	"net"

	"github.com/shadowsocks/go-shadowsocks2/socks"

	"rixcloud.moe/aiocloud/tun2socks/core"
	"rixcloud.moe/aiocloud/tun2socks/structs"
	"rixcloud.moe/aiocloud/tun2socks/tools"
)

type tcpHandler struct {
	t *structs.ServerInfo
}

// NewTCPHandler relays TCP connections through the SOCKS5 server described by t.
func NewTCPHandler(t *structs.ServerInfo) core.TCPConnHandler {
	return &tcpHandler{t: t}
}

func (h *tcpHandler) Handle(conn net.Conn, target *net.TCPAddr) error {
	buf := make([]byte, socks.MaxAddrLen)

	remote, err := handshake(h.t, buf)
	if err != nil {
		log.Printf("[tun2socks][TCP][socks][handshake] %v", err)
		return err
	}

	// CONNECT request: version 5, command 1, reserved 0, then the target address
	request := append([]byte{5, 1, 0}, socks.ParseAddr(target.String())...)
	if _, err = remote.Write(request); err != nil {
		log.Printf("[tun2socks][TCP][socks][remote.Write] %v", err)
		remote.Close()
		return err
	}

	// the reply code is not checked, only the reply is consumed
	if _, err = io.ReadAtLeast(remote, buf[:3], 3); err != nil {
		log.Printf("[tun2socks][TCP][socks][io.ReadFull] %v", err)
		remote.Close()
		return err
	}

	if _, err = socks.ReadAddr(remote); err != nil {
		log.Printf("[tun2socks][TCP][socks][socks.ReadAddr] %v", err)
		remote.Close()
		return err
	}

	go tools.CopyBuffer(conn, remote)

	return nil
}
