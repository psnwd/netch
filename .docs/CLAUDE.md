# CLAUDE.md

Netrix 2.0: Windows proxy client (WinForms, .NET 10, x64 only). Needs admin at runtime (see `Netrix/App.manifest`). GPLv3.

## Layout

- `Netrix/` — main WinForms app (`net10.0-windows`). Entry: `Program.cs`.
  - `Controllers/` — runtime pieces started/stopped by `MainController` (`NFController`, `TUNController`, `PcapController`, `DNSController`, `Guard`, `UpdateChecker`). Implement `IController` / `IModeController`.
  - `Servers/<Protocol>/` — one folder per protocol (Shadowsocks, ShadowsocksR, Socks5, SSH, Trojan, V2ray, VLESS, VMess, WireGuard). Each has `*Server` (model), `*Util` (`IServerUtil`: URI parse, share link, controller factory), `*Form` (edit UI), and often `*Controller`.
  - `Models/Modes/` — `ProcessMode`, `TunMode`, `ShareMode`; JSON polymorphism via `JsonConverter/*TypeDiscriminator.cs`.
  - `Interops/` — P/Invoke wrappers for native bins (Redirector, RouteHelper, tun2socks, aiodns). `NativeMethods.txt` feeds CsWin32.
  - `Utils/`, `Services/`, `Forms/`, `Models/Settings/` — helpers, services, UI, config.
- `Redirector/` — C++ (vcxproj) Netfilter-based process redirector → `nfapi.dll`, `Redirector.bin`.
- `RouteHelper/` — C++ (vcxproj) route table helper → `RouteHelper.bin`.
- `Other/` — third-party deps built from source (aiodns, pcap2socks, v2ray-sn, wintun); each has own `build.ps1`; `Other/build.ps1` runs all.
- `Storage/` — runtime assets copied into release: `i18n/`, `mode/`, `stun.txt`, `nfdriver.sys`, `aiodns.conf`, prebuilt `tun2socks.bin`.
- `Tests/` — MSTest project (net10.0-windows): share-link and mode/setting JSON round-trips. Run `dotnet test Tests\Tests.csproj -p:Platform=x64`.

## Build

Windows only. Needs .NET 10 SDK, MSBuild + C++ toolchain (for Redirector/RouteHelper), Go etc. for `Other/`.

```powershell
.\build.ps1                          # full build into .\.build (builds Other, Netrix, Redirector, RouteHelper); keeps .build\data
.\build.ps1 -PlatformToolset v145    # machine with only Visual Studio 2026 (the .vcxproj files say v143); needs msbuild on PATH, Go 1.17.1/1.18.10 via GO117/GO118, run under pwsh
.\build.ps1 -Configuration Debug     # debug
dotnet build Netrix\Netrix.csproj -c Debug -p:Platform=x64   # app only
dotnet test Tests\Tests.csproj
.\clean.ps1
```

`build.ps1` skips a component if its output dir (`Netrix\bin\<cfg>`, `Redirector\bin\<cfg>`, `RouteHelper\bin\<cfg>`, `Other\release`) already exists — run `.\clean.ps1` to force rebuild. It downloads `GeoLite2-Country.mmdb` at build time. CI: `.github/workflows/build.yml` (cache keyed by `sha256.ps1` of each source dir).

## Conventions

- `common.props` + csproj: nullable/implicit usings on, analyzers with `CodeAnalysisTreatWarningsAsErrors` and `EnforceCodeStyleInBuild` — style/analyzer warnings break the build.
- Uses `Microsoft.VisualStudio.Threading` analyzers (VSTHRD*) and Fody (`ConfigureAwait.Fody`): don't hand-write `ConfigureAwait`; async-void only for event handlers.
- Runtime layout expects `bin\` next to `Netrix.exe` (added to PATH); user data in `data\`, `mode\`, `i18n\`, `logging\`.
- Adding a protocol: new `Servers/<Name>/` with Server/Util/Form(/Controller). `ServerHelper` finds every public `IServerUtil` by reflection and the JSON converter resolves server types through it, so no registration is needed. Add a round-trip test in `Tests/ShareLinkTests.cs`.
- Translations: `Storage/i18n/` and `Netrix/Resources/zh-CN`; strings go through `i18N.Translate`.
- Dependabot PRs dominate git history; keep commits small.
- Modernization plan/status: `.docs/PLAN.md`, `.docs/TASK.md`, `.docs/QA.md`.
