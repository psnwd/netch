# RISKS

| Risk | Impact | Mitigation |
|---|---|---|
| CsWin32 stable renames/changes APIs vs `0.1.588-beta` | Build breaks in Interops/Utils | Bump alone in its own PR; fix callers |
| `nfdriver.sys` is a signed prebuilt; Redirector is tied to it | Process mode breaks | Do not rebuild driver; only rebuild user-mode Redirector; test with RedirectorTester |
| Netfilter / wintun / tun2socks update changes behavior | TUN/process regressions | Pin versions + hashes; QA matrix per mode |
| Warnings-as-errors with newer analyzers | Many build errors on TFM bump | Fix per rule; suppress with justification in `.editorconfig`, not blanket `NoWarn` |
| Config JSON compatibility | Users lose servers/modes | Characterization tests (P1); upgrade case in QA |
| Single-file publish differences on new runtime | Startup/extract failures | Test on clean VM; keep `IncludeNativeLibrariesForSelfExtract` |
| CI toolchain drift (Go stable, Rust nightly, MSYS2 rolling) | Flaky, irreproducible builds | Pin versions; record in TASK P5 |
| GeoIP file downloaded from third party at build | Supply-chain / build break | Pin release tag + sha256 |
| Windows-only; cannot run elsewhere | Slow feedback | Keep Windows runners; keep unit tests platform-neutral where possible |
