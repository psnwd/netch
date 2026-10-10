// Ghidra 12.1.4 decompile of rixcloud.moe/aiocloud/tun2socks/proxies/socks.(*udpHandler).Close @ 2394ef3a0 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// Golang function info: {@address 239739498 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/proxies/socks/udp.go:171
// Golang stacktrace signature: func
// rixcloud.moe/aiocloud/tun2socks/proxies/socks.(*udpHandler).Close() ???
// Golang signature [params_missing, return_info_missing]: func (*udpHandler) Close(???) ???
// Golang method in type {@address 239585ca0 *socks.udpHandler}

void rixcloud.moe/aiocloud/tun2socks/proxies/socks::
     rixcloud_moe_aiocloud_tun2socks_proxies_socks___udpHandler__Close
               (rixcloud_moe_aiocloud_tun2socks_proxies_socks_udpHandler *self)

{
  int *piVar1;
  undefined8 in_RCX;
  int in_RBX;
  bool bVar2;
  multireturn_unsafe_Pointer_bool_ mVar3;
  rixcloud_moe_aiocloud_tun2socks_proxies_socks_udpHandler *self_spill;
  int iStack0000000000000010;
  undefined8 uStack0000000000000018;
  int local_30;
  undefined8 local_28;
  code *local_20;
  rixcloud_moe_aiocloud_tun2socks_proxies_socks_udpHandler *prStack_18;
  code **local_10;
  
  uStack0000000000000018 = in_RCX;
  iStack0000000000000010 = in_RBX;
  local_10 = (code **)0x0;
  LOCK();
  bVar2 = (self->Mutex).state == 0;
  if (bVar2) {
    (self->Mutex).state = 1;
  }
  UNLOCK();
  if (!bVar2) {
    sync::sync___Mutex__lockSlow(&self->Mutex);
  }
  local_20 = rixcloud_moe_aiocloud_tun2socks_proxies_socks___udpHandler__Close_dwrap_6;
  local_10 = &local_20;
  local_30 = iStack0000000000000010;
  local_28 = uStack0000000000000018;
  prStack_18 = self;
  mVar3 = runtime::runtime_mapaccess2
                    (&map_core_UDPConn__socks_udpData___Map_type,self->conns,&local_30);
  piVar1 = *(int **)mVar3.~r0;
  if (mVar3.~r1 != false) {
    if (*piVar1 != 0) {
      (**(code **)(*piVar1 + 0x18))(piVar1[1]);
    }
    if (piVar1[2] != 0) {
      (**(code **)(piVar1[2] + 0x18))(piVar1[3]);
    }
    local_30 = iStack0000000000000010;
    local_28 = uStack0000000000000018;
    runtime::runtime_mapdelete(&map_core_UDPConn__socks_udpData___Map_type,self->conns,&local_30);
    (**(code **)(iStack0000000000000010 + 0x18))(uStack0000000000000018);
  }
  (**local_10)();
  return;
}

