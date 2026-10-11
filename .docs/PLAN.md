# PLAN — Modernize Netrix 1.x

Goal: move off EOL .NET 6, refresh deps and bundled binaries, make CI/build reliable, add real tests. No new features, no behavior change.

See [ARCHITECTURE.md](ARCHITECTURE.md), [TASK.md](TASK.md), [QA.md](QA.md), [RISKS.md](RISKS.md).

## Why
- `net6.0-windows` EOL since Nov 2024; `Tests` still on `net5.0`; `RedirectorTester` on .NET Framework 4.8.
- GitHub Actions pinned to old majors (`upload-artifact@v3`, `cache@v3`, `checkout@v3`, `setup-go@v3`, `actions-rs/toolchain@v1` unmaintained).
- Pre-release package (`CsWin32 0.1.588-beta`); `Stun.Net`, `WindowsJobAPI` possibly stale.
- Bundled binaries (`tun2socks.bin`, `nfdriver.sys`, xray/v2ray, wintun) likely behind upstream.
- Tests near-empty: no safety net for refactors.

## Decisions
| Decision | Choice | Reason |
|---|---|---|
| Target framework | `net10.0-windows` (LTS). Fallback `net8.0-windows` | net8 EOL Nov 2026; SDK 10 already installed on dev box |
| Scope | x64 only, WinForms kept | Matches current product |
| Behavior | Unchanged; config/mode JSON stay compatible | Users keep `data\` and `mode\` |
| Tests first | Characterization tests before framework bump | Catch regressions |

## Phases
1. **Baseline** — build current `main` clean, record output tree, add characterization tests (share-link parse/format, mode JSON round-trip, config load). Exit: green CI on unchanged code.
2. **Framework bump** — `common.props`, `Netrix.csproj`, `Tests` to new TFM; fix analyzer/CsWin32 breaks (warnings are errors). Exit: app starts, connects in each mode.
3. **NuGet refresh** — bump one group at a time (Serilog, System.*, Fody/ConfigureAwait, CsWin32 to stable, MaxMind, TraceEvent, TaskScheduler, Stun.Net). Replace dead packages if needed. Exit: `dotnet list package --vulnerable --deprecated` clean.
4. **Native + bundled binaries** — rebuild `Redirector`/`RouteHelper` on current VS toolset; update `Other/*` (aiodns Go modules, pcap2socks, v2ray-sn, wintun) and prebuilt `Storage/` binaries; record versions + hashes. Exit: Process/TUN/Share modes pass QA.
5. **CI/CD** — bump actions, replace `actions-rs`, `upload-artifact@v4`, add `dotnet test` step, pin Go/Rust, fix cache keys. Exit: green PR build, release artifact layout identical.
6. **Cleanup + release** — remove dead code, update README/CLAUDE.md, tag RC, full QA pass.

7. **Performance and optimization** (after the release candidate) - measure first, then change one thing at a time; see the Performance section in [TASK.md](TASK.md). Goals: higher throughput and lower latency through TUN mode, faster start, smaller and lighter app. Rule: no optimization without a before/after number from the same test.

## Out of scope
New protocols, UI redesign, ARM64 (blocked on x64-only prebuilt binaries), Netrix 2.0 rewrite. WebMode and IPv6 in TUN mode were added during the update. A sing-box based core (TUN and process routing) is an open option, see TASK.md.

## Done when
All [QA.md](QA.md) release gates pass and every [TASK.md](TASK.md) item is checked.

## Status notes
- 2026-10-11: a full local build ran end to end. TUN mode was started on Windows by the owner and carries traffic after two fixes found in testing: `aiodns` panicked on a failed upstream DNS lookup (killed the process), and IPv6 bypassed the tunnel (now routed into it, setting `TUNTAP.ProxyIPv6`). Process mode, Share mode and WebMode are still untested on real traffic.
