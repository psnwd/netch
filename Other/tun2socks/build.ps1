Set-Location (Split-Path $MyInvocation.MyCommand.Path -Parent)

# ponytail: the original tun2socks.bin was built with go1.17.1; CI installs it and exports GO117 (its GOROOT)
$go = if ( $Env:GO117 ) { Join-Path $Env:GO117 'bin\go.exe' } else { 'go' }

$Env:CGO_ENABLED='1'
$Env:GOROOT_FINAL='/usr'

$Env:GOOS='windows'
$Env:GOARCH='amd64'
& $go build -a -buildmode=c-shared -trimpath -asmflags '-s -w' -ldflags '-s -w' -o '..\release\tun2socks.bin'
exit $lastExitCode
