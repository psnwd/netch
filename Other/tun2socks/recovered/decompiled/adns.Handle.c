// Ghidra 12.1.4 decompile of rixcloud.moe/aiocloud/tun2socks/adns.Handle @ 2394ec180 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// Golang function info: {@address 239738ca8 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/adns/adns.go:21
// Golang stacktrace signature: func rixcloud.moe/aiocloud/tun2socks/adns.Handle() ???

void rixcloud.moe/aiocloud/tun2socks/adns::rixcloud_moe_aiocloud_tun2socks_adns_Handle
               (int param_1,undefined8 param_2,undefined8 param_3,undefined8 param_4,int param_5,
               undefined8 param_6)

{
  uint16 uVar1;
  uint16 uVar2;
  char *pcVar3;
  github_com_miekg_dns_RR_itab *extraout_RAX;
  undefined8 extraout_RAX_00;
  github_com_miekg_dns_Msg *self;
  undefined8 extraout_RAX_01;
  int extraout_RCX;
  undefined8 extraout_RCX_00;
  int extraout_RCX_01;
  undefined8 extraout_RCX_02;
  unsafe_Pointer extraout_RBX;
  undefined8 extraout_RBX_00;
  undefined8 extraout_RBX_01;
  undefined8 extraout_RSI;
  undefined8 extraout_RSI_00;
  undefined8 extraout_RDI;
  int extraout_RDI_00;
  undefined8 extraout_RDI_01;
  int extraout_RDI_02;
  runtime__type *prVar4;
  undefined8 uVar5;
  error eVar6;
  string sVar7;
  string format_00;
  string format_01;
  string format_02;
  string format_03;
  __github_com_miekg_dns_Question _Var8;
  __github_com_miekg_dns_RR _Var9;
  __uint8 _Var10;
  __interface___ v;
  __interface___ a;
  __interface___ v_00;
  __interface___ v_01;
  __interface___ v_02;
  __interface___ v_03;
  int iStack0000000000000008;
  undefined8 uStack0000000000000010;
  undefined8 uStack0000000000000018;
  runtime_slice in_stack_fffffffffffffe80;
  runtime__type *local_128;
  unsafe_Pointer pvStack_120;
  github_com_miekg_dns_Msg local_118;
  github_com_miekg_dns_Msg local_90;
  string format;
  
  prVar4 = (runtime__type *)0x0;
  uVar5 = 0;
  iStack0000000000000008 = param_1;
  uStack0000000000000010 = param_2;
  while (uStack0000000000000018 = param_3, &local_118.MsgHdr.Rcode <= CURRENT_G.stackguard0) {
    runtime::runtime_morestack_noctxt();
    param_3 = uStack0000000000000018;
  }
  local_90.MsgHdr.Id = 0;
  local_90.MsgHdr.Response = false;
  local_90.MsgHdr._3_5_ = 0;
  runtime::runtime_duffzero_239343a62(&local_90.MsgHdr.Opcode);
  _Var10.len = param_4;
  _Var10.array = (uint8 *)param_3;
  _Var10.cap = param_5;
  eVar6 = github.com/miekg/dns::github_com_miekg_dns___Msg__Unpack(&local_90,_Var10);
  pvStack_120 = eVar6.data;
  if (eVar6.tab != (error_itab *)0x0) {
    local_128 = (eVar6.tab)->_type;
    sVar7.len = 0x2b;
    sVar7.str = "[tun2socks][adns] Failed to unpack data: %v";
    v.len = 1;
    v.array = (interface___ *)&local_128;
    v.cap = 1;
    log::log_Printf(sVar7,v);
    return;
  }
  if ((local_90.Question.len == 1) && ((local_90.Question.array)->Qtype == 0xc)) {
    local_128 = prVar4;
    pvStack_120 = (unsafe_Pointer)uVar5;
    pvStack_120 = runtime::runtime_convTstring((local_90.Question.array)->Name);
    local_128 = &string___String_type;
    format.len = 0xf;
    format.str = "%s PTR aioCloud";
    a.len = 1;
    a.array = (interface___ *)&local_128;
    a.cap = 1;
    sVar7 = fmt::fmt_Sprintf(format,a);
    github.com/miekg/dns::github_com_miekg_dns_NewRR(sVar7.str,sVar7.len);
    if (extraout_RCX != 0) {
      local_128 = *(runtime__type **)(extraout_RCX + 8);
      format_00.len = 0x22;
      format_00.str = "[tun2socks][adns] NewRR failed: %v";
      v_00.len = 1;
      v_00.array = (interface___ *)&local_128;
      v_00.cap = 1;
      pvStack_120 = (unsafe_Pointer)extraout_RDI;
      log::log_Printf(format_00,v_00);
      return;
    }
    local_118.MsgHdr.Id = 0;
    local_118.MsgHdr.Response = false;
    local_118.MsgHdr._3_5_ = 0;
    runtime::runtime_duffzero_239343a62(&local_118.MsgHdr.Opcode);
    local_118.MsgHdr.Response = true;
    local_118.MsgHdr.Id = local_90.MsgHdr.Id;
    local_118.MsgHdr.Opcode = local_90.MsgHdr.Opcode;
    if (local_90.MsgHdr.Opcode == 0) {
      local_118.MsgHdr.RecursionDesired = local_90.MsgHdr.RecursionDesired;
      local_118.MsgHdr.CheckingDisabled = local_90.MsgHdr.CheckingDisabled;
    }
    local_118.MsgHdr.Rcode = 0;
    if (0 < local_90.Question.len) {
      _Var8 = runtime::runtime_makeslice
                        (&github.com/miekg/dns::github_com_miekg_dns_Question___Struct_type.typ,1,1)
      ;
      local_118.Question.array = _Var8.array;
      local_118.Question.len = 1;
      local_118.Question.cap = 1;
      if (local_90.Question.len == 0) {
                    // WARNING: Subroutine does not return
        runtime::runtime_panicIndex(0,_Var8.len);
      }
      pcVar3 = ((local_90.Question.array)->Name).str;
      uVar1 = (local_90.Question.array)->Qtype;
      uVar2 = (local_90.Question.array)->Qclass;
      ((local_118.Question.array)->Name).len = ((local_90.Question.array)->Name).len;
      (local_118.Question.array)->Qtype = uVar1;
      (local_118.Question.array)->Qclass = uVar2;
      ((local_118.Question.array)->Name).str = pcVar3;
    }
    if ((uint)local_118.Answer.cap < local_118.Answer.len + 1U) {
      _Var9 = runtime::runtime_growslice
                        (&github.com/miekg/dns::github_com_miekg_dns_RR___Interface_type.typ,
                         in_stack_fffffffffffffe80,(int)local_118.Answer.array);
      local_118.Answer.cap = _Var9.cap;
      local_118.Answer.len = _Var9.len;
      local_118.Answer.array = _Var9.array;
    }
    local_118.Answer.array[local_118.Answer.len].tab = extraout_RAX;
    local_118.Answer.array[local_118.Answer.len].data = extraout_RBX;
    local_118.Answer.len = local_118.Answer.len + 1;
    github.com/miekg/dns::github_com_miekg_dns___Msg__PackBuffer(&local_118);
    if (extraout_RDI_00 == 0) {
      if (iStack0000000000000008 != 0) {
        (**(code **)(iStack0000000000000008 + 0x30))
                  (uStack0000000000000010,extraout_RAX_00,extraout_RBX_00,extraout_RCX_00,param_6);
      }
      return;
    }
    local_128 = *(runtime__type **)(extraout_RDI_00 + 8);
    format_01.len = 0x29;
    format_01.str = "[tun2socks][adns] Failed to pack data: %v";
    v_01.len = 1;
    v_01.array = (interface___ *)&local_128;
    v_01.cap = 1;
    pvStack_120 = (unsafe_Pointer)extraout_RSI;
    log::log_Printf(format_01,v_01);
    return;
  }
  self = (github_com_miekg_dns_Msg *)
         github.com/miekg/dns::github_com_miekg_dns___Client__Exchange(&PTR_s_tcp_239542080);
  if (extraout_RCX_01 != 0) {
    local_128 = *(runtime__type **)(extraout_RCX_01 + 8);
    format_02.len = 0x2b;
    format_02.str = "[tun2socks][adns] DNS resolution failed: %v";
    v_02.len = 1;
    v_02.array = (interface___ *)&local_128;
    v_02.cap = 1;
    pvStack_120 = (unsafe_Pointer)extraout_RDI_01;
    log::log_Printf(format_02,v_02);
    return;
  }
  github.com/miekg/dns::github_com_miekg_dns___Msg__PackBuffer(self);
  if (extraout_RDI_02 == 0) {
    if (iStack0000000000000008 != 0) {
      (**(code **)(iStack0000000000000008 + 0x30))
                (uStack0000000000000010,extraout_RAX_01,extraout_RBX_01,extraout_RCX_02,param_6);
    }
    return;
  }
  local_128 = *(runtime__type **)(extraout_RDI_02 + 8);
  format_03.len = 0x29;
  format_03.str = "[tun2socks][adns] Failed to pack data: %v";
  v_03.len = 1;
  v_03.array = (interface___ *)&local_128;
  v_03.cap = 1;
  pvStack_120 = (unsafe_Pointer)extraout_RSI_00;
  log::log_Printf(format_03,v_03);
  return;
}

