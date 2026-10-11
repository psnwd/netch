package proxies

import (
	"errors"
	"net"
	"time"

	"github.com/yl2chen/cidranger"

	"rixcloud.moe/aiocloud/tun2socks/adns"
	"rixcloud.moe/aiocloud/tun2socks/core"
	"rixcloud.moe/aiocloud/tun2socks/tools"
)

const dialTimeout = 10 * time.Second

// BlockIPv6 rejects every IPv6 connection: used when the server has no IPv6 egress, so clients fall back to IPv4
// at once instead of hanging on a connection the server can never open.
var BlockIPv6 bool

var errBlockedIPv6 = errors.New("IPv6 blocked: the server has no IPv6 path")

type tcpHandler struct {
	h      core.TCPConnHandler
	binder *net.TCPAddr
	ranger cidranger.Ranger
}

// NewTCPHandler sends connections to IPv4 addresses listed in ranger straight out of the
// local address binder; everything else goes through h.
func NewTCPHandler(h core.TCPConnHandler, binder *net.TCPAddr, ranger cidranger.Ranger) core.TCPConnHandler {
	return &tcpHandler{h: h, binder: binder, ranger: ranger}
}

func (h *tcpHandler) Handle(conn net.Conn, target *net.TCPAddr) error {
	if BlockIPv6 && target.IP.To4() == nil {
		return errBlockedIPv6
	}

	if h.ranger != nil {
		if ip := target.IP.To4(); ip != nil {
			bypass, err := h.ranger.Contains(ip)
			if err != nil {
				return err
			}

			if bypass {
				dialer := &net.Dialer{Timeout: dialTimeout, KeepAlive: dialTimeout, LocalAddr: h.binder}

				remote, err := dialer.Dial("tcp", target.String())
				if err != nil {
					return err
				}

				go tools.CopyBuffer(conn, remote)

				return nil
			}
		}
	}

	return h.h.Handle(conn, target)
}

type udpHandler struct {
	h core.UDPConnHandler
}

// NewUDPHandler answers DNS (port 53) itself and passes everything else to h.
func NewUDPHandler(h core.UDPConnHandler) core.UDPConnHandler {
	return &udpHandler{h: h}
}

func (h *udpHandler) Connect(conn core.UDPConn, target *net.UDPAddr) error {
	if BlockIPv6 && target != nil && target.IP.To4() == nil {
		return errBlockedIPv6
	}

	return h.h.Connect(conn, target)
}

func (h *udpHandler) ReceiveTo(conn core.UDPConn, data []byte, addr *net.UDPAddr) error {
	if addr.Port != 53 {
		return h.h.ReceiveTo(conn, data, addr)
	}

	go adns.Handle(conn, data, addr)

	return nil
}
