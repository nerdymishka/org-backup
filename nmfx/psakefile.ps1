#Requires -Module "Psake"

$buildConfiguration = if($Env:BUILD_CONFIGURATION) { $Env:BuildConfiguration } else { "Release" }

if($null -eq $psake.context -or $psake.context.Count -eq 0)
{
    Invoke-psake "$PSScriptRoot/psakefile.ps1"
    return
}


task "build" {
    Exec {
        dotnet build -c $buildConfiguration
    }
}

task "restore" {
    & "$PsScriptRoot/restore.ps1" -Install
}

task "default" -depends "build"