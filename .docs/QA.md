# QA

Verify no regression after each phase in [PLAN.md](PLAN.md). Windows 10 22H2 and Windows 11, x64, admin account, clean VM snapshot per run.

## Automated (CI)
- `dotnet build -c Release`: zero warnings
- `dotnet test`: green
- `dotnet list package --vulnerable --deprecated`: empty
- Release artifact layout diff vs baseline: same files in `release\` and `release\bin\`

## Manual matrix
| Area | Cases |
|---|---|
| Startup | Fresh extract; missing `bin\` shows error; second instance blocked; non-admin prompts UAC |
| Upgrade | Old `data\`, `mode\Custom` from previous release load unchanged |
| Protocols | Add via form, share link/clipboard, subscription: SS, SSR, VMess, VLESS, Trojan, Socks5, WireGuard, SSH |
| Share links | Export then re-import gives identical server |
| Process mode | Chosen app proxied; others direct; stop restores traffic; driver install/uninstall ok |
| TUN mode | Adapter created, routes set, DNS works; stop removes adapter/routes; no leak after crash + restart |
| Share mode | Second device via hotspot/Npcap routes through proxy |
| DNS | aiodns rule split; custom DNS setting honored |
| Tools | Delay test, STUN NAT test, bandwidth counter, GeoIP flag, update check |
| UI | i18n en + zh-CN, high-DPI 125/150/200%, log window, tray icon |
| Failure | Bad server, port in use, core crash (Guard cleans up), network drop |
| Shutdown | Exit while running leaves no stray processes, routes, firewall rules, scheduled tasks |

## Release gates
1. Manual matrix passes on Win10 + Win11.
2. No crash in 30 min soak per mode.
3. Memory flat (±10%) over soak.
4. Artifact SHA256 recorded; binaries table in `Storage/README.md` current.
5. Rollback: previous release still opens the new `data\`.

## Bug triage
- S1: data loss, traffic leak, system network left broken. Blocks release.
- S2: a mode is broken. Blocks release.
- S3: UI/cosmetic. Ship.
