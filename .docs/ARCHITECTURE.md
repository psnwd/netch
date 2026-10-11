# ARCHITECTURE

Current state (`main` @ 9d99eb1) plus target-state notes for the modernization in [PLAN.md](PLAN.md).

## Components
```
Netrix.exe (WinForms, .NET)              native / external (bin\)
├─ Forms/        UI                     ├─ Redirector.bin + nfapi.dll + nfdriver.sys  process capture (Netfilter)
├─ Controllers/  runtime lifecycle  ──► ├─ tun2socks.bin + wintun.dll                 TUN mode
├─ Servers/      per-protocol           ├─ pcap2socks / Npcap                         Share mode
├─ Models/       Server, Mode, Setting  ├─ aiodns.bin                                 DNS
├─ Interops/     P/Invoke wrappers      ├─ RouteHelper.bin                            route table
├─ Services/ Utils/                     └─ xray / v2ray-sn / ss / ssr / trojan cores  protocol back-ends
└─ Program.cs
```

## Runtime flow
1. `Program.Main`: single-instance guard, set cwd, append `bin\` to PATH, create dirs (`mode\Custom`, `data`, `i18n`, `logging`), load config + modes + i18n, show `MainForm`.
2. User picks Server + Mode; `MainController` starts:
   1. Server's `IServerUtil.GetController()` returns an `IServerController` that starts the protocol core and exposes a local SOCKS5 (`Socks5Server`).
   2. Mode controller (`IModeController`: `NFController`, `TUNController`, `PcapController`) starts with that SOCKS5 + the `Mode`.
   3. `DNSController` (aiodns) starts as needed; `Guard` watches child processes.
3. Stop reverses order via `IController.StopAsync`.

## Extension points
- **Protocol**: `Servers/<Name>/` = `*Server` (model) + `*Util : IServerUtil` (URI scheme, share link, parse, check) + `*Form` + optional `*Controller : IServerController`. Polymorphic JSON via `ServerConverterWithTypeDiscriminator`.
- **Mode**: `Models/Modes/*` + `ModeConverterWithTypeDiscriminator`; behavior in matching `IModeController`.
- **Native call**: declare in `NativeMethods.txt` (CsWin32) or hand-write in `Interops/`.

## Persistence
`data\` (settings, servers, subscriptions JSON), `mode\` (mode files), `i18n\`, `logging\` (Serilog). Formats must stay backward compatible.

## Build graph
`build.ps1` runs `Other/build.ps1` (Go, MSYS2 C++, Rust builds), then `dotnet publish Netrix` (win-x64, single-file, self-contained), then `msbuild Redirector`, then `msbuild RouteHelper`, then assembles `release\` (+ `Storage\` assets, GeoIP mmdb download).

## Target-state changes
- TFM `net10.0-windows`; `Tests` same TFM (currently `net5.0`).
- CsWin32 on stable release; adjust `NativeMethods.txt` if APIs renamed.
- Keep `System.Text.Json` converters; no source-gen unless trimming is enabled (not planned).
- No structural refactor: controller/server interfaces stay as is.
- Binary provenance table (name, version, source URL, sha256) kept in `Storage/README.md`.
