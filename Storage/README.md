# Netch
Prebuilt binaries for Netch version 1

## aiodns
https://github.com/netchx/netch

## Redirector
https://github.com/netchx/netch

## RouteHelper
https://github.com/netchx/netch

## ck-client
https://github.com/cbeuw/Cloak

## GeoLite2
https://www.maxmind.com

## xray-core
https://github.com/xtls/xray-core

## shadowsocks-libev
https://github.com/shadowsocks/shadowsocks-rust

## shadowsocksr-libev
https://github.com/shadowsocksrr/shadowsocksr-libev

## simple-obfs
https://github.com/shadowsocks/simple-obfs

## tun2socks
https://github.com/xjasonlyu/tun2socks

## v2ray-plugin
https://github.com/teddysun/v2ray-plugin

## WinTUN
https://www.wintun.net

## Bundled files (provenance)

| File | Version / origin | SHA256 |
| :- | :- | :- |
| `tun2socks.bin` | c-shared build of xjasonlyu/tun2socks (Netch-specific exports, see `Netch/Interops/tun2socks.cs`); built with go1.17.1 per embedded string. Source of the Netch patch is not in this repo, so it cannot be refreshed from an upstream release | `1eaca52e1b73491747df71403fdd1228481c7f43691af623aac29889619f0d23` |
| `nfdriver.sys` | Netfilter SDK driver, file version 1.6.3.0, signed prebuilt. `Redirector` (nfapi) is tied to it: do not swap one without the other | `4af6f672119f4f13e33b8914630eedea97d299b5c92c2105a4015dd0ca6e933e` |
| `aiodns.conf` | rule data for aiodns | `d161dbd19b3dc9e20904e5f48d5f6757e9c28d33cb9aeb031495c678c0467e95` |
| `stun.txt` | default STUN servers | `8e80f86f425e182e433a6f8305d94ef302d8d66cc958b2ec50112ab8eb50c377` |

Built from source in this repo by `build.ps1` / `Other/`: `aiodns.bin` (Go, miekg/dns 1.1.73), `Redirector.bin` + `nfapi.dll`, `RouteHelper.bin`. Downloaded at build time: `GeoLite2-Country.mmdb` (Loyalsoldier/geoip `release` branch, rolling, so not hash-pinned), wintun 0.13, pcap2socks v0.6.2.
