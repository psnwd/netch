package socks

import (
	"bytes"
	"errors"
	"io"
	"net"

	"rixcloud.moe/aiocloud/tun2socks/structs"
)

// handshake dials the SOCKS5 server and negotiates the authentication method
// (no authentication, or username/password). buf needs at least 2 bytes.
func handshake(t *structs.ServerInfo, buf []byte) (net.Conn, error) {
	c, err := net.Dial("tcp", t.Hostname)
	if err != nil {
		return nil, err
	}

	// version 5, 2 methods: 0x00 no authentication, 0x02 username/password
	if _, err = c.Write([]byte{5, 2, 0, 2}); err != nil {
		c.Close()
		return nil, err
	}

	if _, err = io.ReadAtLeast(c, buf[:2], 2); err != nil {
		c.Close()
		return nil, err
	}

	switch buf[1] {
	case 0:
	case 2:
		auth := bytes.NewBuffer([]byte{1})
		auth.WriteByte(byte(len(t.Username)))
		auth.Write([]byte(t.Username))
		auth.WriteByte(byte(len(t.Password)))
		auth.Write([]byte(t.Password))

		if _, err = c.Write(auth.Bytes()); err != nil {
			c.Close()
			return nil, err
		}

		if _, err = io.ReadAtLeast(c, buf[:2], 2); err != nil {
			c.Close()
			return nil, err
		}

		if buf[1] != 0 {
			c.Close()
			return nil, errors.New("authentication failed")
		}
	default:
		c.Close()
		return nil, errors.New("unknown method")
	}

	return c, nil
}
