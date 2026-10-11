param (
	[Parameter()]
	[ValidateSet('Debug', 'Release')]
	[string]
	$Configuration = 'Release',

	# default: .build\dev, or .build\release with -Release
	[Parameter()]
	[string]
	$OutputPath = '',

	# build into .build\release (what CI packages) instead of .build\dev
	[Parameter()]
	[switch]
	$Release,

	[Parameter()]
	[bool]
	$SelfContained = $True,

	[Parameter()]
	[bool]
	$PublishSingleFile = $True,

	[Parameter()]
	[bool]
	$PublishReadyToRun = $False,

	# empty = whatever the .vcxproj says (v143, VS 2022); pass v145 on a machine that only has Visual Studio 2026
	[Parameter()]
	[string]
	$PlatformToolset = ''
)

Push-Location (Split-Path $MyInvocation.MyCommand.Path -Parent)

$root = (Get-Location).Path
if ( -Not $OutputPath ) { $OutputPath = if ( $Release ) { '.build\release' } else { '.build\dev' } }

$toolsetArgs = @()
if ( $PlatformToolset ) { $toolsetArgs = @("-property:PlatformToolset=$PlatformToolset") }

# keep data\ (settings, when Netrix runs from the output folder); everything else is replaced
if ( Test-Path -Path $OutputPath ) {
    Get-ChildItem $OutputPath -Force -Exclude 'data' | Remove-Item -Recurse -Force
} else {
    New-Item -ItemType Directory -Force -Path $OutputPath | Out-Null
}

Push-Location $OutputPath
New-Item -ItemType Directory -Name 'bin'  | Out-Null
cp -Recurse -Force "$root\Storage\i18n" '.'  | Out-Null
cp -Recurse -Force "$root\Storage\mode" '.'  | Out-Null
cp -Recurse -Force "$root\Storage\stun.txt" 'bin'  | Out-Null
cp -Recurse -Force "$root\Storage\nfdriver.sys" 'bin'  | Out-Null
cp -Recurse -Force "$root\Storage\aiodns.conf" 'bin'  | Out-Null
Invoke-WebRequest -Uri 'https://raw.githubusercontent.com/Loyalsoldier/geoip/release/Country.mmdb' -ErrorAction Stop -OutFile 'bin\GeoLite2-Country.mmdb'
#cp -Recurse -Force "$root\Storage\GeoLite2-Country.mmdb" 'bin'  | Out-Null
cp -Recurse -Force "$root\Storage\tun2socks.bin" 'bin'  | Out-Null
cp -Recurse -Force "$root\Storage\README.md" 'bin'  | Out-Null
Pop-Location

if ( -Not ( Test-Path '.\Other\release' ) ) {
	.\Other\build.ps1
	if ( -Not $? ) {
		exit $lastExitCode
	}
}
cp -Force '.\Other\release\*.bin' "$OutputPath\bin"
cp -Force '.\Other\release\*.dll' "$OutputPath\bin"
cp -Force '.\Other\release\*.exe' "$OutputPath\bin"

if ( -Not ( Test-Path ".\Netrix\bin\$Configuration" ) ) {
	Write-Host
	Write-Host 'Building Netrix'

	dotnet publish `
		-c $Configuration `
		-r 'win-x64' `
		-p:Platform='x64' `
		-p:SelfContained=$SelfContained `
		-p:PublishTrimmed=$PublishReadyToRun `
		-p:PublishSingleFile=$PublishSingleFile `
		-p:PublishReadyToRun=$PublishReadyToRun `
		-p:PublishReadyToRunShowWarnings=$PublishReadyToRun `
		-p:IncludeNativeLibrariesForSelfExtract=$SelfContained `
		-o ".\Netrix\bin\$Configuration" `
		'.\Netrix\Netrix.csproj'
	if ( -Not $? ) { exit $lastExitCode }
}
cp -Force ".\Netrix\bin\$Configuration\Netrix.exe" $OutputPath

if ( -Not ( Test-Path ".\Redirector\bin\$Configuration" ) ) {
	Write-Host
	Write-Host 'Building Redirector'

	msbuild `
		-property:Configuration=$Configuration `
		-property:Platform=x64 `
		@toolsetArgs `
		'.\Redirector\Redirector.vcxproj'
	if ( -Not $? ) { exit $lastExitCode }
}
cp -Force ".\Redirector\bin\$Configuration\nfapi.dll"      "$OutputPath\bin"
cp -Force ".\Redirector\bin\$Configuration\Redirector.bin" "$OutputPath\bin"

if ( -Not ( Test-Path ".\RouteHelper\bin\$Configuration" ) ) {
	Write-Host
	Write-Host 'Building RouteHelper'

	msbuild `
		-property:Configuration=$Configuration `
		-property:Platform=x64 `
		@toolsetArgs `
		'.\RouteHelper\RouteHelper.vcxproj'
	if ( -Not $? ) { exit $lastExitCode }
}
cp -Force ".\RouteHelper\bin\$Configuration\RouteHelper.bin" "$OutputPath\bin"

if ( $Configuration.Equals('Release') ) {
	rm -Force "$OutputPath\*.pdb"
	rm -Force "$OutputPath\*.xml"
}

Pop-Location
exit 0
