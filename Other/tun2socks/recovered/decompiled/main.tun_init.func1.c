// Ghidra 12.1.4 decompile of main.tun_init.func1 @ 2394f14a0 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// Golang function info: {@address 239739c58 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/main.go:173
// Golang stacktrace signature: func main.tun_init.func1() ???
// Golang signature [params_missing, return_info_missing, closure]: func(.context, ???) ???

int main::main_tun_init_func1(_closure *_context)

{
  sdword sVar1;
  undefined8 in_RAX;
  int iVar2;
  undefined8 in_RCX;
  int in_RBX;
  bool bVar3;
  undefined8 uStack0000000000000008;
  int iStack0000000000000010;
  undefined8 uStack0000000000000018;
  
  uStack0000000000000008 = in_RAX;
  uStack0000000000000018 = in_RCX;
  iStack0000000000000010 = in_RBX;
  if (DAT_239798e8b == '\0') {
    return iStack0000000000000010;
  }
  LOCK();
  bVar3 = DAT_239798f58 == 0;
  if (bVar3) {
    DAT_239798f58 = 1;
  }
  UNLOCK();
  if (!bVar3) {
    sync::sync___Mutex__lockSlow((sync_Mutex *)&DAT_239798f58);
  }
  DAT_239798f50 = DAT_239798f50 + iStack0000000000000010;
  LOCK();
  sVar1 = DAT_239798f58 + -1;
  UNLOCK();
  bVar3 = DAT_239798f58 != 1;
  DAT_239798f58 = sVar1;
  if (bVar3) {
    sync::sync___Mutex__unlockSlow((sync_Mutex *)&DAT_239798f58);
  }
  iVar2 = (**(code **)(DAT_239743be0 + 0x50))
                    (DAT_239743be8,uStack0000000000000008,iStack0000000000000010,
                     uStack0000000000000018,0);
  return iVar2;
}

