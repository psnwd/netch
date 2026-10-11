// Ghidra 12.1.4 decompile of rixcloud.moe/aiocloud/tun2socks/proxies/socks.(*udpHandler).fetchInput @ 2394ee2a0 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// Golang function info: {@address 239739288 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/proxies/socks/udp.go:40
// Golang stacktrace signature: func
// rixcloud.moe/aiocloud/tun2socks/proxies/socks.(*udpHandler).fetchInput() ???
// Golang signature [params_missing, return_info_missing]: func (*udpHandler) fetchInput(???) ???
// Golang method in type {@address 239585ca0 *socks.udpHandler}

void rixcloud.moe/aiocloud/tun2socks/proxies/socks::
     rixcloud_moe_aiocloud_tun2socks_proxies_socks___udpHandler__fetchInput
               (rixcloud_moe_aiocloud_tun2socks_proxies_socks_udpHandler *self)

{
  int iVar1;
  uint x;
  uint8 uVar2;
  uint8 *puVar3;
  undefined8 extraout_RAX;
  uint x_00;
  undefined8 in_RCX;
  undefined8 extraout_RCX;
  undefined8 uVar4;
  undefined8 extraout_RCX_00;
  uint8 *puVar5;
  int in_RBX;
  undefined8 extraout_RBX;
  int extraout_RBX_00;
  undefined8 in_RSI;
  int in_RDI;
  int extraout_RDI;
  uint uVar6;
  undefined1 auVar7 [16];
  string network;
  string format;
  __uint8 _Var8;
  time_Time self_00;
  multireturn_net_UDPAddr___error_ mVar9;
  __interface___ v;
  __interface___ v_00;
  string sVar10;
  __interface___ v_01;
  __interface___ v_02;
  rixcloud_moe_aiocloud_tun2socks_proxies_socks_udpHandler *self_spill;
  int iStack0000000000000010;
  undefined8 uStack0000000000000018;
  int iStack0000000000000020;
  undefined8 uStack0000000000000028;
  runtime__type *local_40;
  undefined **ppuStack_38;
  code *local_30 [3];
  rixcloud_moe_aiocloud_tun2socks_proxies_socks_udpHandler *local_18;
  code **local_10;
  
  uStack0000000000000018 = in_RCX;
  iStack0000000000000010 = in_RBX;
  uStack0000000000000028 = in_RSI;
  iStack0000000000000020 = in_RDI;
  local_30[0] = rixcloud_moe_aiocloud_tun2socks_proxies_socks___udpHandler__fetchInput_dwrap_3;
  local_10 = local_30;
  local_18 = self;
  _Var8 = runtime::runtime_makeslice(&uint8___Uint8_type,0x5dc,0x5dc);
  puVar3 = _Var8.array;
  do {
    while( true ) {
      self_00 = time::time_Now();
      time::time_Time_Add(self_00);
      (**(code **)(iStack0000000000000020 + 0x38))
                (uStack0000000000000028,extraout_RAX,extraout_RBX,extraout_RCX);
      auVar7 = (**(code **)(iStack0000000000000020 + 0x28))(uStack0000000000000028,puVar3,0x5dc);
      x_00 = auVar7._0_8_;
      if (extraout_RDI != 0) {
        (**local_10)();
        return;
      }
      if (2 < (int)x_00) break;
      local_40 = &string___String_type;
      ppuStack_38 = &PTR_DAT_2395f11e0;
      v_00.len = 1;
      v_00.array = (interface___ *)&local_40;
      v_00.cap = 1;
      log::log_Println(v_00);
    }
    if (0x5dc < x_00) {
                    // WARNING: Subroutine does not return
      runtime::runtime_panicSliceAcap(x_00,auVar7._8_8_);
    }
    iVar1 = x_00 - 3;
    if (iVar1 < 1) {
      uVar6 = 0;
      puVar5 = (uint8 *)0x0;
      uVar4 = 0;
      goto LAB_2394ee460;
    }
    uVar2 = puVar3[3];
    if (uVar2 == 1) {
      uVar6 = 7;
LAB_2394ee42e:
      if (iVar1 < (int)uVar6) {
        uVar6 = 0;
        puVar5 = (uint8 *)0x0;
        uVar4 = 0;
      }
      else {
        if (0x5d9 < uVar6) {
                    // WARNING: Subroutine does not return
          runtime::runtime_panicSliceAcap(x_00,auVar7._8_8_);
        }
        puVar5 = puVar3 + 3;
        uVar4 = 0x5d9;
      }
    }
    else if (uVar2 == 3) {
      if (1 < iVar1) {
        uVar6 = puVar3[4] + 4;
        goto LAB_2394ee42e;
      }
      uVar6 = 0;
      puVar5 = (uint8 *)0x0;
      uVar4 = 0;
    }
    else {
      if (uVar2 == 4) {
        uVar6 = 0x13;
        goto LAB_2394ee42e;
      }
      uVar6 = 0;
      puVar5 = (uint8 *)0x0;
      uVar4 = 0;
    }
LAB_2394ee460:
    if (puVar5 == (uint8 *)0x0) {
      local_40 = &string___String_type;
      ppuStack_38 = &PTR_DAT_2395f11f0;
      v.len = 1;
      v.array = (interface___ *)&local_40;
      v.cap = 1;
      log::log_Println(v);
    }
    else {
      sVar10 = (string)github.com/aiocloud/shadowsocks/socks::
                       github_com_aiocloud_shadowsocks_socks_Addr_String(puVar5,uVar6,uVar4,puVar3);
      network.len = 3;
      network.str = "udp";
      mVar9 = net::net_ResolveUDPAddr(network,sVar10);
      if (mVar9.~r1.tab == (error_itab *)0x0) {
        x = uVar6 + 3;
        if (x_00 < x) {
                    // WARNING: Subroutine does not return
          runtime::runtime_panicSliceB(x,0);
        }
        (**(code **)(iStack0000000000000010 + 0x30))
                  (uStack0000000000000018,puVar3 + (x & (int)(uVar6 - 0x5d9) >> 0x3f),
                   (x_00 - uVar6) + -3,-(uVar6 - 0x5d9),mVar9.~r0);
        if (extraout_RBX_00 != 0) {
          local_40 = *(runtime__type **)(extraout_RBX_00 + 8);
          sVar10.len = 0x43;
          sVar10.str = "[tun2socks][UDP][socks][fetchInput] Unable to write data packet: %v";
          v_02.len = 1;
          v_02.array = (interface___ *)&local_40;
          v_02.cap = 1;
          ppuStack_38 = (undefined **)extraout_RCX_00;
          log::log_Printf(sVar10,v_02);
          (**local_10)();
          return;
        }
      }
      else {
        local_40 = (mVar9.~r1.tab)->_type;
        format.len = 0x48;
        format.str = "[tun2socks][UDP][socks][fetchInput] Failed to resolve target address: %v";
        v_01.len = 1;
        v_01.array = (interface___ *)&local_40;
        v_01.cap = 1;
        ppuStack_38 = mVar9.~r1.data;
        log::log_Printf(format,v_01);
      }
    }
  } while( true );
}

