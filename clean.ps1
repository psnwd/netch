Push-Location (Split-Path $MyInvocation.MyCommand.Path -Parent)

function Delete {
    param (
        [string]
        $Path
    )

    if (Test-Path $Path) {
        rm -Recurse -Force $Path | Out-Null
    }
}

Delete '.vs'
Delete 'release'
Delete 'Netrix\bin'
Delete 'Netrix\obj'
Delete 'Tests\bin'
Delete 'Tests\obj'
Delete 'TestResults'
Delete 'Redirector\bin'
Delete 'Redirector\obj'
Delete 'RouteHelper\bin'
Delete 'RouteHelper\obj'

Delete 'Netrix\*.csproj.user'
Delete 'Redirector\*.vcxproj.user'
Delete 'RouteHelper\*.vcxproj.user'

.\other\clean.ps1

Pop-Location
exit $lastExitCode