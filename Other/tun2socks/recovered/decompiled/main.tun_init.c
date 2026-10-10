// Ghidra 12.1.4 decompile of main.tun_init @ 2394f0540 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// Golang function info: {@address 239739980 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/main.go:145
// Golang stacktrace signature: func main.tun_init() ???

undefined8 main::main_tun_init(void)

{
  sdword *psVar1;
  int iVar2;
  int iVar3;
  github_com_yl2chen_cidranger_Ranger_itab *pgVar4;
  char cVar5;
  undefined8 extraout_RAX;
  int extraout_RCX;
  undefined8 extraout_RBX;
  undefined8 extraout_RDI;
  undefined1 auVar6 [16];
  string format;
  string format_00;
  string s;
  string format_01;
  net_IP nVar7;
  __interface___ v;
  __interface___ v_00;
  __interface___ v_01;
  undefined8 local_d8;
  undefined8 uStack_d0;
  undefined4 local_88;
  undefined4 uStack_84;
  undefined4 uStack_80;
  undefined4 uStack_7c;
  undefined4 local_78;
  undefined4 uStack_74;
  undefined4 uStack_70;
  undefined4 uStack_6c;
  rixcloud_moe_aiocloud_tun2socks_structs_ServerInfo *local_68;
  rixcloud_moe_aiocloud_tun2socks_structs_ServerInfo *local_60;
  unsafe_Pointer local_58;
  unsafe_Pointer local_50;
  char *local_48;
  net_TCPAddr *local_40;
  rixcloud_moe_aiocloud_tun2socks_proxies_socks_udpHandler *local_38;
  runtime_hmap *local_30;
  rixcloud_moe_aiocloud_tun2socks_proxies_socks_tcpHandler *local_28;
  uint8 *local_20;
  undefined8 local_18;
  undefined8 uStack_10;
  DAT_239798f60 = 0;
  DAT_239798f50 = 0;
  cVar5 = main_newRanger();
  if (cVar5 == '\0') {
    return 0;
  }
  auVar6 = golang.org/x/sys/windows::golang_org_x_sys_windows_GUIDFromString
                     ("{14EA48E9-69FA-409F-8752-22563978E802}",0x26);
  uStack_10 = auVar6._8_8_;
  local_78 = (undefined4)local_d8;
  uStack_74 = (undefined4)((uint)local_d8 >> 0x20);
  uStack_70 = (undefined4)uStack_d0;
  uStack_6c = (undefined4)((uint)uStack_d0 >> 0x20);
  local_88 = local_78;
  uStack_84 = uStack_74;
  uStack_80 = uStack_70;
  uStack_7c = uStack_6c;
  if (auVar6._0_8_ != 0) {
    local_18 = *(undefined8 *)(auVar6._0_8_ + 8);
    format.len = 0x2e;
    format.str = "[tun2socks][newTUN][windows.GUIDFromString] %v";
    v.len = 1;
    v.array = (interface___ *)&local_18;
    v.cap = 1;
    log::log_Printf(format,v);
    return 0;
  }
  golang.zx2c4.com/wireguard/tun::golang_zx2c4_com_wireguard_tun_CreateTUNWithRequestedGUID
            ("aioCloud",8,&local_88,DAT_23950e178);
  DAT_239743be0 = extraout_RAX;
  DAT_239743be8 = extraout_RBX;
  if (extraout_RCX != 0) {
    local_18 = *(undefined8 *)(extraout_RCX + 8);
    format_00.len = 0x25;
    format_00.str = "[tun2socks][newTUN][tun.CreateTUN] %v";
    v_00.len = 1;
    v_00.array = (interface___ *)&local_18;
    v_00.cap = 1;
    uStack_10 = extraout_RDI;
    log::log_Printf(format_00,v_00);
    return 0;
  }
  s.len = DAT_23953cdd8;
  s.str = PTR_DAT_23953cdd0;
  nVar7 = net::net_ParseIP(s);
  local_20 = nVar7.array;
  local_60 = DAT_239743008;
  local_40 = runtime::runtime_newobject(&net::net_TCPAddr___Struct_type.typ);
  (local_40->IP).len = nVar7.len;
  (local_40->IP).cap = nVar7.cap;
  (local_40->IP).array = local_20;
  pgVar4 = DAT_239743c00;
  local_50 = DAT_239743c08;
  psVar1 = (sdword *)(local_60->Type).str;
  iVar2 = (local_60->Type).len;
  if (iVar2 == 2) {
    if ((short)*psVar1 != 0x3553) goto LAB_2394f0874;
code_r0x0002394f07aa:
    local_28 = runtime::runtime_newobject
                         (&rixcloud.moe/aiocloud/tun2socks/proxies/socks::
                           rixcloud_moe_aiocloud_tun2socks_proxies_socks_tcpHandler___Struct_type.
                           typ);
    local_28->t = local_60;
    DAT_239743c18 =
         runtime::runtime_newobject
                   (&rixcloud.moe/aiocloud/tun2socks/proxies::
                     rixcloud_moe_aiocloud_tun2socks_proxies_tcpHandler___Struct_type.typ);
    (DAT_239743c18->h).tab =
         (rixcloud_moe_aiocloud_tun2socks_core_TCPConnHandler_itab *)
         &rixcloud.moe/aiocloud/tun2socks/proxies/socks::
          _rixcloud_moe_aiocloud_tun2socks_proxies_socks_tcpHandler__implements__core_TCPConnHandler__itab
    ;
    (DAT_239743c18->h).data = local_28;
    DAT_239743c18->binder = local_40;
    (DAT_239743c18->ranger).tab = pgVar4;
    (DAT_239743c18->ranger).data = local_50;
    DAT_239743c10 =
         &rixcloud.moe/aiocloud/tun2socks/proxies::
          _rixcloud_moe_aiocloud_tun2socks_proxies_tcpHandler__implements__core_TCPConnHandler__itab
    ;
  }
  else {
    if (iVar2 != 5) {
      if (((iVar2 != 6) || (*psVar1 != 0x6b636f53)) || ((short)psVar1[1] != 0x3573))
      goto LAB_2394f0874;
      goto code_r0x0002394f07aa;
    }
    if ((*psVar1 == 0x6b636f53) && ((char)psVar1[1] == 's')) goto code_r0x0002394f07aa;
LAB_2394f0874:
    DAT_239743c18 = (rixcloud_moe_aiocloud_tun2socks_proxies_tcpHandler *)0x0;
    DAT_239743c10 = (runtime_itab *)0x0;
  }
  pgVar4 = DAT_239743c00;
  iVar3 = DAT_23953cdd8;
  local_68 = DAT_239743010;
  local_48 = PTR_DAT_23953cdd0;
  local_58 = DAT_239743c08;
  psVar1 = (sdword *)(DAT_239743010->Type).str;
  iVar2 = (DAT_239743010->Type).len;
  if (iVar2 == 2) {
    if ((short)*psVar1 == 0x3553) goto code_r0x0002394f094c;
  }
  else if (iVar2 == 5) {
    if ((*psVar1 == 0x6b636f53) && ((char)psVar1[1] == 's')) {
code_r0x0002394f094c:
      local_30 = runtime::runtime_makemap_small();
      local_38 = runtime::runtime_newobject
                           (&rixcloud.moe/aiocloud/tun2socks/proxies/socks::
                             rixcloud_moe_aiocloud_tun2socks_proxies_socks_udpHandler___Struct_type.
                             typ);
      local_38->t = local_68;
      local_38->conns = local_30;
      DAT_239743c28 =
           runtime::runtime_newobject
                     (&rixcloud.moe/aiocloud/tun2socks/proxies::
                       rixcloud_moe_aiocloud_tun2socks_proxies_udpHandler___Struct_type.typ);
      (DAT_239743c28->h).tab =
           (rixcloud_moe_aiocloud_tun2socks_core_UDPConnHandler_itab *)
           &rixcloud.moe/aiocloud/tun2socks/proxies/socks::
            _rixcloud_moe_aiocloud_tun2socks_proxies_socks_udpHandler__implements__core_UDPConnHandler__itab
      ;
      (DAT_239743c28->h).data = local_38;
      (DAT_239743c28->binder).len = iVar3;
      (DAT_239743c28->binder).str = local_48;
      (DAT_239743c28->ranger).tab = pgVar4;
      (DAT_239743c28->ranger).data = local_58;
      DAT_239743c20 =
           &rixcloud.moe/aiocloud/tun2socks/proxies::
            _rixcloud_moe_aiocloud_tun2socks_proxies_udpHandler__implements__core_UDPConnHandler__itab
      ;
      goto LAB_2394f0a5a;
    }
  }
  else if (((iVar2 == 6) && (*psVar1 == 0x6b636f53)) && ((short)psVar1[1] == 0x3573))
  goto code_r0x0002394f094c;
  DAT_239743c20 = (runtime_itab *)0x0;
  DAT_239743c28 = (rixcloud_moe_aiocloud_tun2socks_proxies_udpHandler *)0x0;
LAB_2394f0a5a:
  if ((DAT_239743c10 != (runtime_itab *)0x0) && (DAT_239743c20 != (runtime_itab *)0x0)) {
    auVar6 = rixcloud.moe/aiocloud/tun2socks/core::rixcloud_moe_aiocloud_tun2socks_core_NewLWIPStack
                       ();
    DAT_239743bf8 = auVar6._8_8_;
    DAT_239743bf0 = auVar6._0_8_;
    DAT_239743060 = &PTR_main_tun_init_func1_2395aec40;
    rixcloud.moe/aiocloud/tun2socks/core::rixcloud_moe_aiocloud_tun2socks_core__Cfunc_set_output();
    DAT_239743dc0 = DAT_239743c10;
    DAT_239743dc8 = DAT_239743c18;
    DAT_239743dd0 = DAT_239743c20;
    DAT_239743dd8 = DAT_239743c28;
    DAT_239798e8b = 1;
    runtime::runtime_newproc(0,(runtime_funcval *)&PTR_main_tun_init_func2_2395aec48);
    return 1;
  }
  format_01.len = 0x2c;
  format_01.str = "[tun2socks][proxies.NewHandler] Unknown Type";
  v_01.cap = 0;
  v_01.array = (interface___ *)0x0;
  v_01.len = 0;
  log::log_Printf(format_01,v_01);
  return 0;
}

