// Ghidra 12.1.4 decompile of rixcloud.moe/aiocloud/tun2socks/tools.CopyBuffer @ 2394ed2c0 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// Golang function info: {@address 239738f80 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/tools/tools.go:19
// Golang stacktrace signature: func rixcloud.moe/aiocloud/tun2socks/tools.CopyBuffer() ???

void rixcloud.moe/aiocloud/tun2socks/tools::rixcloud_moe_aiocloud_tun2socks_tools_CopyBuffer
               (net_Conn_itab *param_1,unsafe_Pointer param_2,net_Conn_itab *param_3,
               unsafe_Pointer param_4)

{
  rixcloud_moe_aiocloud_tun2socks_tools_struct___F_uintptr__client_net_Conn__remote_net_Conn__ *fn;
  io_Writer dst;
  io_Reader src;
  error eVar1;
  __uint8 _Var2;
  runtime_iface i;
  runtime_iface i_00;
  __byte buf;
  net_Conn_itab *pnStack0000000000000008;
  unsafe_Pointer pvStack0000000000000010;
  net_Conn_itab *pnStack0000000000000018;
  unsafe_Pointer pvStack0000000000000020;
  time_Time in_stack_ffffffffffffff50;
  code *local_48;
  net_Conn_itab *pnStack_40;
  unsafe_Pointer local_38;
  code *local_30;
  net_Conn_itab *pnStack_28;
  unsafe_Pointer local_20;
  code **local_18;
  code **ppcStack_10;
  
  pvStack0000000000000010 = param_2;
  pvStack0000000000000020 = param_4;
  while (pnStack0000000000000008 = param_1, pnStack0000000000000018 = param_3,
        &local_30 <= CURRENT_G.stackguard0) {
    runtime::runtime_morestack_noctxt();
    param_1 = pnStack0000000000000008;
    param_3 = pnStack0000000000000018;
  }
  local_30 = rixcloud_moe_aiocloud_tun2socks_tools_CopyBuffer_dwrap_1;
  ppcStack_10 = &local_30;
  local_48 = rixcloud_moe_aiocloud_tun2socks_tools_CopyBuffer_dwrap_2;
  local_18 = &local_48;
  pnStack_40 = param_3;
  local_38 = pvStack0000000000000020;
  pnStack_28 = param_1;
  local_20 = pvStack0000000000000010;
  fn = runtime::runtime_newobject
                 (&rixcloud_moe_aiocloud_tun2socks_tools_struct___F_uintptr__client_net_Conn__remote_net_Conn_____Struct_type
                   .typ);
  fn->_F = (uintptr)rixcloud_moe_aiocloud_tun2socks_tools_CopyBuffer_func1;
  (fn->client).tab = param_1;
  (fn->client).data = pvStack0000000000000010;
  (fn->remote).tab = param_3;
  (fn->remote).data = pvStack0000000000000020;
  runtime::runtime_newproc(0,(runtime_funcval *)fn);
  _Var2 = runtime::runtime_makeslice(&uint8___Uint8_type,0x578,0x578);
  i.data = pvStack0000000000000020;
  i.tab = (runtime_itab *)param_3;
  dst = (io_Writer)runtime::runtime_convI2I(&io::io_Writer___Interface_type,i);
  i_00.data = pvStack0000000000000010;
  i_00.tab = (runtime_itab *)param_1;
  src = (io_Reader)runtime::runtime_convI2I(&io::io_Reader___Interface_type,i_00);
  buf.len = 0x578;
  buf.array = _Var2.array;
  buf.cap = 0x578;
  io::io_CopyBuffer(dst,src,buf);
  time::time_Now();
  (*param_1->SetDeadline)(pvStack0000000000000010,in_stack_ffffffffffffff50);
  time::time_Now();
  eVar1 = (*param_3->SetDeadline)(pvStack0000000000000020,in_stack_ffffffffffffff50);
  (**local_18)(eVar1.tab,eVar1.data);
  (**ppcStack_10)();
  return;
}

