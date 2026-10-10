// Ghidra 12.1.4 decompile of rixcloud.moe/aiocloud/tun2socks/proxies/socks.(*udpHandler).ReceiveTo @ 2394eefe0 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// Golang function info: {@address 239739430 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/proxies/socks/udp.go:143
// Golang stacktrace signature: func
// rixcloud.moe/aiocloud/tun2socks/proxies/socks.(*udpHandler).ReceiveTo() ???
// Golang signature [from_rtti_method]: func (*udpHandler) ReceiveTo(core.UDPConn, []uint8,
// *net.UDPAddr) error
// Golang method in type {@address 239585ca0 *socks.udpHandler}

error rixcloud.moe/aiocloud/tun2socks/proxies/socks::
      rixcloud_moe_aiocloud_tun2socks_proxies_socks___udpHandler__ReceiveTo
                (rixcloud_moe_aiocloud_tun2socks_proxies_socks_udpHandler *self,
                rixcloud_moe_aiocloud_tun2socks_core_UDPConn param_2,__uint8 param_3,
                net_UDPAddr *param_4)

{
  uint len;
  uintptr y;
  sdword sVar1;
  int iVar2;
  uint8 *puVar3;
  undefined **extraout_RCX;
  uint x;
  uintptr uVar4;
  uintptr extraout_RBX;
  uintptr n;
  int extraout_RBX_00;
  uintptr n_00;
  bool bVar5;
  string sVar6;
  undefined1 auVar7 [16];
  error eVar8;
  error eVar9;
  __uint8 _Var10;
  __interface___ v;
  multireturn_unsafe_Pointer_bool_ mVar11;
  __interface___ v_00;
  rixcloud_moe_aiocloud_tun2socks_proxies_socks_udpHandler *self_spill;
  rixcloud_moe_aiocloud_tun2socks_core_UDPConn param_2_spill;
  __uint8 param_3_spill;
  net_UDPAddr *param_4_spill;
  runtime__type *local_28;
  undefined **ppuStack_20;
  rixcloud_moe_aiocloud_tun2socks_core_UDPConn_itab *local_18;
  unsafe_Pointer local_10;
  
  n_00 = param_3.len;
  LOCK();
  bVar5 = (self->Mutex).state == 0;
  if (bVar5) {
    (self->Mutex).state = 1;
  }
  UNLOCK();
  if (!bVar5) {
    sync::sync___Mutex__lockSlow(&self->Mutex);
  }
  local_18 = param_2.tab;
  local_10 = param_2.data;
  mVar11 = runtime::runtime_mapaccess2
                     (&map_core_UDPConn__socks_udpData___Map_type,self->conns,&local_18);
  iVar2 = *(int *)mVar11.~r0;
  LOCK();
  sVar1 = (self->Mutex).state;
  (self->Mutex).state = (self->Mutex).state + -1;
  UNLOCK();
  if (sVar1 != 1) {
    sync::sync___Mutex__unlockSlow(&self->Mutex);
  }
  if (mVar11.~r1 == false) {
    rixcloud_moe_aiocloud_tun2socks_proxies_socks___udpHandler__Close(self);
    local_28 = &string___String_type;
    ppuStack_20 = &PTR_DAT_2395f1200;
    v.len = 1;
    v.array = (interface___ *)&local_28;
    v.cap = 1;
    log::log_Println(v);
    eVar9.data = DAT_239743af8;
    eVar9.tab = DAT_239743af0;
    return eVar9;
  }
  sVar6 = net::net___UDPAddr__String(param_4);
  auVar7 = github.com/aiocloud/shadowsocks/socks::github_com_aiocloud_shadowsocks_socks_ParseAddr
                     (sVar6.str,sVar6.len);
  uVar4 = auVar7._8_8_;
  len = uVar4 + n_00 + 3;
  _Var10 = runtime::runtime_makeslice(&uint8___Uint8_type,len,len);
  puVar3 = _Var10.array;
  y = uVar4 + n_00;
  if (len < 3) {
                    // WARNING: Subroutine does not return
    runtime::runtime_panicSliceB(3,y);
  }
  n = y;
  if ((int)uVar4 < (int)y) {
    n = uVar4;
  }
  if (auVar7._0_8_ != puVar3 + ((int)-y >> 0x3f & 3)) {
    runtime::runtime_memmove(puVar3 + ((int)-y >> 0x3f & 3),auVar7._0_8_,n);
    n = extraout_RBX;
  }
  x = uVar4 + 3;
  if (len < x) {
                    // WARNING: Subroutine does not return
    runtime::runtime_panicSliceB(x,n);
  }
  if (param_3.array != puVar3 + ((int)-n_00 >> 0x3f & x)) {
    runtime::runtime_memmove(puVar3 + ((int)-n_00 >> 0x3f & x),param_3.array,n_00);
  }
  (**(code **)(*(int *)(iVar2 + 0x10) + 0x48))
            (*(undefined8 *)(iVar2 + 0x18),puVar3,len,len,
             &net::_net_UDPAddr__implements__net_Addr__itab,*(undefined8 *)(iVar2 + 0x20));
  if (extraout_RBX_00 == 0) {
    return (error)ZEXT816(0);
  }
  rixcloud_moe_aiocloud_tun2socks_proxies_socks___udpHandler__Close(self);
  if (extraout_RBX_00 == 0) {
    local_28 = (runtime__type *)0x0;
  }
  else {
    local_28 = *(runtime__type **)(extraout_RBX_00 + 8);
  }
  sVar6.len = 0x3a;
  sVar6.str = "[tun2socks][UDP][socks][ReceiveTo] Failed to send data: %v";
  v_00.len = 1;
  v_00.array = (interface___ *)&local_28;
  v_00.cap = 1;
  ppuStack_20 = extraout_RCX;
  log::log_Printf(sVar6,v_00);
  eVar8.data = DAT_239743af8;
  eVar8.tab = DAT_239743af0;
  return eVar8;
}

