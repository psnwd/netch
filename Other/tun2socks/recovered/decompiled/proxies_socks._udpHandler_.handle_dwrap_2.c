// Ghidra 12.1.4 decompile of rixcloud.moe/aiocloud/tun2socks/proxies/socks.(*udpHandler).handleÂ·dwrapÂ·2 @ 2394ee240 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// Golang function info: {@address 239739240 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/proxies/socks/udp.go:30
// Golang stacktrace signature: func
// rixcloud.moe/aiocloud/tun2socks/proxies/socks.(*udpHandler).handle·dwrap·2() ???
// Golang signature [recv_artificial, params_missing, return_info_missing]: func (*udpHandler)
// handle.dwrap.2(???) ???
// Golang method in type *udpHandler

void rixcloud.moe/aiocloud/tun2socks/proxies/socks::
     rixcloud_moe_aiocloud_tun2socks_proxies_socks___udpHandler__handle_dwrap_2(void *self)

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
  (**(code **)(*(int *)(unaff_RDX + 8) + 0x18))(*(undefined8 *)(unaff_RDX + 0x10));
  return;
}

