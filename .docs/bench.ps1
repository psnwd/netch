# Benchmark one path (direct / TUN / WebMode). Switch Netch mode by hand, then run:
#   pwsh .docs\bench.ps1 -Label "tun-baseline"
# Appends a row to .docs\PERF.md. Keep -Url/-PingHost fixed so builds compare.
param(
    [Parameter(Mandatory)][string]$Label,
    [string]$Url = 'https://proof.ovh.net/files/100Mb.dat',
    [string]$SmallUrl = 'https://www.cloudflare.com/cdn-cgi/trace',
    [string]$PingHost = '1.1.1.1',
    [string]$DnsName = 'example.com',
    [int]$Streams = 16,
    [string[]]$Procs = @('Netch', 'v2ray-sn')
)
$ErrorActionPreference = 'Stop'

function Get-Cpu { ($Procs | ForEach-Object { Get-Process $_ -ErrorAction Ignore } | Measure-Object CPU -Sum).Sum }
function Get-MemMB { [math]::Round((($Procs | ForEach-Object { Get-Process $_ -ErrorAction Ignore } | Measure-Object WorkingSet64 -Sum).Sum) / 1MB) }

function Get-Mbps([int]$n) {
    $cpu0 = Get-Cpu
    $sw = [Diagnostics.Stopwatch]::StartNew()
    $bytes = 1..$n | ForEach-Object -Parallel {
        [long](curl.exe -s -o NUL -w '%{size_download}' --max-time 60 $using:Url)
    } -ThrottleLimit $n | Measure-Object -Sum
    $sw.Stop()
    if (-not $bytes.Sum) { throw "download failed: $Url" }
    [pscustomobject]@{
        Mbps = [math]::Round($bytes.Sum * 8 / 1e6 / $sw.Elapsed.TotalSeconds, 1)
        Cpu  = [math]::Round(((Get-Cpu) - $cpu0) / $sw.Elapsed.TotalSeconds * 100) # % of one core
    }
}

$ping = (Test-Connection $PingHost -Count 10 | Measure-Object Latency -Average).Average
$dns = (1..5 | ForEach-Object { (Measure-Command { Resolve-DnsName $DnsName -DnsOnly -NoHostsFile | Out-Null }).TotalMilliseconds } | Measure-Object -Average).Average
$one = Get-Mbps 1
$many = Get-Mbps $Streams
# 40 tiny requests, 8 at a time, each a new connection: shows connection setup cost (what mux and TCP fast open change)
$small = [math]::Round((Measure-Command { 1..40 | ForEach-Object -Parallel { curl.exe -s -o NUL --max-time 30 $using:SmallUrl } -ThrottleLimit 8 }).TotalMilliseconds)

$perf = Join-Path $PSScriptRoot 'PERF.md'
if (-not (Test-Path $perf)) {
    "# Performance results`n`nCPU = % of one core summed over $($Procs -join ', ') during the run. Url/ping host fixed in bench.ps1 defaults.`n`n| date | label | 1 stream Mbps | CPU% | $Streams streams Mbps | CPU% | ping ms | dns ms | mem MB | 40 small req ms |`n|---|---|---|---|---|---|---|---|---|---|" | Set-Content $perf
}
$row = '| {0:yyyy-MM-dd HH:mm} | {1} | {2} | {3} | {4} | {5} | {6:N1} | {7:N0} | {8} | {9} |' -f (Get-Date), $Label, $one.Mbps, $one.Cpu, $many.Mbps, $many.Cpu, $ping, $dns, (Get-MemMB), $small
Add-Content $perf $row
$row
