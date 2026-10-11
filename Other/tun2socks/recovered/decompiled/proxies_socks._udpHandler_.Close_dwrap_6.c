// Ghidra 12.1.4 decompile of rixcloud.moe/aiocloud/tun2socks/proxies/socks.(*udpHandler).CloseÂ·dwrapÂ·6 @ 2394ef520 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// Golang function info: {@address 239739500 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/proxies/socks/udp.go:173
// Golang stacktrace signature: func
// rixcloud.moe/aiocloud/tun2socks/proxies/socks.(*udpHandler).Close·dwrap·6() ???
// Golang signature [recv_artificial, params_missing, return_info_missing]: func (*udpHandler)
// Close.dwrap.6(???) ???
// Golang method in type *udpHandler

void rixcloud.moe/aiocloud/tun2socks/proxies/socks::
     rixcloud_moe_aiocloud_tun2socks_proxies_socks___udpHandler__Close_dwrap_6(void *self)

{
  int unaff_RDX;
  void *self_spill;
  undefined1 auStack_10 [8];
  
  while (&stack0x00000000 <= CURRENT_G.stackguard0) {
    self = (void *)runtime::runtime_morestack(self);
  }
  if ((CURRENT_G._panic != (runtime__panic *)0x0) && ((CURRENT_G._panic)->argp == &self_spill)) {
    (CURRENT_G._panic)->argp = auStack_10;
  }
  sync::sync___Mutex__Unlock(*(sync_Mutex **)(unaff_RDX + 8));
  return;
}

