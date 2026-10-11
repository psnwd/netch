# Netrix
Files shipped with a Netrix release, and where each part comes from.

## Built from source in this repository
| Part | Source | Based on |
| :- | :- | :- |
| `aiodns.bin` | [`Other/aiodns`](../Other/aiodns) | the aiodns component of [Netch](https://github.com/NetchX/Netch) |
| `Redirector.bin`, `nfapi.dll` | [`Redirector`](../Redirector) | the Redirector component of Netch (NetFilter SDK) |
| `RouteHelper.bin` | [`RouteHelper`](../RouteHelper) | the RouteHelper component of Netch |
| `tun2socks.bin` | [`Other/tun2socks`](../Other/tun2socks) (recovered source, see its `RECOVERY.md`) | the closed-source DLL shipped with Netch; not [xjasonlyu/tun2socks](https://github.com/xjasonlyu/tun2socks) |
| `pcap2socks.exe` | [`Other/pcap2socks`](../Other/pcap2socks) (vendored) | [zhxie/pcap2socks](https://github.com/zhxie/pcap2socks) |
| `v2ray-sn.exe` | [`Other/v2ray-sn`](../Other/v2ray-sn) | [SagerNet/v2ray-core](https://github.com/SagerNet/v2ray-core) |

## Downloaded at build time
- `wintun.dll`: [WinTUN](https://www.wintun.net) 0.13
- `GeoLite2-Country.mmdb`: [Loyalsoldier/geoip](https://github.com/Loyalsoldier/geoip) `release` branch (rolling, so not hash-pinned)

## Bundled files (provenance)

| File | Version / origin | SHA256 |
| :- | :- | :- |
| `tun2socks.bin` | Original closed-source DLL (module `rixcloud.moe/aiocloud/tun2socks`, go1.17.1, not upstream xjasonlyu). Kept as reference/fallback: the release build now compiles `Other/tun2socks` (recovered source, see its `RECOVERY.md`) and ships that instead | `1eaca52e1b73491747df71403fdd1228481c7f43691af623aac29889619f0d23` |
| `nfdriver.sys` | Netfilter SDK driver, file version 1.6.3.0, signed prebuilt. `Redirector` (nfapi) is tied to it: do not swap one without the other | `4af6f672119f4f13e33b8914630eedea97d299b5c92c2105a4015dd0ca6e933e` |
| `aiodns.conf` | rule data for aiodns | `d161dbd19b3dc9e20904e5f48d5f6757e9c28d33cb9aeb031495c678c0467e95` |
| `stun.txt` | default STUN servers | `f51c11fa9f15c2bd64f0094e6c45be0186dbb46d9e7997f3d9ef09dd830d0b52` |

