# Changelog

All notable changes to Netrix. The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [2.0.0] - 2026-10-11

Netrix is a fork of [Netch](https://github.com/NetchX/Netch) 1.9.7 by AmazingDM and contributors. Version 2.0.0 moves the project to .NET 10, rebuilds the bundled helper programs from source, and adds a new name, logo, documentation and tests.

### Added
- **Web mode**: sets the Windows system proxy and restores the old values when you stop, even after a crash.
- **IPv6 in TUN mode** (`ProxyIPv6`, on by default). IPv6 now goes through the tunnel instead of bypassing it. If the server has no working IPv6 path, Netrix detects it at start and rejects IPv6 connections so apps fall back to IPv4 at once.
- **Download and upload speed** shown separately in TUN mode, read from the tunnel itself.
- **Built-in Help window** (Help > Documentation) with topics for servers, modes, skipping apps and IPs, the status bar, files and troubleshooting.
- **Tooltips** on the main window, Settings, the mode editors and the Subscription window.
- **Signed updates**: releases are signed with an ECDSA P-256 key (`sign.ps1`); the app refuses an unsigned or badly signed update.
- **Global Bypass Apps** sample mode: proxy everything except a list of apps.
- **DNS response cache** in aiodns and the tun2socks DNS handler.
- **Benchmark script** `.docs/bench.ps1` and results in `.docs/PERF.md`.
- **Tests**: 41 unit tests (share links, mode and settings JSON, controllers, updater, signatures, IPv6 probe, update checker).
- Keyboard shortcuts: Enter saves Settings, Esc closes Help and About.
- `.\build.ps1 -Release` builds into `.build\release`; the default builds into `.build\dev`.
- `CHANGELOG.md`.

### Changed
- **Renamed Netch X to Netrix**: project, namespace, folders, executable (`Netrix.exe`), release archive (`Netrix.7z`), window text, new logo and icon. The original Netch is credited in the README and `NOTICE.txt`.
- **.NET 6 to .NET 10** (x64); NuGet packages refreshed (Serilog, CsWin32 stable, MaxMind, TraceEvent, Stun.Net, SingleInstance and others).
- **tun2socks** is rebuilt from recovered source (`Other/tun2socks`): larger relay buffer, larger TCP window, panics and logs written to `logging\tun2socks.log`.
- **pcap2socks** source is vendored (`Other/pcap2socks`) and built from source; the upstream project is archived.
- **aiodns**: Go 1.25 and miekg/dns 1.1.73.
- The interface metric is set with the Windows IP Helper API instead of starting `netsh.exe`.
- `WebRequest` replaced by `HttpClient`.
- Default STUN server is `stun.1und1.de`; the server list only has servers that support the NAT test (`stun.1und1.de`, `stun.miwifi.com`, `stun.fitauto.ru`).
- Font is Segoe UI.
- Start-up: the slow logging work runs after the window is shown; the delay test pauses while the window is minimized or in the tray; the `wintun.dll` check skips hashing when size and time match.
- CI: GitHub Actions updated, tests run on every build.
- The update checker and links point to `github.com/psnwd/netrix`.
- All Chinese comments and developer messages translated to English. The Chinese interface translation (`zh-CN`) is unchanged.
- README rewritten.

### Fixed
- aiodns crashed Netrix when an upstream DNS lookup failed.
- TUN mode leaked IPv6 traffic outside the tunnel.
- IPv6 connections hung when the server had no IPv6 path.
- Stop could leave the window stuck in "Stopping" or throw when the helper process had already died; each part now stops on its own with a 15 s limit, and route cleanup steps no longer skip each other.
- A helper process that exits before printing a start keyword now fails at once instead of after a 50 s wait.
- Share links: SOCKS5 wrote the password as `&user=`; the SOCKS5 type name did not match; WireGuard links threw on write and parse; `ShareMode.Argument` was not saved.
- Update: a failed apply now restores every `.old` file; the asset is picked by name, not first in the list; a download with the wrong hash is deleted.
- The update check threw when the repository had no releases.
- The Help window opens in the centre of the screen.
- Unfreed memory in the port helper.

### Security
- Updates are verified with a signature, not only a hash from the same release.
- `TLS AllowInsecure` has a warning in its tooltip.

### Removed
- Telegram links and badges, and the website links (there is no site yet).
- `RedirectorTester` (unused .NET Framework test harness) and `Other/_Archive` (build scripts of removed components).
- The Chinese issue templates and the JetBrains sponsor block.

[2.0.0]: https://github.com/psnwd/netrix/releases/tag/2.0.0
