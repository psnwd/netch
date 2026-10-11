# Performance results

CPU = % of one core summed over Netrix, v2ray-sn during the run. Url/ping host fixed in bench.ps1 defaults.

| date | label | 1 stream Mbps | CPU% | 16 streams Mbps | CPU% | ping ms | dns ms | mem MB | 40 small req ms |
|---|---|---|---|---|---|---|---|---|---|
| 2026-10-11 12:06 | direct-baseline | 178.8 | 0 | 34.1 | 0 | 5.1 | 86 | 0 | n/a |
| 2026-10-11 12:29 | direct-with-small | 87.4 | 7 | 270.3 | 16 | 0.0 | 728 | 169 | 321 |
| 2026-10-11 17:39 | tun-current-running-build | 144 | 12 | 14.6 | 2 | 0.0 | 105 | 377 | 1667 |
| 2026-10-11 17:41 | tun-current-running-build-repeat | 148.4 | 1 | 29.3 | 2 | 0.0 | 661 | 361 | 532 |
| 2026-10-11 17:49 | old-r2 | 25.3 | 6 | 7.3 | 8 | 0.0 | 247 | 340 | 1528 |
| 2026-10-11 17:55 | old-r1 | 55 | 17 | 1.7 | 8 | 0.0 | 80 | 349 | 1564 |
