// Ghidra 12.1.4 decompile of main.newRangerÂ·dwrapÂ·3 @ 2394f1420 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// Golang function info: {@address 239739bd0 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/main.go:283
// Golang stacktrace signature: func main.newRanger·dwrap·3() ???

error main::main_newRanger_dwrap_3(void)

{
  int unaff_RDX;
  error eVar1;
  undefined1 auStack_10 [8];
  if ((CURRENT_G._panic != (runtime__panic *)0x0) && ((CURRENT_G._panic)->argp == &stack0x00000008))
  {
    (CURRENT_G._panic)->argp = auStack_10;
  }
  eVar1 = os::os___File__Close(*(os_File **)(unaff_RDX + 8));
  return eVar1;
}

