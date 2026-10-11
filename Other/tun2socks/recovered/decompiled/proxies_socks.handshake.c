// Ghidra 12.1.4 decompile of rixcloud.moe/aiocloud/tun2socks/proxies/socks.handshake @ 2394ed720 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// Golang function info: {@address 2397390c0 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/proxies/socks/socks.go:12
// Golang stacktrace signature: func rixcloud.moe/aiocloud/tun2socks/proxies/socks.handshake() ???

undefined1  [32]
rixcloud.moe/aiocloud/tun2socks/proxies/socks::
rixcloud_moe_aiocloud_tun2socks_proxies_socks_handshake
          (int param_1,int param_2,uint param_3,uint param_4)

{
  uint8 (*pauVar1) [4];
  errors_errorString *peVar2;
  undefined1 auVar3 [32];
  undefined1 auVar4 [32];
  undefined1 auVar5 [32];
  undefined1 auVar6 [32];
  undefined1 auVar7 [32];
  undefined1 auVar8 [32];
  undefined1 auVar9 [32];
  uint uVar10;
  undefined8 uVar11;
  string network;
  io_Reader iVar12;
  multireturn_int_error_ mVar13;
  __byte _Var14;
  multireturn_net_Conn_error_ mVar15;
  runtime_iface i;
  runtime_iface i_00;
  __byte buf;
  int iStack0000000000000008;
  int iStack0000000000000010;
  uint uStack0000000000000018;
  uint uStack0000000000000020;
  __uint8 in_stack_ffffffffffffff40;
  uint8 local_88 [32];
  uint8 local_68 [32];
  unsafe_Pointer local_48;
  void *local_40;
  net_Conn_itab *local_38;
  bytes_Buffer local_30;
  
  uVar10 = 0;
  uVar11._0_1_ = 0;
  uVar11._1_7_ = 0;
  iStack0000000000000008 = param_1;
  uStack0000000000000018 = param_3;
  iStack0000000000000010 = param_2;
  uStack0000000000000020 = param_4;
  network.len = 3;
  network.str = "tcp";
  mVar15 = net::net_Dial(network,*(string *)(iStack0000000000000008 + 0x10));
  local_40 = mVar15.~r0.data;
  local_38 = mVar15.~r0.tab;
  if (mVar15.~r1.tab != (error_itab *)0x0) {
    auVar9._0_16_ = ZEXT816(0);
    auVar9._16_8_ = mVar15.~r1.tab;
    auVar9._24_8_ = mVar15.~r1.data;
    return auVar9;
  }
  pauVar1 = runtime::runtime_newobject(&_4_uint8___Array_type.typ);
  *pauVar1 = (uint8  [4])0x2000205;
  mVar13 = (*local_38->Write)(local_40,in_stack_ffffffffffffff40);
  local_48 = mVar13.~r1.data;
  if (mVar13.~r1.tab != (error_itab *)0x0) {
    (*local_38->Close)(local_40);
    auVar8._0_16_ = ZEXT816(0);
    auVar8._16_8_ = mVar13.~r1.tab;
    auVar8._24_8_ = local_48;
    return auVar8;
  }
  if (uStack0000000000000020 < 2) {
                    // WARNING: Subroutine does not return
    runtime::runtime_panicSliceAcap(mVar13.~r0,0);
  }
  i.data = local_40;
  i.tab = (runtime_itab *)local_38;
  iVar12 = (io_Reader)runtime::runtime_convI2I(&io::io_Reader___Interface_type,i);
  _Var14.len = 2;
  _Var14.array = (uint8 *)iStack0000000000000010;
  _Var14.cap = uStack0000000000000020;
  mVar13 = io::io_ReadAtLeast(iVar12,_Var14,2);
  local_48 = mVar13.~r1.data;
  if (mVar13.~r1.tab != (error_itab *)0x0) {
    (*local_38->Close)(local_40);
    auVar7._0_16_ = ZEXT816(0);
    auVar7._16_8_ = mVar13.~r1.tab;
    auVar7._24_8_ = local_48;
    return auVar7;
  }
  if (uStack0000000000000018 < 2) {
                    // WARNING: Subroutine does not return
    runtime::runtime_panicIndex(1,0);
  }
  if (*(char *)(iStack0000000000000010 + 1) == '\x02') {
    local_30.buf.array = *runtime::runtime_newobject(&_1_uint8___Array_type.typ);
    (*(uint8 (*) [1])local_30.buf.array)[0] = 1;
    local_30.buf.len = 1;
    local_30.buf.cap = 1;
    local_30.off = uVar10;
    local_30._32_8_ = uVar11;
    bytes::bytes___Buffer__WriteByte(&local_30);
    _Var14 = runtime::runtime_stringtoslicebyte
                       (&local_68,*(string *)(iStack0000000000000008 + 0x20));
    bytes::bytes___Buffer__Write(&local_30,(__uint8)_Var14);
    bytes::bytes___Buffer__WriteByte(&local_30);
    _Var14 = runtime::runtime_stringtoslicebyte
                       (&local_88,*(string *)(iStack0000000000000008 + 0x30));
    mVar13 = bytes::bytes___Buffer__Write(&local_30,(__uint8)_Var14);
    if ((uint)local_30.buf.len < (uint)local_30.off) {
                    // WARNING: Subroutine does not return
      runtime::runtime_panicSliceB(local_30.off,(int)mVar13.~r1.tab);
    }
    mVar13 = (*local_38->Write)(local_40,in_stack_ffffffffffffff40);
    local_48 = mVar13.~r1.data;
    if (mVar13.~r1.tab != (error_itab *)0x0) {
      (*local_38->Close)(local_40);
      auVar6._0_16_ = ZEXT816(0);
      auVar6._16_8_ = mVar13.~r1.tab;
      auVar6._24_8_ = local_48;
      return auVar6;
    }
    i_00.data = local_40;
    i_00.tab = (runtime_itab *)local_38;
    iVar12 = (io_Reader)runtime::runtime_convI2I(&io::io_Reader___Interface_type,i_00);
    buf.len = 2;
    buf.array = (uint8 *)iStack0000000000000010;
    buf.cap = uStack0000000000000020;
    mVar13 = io::io_ReadAtLeast(iVar12,buf,2);
    local_48 = mVar13.~r1.data;
    if (mVar13.~r1.tab != (error_itab *)0x0) {
      (*local_38->Close)(local_40);
      auVar5._0_16_ = ZEXT816(0);
      auVar5._16_8_ = mVar13.~r1.tab;
      auVar5._24_8_ = local_48;
      return auVar5;
    }
    if (*(char *)(iStack0000000000000010 + 1) != '\0') {
      (*local_38->Close)(local_40);
      peVar2 = runtime::runtime_newobject(&errors::errors_errorString___Struct_type.typ);
      (peVar2->s).len = 0x15;
      (peVar2->s).str = &DAT_2395a0536;
      auVar4._0_16_ = ZEXT816(0);
      auVar4._16_8_ = &errors::_errors_errorString__implements__error__itab;
      auVar4._24_8_ = peVar2;
      return auVar4;
    }
  }
  else if (*(char *)(iStack0000000000000010 + 1) != '\0') {
    (*local_38->Close)(local_40);
    peVar2 = runtime::runtime_newobject(&errors::errors_errorString___Struct_type.typ);
    (peVar2->s).len = 0xe;
    (peVar2->s).str = "unknown method";
    auVar3._0_16_ = ZEXT816(0);
    auVar3._16_8_ = &errors::_errors_errorString__implements__error__itab;
    auVar3._24_8_ = peVar2;
    return auVar3;
  }
  return ZEXT1632(CONCAT88(local_40,local_38));
}

