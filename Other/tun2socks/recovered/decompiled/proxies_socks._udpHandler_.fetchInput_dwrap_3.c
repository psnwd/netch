// Ghidra 12.1.4 decompile of rixcloud.moe/aiocloud/tun2socks/proxies/socks.(*udpHandler).fetchInputÂ·dwrapÂ·3 @ 2394ee6a0 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// Golang function info: {@address 2397392f0 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/proxies/socks/udp.go:41
// Golang stacktrace signature: func
// rixcloud.moe/aiocloud/tun2socks/proxies/socks.(*udpHandler).fetchInput·dwrap·3() ???
// Golang signature [recv_artificial, params_missing, return_info_missing]: func (*udpHandler)
// fetchInput.dwrap.3(???) ???
// Golang method in type *udpHandler

void rixcloud.moe/aiocloud/tun2socks/proxies/socks::
     rixcloud_moe_aiocloud_tun2socks_proxies_socks___udpHandler__fetchInput_dwrap_3(void *self)

{
  int unaff_RDX;
  void *self_spill;
  undefined1 auStack_20 [24];
  
  while (&stack0x00000000 <= CURRENT_G.stackguard0) {
    self = (void *)runtime::runtime_morestack(self);
  }
  if ((CURRENT_G._panic != (runtime__panic *)0x0) && ((CURRENT_G._panic)->argp == &self_spill)) {
    (CURRENT_G._panic)->argp = auStack_20;
  }
  rixcloud_moe_aiocloud_tun2socks_proxies_socks___udpHandler__Close
            (*(rixcloud_moe_aiocloud_tun2socks_proxies_socks_udpHandler **)(unaff_RDX + 0x18));
  return;
}

