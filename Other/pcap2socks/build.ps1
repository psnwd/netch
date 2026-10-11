Set-Location (Split-Path $MyInvocation.MyCommand.Path -Parent)

# pnet links against Packet.lib. The Npcap SDK ships it (needed at build time only, Npcap itself is needed at run time).
$sdkUrl = 'https://npcap.com/dist/npcap-sdk-1.16.zip'
$sdkSha256 = 'f0a8be7778ee3ae1b99bbbecb27a3ff0f6c111a4093f1c78c5c5a099607184db'

try {
    Invoke-WebRequest -Uri $sdkUrl -OutFile 'npcap-sdk.zip'

    if ( (Get-FileHash 'npcap-sdk.zip' -Algorithm SHA256).Hash -ne $sdkSha256 ) {
        throw 'npcap-sdk.zip has an unexpected SHA256'
    }

    Expand-Archive -Force -Path 'npcap-sdk.zip' -DestinationPath 'npcap-sdk'
}
catch {
    Write-Host $_
    exit 1
}

$Env:LIB = "$PWD\npcap-sdk\Lib\x64;$Env:LIB"

Set-Location 'crate'
cargo build --release --locked
if ( -Not $? ) {
    exit 1
}

cp -Force 'target\release\pcap2socks.exe' '..\..\release\pcap2socks.exe'
exit 0
