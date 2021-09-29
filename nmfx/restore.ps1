[CmdletBinding()]
param (
    [Parameter()]
    [switch]
    $Install
)

if($Install)
{
    $mod = Get-Module "PsDepend" -ListAvailable -EA SilentlyContinue
    if(!$mod)
    {
        Install-Module "PsDepend" -Force
    }

    Invoke-PSDepend "$PsScriptRoot/requirements.psd1" -Force
}

Set-Alias "psake" "invoke-psake"
Set-Alias "psdepend" "invoke-psdepend"