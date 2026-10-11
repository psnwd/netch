// Ghidra 12.1.4 decompile of main.newRanger @ 2394f0fe0 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// Golang function info: {@address 239739b70 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/main.go:272
// Golang stacktrace signature: func main.newRanger() ???

undefined8 main::main_newRanger(void)

{
  char cVar1;
  github_com_yl2chen_cidranger_basicRangerEntry *pgVar2;
  net_IPNet *pnVar3;
  undefined1 auVar4 [16];
  string format;
  string sVar5;
  string format_00;
  string format_01;
  multireturn_os_File___error_ mVar6;
  multireturn_net_IP_net_IPNet___error_ mVar7;
  __interface___ v;
  __interface___ v_00;
  __interface___ v_01;
  code *local_e0;
  os_File *poStack_d8;
  runtime__type *local_d0;
  unsafe_Pointer pvStack_c8;
  uint8 *local_c0;
  int iStack_b8;
  int local_b0;
  uint8 *puStack_a8;
  int local_a0;
  int iStack_98;
  runtime_itab *local_90;
  os_File *local_88;
  undefined **local_80;
  undefined8 local_78;
  uint8 *local_70;
  int local_68;
  code **local_10;
  local_10 = (code **)0x0;
  if ((DAT_23953cde8 == 8) && (*(int *)PTR_DAT_23953cde0 == 0x64656c6261736964)) {
    DAT_239743c08 = 0;
    DAT_239743c00 = 0;
    return 1;
  }
  sVar5.len = DAT_23953cde8;
  sVar5.str = PTR_DAT_23953cde0;
  mVar6 = os::os_OpenFile(sVar5,0,0);
  local_88 = mVar6.~r0;
  if (mVar6.~r1.tab != (error_itab *)0x0) {
    local_d0 = (mVar6.~r1.tab)->_type;
    format.len = 0x22;
    format.str = "[tun2socks][newRanger][os.Open] %v";
    v.len = 1;
    v.array = (interface___ *)&local_d0;
    v.cap = 1;
    pvStack_c8 = mVar6.~r1.data;
    log::log_Printf(format,v);
    return 0;
  }
  local_e0 = main_newRanger_dwrap_3;
  local_10 = &local_e0;
  poStack_d8 = local_88;
  auVar4 = github.com/yl2chen/cidranger::github_com_yl2chen_cidranger_newVersionedRanger
                     (&PTR_github_com_yl2chen_cidranger_newPrefixTree_2395aebb0);
  DAT_239743c08 = auVar4._8_8_;
  DAT_239743c00 = auVar4._0_8_;
  runtime::runtime_duffzero_239343a62(&local_90);
  local_90 = &os::_os_File__implements__io_Reader__itab;
  local_80 = &PTR_bufio_ScanLines_2395ae7b8;
  local_78 = 0x10000;
  while (cVar1 = bufio::bufio___Scanner__Scan(&local_90), cVar1 != '\0') {
    sVar5 = runtime::runtime_slicebytetostring((runtime_tmpBuf *)0x0,local_70,local_68);
    sVar5 = strings::strings_TrimSpace(sVar5);
    mVar7 = net::net_ParseCIDR(sVar5);
    pnVar3 = mVar7.~r1;
    if (mVar7.~r2.tab == (error_itab *)0x0) {
      local_c0 = (pnVar3->IP).array;
      iStack_b8 = (pnVar3->IP).len;
      local_b0 = (pnVar3->IP).cap;
      puStack_a8 = (pnVar3->Mask).array;
      local_a0 = (pnVar3->Mask).len;
      iStack_98 = (pnVar3->Mask).cap;
      pgVar2 = runtime::runtime_newobject
                         (&github.com/yl2chen/cidranger::
                           github_com_yl2chen_cidranger_basicRangerEntry___Struct_type.typ);
      (pgVar2->ipNet).IP.array = local_c0;
      (pgVar2->ipNet).IP.len = iStack_b8;
      (pgVar2->ipNet).IP.cap = local_b0;
      (pgVar2->ipNet).Mask.array = puStack_a8;
      (pgVar2->ipNet).Mask.len = local_a0;
      (pgVar2->ipNet).Mask.cap = iStack_98;
      auVar4 = (**(code **)(DAT_239743c00 + 0x30))
                         (DAT_239743c08,
                          &github.com/yl2chen/cidranger::
                           _github_com_yl2chen_cidranger_basicRangerEntry__implements__cidranger_RangerEntry__itab
                          ,pgVar2);
      if (auVar4._0_8_ != 0) {
        local_d0 = *(runtime__type **)(auVar4._0_8_ + 8);
        format_01.len = 0x28;
        format_01.str = "[tun2socks][newRanger][ranger.Insert] %v";
        v_01.len = 1;
        v_01.array = (interface___ *)&local_d0;
        v_01.cap = 1;
        pvStack_c8 = auVar4._8_8_;
        log::log_Printf(format_01,v_01);
      }
    }
    else {
      local_d0 = (mVar7.~r2.tab)->_type;
      format_00.len = 0x28;
      format_00.str = "[tun2socks][newRanger][net.ParseCIDR] %v";
      v_00.len = 1;
      v_00.array = (interface___ *)&local_d0;
      v_00.cap = 1;
      pvStack_c8 = mVar7.~r2.data;
      log::log_Printf(format_00,v_00);
    }
  }
  (**local_10)();
  return 1;
}

