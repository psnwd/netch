// Ghidra 12.1.4 decompile of rixcloud.moe/aiocloud/tun2socks/proxies.(*udpHandler).Connect @ 2394efa40 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// Golang function info: {@address 239739730 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/proxies/udp.go:18
// Golang stacktrace signature: func rixcloud.moe/aiocloud/tun2socks/proxies.(*udpHandler).Connect()
// ???
// Golang signature [from_rtti_method]: func (*udpHandler) Connect(core.UDPConn, *net.UDPAddr) error
// Golang method in type {@address 239567ba0 *proxies.udpHandler}

error rixcloud.moe/aiocloud/tun2socks/proxies::
      rixcloud_moe_aiocloud_tun2socks_proxies___udpHandler__Connect
                (rixcloud_moe_aiocloud_tun2socks_proxies_udpHandler *self,
                rixcloud_moe_aiocloud_tun2socks_core_UDPConn param_2,net_UDPAddr *param_3)

{
  error eVar1;
  rixcloud_moe_aiocloud_tun2socks_proxies_udpHandler *self_spill;
  rixcloud_moe_aiocloud_tun2socks_core_UDPConn param_2_spill;
  net_UDPAddr *param_3_spill;
  rixcloud_moe_aiocloud_tun2socks_core_UDPConn in_stack_ffffffffffffffd8;
  eVar1 = (*((self->h).tab)->Connect)
                    ((self->h).data,in_stack_ffffffffffffffd8,(net_UDPAddr *)param_2.tab);
  return eVar1;
}

