Set-Location (Split-Path $MyInvocation.MyCommand.Path -Parent)

git clone https://github.com/SagerNet/v2ray-core.git -b 'v5.0.16' src
if ( -Not $? ) {
    exit $lastExitCode
}
Set-Location src

# Download SSR plugin
Invoke-WebRequest -Uri 'https://gist.githubusercontent.com/H1JK/b3165a99b635dcc06101690e4c43b5fd/raw/691b471f3b395a949d03a3d064d93d319d4997b7/ssr.go' -OutFile '.\proxy\shadowsocks\plugin\self\ssr.go'

# Download Simple-Obfs plugin
Invoke-WebRequest -Uri 'https://gist.githubusercontent.com/H1JK/b3165a99b635dcc06101690e4c43b5fd/raw/691b471f3b395a949d03a3d064d93d319d4997b7/obfs.go' -OutFile '.\proxy\shadowsocks\plugin\self\obfs.go'

# Enable ReadV (Use old ReadV code)
Remove-Item '.\common\buf\io.go'
Remove-Item '.\common\buf\readv_reader.go'
Invoke-WebRequest -Uri 'https://raw.githubusercontent.com/SagerNet/v2ray-core/2711fd1/common/buf/io.go' -OutFile '.\common\buf\io.go'
Invoke-WebRequest -Uri 'https://raw.githubusercontent.com/SagerNet/v2ray-core/2711fd1/common/buf/readv_reader.go' -OutFile '.\common\buf\readv_reader.go'

# ponytail: with current Go, CI failed on SagerNet/gvisor (gohacks: "build constraints exclude all Go files").
# Use Go 1.18, the version upstream v2ray-core v5.0.16 CI builds with. CI installs it and exports GO118 (its GOROOT).
$go = if ($Env:GO118) { Join-Path $Env:GO118 'bin\go.exe' } else { 'go' }

$Env:CGO_ENABLED='0'
$Env:GOROOT_FINAL='/usr'

$Env:GOOS='windows'
$Env:GOARCH='amd64'
# ponytail: no "go get -u": build with the dependency versions pinned in go.mod
# ponytail: pin clash for the SSR/obfs patch files above; the newest clash (1.16+) needs Go 1.20 (protobytes), which gVisor rejects
& $go get -d github.com/Dreamacro/clash@v1.11.0
& $go mod tidy
& $go build -a -trimpath -asmflags '-s -w' -ldflags '-s -w -buildid=' -o '..\..\release\v2ray-sn.exe' '.\main'
exit $lastExitCode
