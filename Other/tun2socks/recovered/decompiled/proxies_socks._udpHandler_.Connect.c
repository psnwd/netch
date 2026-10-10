// Ghidra 12.1.4 decompile of rixcloud.moe/aiocloud/tun2socks/proxies/socks.(*udpHandler).Connect @ 2394ee700 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// Golang function info: {@address 239739338 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/proxies/socks/udp.go:75
// Golang stacktrace signature: func
// rixcloud.moe/aiocloud/tun2socks/proxies/socks.(*udpHandler).Connect() ???
// Golang signature [from_rtti_method]: func (*udpHandler) Connect(core.UDPConn, *net.UDPAddr) error
// Golang method in type {@address 239585ca0 *socks.udpHandler}

error rixcloud.moe/aiocloud/tun2socks/proxies/socks::
      rixcloud_moe_aiocloud_tun2socks_proxies_socks___udpHandler__Connect
                (rixcloud_moe_aiocloud_tun2socks_proxies_socks_udpHandler *self,
                rixcloud_moe_aiocloud_tun2socks_core_UDPConn param_2,net_UDPAddr *param_3)

{
  sdword sVar1;
  uint8 *puVar2;
  net_Conn_itab *extraout_RAX;
  uint8 (*pauVar3) [10];
  rixcloud_moe_aiocloud_tun2socks_proxies_socks_struct___F_uintptr___autotmp_33_core_UDPConn___autotmp_24_net_Conn___autotmp_34__socks_udpHandler__
  *fn;
  rixcloud_moe_aiocloud_tun2socks_proxies_socks_udpData *prVar4;
  undefined8 *puVar5;
  rixcloud_moe_aiocloud_tun2socks_proxies_socks_struct___F_uintptr___autotmp_33_core_UDPConn___autotmp_35_net_PacketConn___autotmp_34__socks_udpHandler__
  *fn_00;
  unsafe_Pointer pvVar6;
  error_itab *extraout_RCX;
  rixcloud_moe_aiocloud_tun2socks_core_UDPConn_itab *prVar7;
  void *self_00;
  unsafe_Pointer extraout_RSI;
  unsafe_Pointer extraout_RDI;
  error_itab *extraout_RDI_00;
  bool bVar8;
  string format;
  io_Reader r;
  string format_00;
  runtime_iface rVar9;
  string format_01;
  error eVar10;
  string network;
  string format_02;
  string network_00;
  string format_03;
  string format_04;
  error eVar11;
  __uint8 _Var12;
  multireturn_int_error_ mVar13;
  multireturn_net_UDPAddr___error_ mVar14;
  multireturn_net_PacketConn_error_ mVar15;
  runtime_iface i;
  __interface___ v;
  __interface___ v_00;
  __byte buf;
  __interface___ v_01;
  __interface___ v_02;
  __interface___ v_03;
  __interface___ v_04;
  __interface___ v_05;
  __interface___ a;
  string sVar16;
  rixcloud_moe_aiocloud_tun2socks_proxies_socks_udpHandler *self_spill;
  rixcloud_moe_aiocloud_tun2socks_core_UDPConn param_2_spill;
  net_UDPAddr *param_3_spill;
  __uint8 in_stack_ffffffffffffff58;
  rixcloud_moe_aiocloud_tun2socks_core_UDPConn_itab *local_28;
  unsafe_Pointer local_20;
  runtime__type *local_18;
  undefined *puStack_10;
  string format_05;
  
  pvVar6 = param_2.data;
  prVar7 = param_2.tab;
  _Var12 = runtime::runtime_makeslice(&uint8___Uint8_type,0x103,0x103);
  puVar2 = _Var12.array;
  rixcloud_moe_aiocloud_tun2socks_proxies_socks_handshake(self->t,puVar2,0x103,0x103);
  if (extraout_RCX != (error_itab *)0x0) {
    local_18 = extraout_RCX->_type;
    sVar16.len = 0x35;
    sVar16.str = "[tun2socks][UDP][socks][Connect] Handshake failed: %v";
    v.len = 1;
    v.array = (interface___ *)&local_18;
    v.cap = 1;
    puStack_10 = extraout_RDI;
    log::log_Printf(sVar16,v);
    eVar11.data = extraout_RDI;
    eVar11.tab = extraout_RCX;
    return eVar11;
  }
  pauVar3 = runtime::runtime_newobject(&_10_uint8___Array_type.typ);
  *(uint8 *)((int)pauVar3 + 0) = 5;
  *(uint8 *)((int)pauVar3 + 1) = 3;
  *(uint8 *)((int)pauVar3 + 2) = 0;
  *(uint8 *)((int)pauVar3 + 3) = 1;
  *(uint8 *)((int)pauVar3 + 4) = 0;
  *(uint8 *)((int)pauVar3 + 5) = 0;
  *(uint8 *)((int)pauVar3 + 6) = 0;
  *(uint8 *)((int)pauVar3 + 7) = 0;
  *(uint8 *)((int)pauVar3 + 8) = 0;
  *(uint8 *)((int)pauVar3 + 9) = 0;
  mVar13 = (*extraout_RAX->Write)(self_00,in_stack_ffffffffffffff58);
  puStack_10 = mVar13.~r1.data;
  if (mVar13.~r1.tab != (error_itab *)0x0) {
    local_18 = (mVar13.~r1.tab)->_type;
    format.len = 0x43;
    format.str = "[tun2socks][UDP][socks][Connect] Failed to write handshake data: %v";
    v_00.len = 1;
    v_00.array = (interface___ *)&local_18;
    v_00.cap = 1;
    log::log_Printf(format,v_00);
    (*extraout_RAX->Close)(self_00);
    return mVar13.~r1;
  }
  rVar9.data = self_00;
  rVar9.tab = (runtime_itab *)extraout_RAX;
  r = (io_Reader)runtime::runtime_convI2I(&io::io_Reader___Interface_type,rVar9);
  buf.len = 3;
  buf.array = puVar2;
  buf.cap = 0x103;
  mVar13 = io::io_ReadAtLeast(r,buf,3);
  puStack_10 = mVar13.~r1.data;
  if (mVar13.~r1.tab != (error_itab *)0x0) {
    local_18 = (mVar13.~r1.tab)->_type;
    format_00.len = 0x42;
    format_00.str = "[tun2socks][UDP][socks][Connect] Failed to read handshake data: %v";
    v_01.len = 1;
    v_01.array = (interface___ *)&local_18;
    v_01.cap = 1;
    log::log_Printf(format_00,v_01);
    (*extraout_RAX->Close)(self_00);
    return mVar13.~r1;
  }
  if (puVar2[1] != 0) {
    local_18 = &uint8___Uint8_type;
    puStack_10 = &DAT_23951ece0 + (uint)puVar2[1] * 8;
    format_04.len = 0x46;
    format_04.str = "[tun2socks][UDP][socks][Connect] The server refused the connection: %x";
    v_05.len = 1;
    v_05.array = (interface___ *)&local_18;
    v_05.cap = 1;
    log::log_Printf(format_04,v_05);
    (*extraout_RAX->Close)(self_00);
    local_18 = &uint8___Uint8_type;
    puStack_10 = &DAT_23951ece0 + (uint)puVar2[1] * 8;
    format_05.len = 0x46;
    format_05.str = "[tun2socks][UDP][socks][Connect] The server refused the connection: %x";
    a.len = 1;
    a.array = (interface___ *)&local_18;
    a.cap = 1;
    eVar11 = fmt::fmt_Errorf(format_05,a);
    return eVar11;
  }
  i.data = self_00;
  i.tab = (runtime_itab *)extraout_RAX;
  rVar9 = runtime::runtime_convI2I(&io::io_Reader___Interface_type,i);
  _Var12 = runtime::runtime_makeslice(&uint8___Uint8_type,0x103,0x103);
  github.com/aiocloud/shadowsocks/socks::github_com_aiocloud_shadowsocks_socks_readAddr
            (rVar9.tab,rVar9.data,_Var12.array,0x103,0x103);
  if (extraout_RDI_00 != (error_itab *)0x0) {
    local_18 = extraout_RDI_00->_type;
    format_01.len = 0x42;
    format_01.str = "[tun2socks][UDP][socks][Connect] Failed to read remote address: %v";
    v_02.len = 1;
    v_02.array = (interface___ *)&local_18;
    v_02.cap = 1;
    puStack_10 = extraout_RSI;
    log::log_Printf(format_01,v_02);
    (*extraout_RAX->Close)(self_00);
    eVar10.data = extraout_RSI;
    eVar10.tab = extraout_RDI_00;
    return eVar10;
  }
  sVar16 = (string)github.com/aiocloud/shadowsocks/socks::
                   github_com_aiocloud_shadowsocks_socks_Addr_String();
  network.len = 3;
  network.str = "udp";
  mVar14 = net::net_ResolveUDPAddr(network,sVar16);
  puStack_10 = mVar14.~r1.data;
  if (mVar14.~r1.tab != (error_itab *)0x0) {
    local_18 = (mVar14.~r1.tab)->_type;
    format_02.len = 0x45;
    format_02.str = "[tun2socks][UDP][socks][Connect] Failed to resolve remote address: %v";
    v_03.len = 1;
    v_03.array = (interface___ *)&local_18;
    v_03.cap = 1;
    log::log_Printf(format_02,v_03);
    (*extraout_RAX->Close)(self_00);
    return mVar14.~r1;
  }
  local_28 = prVar7;
  local_20 = pvVar6;
  fn = runtime::runtime_newobject
                 (&rixcloud_moe_aiocloud_tun2socks_proxies_socks_struct___F_uintptr___autotmp_33_core_UDPConn___autotmp_24_net_Conn___autotmp_34__socks_udpHandler_____Struct_type
                   .typ);
  fn->_F = (uintptr)rixcloud_moe_aiocloud_tun2socks_proxies_socks___udpHandler__Connect_dwrap_4;
  (fn->_autotmp_33).tab = local_28;
  (fn->_autotmp_33).data = local_20;
  (fn->_autotmp_24).tab = extraout_RAX;
  (fn->_autotmp_24).data = self_00;
  fn->_autotmp_34 = self;
  runtime::runtime_newproc(0,(runtime_funcval *)fn);
  network_00.len = 3;
  network_00.str = "udp";
  mVar15 = net::net_ListenPacket(network_00,(string)ZEXT816(0));
  puStack_10 = mVar15.~r1.data;
  if (mVar15.~r1.tab == (error_itab *)0x0) {
    LOCK();
    bVar8 = (self->Mutex).state == 0;
    if (bVar8) {
      (self->Mutex).state = 1;
    }
    UNLOCK();
    if (!bVar8) {
      sync::sync___Mutex__lockSlow(&self->Mutex);
    }
    local_28 = prVar7;
    local_20 = pvVar6;
    prVar4 = runtime::runtime_newobject
                       (&rixcloud_moe_aiocloud_tun2socks_proxies_socks_udpData___Struct_type.typ);
    (prVar4->StreamConn).tab = extraout_RAX;
    (prVar4->StreamConn).data = self_00;
    (prVar4->PacketConn).tab = mVar15.~r0.tab;
    (prVar4->PacketConn).data = mVar15.~r0.data;
    prVar4->Target = mVar14.~r0;
    puVar5 = runtime::runtime_mapassign
                       (&map_core_UDPConn__socks_udpData___Map_type,self->conns,&local_28);
    *puVar5 = prVar4;
    LOCK();
    sVar1 = (self->Mutex).state;
    (self->Mutex).state = (self->Mutex).state + -1;
    UNLOCK();
    if (sVar1 != 1) {
      sync::sync___Mutex__unlockSlow(&self->Mutex);
    }
    local_28 = prVar7;
    local_20 = pvVar6;
    fn_00 = runtime::runtime_newobject
                      (&rixcloud_moe_aiocloud_tun2socks_proxies_socks_struct___F_uintptr___autotmp_33_core_UDPConn___autotmp_35_net_PacketConn___autotmp_34__socks_udpHandler_____Struct_type
                        .typ);
    fn_00->_F = (uintptr)
                rixcloud_moe_aiocloud_tun2socks_proxies_socks___udpHandler__Connect_dwrap_5;
    (fn_00->_autotmp_33).tab = local_28;
    (fn_00->_autotmp_33).data = local_20;
    (fn_00->_autotmp_35).tab = mVar15.~r0.tab;
    (fn_00->_autotmp_35).data = mVar15.~r0.data;
    fn_00->_autotmp_34 = self;
    runtime::runtime_newproc(0,(runtime_funcval *)fn_00);
    return (error)ZEXT816(0);
  }
  local_18 = (mVar15.~r1.tab)->_type;
  format_03.len = 0x39;
  format_03.str = "[tun2socks][UDP][socks][Connect]Failed to bind socket: %v";
  v_04.len = 1;
  v_04.array = (interface___ *)&local_18;
  v_04.cap = 1;
  log::log_Printf(format_03,v_04);
  (*extraout_RAX->Close)(self_00);
  return mVar15.~r1;
}

