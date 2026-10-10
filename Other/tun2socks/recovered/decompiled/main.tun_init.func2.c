// Ghidra 12.1.4 decompile of main.tun_init.func2 @ 2394f15c0 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// Golang function info: {@address 239739cc0 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/main.go:189
// Golang stacktrace signature: func main.tun_init.func2() ???
// Golang signature [params_missing, return_info_missing, closure]: func(.context, ???) ???

void main::main_tun_init_func2(_closure *_context)

{
  dword dVar1;
  uint uVar2;
  bool bVar3;
  byte *y;
  uint extraout_RAX;
  uint uVar4;
  uint extraout_RAX_00;
  unsafe_Pointer x;
  undefined8 extraout_RCX;
  sdword sVar5;
  runtime_itab *tab;
  int extraout_RBX;
  byte bVar6;
  bool bVar7;
  undefined1 auVar8 [16];
  string format;
  string format_00;
  __uint8 _Var9;
  __interface___ v;
  __interface___ v_00;
  __interface___ v_01;
  runtime__type *local_18;
  undefined **ppuStack_10;
  _Var9 = runtime::runtime_makeslice(&uint8___Uint8_type,DAT_23950e178,DAT_23950e178);
  uVar2 = DAT_23950e178;
  y = _Var9.array;
  do {
    do {
      do {
        if (DAT_239798e8b == '\0') {
          return;
        }
        (**(code **)(DAT_239743be0 + 0x48))(DAT_239743be8,y,uVar2,uVar2,0);
        if (tab != (runtime_itab *)0x0) {
          if ((DAT_239743cf0 == tab) &&
             (bVar3 = runtime::runtime_ifaceeq(tab,x,DAT_239743cf8), bVar3)) {
            local_18 = &string___String_type;
            ppuStack_10 = &PTR_DAT_2395f0e70;
            v.len = 1;
            v.array = (interface___ *)&local_18;
            v.cap = 1;
            log::log_Println(v);
            return;
          }
          local_18 = (runtime__type *)0x0;
          if (tab != (runtime_itab *)0x0) {
            local_18 = tab->_type;
          }
          format_00.len = 0x1b;
          format_00.str = "[tun2socks][device.Read] %v";
          v_01.len = 1;
          v_01.array = (interface___ *)&local_18;
          v_01.cap = 1;
          ppuStack_10 = x;
          log::log_Printf(format_00,v_01);
          return;
        }
      } while ((int)DAT_23950e178 < (int)extraout_RAX);
      uVar4 = 0;
      LOCK();
      bVar7 = DAT_239798f68 == 0;
      if (bVar7) {
        DAT_239798f68 = 1;
      }
      else {
        uVar4 = (uint)DAT_239798f68;
      }
      UNLOCK();
      if (!bVar7) {
        sync::sync___Mutex__lockSlow((sync_Mutex *)&DAT_239798f68);
        uVar4 = extraout_RAX_00;
      }
      DAT_239798f60 = DAT_239798f60 + extraout_RAX;
      LOCK();
      dVar1 = DAT_239798f68 - 1;
      UNLOCK();
      sVar5 = DAT_239798f68 - 1;
      auVar8._8_4_ = sVar5;
      auVar8._0_8_ = uVar4;
      auVar8._12_4_ = 0;
      DAT_239798f68 = dVar1;
      if (sVar5 != 0) {
        auVar8 = sync::sync___Mutex__unlockSlow((sync_Mutex *)&DAT_239798f68);
      }
      if (uVar2 == 0) {
                    // WARNING: Subroutine does not return
        runtime::runtime_panicIndex(0,auVar8._8_8_);
      }
      bVar6 = *y >> 4;
    } while ((bVar6 != 4) && (bVar6 != 6));
    if (uVar2 < extraout_RAX) {
                    // WARNING: Subroutine does not return
      runtime::runtime_panicSliceAcap(auVar8._0_8_,(int)y);
    }
    (**(code **)(DAT_239743bf0 + 0x28))
              (DAT_239743bf8,y,extraout_RAX,uVar2,*(code **)(DAT_239743bf0 + 0x28),bVar6,1);
  } while (extraout_RBX == 0);
  local_18 = *(runtime__type **)(extraout_RBX + 8);
  format.len = 0x1a;
  format.str = "[tun2socks][lwip.Write] %v";
  v_00.len = 1;
  v_00.array = (interface___ *)&local_18;
  v_00.cap = 1;
  ppuStack_10 = (undefined **)extraout_RCX;
  log::log_Printf(format,v_00);
  return;
}

