// Ghidra 12.1.4 decompile of main.tun_getDLÂ·dwrapÂ·2 @ 2394f0f80 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// Golang function info: {@address 239739b28 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/main.go:267
// Golang stacktrace signature: func main.tun_getDL·dwrap·2() ???

void main::main_tun_getDL_dwrap_2(void)

{
  int unaff_RDX;
  undefined1 auStack_10 [8];
  if ((CURRENT_G._panic != (runtime__panic *)0x0) && ((CURRENT_G._panic)->argp == &stack0x00000008))
  {
    (CURRENT_G._panic)->argp = auStack_10;
  }
  sync::sync___Mutex__Unlock(*(sync_Mutex **)(unaff_RDX + 8));
  return;
}

