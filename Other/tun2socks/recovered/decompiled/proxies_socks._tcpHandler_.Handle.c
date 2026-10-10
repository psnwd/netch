// Ghidra 12.1.4 decompile of rixcloud.moe/aiocloud/tun2socks/proxies/socks.(*tcpHandler).Handle @ 2394edc40 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// Golang function info: {@address 239739128 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/proxies/socks/tcp.go:18
// Golang stacktrace signature: func
// rixcloud.moe/aiocloud/tun2socks/proxies/socks.(*tcpHandler).Handle() ???
// Golang signature [from_rtti_method]: func (*tcpHandler) Handle(net.Conn, *net.TCPAddr) error
// Golang method in type {@address 239563320 *socks.tcpHandler}

error rixcloud.moe/aiocloud/tun2socks/proxies/socks::
      rixcloud_moe_aiocloud_tun2socks_proxies_socks___tcpHandler__Handle
                (rixcloud_moe_aiocloud_tun2socks_proxies_socks_tcpHandler *self,net_Conn param_2,
                net_TCPAddr *param_3)

{
  net_Conn_itab *extraout_RAX;
  uint8 (*cap) [3];
  rixcloud_moe_aiocloud_tun2socks_proxies_socks_struct___F_uintptr___autotmp_18_net_Conn___autotmp_26_net_Conn__
  *fn;
  error_itab *extraout_RCX;
  void *self_00;
  unsafe_Pointer extraout_RSI;
  unsafe_Pointer extraout_RDI;
  error_itab *extraout_RDI_00;
  error eVar1;
  string sVar2;
  undefined1 auVar3 [16];
  string format;
  io_Reader r;
  string format_00;
  runtime_iface rVar4;
  string format_01;
  error eVar5;
  __uint8 _Var6;
  multireturn_int_error_ mVar7;
  runtime_iface i;
  __interface___ v;
  __interface___ v_00;
  __byte buf;
  __interface___ v_01;
  __interface___ v_02;
  rixcloud_moe_aiocloud_tun2socks_proxies_socks_tcpHandler *self_spill;
  net_Conn param_2_spill;
  net_TCPAddr *param_3_spill;
  __uint8 in_stack_ffffffffffffff50;
  uint8 *local_30;
  net_Conn_itab *local_28;
  net_TCPAddr *local_20;
  runtime__type *local_18;
  unsafe_Pointer pvStack_10;
  local_28 = param_2.tab;
  local_20 = param_3;
  _Var6 = runtime::runtime_makeslice(&uint8___Uint8_type,0x103,0x103);
  local_30 = _Var6.array;
  rixcloud_moe_aiocloud_tun2socks_proxies_socks_handshake(self->t,local_30,0x103,0x103);
  if (extraout_RCX != (error_itab *)0x0) {
    local_18 = extraout_RCX->_type;
    sVar2.len = 0x25;
    sVar2.str = "[tun2socks][TCP][socks][handshake] %v";
    v.len = 1;
    v.array = (interface___ *)&local_18;
    v.cap = 1;
    pvStack_10 = extraout_RDI;
    log::log_Printf(sVar2,v);
    eVar1.data = extraout_RDI;
    eVar1.tab = extraout_RCX;
    return eVar1;
  }
  sVar2 = net::net___TCPAddr__String(local_20);
  auVar3 = github.com/aiocloud/shadowsocks/socks::github_com_aiocloud_shadowsocks_socks_ParseAddr
                     (sVar2.str,sVar2.len);
  cap = runtime::runtime_newobject(&_3_uint8___Array_type.typ);
  *(uint8 *)((int)cap + 0) = 5;
  *(uint8 *)((int)cap + 1) = 1;
  (*cap)[2] = 0;
  if (3 < auVar3._8_8_ + 3) {
    _Var6 = runtime::runtime_growslice
                      (&uint8___Uint8_type,(runtime_slice)in_stack_ffffffffffffff50,(int)cap);
    cap = (uint8 (*) [3])_Var6.array;
  }
  runtime::runtime_memmove(cap + 1,auVar3._0_8_,auVar3._8_8_);
  mVar7 = (*extraout_RAX->Write)(self_00,in_stack_ffffffffffffff50);
  pvStack_10 = mVar7.~r1.data;
  if (mVar7.~r1.tab != (error_itab *)0x0) {
    local_18 = (mVar7.~r1.tab)->_type;
    format.len = 0x28;
    format.str = "[tun2socks][TCP][socks][remote.Write] %v";
    v_00.len = 1;
    v_00.array = (interface___ *)&local_18;
    v_00.cap = 1;
    log::log_Printf(format,v_00);
    (*extraout_RAX->Close)(self_00);
    return mVar7.~r1;
  }
  rVar4.data = self_00;
  rVar4.tab = (runtime_itab *)extraout_RAX;
  r = (io_Reader)runtime::runtime_convI2I(&io::io_Reader___Interface_type,rVar4);
  buf.len = 3;
  buf.array = local_30;
  buf.cap = 0x103;
  mVar7 = io::io_ReadAtLeast(r,buf,3);
  pvStack_10 = mVar7.~r1.data;
  if (mVar7.~r1.tab != (error_itab *)0x0) {
    local_18 = (mVar7.~r1.tab)->_type;
    format_00.len = 0x27;
    format_00.str = "[tun2socks][TCP][socks][io.ReadFull] %v";
    v_01.len = 1;
    v_01.array = (interface___ *)&local_18;
    v_01.cap = 1;
    log::log_Printf(format_00,v_01);
    (*extraout_RAX->Close)(self_00);
    return mVar7.~r1;
  }
  i.data = self_00;
  i.tab = (runtime_itab *)extraout_RAX;
  rVar4 = runtime::runtime_convI2I(&io::io_Reader___Interface_type,i);
  _Var6 = runtime::runtime_makeslice(&uint8___Uint8_type,0x103,0x103);
  github.com/aiocloud/shadowsocks/socks::github_com_aiocloud_shadowsocks_socks_readAddr
            (rVar4.tab,rVar4.data,_Var6.array,0x103,0x103);
  if (extraout_RDI_00 == (error_itab *)0x0) {
    fn = runtime::runtime_newobject
                   (&rixcloud_moe_aiocloud_tun2socks_proxies_socks_struct___F_uintptr___autotmp_18_net_Conn___autotmp_26_net_Conn_____Struct_type
                     .typ);
    fn->_F = (uintptr)rixcloud_moe_aiocloud_tun2socks_proxies_socks___tcpHandler__Handle_dwrap_1;
    (fn->_autotmp_18).tab = local_28;
    (fn->_autotmp_18).data = param_2.data;
    (fn->_autotmp_26).tab = extraout_RAX;
    (fn->_autotmp_26).data = self_00;
    runtime::runtime_newproc(0,(runtime_funcval *)fn);
    return (error)ZEXT816(0);
  }
  local_18 = extraout_RDI_00->_type;
  format_01.len = 0x2a;
  format_01.str = "[tun2socks][TCP][socks][socks.ReadAddr] %v";
  v_02.len = 1;
  v_02.array = (interface___ *)&local_18;
  v_02.cap = 1;
  pvStack_10 = extraout_RSI;
  log::log_Printf(format_01,v_02);
  (*extraout_RAX->Close)(self_00);
  eVar5.data = extraout_RSI;
  eVar5.tab = extraout_RDI_00;
  return eVar5;
}

