[CmdletBinding()]
param(
    [string]$SPTPath = $env:SPT_PATH,
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [switch]$NoRestore,
    [switch]$NonInteractive
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path

if ([string]::IsNullOrWhiteSpace($SPTPath)) {
    if ($NonInteractive -or [Console]::IsInputRedirected) {
        throw 'SPTPath is required. Pass -SPTPath <path> or set SPT_PATH. For a double-click build, use build.cmd.'
    }

    $SPTPath = Read-Host 'Enter your SPT installation folder (the folder containing EscapeFromTarkov.exe)'
    if ([string]::IsNullOrWhiteSpace($SPTPath)) {
        throw 'No SPT installation folder was provided.'
    }
}

$SPTPath = $SPTPath.Trim().Trim('"')
$resolvedSptPath = (Resolve-Path -LiteralPath $SPTPath -ErrorAction Stop).Path
$requiredPaths = @(
    'EscapeFromTarkov_Data\Managed\Assembly-CSharp.dll',
    'BepInEx\core\BepInEx.dll',
    'BepInEx\plugins\spt\spt-reflection.dll',
    'BepInEx\plugins\UnityToolkit\ZLinq.dll'
)

foreach ($relativePath in $requiredPaths) {
    $requiredPath = Join-Path $resolvedSptPath $relativePath
    if (-not (Test-Path -LiteralPath $requiredPath -PathType Leaf)) {
        throw "SPT installation is missing '$relativePath': $requiredPath"
    }
}

$dotnetCommand = Get-Command dotnet -CommandType Application -ErrorAction SilentlyContinue | Select-Object -First 1
if ($null -eq $dotnetCommand) {
    throw 'The .NET SDK was not found. Install the .NET SDK, reopen the terminal, then run build.cmd again.'
}

$installedSdks = @(& $dotnetCommand.Source --list-sdks)
if ($LASTEXITCODE -ne 0 -or $installedSdks.Count -eq 0) {
    throw 'No .NET SDK is available. Install the .NET SDK (not just the runtime), then run build.cmd again.'
}

# Forward slashes preserve filesystem roots and avoid PowerShell 5.1 escaping
# a closing native-argument quote when a spaced path ends in a backslash.
$msbuildSptPath = $resolvedSptPath.Replace('\', '/')
$buildArguments = @(
    'build',
    (Join-Path $repoRoot 'ScrollableAttachments\ScrollableAttachments.csproj'),
    '--configuration',
    $Configuration,
    "-p:SPTPath=$msbuildSptPath"
)

if ($NoRestore) {
    $buildArguments += '--no-restore'
}

& $dotnetCommand.Source @buildArguments
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}
