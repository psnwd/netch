// Ghidra 12.1.4 decompile of main.tun_dial @ 2394efe20 (Storage/tun2socks.bin)
// RAW OUTPUT, NOT SOURCE. Pseudo-C of Go code.
// WARNING: Globals starting with '_' overlap smaller symbols at the same address
// Golang function info: {@address 239739918 "Flags: []"}
// Golang source: rixcloud.moe/aiocloud/tun2socks/main.go:73
// Golang stacktrace signature: func main.tun_dial() ???

undefined8 main::main_tun_dial(int param_1,uint8 *param_2)

{
  string sVar1;
  string sVar2;
  int iVar3;
  string *psVar4;
  string sVar5;
  string sVar6;
  string sVar7;
  interface___ e;
  __string _Var8;
  multireturn_int_error_ mVar9;
  string sep;
  if (param_1 < 0xc) {
    if (param_1 < 6) {
      if (param_1 < 3) {
        if (param_1 == 0) {
          sVar7 = runtime::runtime_gostring(param_2);
          sVar6.len = DAT_23953ce18;
          sVar6.str = PTR_DAT_23953ce10;
          sVar5.len = DAT_23953cde8;
          sVar5.str = PTR_DAT_23953cde0;
          iVar3 = DAT_23950e178;
        }
        else if (param_1 == 1) {
          sVar5 = runtime::runtime_gostring(param_2);
          sVar6.len = DAT_23953ce18;
          sVar6.str = PTR_DAT_23953ce10;
          sVar7.len = DAT_23953cdd8;
          sVar7.str = PTR_DAT_23953cdd0;
          iVar3 = DAT_23950e178;
        }
        else {
          if (param_1 != 2) {
            return 0;
          }
          sVar5 = runtime::runtime_gostring(param_2);
          sep.len = 3;
          sep.str = "://";
          _Var8 = strings::strings_genSplit(sVar5,sep,0,2);
          sVar7.len = DAT_23953cdd8;
          sVar7.str = PTR_DAT_23953cdd0;
          sVar2.len = DAT_23953cdd8;
          sVar2.str = PTR_DAT_23953cdd0;
          sVar5.len = DAT_23953cde8;
          sVar5.str = PTR_DAT_23953cde0;
          sVar1.len = DAT_23953cde8;
          sVar1.str = PTR_DAT_23953cde0;
          psVar4 = _Var8.array;
          if (_Var8.len == 2) {
            sVar6 = psVar4[1];
            iVar3 = DAT_23950e178;
            if (((psVar4->len == 3) && (*(short *)psVar4->str == 0x6c74)) &&
               ((char)*(short *)((int)psVar4->str + 2) == 's')) {
              _DAT_239542088 = 7;
              PTR_s_tcp_239542080 = &DAT_23959b1c9;
              sVar5 = sVar1;
              sVar7 = sVar2;
              sVar6 = psVar4[1];
            }
            else {
              _DAT_239542088 = 3;
              PTR_s_tcp_239542080 = s_tcp_23959a6fe;
            }
          }
          else {
            sVar6 = runtime::runtime_gostring(param_2);
            sVar7.len = DAT_23953cdd8;
            sVar7.str = PTR_DAT_23953cdd0;
            sVar5.len = DAT_23953cde8;
            sVar5.str = PTR_DAT_23953cde0;
            iVar3 = DAT_23950e178;
          }
        }
      }
      else if (param_1 == 3) {
        sVar5 = runtime::runtime_gostring(param_2);
        mVar9 = strconv::strconv_Atoi(sVar5);
        sVar6.len = DAT_23953ce18;
        sVar6.str = PTR_DAT_23953ce10;
        sVar7.len = DAT_23953cdd8;
        sVar7.str = PTR_DAT_23953cdd0;
        sVar5.len = DAT_23953cde8;
        sVar5.str = PTR_DAT_23953cde0;
        iVar3 = mVar9.~r0;
        if (mVar9.~r1.tab != (error_itab *)0x0) {
          e.data = mVar9.~r1.data;
          e.tab = (interface____itab *)(mVar9.~r1.tab)->_type;
                    // WARNING: Subroutine does not return
          runtime::runtime_gopanic(e);
        }
      }
      else if (param_1 == 4) {
        DAT_239743008 =
             runtime::runtime_newobject
                       (&rixcloud.moe/aiocloud/tun2socks/structs::
                         rixcloud_moe_aiocloud_tun2socks_structs_ServerInfo___Struct_type.typ);
        sVar6.len = DAT_23953ce18;
        sVar6.str = PTR_DAT_23953ce10;
        sVar7.len = DAT_23953cdd8;
        sVar7.str = PTR_DAT_23953cdd0;
        sVar5.len = DAT_23953cde8;
        sVar5.str = PTR_DAT_23953cde0;
        iVar3 = DAT_23950e178;
      }
      else {
        sVar5 = runtime::runtime_gostring(param_2);
        DAT_239743008->Type = sVar5;
        sVar6.len = DAT_23953ce18;
        sVar6.str = PTR_DAT_23953ce10;
        sVar7.len = DAT_23953cdd8;
        sVar7.str = PTR_DAT_23953cdd0;
        sVar5.len = DAT_23953cde8;
        sVar5.str = PTR_DAT_23953cde0;
        iVar3 = DAT_23950e178;
      }
    }
    else if (param_1 < 9) {
      if (param_1 == 6) {
        sVar5 = runtime::runtime_gostring(param_2);
        DAT_239743008->Hostname = sVar5;
        sVar6.len = DAT_23953ce18;
        sVar6.str = PTR_DAT_23953ce10;
        sVar7.len = DAT_23953cdd8;
        sVar7.str = PTR_DAT_23953cdd0;
        sVar5.len = DAT_23953cde8;
        sVar5.str = PTR_DAT_23953cde0;
        iVar3 = DAT_23950e178;
      }
      else if (param_1 == 7) {
        sVar5 = runtime::runtime_gostring(param_2);
        DAT_239743008->Username = sVar5;
        sVar6.len = DAT_23953ce18;
        sVar6.str = PTR_DAT_23953ce10;
        sVar7.len = DAT_23953cdd8;
        sVar7.str = PTR_DAT_23953cdd0;
        sVar5.len = DAT_23953cde8;
        sVar5.str = PTR_DAT_23953cde0;
        iVar3 = DAT_23950e178;
      }
      else {
        sVar5 = runtime::runtime_gostring(param_2);
        DAT_239743008->Password = sVar5;
        sVar6.len = DAT_23953ce18;
        sVar6.str = PTR_DAT_23953ce10;
        sVar7.len = DAT_23953cdd8;
        sVar7.str = PTR_DAT_23953cdd0;
        sVar5.len = DAT_23953cde8;
        sVar5.str = PTR_DAT_23953cde0;
        iVar3 = DAT_23950e178;
      }
    }
    else if (param_1 == 9) {
      sVar5 = runtime::runtime_gostring(param_2);
      DAT_239743008->Method = sVar5;
      sVar6.len = DAT_23953ce18;
      sVar6.str = PTR_DAT_23953ce10;
      sVar7.len = DAT_23953cdd8;
      sVar7.str = PTR_DAT_23953cdd0;
      sVar5.len = DAT_23953cde8;
      sVar5.str = PTR_DAT_23953cde0;
      iVar3 = DAT_23950e178;
    }
    else if (param_1 == 10) {
      sVar5 = runtime::runtime_gostring(param_2);
      DAT_239743008->Prot = sVar5;
      sVar6.len = DAT_23953ce18;
      sVar6.str = PTR_DAT_23953ce10;
      sVar7.len = DAT_23953cdd8;
      sVar7.str = PTR_DAT_23953cdd0;
      sVar5.len = DAT_23953cde8;
      sVar5.str = PTR_DAT_23953cde0;
      iVar3 = DAT_23950e178;
    }
    else {
      sVar5 = runtime::runtime_gostring(param_2);
      DAT_239743008->ProtParam = sVar5;
      sVar6.len = DAT_23953ce18;
      sVar6.str = PTR_DAT_23953ce10;
      sVar7.len = DAT_23953cdd8;
      sVar7.str = PTR_DAT_23953cdd0;
      sVar5.len = DAT_23953cde8;
      sVar5.str = PTR_DAT_23953cde0;
      iVar3 = DAT_23950e178;
    }
  }
  else if (param_1 < 0x12) {
    if (param_1 < 0xf) {
      if (param_1 == 0xc) {
        sVar5 = runtime::runtime_gostring(param_2);
        DAT_239743008->OBFS = sVar5;
        sVar6.len = DAT_23953ce18;
        sVar6.str = PTR_DAT_23953ce10;
        sVar7.len = DAT_23953cdd8;
        sVar7.str = PTR_DAT_23953cdd0;
        sVar5.len = DAT_23953cde8;
        sVar5.str = PTR_DAT_23953cde0;
        iVar3 = DAT_23950e178;
      }
      else if (param_1 == 0xd) {
        sVar5 = runtime::runtime_gostring(param_2);
        DAT_239743008->OBFSParam = sVar5;
        sVar6.len = DAT_23953ce18;
        sVar6.str = PTR_DAT_23953ce10;
        sVar7.len = DAT_23953cdd8;
        sVar7.str = PTR_DAT_23953cdd0;
        sVar5.len = DAT_23953cde8;
        sVar5.str = PTR_DAT_23953cde0;
        iVar3 = DAT_23950e178;
      }
      else {
        DAT_239743010 =
             runtime::runtime_newobject
                       (&rixcloud.moe/aiocloud/tun2socks/structs::
                         rixcloud_moe_aiocloud_tun2socks_structs_ServerInfo___Struct_type.typ);
        sVar6.len = DAT_23953ce18;
        sVar6.str = PTR_DAT_23953ce10;
        sVar7.len = DAT_23953cdd8;
        sVar7.str = PTR_DAT_23953cdd0;
        sVar5.len = DAT_23953cde8;
        sVar5.str = PTR_DAT_23953cde0;
        iVar3 = DAT_23950e178;
      }
    }
    else if (param_1 == 0xf) {
      sVar5 = runtime::runtime_gostring(param_2);
      DAT_239743010->Type = sVar5;
      sVar6.len = DAT_23953ce18;
      sVar6.str = PTR_DAT_23953ce10;
      sVar7.len = DAT_23953cdd8;
      sVar7.str = PTR_DAT_23953cdd0;
      sVar5.len = DAT_23953cde8;
      sVar5.str = PTR_DAT_23953cde0;
      iVar3 = DAT_23950e178;
    }
    else if (param_1 == 0x10) {
      sVar5 = runtime::runtime_gostring(param_2);
      DAT_239743010->Hostname = sVar5;
      sVar6.len = DAT_23953ce18;
      sVar6.str = PTR_DAT_23953ce10;
      sVar7.len = DAT_23953cdd8;
      sVar7.str = PTR_DAT_23953cdd0;
      sVar5.len = DAT_23953cde8;
      sVar5.str = PTR_DAT_23953cde0;
      iVar3 = DAT_23950e178;
    }
    else {
      sVar5 = runtime::runtime_gostring(param_2);
      DAT_239743010->Username = sVar5;
      sVar6.len = DAT_23953ce18;
      sVar6.str = PTR_DAT_23953ce10;
      sVar7.len = DAT_23953cdd8;
      sVar7.str = PTR_DAT_23953cdd0;
      sVar5.len = DAT_23953cde8;
      sVar5.str = PTR_DAT_23953cde0;
      iVar3 = DAT_23950e178;
    }
  }
  else if (param_1 < 0x15) {
    if (param_1 == 0x12) {
      sVar5 = runtime::runtime_gostring(param_2);
      DAT_239743010->Password = sVar5;
      sVar6.len = DAT_23953ce18;
      sVar6.str = PTR_DAT_23953ce10;
      sVar7.len = DAT_23953cdd8;
      sVar7.str = PTR_DAT_23953cdd0;
      sVar5.len = DAT_23953cde8;
      sVar5.str = PTR_DAT_23953cde0;
      iVar3 = DAT_23950e178;
    }
    else if (param_1 == 0x13) {
      sVar5 = runtime::runtime_gostring(param_2);
      DAT_239743010->Method = sVar5;
      sVar6.len = DAT_23953ce18;
      sVar6.str = PTR_DAT_23953ce10;
      sVar7.len = DAT_23953cdd8;
      sVar7.str = PTR_DAT_23953cdd0;
      sVar5.len = DAT_23953cde8;
      sVar5.str = PTR_DAT_23953cde0;
      iVar3 = DAT_23950e178;
    }
    else {
      sVar5 = runtime::runtime_gostring(param_2);
      DAT_239743010->Prot = sVar5;
      sVar6.len = DAT_23953ce18;
      sVar6.str = PTR_DAT_23953ce10;
      sVar7.len = DAT_23953cdd8;
      sVar7.str = PTR_DAT_23953cdd0;
      sVar5.len = DAT_23953cde8;
      sVar5.str = PTR_DAT_23953cde0;
      iVar3 = DAT_23950e178;
    }
  }
  else if (param_1 == 0x15) {
    sVar5 = runtime::runtime_gostring(param_2);
    DAT_239743010->ProtParam = sVar5;
    sVar6.len = DAT_23953ce18;
    sVar6.str = PTR_DAT_23953ce10;
    sVar7.len = DAT_23953cdd8;
    sVar7.str = PTR_DAT_23953cdd0;
    sVar5.len = DAT_23953cde8;
    sVar5.str = PTR_DAT_23953cde0;
    iVar3 = DAT_23950e178;
  }
  else if (param_1 == 0x16) {
    sVar5 = runtime::runtime_gostring(param_2);
    DAT_239743010->OBFS = sVar5;
    sVar6.len = DAT_23953ce18;
    sVar6.str = PTR_DAT_23953ce10;
    sVar7.len = DAT_23953cdd8;
    sVar7.str = PTR_DAT_23953cdd0;
    sVar5.len = DAT_23953cde8;
    sVar5.str = PTR_DAT_23953cde0;
    iVar3 = DAT_23950e178;
  }
  else {
    if (param_1 != 0x17) {
      return 0;
    }
    sVar5 = runtime::runtime_gostring(param_2);
    DAT_239743010->OBFSParam = sVar5;
    sVar6.len = DAT_23953ce18;
    sVar6.str = PTR_DAT_23953ce10;
    sVar7.len = DAT_23953cdd8;
    sVar7.str = PTR_DAT_23953cdd0;
    sVar5.len = DAT_23953cde8;
    sVar5.str = PTR_DAT_23953cde0;
    iVar3 = DAT_23950e178;
  }
  DAT_23950e178 = iVar3;
  DAT_23953ce18 = sVar6.len;
  PTR_DAT_23953ce10 = sVar6.str;
  DAT_23953cdd8 = sVar7.len;
  PTR_DAT_23953cdd0 = sVar7.str;
  DAT_23953cde8 = sVar5.len;
  PTR_DAT_23953cde0 = sVar5.str;
  return 1;
}

