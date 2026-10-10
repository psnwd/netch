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
- [x] MaxMind.GeoIP2, TaskScheduler, WindowsJobAPI, Stun.Net 10.0.10 (ProxyFactory.CreateProxy replaced by `new Socks5UdpProxy(...)`; NAT test needs manual QA with a real SOCKS5 server), HMBSbige.SingleInstance 7.0.0 (int command instead of string; handler marshals to UI thread via BeginInvoke; identifier is now plain `Netch`, no `Global\` prefix so single-instance is per session; QA: second launch must focus the first window). WindowsFirewallHelper: already latest
- [x] System.* 7.0 to match TFM (Management, ServiceController; CodePages removed, in-box)
- [x] Microsoft.Diagnostics.Tracing.TraceEvent, VisualStudio.Threading
- [ ] Dependabot: add grouping to cut PR noise

## P4 Native + binaries
- [x] Redirector/RouteHelper rebuild: builds in CI (run 38080196958, windows-2022). RedirectorTester not run (needs driver + admin)
- [x] `Other/aiodns`: go 1.25.0, miekg/dns 1.1.73; c-shared build verified locally. Runtime check in QA (DNS section)
- [x] `Other/*` build in CI. `Other/pcap2socks` v0.6.2, `wintun` 0.13, `v2ray-sn` v5.0.16: URLs still resolve (HTTP 200). Version bumps not done (need Windows runtime testing). v2ray-sn needed Go 1.18 (SagerNet gVisor fork) and clash pinned to v1.11.0 (newest clash needs Go 1.20); both fixed in CI
- [x] `Storage/tun2socks.bin`, `nfdriver.sys`: documented. tun2socks.bin is a Netch-specific c-shared build (go1.17.1), not an upstream exe; patch source not in repo, cannot refresh. nfdriver.sys 1.6.3.0 tied to Redirector
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
- [ ] CI uses `windows-2022` runner; confirm `setup-dotnet` 10 works there

## CI status
- [x] `Netch Build CI` green on `modernize-net10` (commit 2e945ec): build 188 s, tests pass, `release` artifact 97.2 MB
- [ ] `release.yml` carries the same Go 1.18 / .NET 10 changes but only runs on tag push; untested
- [ ] Compare artifact layout with a build of `main` (QA automated check)
- [ ] Failure diagnostics: build.yml wraps build.ps1 and emits an error annotation with the output tail (job logs need login); keep or drop
