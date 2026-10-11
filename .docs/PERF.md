# Performance results

CPU = % of one core summed over Netch, v2ray-sn during the run. Url/ping host fixed in bench.ps1 defaults.

| date | label | 1 stream Mbps | CPU% | 16 streams Mbps | CPU% | ping ms | dns ms | mem MB | 40 small req ms |
|---|---|---|---|---|---|---|---|---|---|
| 2026-10-11 12:06 | direct-baseline | 178.8 | 0 | 34.1 | 0 | 5.1 | 86 | 0 | n/a |
| 2026-10-11 12:29 | direct-with-small | 87.4 | 7 | 270.3 | 16 | 0.0 | 728 | 169 | 321 |
