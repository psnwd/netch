<p align="center"><img src="Netrix/Resources/Netrix.png?raw=true" width="128" /></p>

<div align="center">

# Netrix
A simple proxy client for Windows
</div>

> **Netrix is a modernised fork of [Netch](https://github.com/NetchX/Netch) (1.x) by AmazingDM and contributors.**
> It keeps the original features and GPLv3 license, and moves the project to current tooling: .NET 10 (x64), rebuildable native components
> (tun2socks, aiodns, pcap2socks, v2ray-sn, wintun), a safer update path, tests, and performance and telemetry work.
> See [.docs/PLAN.md](.docs/PLAN.md) and [.docs/TASK.md](.docs/TASK.md) for what changed and what is still open.

## Features
Some features may not be implemented in version 1

### Modes
- `ProcessMode` - Use Netfilter driver to intercept process traffic
- `ShareMode` - Share your network based on WinPcap / Npcap
- `TunMode` - Use WinTUN driver to create virtual adapter
- `WebMode` - Set the Windows system proxy (SOCKS5); only apps that honour it are proxied. Restored on stop

### Protocols
- [`Socks5`](https://www.wikiwand.com/en/SOCKS)
- [`Shadowsocks`](https://shadowsocks.org)
- [`ShadowsocksR`](https://github.com/shadowsocksrr/shadowsocksr-libev)
- [`WireGuard`](https://www.wireguard.com)
- [`Trojan`](https://trojan-gfw.github.io/trojan)
- [`VMess`](https://www.v2fly.org)
- [`VLESS`](https://xtls.github.io)

### Others
- UDP NAT FullCone (Limited by your server)
- .NET 10 x64 (Windows)

## Build
```powershell
.\build.ps1        # full build into .\.build (keeps .build\data)
```
See [.docs/CLAUDE.md](.docs/CLAUDE.md) for toolchain notes (Go 1.17.1 / 1.18.10, msbuild, `-PlatformToolset v145`).

## Credits
Netrix is based on [Netch](https://github.com/NetchX/Netch), Copyright (c) 2019 AmazingDM and Contributors. Third-party libraries are listed in [Netrix/NOTICE.txt](Netrix/NOTICE.txt).

## License
Netrix is licensed under the [GPLv3](LICENSE) license
