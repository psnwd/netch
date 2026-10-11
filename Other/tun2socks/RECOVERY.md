# tun2socks (rebuilt from `Storage/tun2socks.bin`)

`Storage/tun2socks.bin` is a closed-source DLL that Netrix loads for TUN mode. Its source is gone, so this directory is a
**rewrite recovered from the binary** (Ghidra decompilation plus the Go function tables). It builds, passes its tests and
matches the original function for function, but it has **not** been run inside Netrix (that needs admin and `wintun.dll`).

## What the binary is

Not upstream `xjasonlyu/tun2socks`. It is a Windows DLL (`-buildmode=c-shared`, x64) built with go1.17.1 from the private
module `rixcloud.moe/aiocloud/tun2socks` (AioCloud). The module host and `github.com/aiocloud/shadowsocks` no longer
resolve anywhere. `go version -m` shows the build info the rewrite pins:

| Dependency | Version |
| --- | --- |
| github.com/miekg/dns | v1.1.43 |
| github.com/yl2chen/cidranger | v1.0.2 |
| golang.org/x/sys | v0.0.0-20210917161153-d61c044b1678 |
| golang.zx2c4.com/wireguard | v0.0.0-20210905140043-2ef39d47540c |
| github.com/aiocloud/shadowsocks (socks addr helpers) | replaced by `github.com/shadowsocks/go-shadowsocks2` v0.1.5 `socks` (same helpers) |

## Layout

| Path | Origin |
| --- | --- |
| `main.go` | Recovered. The C ABI Netrix calls (`tun_dial`, `tun_init`, `tun_free`, `tun_luid`, `tun_getUP`, `tun_getDL`). |
| `proxies/` | Recovered. TCP: IPv4 targets in the bypass list are dialed directly from the bind address, everything else goes through SOCKS5. UDP: port 53 goes to `adns`. |
| `proxies/socks/` | Recovered. SOCKS5 CONNECT and UDP ASSOCIATE. |
| `adns/` | Recovered. PTR queries are answered with `aioCloud`, other queries go to the configured DNS server over TCP or TCP-TLS. |
| `tools/`, `structs/` | Recovered. |
| `core/` | `github.com/eycorsican/go-tun2socks/core` (MIT, license in `core/LICENSE`) with lwIP, **plus one fix**: `lwipError.Error()` formats the code with `fmt.Sprintf("error code %d", ...)` as the binary does (upstream uses `string(int)`). Every other `core` function matches the binary's size. |
| `recovered/decompiled/` | Raw Ghidra pseudo-C that the rewrite was written from. Evidence only, not built. |

Recovered defaults (from the data section): bind address `10.0.0.100`, bypass list `disabled`, DNS `1.1.1.1:53` over `tcp`, MTU 1500.
Other constants: wintun adapter `aioCloud` with GUID `{14EA48E9-69FA-409F-8752-22563978E802}`; direct dials time out and
keep alive after 10 s; SOCKS UDP relay replies are dropped after 30 s of silence; relays copy in 0x578 byte chunks.
Only SOCKS5 server types (`S5`, `Socks`, `Socks5`) are supported.

## How it was checked

Built with the same toolchain and flags as the original (`go1.17.1`, `-buildmode=c-shared -trimpath -asmflags '-s -w'
-ldflags '-s -w'`, mingw gcc 15.2 for the lwIP C code):

```
go install golang.org/dl/go1.17.1@latest && go1.17.1 download
set CGO_ENABLED=1 GOOS=windows GOARCH=amd64
go1.17.1 build -a -buildmode=c-shared -trimpath -asmflags "-s -w" -ldflags "-s -w" -o tun2socks.dll .
```

| Check | Result |
| --- | --- |
| DLL size | 4,680,192 bytes (original 4,676,608) |
| Exports | the same six `tun_*` functions, same C types (`GoInt`, `GoUint8*`, `GoUint8`, `GoUint64`) |
| Functions in the Go function table | 4,547 in common; **4,541 have exactly the same machine code size** |
| Functions that differ | `main.tun_dial`, `main.tun_init`, `main.tun_free` (written more compactly), `adns.Handle` (shared helper instead of duplicated code), `socks.(*tcpHandler).Handle`, `socks.(*udpHandler).Connect` (a few bytes; logic reviewed against the decompile) |
| C ABI | A ctypes script calls every export on both DLLs with the same sequence (dial with every field, unknown index, getters, free before init, init without wintun). The outputs are identical. |
| Unit tests | Go tests with fake SOCKS5 servers and a fake DNS upstream: handshake (no auth, user/pass, auth failure, unknown method), TCP relay, UDP associate round trip, bypass versus proxy routing, IPv6 never bypassed, PTR answer, upstream DNS over TCP |

## Not verified

- `tun_init` with a real adapter (needs administrator rights and `wintun.dll`), so no traffic has gone through the rewrite.
- The six functions listed above differ in machine code. Behaviour was compared by reading the decompile and by tests, not by running the
  original side by side.
- Differences from the original that are deliberate: `tun_dial` returns false for a server field set before a `TYPE_TCPREST`/`TYPE_UDPREST`
  (the original would crash with a nil pointer), and `tun_init` returns false if the servers were never described.
- The `aiocloud/shadowsocks/socks` helpers were replaced by the go-shadowsocks2 equivalents; same wire format, not compared byte for byte.
- The exact text of three rarely seen log lines was taken from the data section; the SOCKS auth failure message is `authentication failed`.

**Wired in:** `Other/tun2socks/build.ps1` builds the module with Go 1.17.1 into `Other\release\tun2socks.bin`. `Other/build.ps1` runs it with the other `Other/*` tools, and the root `build.ps1` copies `Other\release\*.bin` over the `Storage\tun2socks.bin` it copied first, so the release now ships the rebuilt DLL. CI installs Go 1.17.1 for it (`GO117`). `Storage/tun2socks.bin` stays as the original reference and as a fallback when `Other\release` has no `tun2socks.bin`. To go back to the original, delete `Other/tun2socks/build.ps1`.

## `Storage/nfdriver.sys` (not recovered, on purpose)

It is the **NetFilter SDK 2 v1.6 WFP driver** by Vitaly Sidorov (PDB path `...\nfsdk2_1.6\driver_wfp\...\netfilter2.pdb`, x64,
file version 1.6.3.0), a commercial closed-source product. The headers in `Redirector/include/` carry "Copyright (C) Vitaly Sidorov,
All rights reserved". Its source is not available and I did not reconstruct it. The user-mode interface Netrix needs is already
documented by the vendored `nfdriver.h` and `nfapi.h`. Realistic ways forward: license the SDK, or replace Process mode with another
capture mechanism (for example WinDivert or your own WFP callout driver).
