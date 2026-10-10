// Ghidra 12.1.4 decompile of rixcloud.moe/aiocloud/tun2socks/proxies/socks.(*tcpHandler).HandleÂ·dwrapÂ·1 @ 2394ee0c0 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// Golang function info: {@address 239739190 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/proxies/socks/tcp.go:48
// Golang stacktrace signature: func
// rixcloud.moe/aiocloud/tun2socks/proxies/socks.(*tcpHandler).Handle·dwrap·1() ???
// Golang signature [recv_artificial, params_missing, return_info_missing]: func (*tcpHandler)
// Handle.dwrap.1(???) ???
// Golang method in type *tcpHandler

void rixcloud.moe/aiocloud/tun2socks/proxies/socks::
     rixcloud_moe_aiocloud_tun2socks_proxies_socks___tcpHandler__Handle_dwrap_1(void *self)

{
  int unaff_RDX;
  void *self_spill;
  undefined1 auStack_28 [32];
  
  while (&stack0x00000000 <= CURRENT_G.stackguard0) {
    self = (void *)runtime::runtime_morestack(self);
  }
  if ((CURRENT_G._panic != (runtime__panic *)0x0) && ((CURRENT_G._panic)->argp == &self_spill)) {
    (CURRENT_G._panic)->argp = auStack_28;
  }
  rixcloud.moe/aiocloud/tun2socks/tools::rixcloud_moe_aiocloud_tun2socks_tools_CopyBuffer
            (*(undefined8 *)(unaff_RDX + 8),*(undefined8 *)(unaff_RDX + 0x10),
             *(undefined8 *)(unaff_RDX + 0x18),*(undefined8 *)(unaff_RDX + 0x20));
  return;
}

