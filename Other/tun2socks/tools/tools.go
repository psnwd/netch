package tools

import (
	"io"
	"net"
	"time"
)

// ponytail: fixed 32 KiB per direction; pool it if memory with many connections matters.
const bufferSize = 32 * 1024

// CopyBuffer relays data in both directions until either side stops, then closes both connections.
func CopyBuffer(client, remote net.Conn) {
	defer client.Close()
	defer remote.Close()

	go func() {
		io.CopyBuffer(client, remote, make([]byte, bufferSize))

		// wake up the other direction
		client.SetDeadline(time.Now())
		remote.SetDeadline(time.Now())
	}()

	io.CopyBuffer(remote, client, make([]byte, bufferSize))

	client.SetDeadline(time.Now())
	remote.SetDeadline(time.Now())
}
