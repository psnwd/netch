// tun2socks.bin: a wintun based TUN to SOCKS5 forwarder, loaded by Netch as a DLL.
//
// Rebuilt from the decompiled Storage/tun2socks.bin (go1.17.1). Netch drives it through
// the exported C functions below, see Netch/Interops/tun2socks.cs.
package main

// #include <stdlib.h>
import "C"

import (
	"bufio"
	"log"
	"net"
	"os"
	"strconv"
	"strings"
	"sync"
	"unsafe"

	"github.com/yl2chen/cidranger"
	"golang.org/x/sys/windows"
	"golang.zx2c4.com/wireguard/tun"

	"rixcloud.moe/aiocloud/tun2socks/adns"
	"rixcloud.moe/aiocloud/tun2socks/core"
	"rixcloud.moe/aiocloud/tun2socks/proxies"
	"rixcloud.moe/aiocloud/tun2socks/proxies/socks"
	"rixcloud.moe/aiocloud/tun2socks/structs"
)

// First argument of tun_dial, same order as NameList in Netch/Interops/tun2socks.cs.
const (
	typeBypassBind = iota // local address that direct connections bind to
	typeBypassList        // file with bypass CIDRs, one per line, or "disabled"
	typeDNSAddress        // "tls://host:port", "tcp://host:port" or "host:port"
	typeAdapterMTU
	typeTCPReset // starts a new TCP server description
	typeTCPType
	typeTCPHost
	typeTCPUser
	typeTCPPass
	typeTCPMethod
	typeTCPProtocol
	typeTCPProtocolParam
	typeTCPOBFS
	typeTCPOBFSParam
	typeUDPReset // starts a new UDP server description
	typeUDPType
	typeUDPHost
	typeUDPUser
	typeUDPPass
	typeUDPMethod
	typeUDPProtocol
	typeUDPProtocolParam
	typeUDPOBFS
	typeUDPOBFSParam
	typeBlockIPv6 // "1": reject IPv6 connections so clients fall back to IPv4, "0": relay them
)

const (
	adapterName = "aioCloud"
	adapterGUID = "{14EA48E9-69FA-409F-8752-22563978E802}"
)

var (
	bypassBind = "10.0.0.100"
	bypassList = "disabled"
	mtu        = 1500

	tcpInfo, udpInfo *structs.ServerInfo

	device tun.Device
	stack  core.LWIPStack
	ranger cidranger.Ranger

	running bool

	upLock, downLock sync.Mutex
	upload, download uint64
)

// init sends log output and Go runtime crash reports (panics, fatal errors) to logging\tun2socks.log:
// Netch is a GUI process without a console, so they would otherwise vanish.
func init() {
	file, err := os.OpenFile(`logging\tun2socks.log`, os.O_CREATE|os.O_WRONLY|os.O_TRUNC, 0o644)
	if err != nil {
		return
	}

	os.Stderr = file
	log.SetOutput(file)
	windows.SetStdHandle(windows.STD_ERROR_HANDLE, windows.Handle(file.Fd()))
}

//export tun_dial
func tun_dial(name int, value *byte) bool {
	str := C.GoString((*C.char)(unsafe.Pointer(value)))

	switch name {
	case typeBypassBind:
		bypassBind = str
	case typeBypassList:
		bypassList = str
	case typeDNSAddress:
		dialDNS(str)
	case typeBlockIPv6:
		proxies.BlockIPv6 = str == "1"
	case typeAdapterMTU:
		n, err := strconv.Atoi(str)
		if err != nil {
			panic(err)
		}
		mtu = n
	case typeTCPReset:
		tcpInfo = new(structs.ServerInfo)
	case typeUDPReset:
		udpInfo = new(structs.ServerInfo)
	default:
		if name >= typeTCPType && name <= typeTCPOBFSParam && tcpInfo != nil {
			setField(tcpInfo, name-typeTCPType, str)
		} else if name >= typeUDPType && name <= typeUDPOBFSParam && udpInfo != nil {
			setField(udpInfo, name-typeUDPType, str)
		} else {
			return false
		}
	}

	return true
}

// dialDNS splits "scheme://address": "tls" selects DNS over TLS, any other scheme plain TCP.
// Without a scheme only the address changes.
func dialDNS(value string) {
	parts := strings.SplitN(value, "://", 2)
	if len(parts) != 2 {
		adns.Address = value
		return
	}

	adns.Address = parts[1]
	if parts[0] == "tls" {
		adns.Client.Net = "tcp-tls"
	} else {
		adns.Client.Net = "tcp"
	}
}

