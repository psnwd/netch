// Ghidra 12.1.4 decompile of main.tun_getDL @ 2394f0ea0 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// WARNING: Restarted to delay deadcode elimination for space: stack
// Golang function info: {@address 239739ac8 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/main.go:265
// Golang stacktrace signature: func main.tun_getDL() ???

undefined8 main::main_tun_getDL(void)

{
  undefined8 uVar1;
  bool bVar2;
  code *local_20;
  sdword *psStack_18;
  code **local_10;
  local_10 = (code **)0x0;
  LOCK();
  bVar2 = DAT_239798f58 == 0;
  if (bVar2) {
    DAT_239798f58 = 1;
  }
  UNLOCK();
  if (!bVar2) {
    sync::sync___Mutex__lockSlow((sync_Mutex *)&DAT_239798f58);
  }
  uVar1 = DAT_239798f50;
  local_20 = main_tun_getDL_dwrap_2;
  psStack_18 = &DAT_239798f58;
  local_10 = &local_20;
  main_tun_getDL_dwrap_2(main_tun_getDL_dwrap_2);
  return uVar1;
}

