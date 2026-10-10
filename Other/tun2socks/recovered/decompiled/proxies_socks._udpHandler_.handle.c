// Ghidra 12.1.4 decompile of rixcloud.moe/aiocloud/tun2socks/proxies/socks.(*udpHandler).handle @ 2394ee120 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// Golang function info: {@address 2397391d8 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/proxies/socks/udp.go:29
// Golang stacktrace signature: func
// rixcloud.moe/aiocloud/tun2socks/proxies/socks.(*udpHandler).handle() ???
// Golang signature [params_missing, return_info_missing]: func (*udpHandler) handle(???) ???
// Golang method in type {@address 239585ca0 *socks.udpHandler}

void rixcloud.moe/aiocloud/tun2socks/proxies/socks::
     rixcloud_moe_aiocloud_tun2socks_proxies_socks___udpHandler__handle
               (rixcloud_moe_aiocloud_tun2socks_proxies_socks_udpHandler *self)

{
  int extraout_RBX;
  undefined8 in_RSI;
  int in_RDI;
  __uint8 _Var1;
  rixcloud_moe_aiocloud_tun2socks_proxies_socks_udpHandler *self_spill;
  int iStack0000000000000020;
  undefined8 uStack0000000000000028;
  code *local_28 [3];
  code **local_10;
  
  uStack0000000000000028 = in_RSI;
  while (&stack0x00000000 <= CURRENT_G.stackguard0) {
    iStack0000000000000020 = in_RDI;
    runtime::runtime_morestack_noctxt();
    in_RDI = iStack0000000000000020;
  }
  local_28[0] = rixcloud_moe_aiocloud_tun2socks_proxies_socks___udpHandler__handle_dwrap_2;
  local_10 = local_28;
  _Var1 = runtime::runtime_makeslice(&uint8___Uint8_type,1,1);
  do {
    (**(code **)(in_RDI + 0x28))(uStack0000000000000028,_Var1.array,1,1);
  } while (extraout_RBX == 0);
  (**local_10)();
  return;
}