// setField sets the n-th field of a server description:
// type, host, user, pass, method, protocol, protocol param, obfs, obfs param.
func setField(info *structs.ServerInfo, n int, value string) {
	fields := [...]*string{
		&info.Type, &info.Hostname, &info.Username, &info.Password, &info.Method,
		&info.Prot, &info.ProtParam, &info.OBFS, &info.OBFSParam,
	}

	*fields[n] = value
}

//export tun_init
func tun_init() bool {
	upload, download = 0, 0

	if !newRanger() {
		return false
	}

	guid, err := windows.GUIDFromString(adapterGUID)
	if err != nil {
		log.Printf("[tun2socks][newTUN][windows.GUIDFromString] %v", err)
		return false
	}

	device, err = tun.CreateTUNWithRequestedGUID(adapterName, &guid, mtu)
	if err != nil {
		log.Printf("[tun2socks][newTUN][tun.CreateTUN] %v", err)
		return false
	}

	// only SOCKS5 is supported
	if tcpInfo == nil || udpInfo == nil || !isSocks(tcpInfo.Type) || !isSocks(udpInfo.Type) {
		log.Printf("[tun2socks][proxies.NewHandler] Unknown Type")
		return false
	}

	binder := &net.TCPAddr{IP: net.ParseIP(bypassBind)}

	core.RegisterTCPConnHandler(proxies.NewTCPHandler(socks.NewTCPHandler(tcpInfo), binder, ranger))
	core.RegisterUDPConnHandler(proxies.NewUDPHandler(socks.NewUDPHandler(udpInfo)))

	stack = core.NewLWIPStack()
	core.RegisterOutputFn(toDevice)

	running = true

	go fromDevice()

	return true
}

func isSocks(kind string) bool {
	return kind == "S5" || kind == "Socks" || kind == "Socks5"
}

// toDevice writes a packet from lwIP to the TUN device.
func toDevice(data []byte) (int, error) {
	if !running {
		return len(data), nil
	}

	downLock.Lock()
	download += uint64(len(data))
	downLock.Unlock()

	return device.Write(data, 0)
}

// fromDevice feeds packets read from the TUN device into lwIP.
func fromDevice() {
	buf := make([]byte, mtu)

	for running {
		n, err := device.Read(buf, 0)
		if err != nil {
			if err == os.ErrClosed {
				log.Println("[tun2socks] Closed")
				return
			}

			log.Printf("[tun2socks][device.Read] %v", err)
			return
		}

		// oversized packet
		if n > mtu {
			continue
		}

		upLock.Lock()
		upload += uint64(n)
		upLock.Unlock()

		// IPv4 or IPv6 only
		if version := buf[0] >> 4; version != 4 && version != 6 {
			continue
		}

		if _, err := stack.Write(buf[:n]); err != nil {
			log.Printf("[tun2socks][lwip.Write] %v", err)
		}
	}
}

//export tun_free
func tun_free() bool {
	running = false

	if stack != nil {
		stack.Close()
	}

	if device != nil {
		device.Close()
	}

	stack, device, ranger = nil, nil, nil

	return true
}

//export tun_luid
func tun_luid() uint64 {
	return device.(*tun.NativeTun).LUID()
}

//export tun_getUP
func tun_getUP() uint64 {
	upLock.Lock()
	defer upLock.Unlock()

	return upload
}

//export tun_getDL
func tun_getDL() uint64 {
	downLock.Lock()
	defer downLock.Unlock()

	return download
}

// newRanger loads the bypass list; "disabled" means no bypass at all.
func newRanger() bool {
	if bypassList == "disabled" {
		ranger = nil
		return true
	}

	file, err := os.OpenFile(bypassList, os.O_RDONLY, 0)
	if err != nil {
		log.Printf("[tun2socks][newRanger][os.Open] %v", err)
		return false
	}
	defer file.Close()

	ranger = cidranger.NewPCTrieRanger()

	scanner := bufio.NewScanner(file)

	for scanner.Scan() {
		_, network, err := net.ParseCIDR(strings.TrimSpace(scanner.Text()))
		if err != nil {
			log.Printf("[tun2socks][newRanger][net.ParseCIDR] %v", err)
			continue
		}

		if err := ranger.Insert(cidranger.NewBasicRangerEntry(*network)); err != nil {
			log.Printf("[tun2socks][newRanger][ranger.Insert] %v", err)
		}
	}

	return true
}

func main() {}
