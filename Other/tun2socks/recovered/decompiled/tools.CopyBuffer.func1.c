// Ghidra 12.1.4 decompile of rixcloud.moe/aiocloud/tun2socks/tools.CopyBuffer.func1 @ 2394ed560 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// Golang function info: {@address 239738fe8 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/tools/tools.go:23
// Golang stacktrace signature: func rixcloud.moe/aiocloud/tun2socks/tools.CopyBuffer.func1() ???
// Golang signature [params_missing, return_info_missing, closure]: func(.context, ???) ???

error rixcloud.moe/aiocloud/tun2socks/tools::rixcloud_moe_aiocloud_tun2socks_tools_CopyBuffer_func1
                (rixcloud_moe_aiocloud_tun2socks_tools_struct___F_uintptr__client_net_Conn__remote_net_Conn__
                 *_context)

{
  net_Conn_itab *pnVar1;
  net_Conn_itab *pnVar2;
  void *self;
  void *self_00;
  net_Conn i;
  net_Conn i_00;
  io_Writer dst;
  io_Reader src;
  error eVar3;
  __uint8 _Var4;
  __byte buf;
  time_Time in_stack_ffffffffffffff88;
  pnVar1 = (_context->client).tab;
  pnVar2 = (_context->remote).tab;
  self = (_context->client).data;
  i = _context->client;
  self_00 = (_context->remote).data;
  i_00 = _context->remote;
  _Var4 = runtime::runtime_makeslice(&uint8___Uint8_type,0x578,0x578);
  dst = (io_Writer)runtime::runtime_convI2I(&io::io_Writer___Interface_type,(runtime_iface)i);
  src = (io_Reader)runtime::runtime_convI2I(&io::io_Reader___Interface_type,(runtime_iface)i_00);
  buf.len = 0x578;
  buf.array = _Var4.array;
  buf.cap = 0x578;
  io::io_CopyBuffer(dst,src,buf);
  time::time_Now();
  (*pnVar1->SetDeadline)(self,in_stack_ffffffffffffff88);
  time::time_Now();
  eVar3 = (*pnVar2->SetDeadline)(self_00,in_stack_ffffffffffffff88);
  return eVar3;
}

