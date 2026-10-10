// Ghidra 12.1.4 decompile of rixcloud.moe/aiocloud/tun2socks/proxies.(*udpHandler).ReceiveTo @ 2394efac0 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// Golang function info: {@address 239739798 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/proxies/udp.go:22
// Golang stacktrace signature: func
// rixcloud.moe/aiocloud/tun2socks/proxies.(*udpHandler).ReceiveTo() ???
// Golang signature [from_rtti_method]: func (*udpHandler) ReceiveTo(core.UDPConn, []uint8,
// *net.UDPAddr) error
// Golang method in type {@address 239567ba0 *proxies.udpHandler}

error rixcloud.moe/aiocloud/tun2socks/proxies::
      rixcloud_moe_aiocloud_tun2socks_proxies___udpHandler__ReceiveTo
                (rixcloud_moe_aiocloud_tun2socks_proxies_udpHandler *self,
                rixcloud_moe_aiocloud_tun2socks_core_UDPConn param_2,__uint8 param_3,
                net_UDPAddr *param_4)

{
  rixcloud_moe_aiocloud_tun2socks_proxies_struct___F_uintptr___autotmp_5_core_UDPConn___autotmp_6___uint8___autotmp_7__net_UDPAddr__
  *fn;
  error eVar1;
  rixcloud_moe_aiocloud_tun2socks_proxies_udpHandler *self_spill;
  rixcloud_moe_aiocloud_tun2socks_core_UDPConn param_2_spill;
  __uint8 param_3_spill;
  net_UDPAddr *param_4_spill;
  rixcloud_moe_aiocloud_tun2socks_core_UDPConn in_stack_ffffffffffffffa8;
  __uint8 in_stack_ffffffffffffffb8;
  if (param_4->Port != 0x35) {
    eVar1 = (*((self->h).tab)->ReceiveTo)
                      ((self->h).data,in_stack_ffffffffffffffa8,in_stack_ffffffffffffffb8,
                       (net_UDPAddr *)param_2.tab);
    return eVar1;
  }
  fn = runtime::runtime_newobject
                 (&rixcloud_moe_aiocloud_tun2socks_proxies_struct___F_uintptr___autotmp_5_core_UDPConn___autotmp_6___uint8___autotmp_7__net_UDPAddr_____Struct_type
                   .typ);
  fn->_F = (uintptr)rixcloud_moe_aiocloud_tun2socks_proxies___udpHandler__ReceiveTo_dwrap_2;
  (fn->_autotmp_5).tab = param_2.tab;
  (fn->_autotmp_5).data = param_2.data;
  (fn->_autotmp_6).len = param_3.len;
  (fn->_autotmp_6).cap = param_3.cap;
  (fn->_autotmp_6).array = param_3.array;
  fn->_autotmp_7 = param_4;
  runtime::runtime_newproc(0,(runtime_funcval *)fn);
  return (error)ZEXT816(0);
}

