# TASK — checklist

Phases map to [PLAN.md](PLAN.md). One task per commit/PR.

## P1 Baseline
- [x] Fix baseline build on SDK 10: `AssemblyInfo.cs` `SupportedOSPlatform` Windows7.0 → Windows8.1 (CA1416 on inet_pton/ntohs)
- [ ] Build `main` clean with `.\build.ps1`; save file list + sizes of `release\`
- [x] Tests: reference `Netrix` from `Tests` (project ref; Tests TFM moved to net6.0-windows + RollForward=Major, drop RollForward in P2)
- [x] Tests: share-link round-trip per protocol (WireGuard `[Ignore]`d, see Known bugs)
- [x] Tests: `Mode` JSON load/save round-trip for Process/TUN/Share
- [x] Tests: `Setting` default load from `{}`
- [x] CI: run `dotnet test`

## P2 Framework
- [x] `common.props`, `Netrix.csproj`: `net10.0-windows`
- [x] `Tests.csproj`: same TFM (MSTest/Test.Sdk versions bump in P3)
- [x] `RedirectorTester` deleted (unused .NET Framework 4.8 harness, never run in CI); `Other/_Archive` (dead build scripts of removed components) deleted
- [x] Fix build errors/analyzer warnings: WFO1000 on `SyncGlobalCheckBox` (DesignerSerializationVisibility.Hidden)
- [x] `global.json`: `rollForward: latestFeature`
- [ ] Verify single-file self-contained publish runs on clean Win10/11 VM (publish succeeds locally: 190MB Netrix.exe; runtime launch untested)

## P3 NuGet
- [x] `dotnet list package --outdated --vulnerable --deprecated`
- [x] Fody + ConfigureAwait.Fody
- [x] Serilog (+ Extensions.Hosting, Sinks.*)
- [x] CsWin32 beta to stable (0.3.360): HWND ctor, ADDRESS_FAMILY/WIN32_ERROR in RouteHelper.cs, PortHelper.cs rewritten with byte[] + fixed (also fixes unfreed AllocHGlobal). PortHelper covered by test; RouteHelper.CreateUnicastIPCS and console show/hide need manual QA (TUN mode)
- [x] MaxMind.GeoIP2, TaskScheduler, WindowsJobAPI, Stun.Net 10.0.10 (ProxyFactory.CreateProxy replaced by `new Socks5UdpProxy(...)`; NAT test needs manual QA with a real SOCKS5 server), HMBSbige.SingleInstance 7.0.0 (int command instead of string; handler marshals to UI thread via BeginInvoke; identifier is now plain `Netrix`, no `Global\` prefix so single-instance is per session; QA: second launch must focus the first window). WindowsFirewallHelper: already latest
- [x] System.* 7.0 to match TFM (Management, ServiceController; CodePages removed, in-box)
- [x] Microsoft.Diagnostics.Tracing.TraceEvent, VisualStudio.Threading
- [ ] Dependabot: add grouping to cut PR noise

## P4 Native + binaries
- [x] Redirector/RouteHelper rebuild: builds in CI (run 38080196958, windows-2022).
- [x] `Other/aiodns`: go 1.25.0, miekg/dns 1.1.73; c-shared build verified locally. Runtime check in QA (DNS section)
- [x] `Other/*` build in CI. `Other/pcap2socks` v0.6.2, `wintun` 0.13, `v2ray-sn` v5.0.16: URLs still resolve (HTTP 200). Version bumps not done (need Windows runtime testing). v2ray-sn needed Go 1.18 (SagerNet gVisor fork) and clash pinned to v1.11.0 (newest clash needs Go 1.20); both fixed in CI
- [x] `Storage/tun2socks.bin`, `nfdriver.sys`: documented. tun2socks.bin is a Netrix-specific c-shared build (go1.17.1), not an upstream exe; patch source not in repo, cannot refresh. nfdriver.sys 1.6.3.0 tied to Redirector
- [x] `Storage/README.md`: version + sha256 table
- [x] `build.ps1`: GeoIP download fails the build on error (`-ErrorAction Stop`). Not hash-pinned: Loyalsoldier `release` branch is rolling

## P5 CI/CD
- [x] checkout@v4, cache@v4, setup-go@v5, setup-msbuild@v2, upload-artifact@v4, action-gh-release@v2, stale@v9, setup-dotnet@v4 (also added to release.yml)
- [x] Replace `actions-rs/toolchain` with `dtolnay/rust-toolchain@nightly` (still unpinned nightly; pin a dated nightly once pcap2socks builds are confirmed)
- [x] Drop `vs-prerelease: true` (vcxproj use v143 = VS2022 stable)
- [x] Add test step (build.yml)
- [ ] Artifact checksum
- [ ] `release.yml`: align with `build.yml`, verify tag flow on a fork
- [x] `dependabot.yml`: gomod directory `/Other/aiodns`

## P6 Release
- [x] Dead code: analyzers flag none (remaining ~30 warnings are nullable/obsolete-API noise, left alone)
- [x] Update README (.NET version), CLAUDE.md (root imports .docs/CLAUDE.md). NOTICE.txt unchanged, no licensing change
- [ ] Full [QA.md](QA.md) pass on clean machine
- [ ] Tag RC, collect feedback, tag final

## Known bugs found by tests (fixed unless noted)
- [x] Socks5 share link wrote password as `&user=`; now `&pass=` (parse threw on duplicate key)
- [x] `ShareLink.ParseUri` looked up util `"Socks5"`; TypeName is `"SOCKS"`
- [x] `ShareMode.Argument` was a field, dropped by JSON; now a property
- [x] WireGuard share link: was cast to `VMessServer` (throw) and parse threw too; now `wireguard://privkey@host:port?publickey=..&address=..&mtu=..[&presharedkey=..]#remark`
- [x] CI uses `windows-2022` runner; `setup-dotnet` 10 works there (build.yml green, see CI status)

## CI status
- [x] `Netrix Build CI` green on `modernize-net10` (commit 2e945ec): build 188 s, tests pass, `release` artifact 97.2 MB
- [ ] `release.yml` carries the same Go 1.18 / .NET 10 changes but only runs on tag push; untested
- [ ] Compare artifact layout with a build of `main` (QA automated check)
- [x] WebRequest replaced by HttpClient (WebUtil; tests use a local HttpListener). Behaviour kept: non-2xx throws, timeout, UA, per-call proxy for subscriptions. Download body has no per-read timeout (WebRequest had ReadWriteTimeout)
- [ ] Failure diagnostics: build.yml wraps build.ps1 and emits an error annotation with the output tail (job logs need login); keep or drop

## WebMode
- [x] `ModeType.WebMode` (appended, value 3), `WebMode` model (`Bypass`), `WebController` (HKCU Internet Settings + InternetSetOption), sample `Storage/mode/System Proxy.json`, tests (JSON round-trip, sample loads)
- [ ] Manual QA: start/stop sets and restores ProxyEnable/ProxyServer/ProxyOverride; browser traffic goes via proxy; behaviour with a SOCKS5-with-auth server (goes through local v2ray)
- [x] Crash recovery: previous proxy values saved to datawebmode.restore.json, restored by `WebController.RecoverAfterCrash()` after the single-instance check. Untested (needs a real crash + relaunch)
- [ ] No editor UI: edit the JSON file (MainForm opens it)

## Update path
- [x] Rollback: a failed apply restores every `*.old` file (was: half-installed app). Test fails without it
- [x] Pick the asset whose name matches the hash table (was `assets[0]`); case-insensitive hash compare; delete a download with a wrong hash
- [x] Signature: releases are signed (ECDSA P-256, `sign.ps1`, GitHub secret `UPDATE_SIGNING_KEY`, asset `Netrix.7z.sig`); `Updater` flow refuses an unsigned or badly signed release (`UpdateSignature`, 4 tests + sign.ps1 round trip). Setup: add the secret from `update-signing.key`, then tag a release. Until a signed release exists the in-app update refuses and the user installs from the release page
- [ ] Not done: update runs inside the running process, no helper exe; real update still needs manual QA on Windows

## Controller tests
- [x] `Guard` (start keyword, failed keyword, stop kills child) via a copied cmd.exe; `WebController` start/stop/crash-recover against a scratch registry key; `V2rayConfigUtils` outbound protocol per server type, inbound, socks auth, vmess address/port
- [ ] Not covered (need drivers/admin/real binaries): NFController, TUNController, PcapController, protocol controllers' real process start, MainController orchestration
- [x] Guard: a child that exits without a start/fail keyword now fails right away (was a 50 s wait); test added

## tun2socks rewrite
- [x] `Other/tun2socks` rebuilt from the binary (see its RECOVERY.md); 4,541 of 4,547 shared functions have identical code size; unit tests pass
- [x] Wired in: `Other/tun2socks/build.ps1` -> `Other/release/tun2socks.bin` overrides `Storage/tun2socks.bin` in the release; CI installs Go 1.17.1 (`GO117`); verified with an isolated local run (same 4,680,192 byte output)
- [ ] Not verified: CI run with the new steps; TUN mode with the rebuilt DLL as admin with `wintun.dll` (nothing has been tested with real traffic)
- [ ] Rollback: delete `Other/tun2socks/build.ps1` to ship the original `Storage/tun2socks.bin` again

## pcap2socks
- [x] Upstream archived: source vendored in `Other/pcap2socks/crate` (see `VENDORED.md`), deps updated, built from source by `Other/pcap2socks/build.ps1`; CI Rust toolchain is now stable
- [x] 11 upstream tests pass; CLI help/version/arg errors identical to v0.6.2
- [ ] Not verified: real traffic through Share mode (needs Npcap); CI run of the new build step
- [ ] Rollback: restore the old build.ps1 (downloads the v0.6.2 release zip)

## Found while testing on Windows (2026-10-11)
- [x] `aiodns` crashed Netrix when an upstream DNS lookup failed (`WriteMsg(nil)` panic, existed before the update): both handlers now answer SERVFAIL; test added
- [x] `tun2socks.bin` rewrite now writes Go panics and logs to `logging/tun2socks.log` (a GUI process has no console, so they were invisible)
- [x] TUN mode leaked IPv6 (YouTube over IPv6 never entered the tunnel, "Used" stayed at 700 KB): IPv6 address + `::/0` on-link route added to the adapter, setting `TUNTAP.ProxyIPv6` (default on). Owner reports TUN mode working
- [ ] Still untested on real traffic: Process mode, Share mode, WebMode, update/rollback, a second launch focusing the first window
- [x] IPv6 hang on servers without IPv6 egress (found 2026-10-11: curl to an AAAA host stalled through a Trojan server): `TUNController` probes `Socks5ServerTestUtils.IPv6WorksAsync` (5 s cap, CONNECT to Cloudflare IPv6 :80, needs data back). On failure the IPv6 route stays and `tun2socks` rejects IPv6 TCP/UDP (`TYPE_BLOCKIPV6`, `proxies.BlockIPv6`): RST at once, clients fall back to IPv4, no leak. Tests: 2 C# (fake SOCKS5), 1 Go. Not run in the real app
- [ ] IPv6 follow-ups: LAN-bypass rules are IPv4 only (IPv6 ULA/other local prefixes go into the tunnel), IPv6 DNS from the router bypasses aiodns, an IPv6-only server address disables the IPv6 route

## Performance and optimization
Rule: measure first. Every item needs a before/after number from the same test, and one change per commit so a regression can be traced.

### Benchmark harness (do first)
- [x] `.docs/bench.ps1 -Label <name>`: curl download (1 and 16 streams), ping, DNS time, CPU, memory; appends to `.docs/PERF.md`. Direct vs TUN vs WebMode = switch mode by hand and rerun. iperf3 skipped (needs a server), add if the HTTPS test is too noisy
- [x] Fixed test server: defaults in the script (OVH 100 MB file, 1.1.1.1); results go to `.docs/PERF.md`
- [ ] Baseline numbers for the current build before touching anything

### Network path (TUN mode: app -> wintun -> lwIP -> tun2socks -> SOCKS5 over loopback -> v2ray-sn -> server)
- [x] Relay buffer: `tools.CopyBuffer` copies in 1400 byte chunks (0x578), so every large transfer needs many small reads and writes; try 32 KiB or a pooled 64 KiB buffer and compare — done: 32 KiB per direction, not yet measured (needs TUN baseline)
- [x] lwIP limits in `core/c/custom/lwipopts.h`: `TCP_WND`/`TCP_SND_BUF` 32 KiB, `MEMP_NUM_TCP_PCB` 16, `MEM_SIZE` 128 KiB; check what limits single-stream speed and many-connection pages, raise only what the benchmark shows — done: only `TCP_WND`/`TCP_SND_BUF` matter (pools are malloc-backed so `MEMP_NUM_TCP_PCB`/`MEM_SIZE` are no-ops); `TCP_WND` raised to 0xffff (max without window scaling), not yet measured
- [x] SOCKS UDP: every UDP flow opens its own TCP control connection, handshake and local socket (cost on first packet, bad for QUIC and DNS bursts); try one association per handler, or a flow table with reuse — checked, no change: core keys UDP conns by local source ip:port only, so one association already serves all destinations of a socket (one handshake per local socket, not per packet); sharing across sockets cannot demux replies. Revisit only if the benchmark shows QUIC setup cost
- [x] UDP read buffer is 1500 bytes and the relay replies time out after 30 s of silence; check whether larger QUIC packets or long-lived flows are affected — checked, no change: reply = SOCKS header (<=22) + payload <= 1472 on a 1500 MTU, so 1500 does not truncate; an idle flow is simply recreated on the next packet
- [x] DNS: `adns` and `aiodns` open a new TCP (or TLS) connection per query; add UDP upstream or a persistent connection, and a response cache in `aiodns` — done: response cache (NOERROR only, TTL capped at 60 s, 4096 entries) in both; test in `aiodns/cache_test.go`. Not done: UDP upstream (changes proxy semantics) and a persistent connection (needs a pool); not yet measured
- [ ] Remove the loopback hop: tun2socks to v2ray-sn is SOCKS5 over TCP on 127.0.0.1; measure the cost, and compare with a core that has its own TUN (sing-box, see below)
- [ ] v2ray config: `UseMux` is off by default; measure mux, TCP fast open and keep-alive settings per transport (ws, grpc, tcp) before changing defaults — no code needed, both knobs already exist (Settings: Use Mux, TCP Fast Open; per-server Use Mux for VMess/VLESS); keep-alive has no setting, add only if the numbers ask for it. Protocol: same server, same mode, `bench.ps1 -Label tun-default`, then flip one knob and rerun (`tun-mux`, `tun-tfo`), per transport (ws, grpc, tcp). The new "40 small req ms" column is the one mux and TFO should move (connection setup); mux usually costs single-stream Mbps. Change a default only if a row shows a win without hurting the others
- [ ] Go runtime in the two DLLs (`aiodns.bin`, `tun2socks.bin`) and `v2ray-sn`: GC settings (`GOGC`, memory limit) and `GOMAXPROCS` when the machine is busy — no code needed to measure: nothing sets these today (defaults GOGC=100, no limit, GOMAXPROCS=all cores) and the Go runtime reads `GOGC`, `GOMEMLIMIT`, `GOMAXPROCS` from the environment, which Netrix, the two DLLs (loaded in-process) and `v2ray-sn` (child) all inherit. Protocol: `$env:GOGC=200; .\Netrix.exe` (or `GOMEMLIMIT=200MiB`, `GOMAXPROCS=2`), same server and mode, `bench.ps1 -Label tun-gogc200`; compare Mbps, CPU% and mem MB against `tun-default`. Hard-code (`debug.SetGCPercent`, `SetMemoryLimit` in `tun_init`/`aiodns_init`, env on the `v2ray-sn` start) only for a value that wins
- [ ] wintun: ring buffer size and packet batching in the read loop (`device.Read` one packet per call today) — checked, deferred: the pinned wireguard-go (2021) has a single-packet `Read` and ring size is fixed at 8 MiB in the library (`StartSession(0x800000)`, allowed 128 KiB to 64 MiB), already large for a 1500 MTU stream so no change planned; batching means upgrading wireguard-go (new `Read([][]byte)` API) — do only if the benchmark shows the read loop is CPU-bound
- [ ] MTU: 1500 on the adapter; check fragmentation against the server path (try 1400 and 1280 if tunnels over PPPoE or mobile)
- [x] Share mode (pcap2socks): same buffer/UDP questions, plus tokio worker threads and the LRU size (256 UDP ports) — checked in `Other/pcap2socks/crate`, no change: relay read buffers are already 64 KiB (`u16::MAX`), receive window is 64 KiB with window scaling up to 16 MiB; `#[tokio::main]` uses the default multi-thread runtime (one worker per core), nothing to tune for a per-connection task design; the 256-entry LRU (`MAX_UDP_PORT`) maps LAN-side UDP source ports to local sockets, only a device with more than 256 live UDP flows evicts, raise it only if one is seen. Not measurable without Npcap and a second device

### Accuracy and cheap telemetry
- [x] Bandwidth counter: Netrix counts bytes with an ETW kernel trace of the `v2ray-sn` process only. `tun_getUP` and `tun_getDL` already exist in the DLL and are unused; use them for TUN mode (cheaper than ETW, includes upload, and counts what really crossed the tunnel) — done for TUN mode: `Bandwidth.NetTraffic` reads `tun_getDL` (download only; ETW still used for other modes). Not run against real traffic; upload display is the next item
- [x] Show upload and download speed separately — done for TUN mode (↓ and ↑ labels from `tun_getDL`/`tun_getUP`); other modes keep the single "↑↓" label because ETW only counts received bytes. Not run on real traffic

### Startup and responsiveness
- [x] Time the start sequence (click Start to traffic flowing) and log each step — done in `MainController.StartAsync`: logs "Start step ...: N ms since click" after server DNS lookup, firewall/DNS cache, server controller and mode controller (4 laps, in the log file). Not broken down inside `TUNController` (adapter wait, routes, netsh): add laps there if the mode step dominates. Not run
- [ ] Routes: a mode like "Bypass LAN and China" adds thousands of routes one native call at a time; batch them or use a route table API that takes many entries — measure first: each rule is one direct `CreateIpForwardEntry2` call in `RouteHelper.bin` (no process or wait per call) and Windows has no bulk route API, so "batching" would only be parallel calls. `TUNController` now logs "Route table: N rules in M ms"; decide from that number (rule count x per-rule cost). Not run
- [x] `netsh.exe` is started to set the interface metric (a process per call); use the IP Helper API instead — done: `NetworkInterfaceUtils.SetInterfaceMetric` uses `GetIpInterfaceEntry`/`SetIpInterfaceEntry` (IPv4, null = automatic metric like netsh). Builds; not run (needs admin + TUN start; check the metric with `Get-NetIPInterface`)
- [x] Adapter wait loop polls 20 x 300 ms; wait on the adapter-added event instead — done by deletion: the loop ran before `tun_init` created the adapter and its result (`_tun.InterfaceIndex`) was overwritten right after by `TemplateBuilder`, so it only cost up to 6 s. The index comes from `tun_luid()`. Builds; not run
- [x] `wintun.dll` hash compare runs on every start; compare size and timestamp first — done: `TUNController.CheckDriver` returns early when size and write time match the installed copy (File.Copy keeps the time); hash path unchanged otherwise. Not run
- [x] Startup logging lists every running process and third-party driver and checks GitHub for updates; do the slow parts after the window is shown — mostly already true: drivers and processes are `Task.Run` and Debug-level only, update check and subscription update are fire-and-forget in `MainForm_Load`. Moved the one real cost, SHA256 of the whole exe (plus the Debug lists), to `MainForm.Shown` in `Program.cs`; version, OS and language lines still log before the window. Builds; not run
- [ ] Time to first window on a cold start of the 187 MB single-file exe (self-extract); try ReadyToRun and `EnableCompressionInSingleFile` trade-offs

### Size and memory
- [ ] `Netrix.exe` is 187 MB self-contained; compare framework-dependent, trimming (WinForms limits), and compression for download size — measured (Release, win-x64, single file, 2026-10-11): self-contained 178.2 MB, self-contained + `EnableCompressionInSingleFile` 77.5 MB, framework-dependent 13.9 MB (needs the .NET 10 desktop runtime installed). Trimming not tried (WinForms is not trim-safe). Compression costs start-up time (assemblies inflate on every launch), so time the cold start of the 77.5 MB build before switching `build.ps1`; framework-dependent would cut the download 13x but changes the install requirement, owner decision
- [ ] Idle memory of `Netrix.exe` was about 180 MB while running; find what holds it (ETW session, form designer resources, logging)
- [ ] Lower background CPU when idle (timers, delay tests, subscription updates running in parallel) — partly done: the 10 s delay test (`DelayTestHelper`, pings every server, 16 at a time) now skips ticks while the window is in the tray or minimized. Subscription updates only run at open (`UpdateServersWhenOpened`), no periodic timer found. Not measured; builds

### Next, in order (added 2026-10-11)
1. [ ] **Commit the work**: ~40 files are modified/untracked on `modernize-net10` (tun2socks rewrite, pcap2socks vendor, aiodns cache, perf changes, bench). One commit per task as the rules say; nothing is safe until committed
2. [ ] **Baseline numbers** (`bench.ps1 -Label baseline`, direct + TUN): every perf item marked "done, not measured" is unproven until this exists. Then compare buffer 32 KiB, `TCP_WND`, DNS cache one change at a time; revert any that do not win
3. [ ] **Real-traffic QA** for what is still untested: Process, Share, WebMode, update/rollback, second-launch focus, `SetInterfaceMetric` (`Get-NetIPInterface`), `CheckDriver` early return
4. [ ] **CI run** for the new tun2socks and pcap2socks build steps (Go 1.17.1 + Rust stable), then the tag flow of `release.yml` on a fork
5. [ ] **Release safety**: artifact checksum (P5); update signature (Update path, hash and file share one origin)
6. [ ] **Tests for new code**: `Bandwidth.NetTraffic` (TUN counters), `NetworkInterfaceUtils.SetInterfaceMetric`, `TUNController.CheckDriver` size/time shortcut. Only where a unit test can run without admin; the rest stays manual QA
7. [ ] **Owner decisions** (blockers, not work): framework-dependent vs self-contained vs compressed exe; sing-box prototype yes/no; drop `RedirectorTester` or port it
8. [ ] Small: Dependabot grouping; IPv6 follow-ups (LAN bypass, router DNS, IPv6-only server)

### Bigger option (decide before investing in the items above)
- [ ] sing-box as the core and TUN: it has its own TUN inbound, process-name routing and per-protocol speed work, and would replace tun2socks, wintun glue and the closed-source NetFilter driver. Loses ShadowsocksR, mKCP and legacy XTLS and needs a new config generator. Prototype for TUN mode only, compare with the harness numbers, then decide


## Rebrand to Netrix (2026-10-11)
- [x] Netch X -> Netrix: folders (`Netrix/`, `Netrix.sln`, `Netrix.csproj`), namespace, `Netrix.exe`, release archive `Netrix.7z` (top-level folder `Netrix`), UI text, tests, docs, CI
- [x] Repo `github.com/psnwd/netrix`: update checker (`UpdateChecker.Owner/Repo`), About, Help, `origin` remote
- [x] Logos in `.assets/`; app icon and About image generated from `.assets/Netrix.png` (`Netrix/Resources/Netrix.ico`, 16-256 px)
- [x] Removed: Telegram links/badges, `docs.netch.org`/`netch.org` links (no website yet), JetBrains sponsor block, About sponsor link. FAQ menu item is now "Report an issue" (GitHub issues)
- [x] Kept as credit: LICENSE, NOTICE.txt and README name the original Netch project (GPLv3 requires it)
- [ ] Upgrade note: Netrix is a new product for the updater: it does not update an old Netch install, and the Windows startup task, firewall rule names and single-instance id are now "Netrix"
- [ ] First release must be signed (see Update path) before in-app update works
