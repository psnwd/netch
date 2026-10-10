// Ghidra 12.1.4 decompile of main.tun_getUP @ 2394f0d60 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// WARNING: Restarted to delay deadcode elimination for space: stack
// Golang function info: {@address 239739a20 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/main.go:257
// Golang stacktrace signature: func main.tun_getUP() ???

undefined8 main::main_tun_getUP(void)

{
  undefined8 uVar1;
  bool bVar2;
  code *local_20;
  sdword *psStack_18;
  code **local_10;
  local_10 = (code **)0x0;
  LOCK();
  bVar2 = DAT_239798f68 == 0;
  if (bVar2) {
    DAT_239798f68 = 1;
  }
  UNLOCK();
  if (!bVar2) {
    sync::sync___Mutex__lockSlow((sync_Mutex *)&DAT_239798f68);
  }
  uVar1 = DAT_239798f60;
  local_20 = main_tun_getUP_dwrap_1;
  psStack_18 = &DAT_239798f68;
  local_10 = &local_20;
  main_tun_getUP_dwrap_1(main_tun_getUP_dwrap_1);
  return uVar1;
}

