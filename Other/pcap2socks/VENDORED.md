# pcap2socks (vendored)

Source of [zhxie/pcap2socks](https://github.com/zhxie/pcap2socks) (MIT, `crate/LICENSE`). The upstream repository is
**archived** (last commit 2022-10-08, v0.6.2 plus 5 commits), so it is built from source here instead of downloading the old release zip.

| | |
| --- | --- |
| Base | upstream `master` @ `07dcd66` ("Fix lints (#33)"), crate version still 0.6.2 |
| Share mode | `Netch/Controllers/PcapController.cs` runs `pcap2socks.exe --interface \Device\NPF_{id} --destination host:port <preset> [--username --password]` and waits for the `└` banner line |
| Build | `build.ps1` downloads the Npcap SDK 1.16 (SHA-256 checked) for `Packet.lib`, then `cargo build --release --locked` in `crate/`; output `..\release\pcap2socks.exe` |
| Run time | Npcap in WinPcap API-compatible mode must be installed (`Packet.dll`), as before |

Changes against upstream:
- Dependencies updated: `Cargo.lock` regenerated (tokio 1.9 -> 1.53, rand, clap 2.34, env_logger, ...) and `pnet` 0.28 -> 0.35, `ipnetwork` 0.18 -> 0.20, `lru` 0.6 -> 0.12.
- `src/lib.rs`: two lines for the `lru` 0.12 API (`NonZeroUsize` capacity, `cap().get()`).
- The `netifs` git dependency (an unmaintained 2020 Apache-2.0 library) is vendored in `crate/vendor/netifs` (own license file inside), with
  the `readme` key removed and `eui48` without default features (its `rustc-serialize` no longer compiles).
- Directory is `crate/` because `Other/clean.ps1` deletes any `Other/<tool>/src`.

Checked: 11 upstream unit tests pass; `--help` and `-V` output and the argument errors match the old v0.6.2 release exe, and both stop at the
same place when no adapter list is available. **Not checked:** capturing and forwarding real traffic (needs Npcap and a game console or second device).
