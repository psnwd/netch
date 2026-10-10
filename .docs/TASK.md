# TASK — checklist

Phases map to [PLAN.md](PLAN.md). One task per commit/PR.

## P1 Baseline
- [x] Fix baseline build on SDK 10: `AssemblyInfo.cs` `SupportedOSPlatform` Windows7.0 → Windows8.1 (CA1416 on inet_pton/ntohs)
- [ ] Build `main` clean with `.\build.ps1`; save file list + sizes of `release\`
- [x] Tests: reference `Netch` from `Tests` (project ref; Tests TFM moved to net6.0-windows + RollForward=Major, drop RollForward in P2)
- [x] Tests: share-link round-trip per protocol (WireGuard `[Ignore]`d, see Known bugs)
- [x] Tests: `Mode` JSON load/save round-trip for Process/TUN/Share
- [x] Tests: `Setting` default load from `{}`
- [x] CI: run `dotnet test`

## P2 Framework
- [x] `common.props`, `Netch.csproj`: `net10.0-windows`
- [x] `Tests.csproj`: same TFM (MSTest/Test.Sdk versions bump in P3)
- [ ] `RedirectorTester`: port to SDK-style or delete if unused
- [x] Fix build errors/analyzer warnings: WFO1000 on `SyncGlobalCheckBox` (DesignerSerializationVisibility.Hidden)
- [x] `global.json`: `rollForward: latestFeature`
- [ ] Verify single-file self-contained publish runs on clean Win10/11 VM (publish succeeds locally: 190MB Netch.exe; runtime launch untested)

## P3 NuGet
- [x] `dotnet list package --outdated --vulnerable --deprecated`
- [x] Fody + ConfigureAwait.Fody
- [x] Serilog (+ Extensions.Hosting, Sinks.*)
- [x] CsWin32 beta to stable (0.3.360): HWND ctor, ADDRESS_FAMILY/WIN32_ERROR in RouteHelper.cs, PortHelper.cs rewritten with byte[] + fixed (also fixes unfreed AllocHGlobal). PortHelper covered by test; RouteHelper.CreateUnicastIPCS and console show/hide need manual QA (TUN mode)
- [x] MaxMind.GeoIP2, TaskScheduler, WindowsJobAPI, Stun.Net 10.0.10 bumped (ProxyFactory.CreateProxy replaced by `new Socks5UdpProxy(...)`; NAT test needs manual QA with a real SOCKS5 server). Held back: HMBSbige.SingleInstance 7.x (API is int-message only, no string `Received` stream; would need a Program.cs rewrite for no benefit, stay on 6.0.0). WindowsFirewallHelper: already latest
- [x] System.* 7.0 to match TFM (Management, ServiceController; CodePages removed, in-box)
- [x] Microsoft.Diagnostics.Tracing.TraceEvent, VisualStudio.Threading
- [ ] Dependabot: add grouping to cut PR noise

## P4 Native + binaries
- [ ] Redirector/RouteHelper: retarget platform toolset, rebuild, run RedirectorTester
- [ ] `Other/aiodns`: bump Go version in `go.mod`, update modules, rebuild
- [ ] `Other/pcap2socks`, `v2ray-sn`, `wintun`: bump pinned versions
- [ ] `Storage/tun2socks.bin`, `nfdriver.sys`: refresh or document source/signature
- [ ] `Storage/README.md`: version + sha256 table
- [ ] `build.ps1`: pin GeoIP download (hash/version), fail on download error

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
- [ ] CI uses `windows-2022` runner; confirm `setup-dotnet` 10 works there
