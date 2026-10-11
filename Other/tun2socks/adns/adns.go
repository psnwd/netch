package adns

import (
	"fmt"
	"log"
	"net"

	"github.com/miekg/dns"

	"rixcloud.moe/aiocloud/tun2socks/core"
)

var (
	// Client talks to Address; main sets Client.Net to "tcp" or "tcp-tls".
	Client = dns.Client{Net: "tcp"}

	// Address is the upstream DNS server, "host:port".
	Address = "1.1.1.1:53"
)

// Handle answers one DNS query that arrived on UDP port 53: PTR questions get a fixed
// "aioCloud" answer, everything else is forwarded to Address over Client.Net.
func Handle(conn core.UDPConn, data []byte, addr *net.UDPAddr) {
	var request dns.Msg
	if err := request.Unpack(data); err != nil {
		log.Printf("[tun2socks][adns] Failed to unpack data: %v", err)
		return
	}

	if len(request.Question) == 1 && request.Question[0].Qtype == dns.TypePTR {
		rr, err := dns.NewRR(fmt.Sprintf("%s PTR aioCloud", request.Question[0].Name))
		if err != nil {
			log.Printf("[tun2socks][adns] NewRR failed: %v", err)
			return
		}

		var reply dns.Msg
		reply.SetReply(&request)
		reply.Answer = append(reply.Answer, rr)

		respond(conn, &reply, addr)
		return
	}

	reply, err := exchange(&Client, &request, Address)
	if err != nil {
		log.Printf("[tun2socks][adns] DNS resolution failed: %v", err)
		return
	}

	respond(conn, reply, addr)
}

func respond(conn core.UDPConn, reply *dns.Msg, addr *net.UDPAddr) {
	packed, err := reply.Pack()
	if err != nil {
		log.Printf("[tun2socks][adns] Failed to pack data: %v", err)
		return
	}

	conn.WriteFrom(packed, addr)
}
