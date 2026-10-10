// Ghidra 12.1.4 decompile of rixcloud.moe/aiocloud/tun2socks/proxies.(*udpHandler).ReceiveToÂ·dwrapÂ·2 @ 2394efc40 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// Golang function info: {@address 239739800 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/proxies/udp.go:24
// Golang stacktrace signature: func
// rixcloud.moe/aiocloud/tun2socks/proxies.(*udpHandler).ReceiveTo·dwrap·2() ???
// Golang signature [recv_artificial, params_missing, return_info_missing]: func (*udpHandler)
// ReceiveTo.dwrap.2(???) ???
// Golang method in type *udpHandler

void rixcloud.moe/aiocloud/tun2socks/proxies::
     rixcloud_moe_aiocloud_tun2socks_proxies___udpHandler__ReceiveTo_dwrap_2(void *self)

{
  int unaff_RDX;
  void *self_spill;
  undefined1 auStack_38 [48];
  
  while (&stack0x00000000 <= CURRENT_G.stackguard0) {
    self = (void *)runtime::runtime_morestack(self);
  }
  if ((CURRENT_G._panic != (runtime__panic *)0x0) && ((CURRENT_G._panic)->argp == &self_spill)) {
    (CURRENT_G._panic)->argp = auStack_38;
  }
  rixcloud.moe/aiocloud/tun2socks/adns::rixcloud_moe_aiocloud_tun2socks_adns_Handle
            (*(undefined8 *)(unaff_RDX + 8),*(undefined8 *)(unaff_RDX + 0x10),
             *(undefined8 *)(unaff_RDX + 0x18),*(undefined8 *)(unaff_RDX + 0x20),
             *(undefined8 *)(unaff_RDX + 0x28),*(undefined8 *)(unaff_RDX + 0x30));
  return;
}

