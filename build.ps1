[CmdletBinding()]
param(
    [string]$SPTPath = $env:SPT_PATH,
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',
    [switch]$NoRestore
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path

if ([string]::IsNullOrWhiteSpace($SPTPath)) {
    throw 'SPTPath is required. Pass -SPTPath <path> or set the SPT_PATH environment variable.'
}

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

$buildArguments = @(
    'build',
    (Join-Path $repoRoot 'ScrollableAttachments\ScrollableAttachments.csproj'),
    '--configuration',
    $Configuration,
    "-p:SPTPath=$resolvedSptPath"
)

if ($NoRestore) {
    $buildArguments += '--no-restore'
}

& dotnet @buildArguments
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}
