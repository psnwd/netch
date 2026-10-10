# Recovering `Storage/tun2socks.bin`

Status: **partial**. `main.go.recovered` is reconstructed from decompiled output; the rest is only decompiled, not rewritten.
Nothing here has been compiled or compared with the original binary.

## What the binary is

`tun2socks.bin` is **not** upstream `xjasonlyu/tun2socks` (which `Storage/README.md` used to link). It is a
Windows DLL (`-buildmode=c-shared`, x64) built with **go1.17.1** from the private module
`rixcloud.moe/aiocloud/tun2socks` (AioCloud, the same author line as Netch 1.x). The module host
`rixcloud.moe` no longer resolves and the module is not on the Go proxy or GitHub, so the original source is not
obtainable. `go version -m tun2socks.bin` reads the build info:

| Dependency | Version |
| --- | --- |
| github.com/aiocloud/shadowsocks | v0.0.0-20210731095610-40f201c6eb4a (not on GitHub either) |
| github.com/miekg/dns | v1.1.43 |
| github.com/yl2chen/cidranger | v1.0.2 |
| golang.org/x/sys | v0.0.0-20210917161153-d61c044b1678 |
| golang.zx2c4.com/wireguard | v0.0.0-20210905140043-2ef39d47540c (`tun`, `tun/wintun`) |

## Package map (from Ghidra's Go analysis, 5193 functions)

| Package | Functions | Origin |
| --- | --- | --- |
| `core` | 188 | Fork of `github.com/eycorsican/go-tun2socks/core` (MIT): lwIP via cgo (`_Cfunc_tcp_*`, `NewLWIPStack`, `tcpConn`, `udpConn`). Not decompiled here; start from upstream. lwIP C sources are needed too. |
| `main` | 12 | Netch/AioCloud specific. **Reconstructed in `main.go.recovered`** (not named `.go` so tools do not try to build it). |
| `proxies` | 6 | Netch specific wrappers that apply the bypass ranger and bind address. Decompiled only. |
| `proxies/socks` | 15 | SOCKS5 TCP and UDP handlers. Decompiled only. |
| `adns` | 1 | `adns.Handle`: DNS interception (port 53 is handled separately, see `proxies.udpHandler.ReceiveTo`). Decompiled only. |
| `tools` | 4 | `CopyBuffer`: bidirectional copy, 0x578 byte buffers, deadlines on finish. Decompiled only. |
| `structs` | 0 | `ServerInfo{Type, Hostname, Username, Password, Method, Prot, ProtParam, OBFS, OBFSParam}`. Type only, no functions. |

## The C ABI (matches `Netch/Interops/tun2socks.cs`)

`tun_dial(name, utf8 value)`, `tun_init()`, `tun_free()`, `tun_luid()`, `tun_getUP()`, `tun_getDL()`.
`tun_dial` index order equals `NameList` in the C# file (see the constants in `main.go.recovered`).

Behaviors worth knowing:
- The adapter is a wintun adapter named `aioCloud` with the fixed GUID `{14EA48E9-69FA-409F-8752-22563978E802}`.
- Only SOCKS5 is wired up: server type must be `S5`, `Socks` or `Socks5`, otherwise `tun_init` logs
  `[tun2socks][proxies.NewHandler] Unknown Type` and fails. The Shadowsocks/SSR dependency is linked but unused by `main`.
- Bypass list is a file of CIDRs (`#`-free, one per line) loaded into a cidranger trie, or the literal `disabled`.
- UDP to port 53 is handled in a goroutine through `adns` instead of the SOCKS UDP handler.
- Up/download counters are plain byte totals behind mutexes.

## Not recovered

- `core` fork differences from upstream go-tun2socks (needs a function-by-function diff).
- `proxies`, `proxies/socks`, `adns`, `tools`, `structs`: only raw pseudo-C in `recovered/decompiled/`.
- Exact log strings that are stored in string tables (marked in `main.go.recovered`).
- `main.tun_luid` body: Ghidra only produced the cgo wrapper.
- No build was attempted. Verifying equivalence needs Go 1.17 and a mingw cgo toolchain, then a comparison of
  the exported symbols and the function list against this binary.

## Method

Ghidra 12.1.4 headless + the GhidraMCP server, auto-analysis with the Go analyzer (function names and original
`file.go:line` come from the pclntab). Raw output is in `recovered/decompiled/`, one file per function.

## `Storage/nfdriver.sys` (not recovered, on purpose)

It is the **NetFilter SDK 2 v1.6 WFP driver** by Vitaly Sidorov (PDB path
`...\nfsdk2_1.6\driver_wfp\...\netfilter2.pdb`, x64, file version 1.6.3.0), a commercial closed-source product
whose headers in `Redirector/include/` carry "Copyright (C) Vitaly Sidorov, All rights reserved". Its source
is not available and I did not reconstruct it. The user-mode interface that Netch needs is already documented by the
vendored `nfdriver.h` / `nfapi.h`. Realistic ways forward are to buy or license the SDK, or to replace Process mode with
another capture mechanism (for example WinDivert or your own WFP callout driver).
