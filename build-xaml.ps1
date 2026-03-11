<#
.SYNOPSIS
    Full XAML build using VS Insider's MSBuild — captures errors dotnet build misses.

.DESCRIPTION
    Runs a clean Rebuild via the Visual Studio Insider MSBuild pipeline, which
    invokes the full WinUI 3 XAML compiler (MC*, WMC*, XBF* errors included).
    Outputs a structured summary file agents can parse without noise.

.PARAMETER Configuration
    Build configuration. Default: Debug

.PARAMETER Target
    MSBuild target. Default: Rebuild. Use 'Build' for incremental.

.PARAMETER Project
    Path to .sln or .csproj. Default: src\Magidesk.sln (relative to script root)

.EXAMPLE
    .\build-xaml.ps1
    .\build-xaml.ps1 -Configuration Release
    .\build-xaml.ps1 -Target Build
    .\build-xaml.ps1 -Project src\Magidesk.Presentation\Magidesk.Presentation.csproj
#>
param(
    [string]$Configuration = "Debug",
    [string]$Target        = "Rebuild",
    [string]$Project       = ""
)

$ErrorActionPreference = "Stop"
$ScriptRoot = $PSScriptRoot

# ── Resolve project path ────────────────────────────────────────────────────
if (-not $Project) {
    $Project = Join-Path $ScriptRoot "src\Magidesk.sln"
}
if (-not [System.IO.Path]::IsPathRooted($Project)) {
    $Project = Join-Path $ScriptRoot $Project
}
if (-not (Test-Path $Project)) {
    Write-Error "Project not found: $Project"
    exit 1
}

# ── Locate MSBuild from VS Insider ──────────────────────────────────────────
$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
if (-not (Test-Path $vswhere)) {
    Write-Error "vswhere.exe not found at: $vswhere`nIs Visual Studio installed?"
    exit 1
}

$msbuild = & $vswhere -prerelease -all -products * `
    -requires Microsoft.Component.MSBuild `
    -find "MSBuild\**\Bin\MSBuild.exe" |
    Select-Object -First 1

if (-not $msbuild -or -not (Test-Path $msbuild)) {
    Write-Error "MSBuild.exe not found via vswhere. Is VS Insider installed with MSBuild component?"
    exit 1
}

Write-Host ""
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "  MAGIDESK XAML BUILD" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "  MSBuild : $msbuild"
Write-Host "  Project : $Project"
Write-Host "  Config  : $Configuration"
Write-Host "  Target  : $Target"
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# ── Prepare log directory ───────────────────────────────────────────────────
$logDir = Join-Path $ScriptRoot "diagnostics\build-logs"
New-Item -ItemType Directory -Force -Path $logDir | Out-Null

$timestamp  = Get-Date -Format "yyyyMMdd_HHmmss"
$fullLog    = Join-Path $logDir "build_full_$timestamp.log"
$errorLog   = Join-Path $logDir "build_errors_$timestamp.log"
$summaryLog = Join-Path $logDir "build_summary_$timestamp.txt"
$latestLog  = Join-Path $logDir "build_summary_LATEST.txt"

# ── Run MSBuild ─────────────────────────────────────────────────────────────
$buildArgs = @(
    $Project,
    "/t:$Target",
    "/p:Configuration=$Configuration",
    "/p:Platform=x64",
    "/p:AppxBundle=Never",
    "/p:GenerateAppxPackageOnBuild=false",
    "/p:DeployOnBuild=false",
    "/flp:LogFile=$fullLog;Verbosity=Detailed",
    "/flp2:LogFile=$errorLog;ErrorsOnly;WarningsOnly",
    "/m",
    "/nologo"
)

Write-Host "Running MSBuild..." -ForegroundColor Yellow
& $msbuild @buildArgs
$exitCode = $LASTEXITCODE

# ── Parse results ────────────────────────────────────────────────────────────
$errorLines = @()
if (Test-Path $errorLog) {
    $errorLines = Get-Content $errorLog
}

$errors   = $errorLines | Where-Object { $_ -match "\s+error\s+" }
$warnings = $errorLines | Where-Object { $_ -match "\s+warning\s+" }

# Categorise XAML-specific errors
$xamlErrors = $errorLines | Where-Object {
    $_ -match "(MC\d+|WMC\d+|XBF\d+|XAML\d*|\.xaml.*error|error.*\.xaml)"
}

$status = if ($exitCode -eq 0) { "SUCCESS" } else { "FAILED" }
$color  = if ($exitCode -eq 0) { "Green" }   else { "Red"   }

# ── Write summary ─────────────────────────────────────────────────────────────
$summary = @"
BUILD RESULT  : $status
Exit Code     : $exitCode
Configuration : $Configuration
Target        : $Target
Timestamp     : $timestamp

Errors        : $($errors.Count)
Warnings      : $($warnings.Count)
XAML Errors   : $($xamlErrors.Count)

Full Log      : $fullLog
Error Log     : $errorLog

"@

if ($errorLines.Count -gt 0) {
    $summary += "--- ALL ERRORS AND WARNINGS ---`r`n"
    $summary += ($errorLines | Out-String)
    $summary += "`r`n"
}

if ($xamlErrors.Count -gt 0) {
    $summary += "--- XAML-SPECIFIC ERRORS (MC* WMC* XBF*) ---`r`n"
    $summary += ($xamlErrors | Out-String)
}

$summary | Out-File $summaryLog  -Encoding utf8
$summary | Out-File $latestLog   -Encoding utf8   # always overwritten — easy agent access

# ── Print to console ──────────────────────────────────────────────────────────
Write-Host ""
Write-Host "==========================================" -ForegroundColor $color
Write-Host "  BUILD $status" -ForegroundColor $color
Write-Host "==========================================" -ForegroundColor $color
Write-Host "  Errors   : $($errors.Count)"
Write-Host "  Warnings : $($warnings.Count)"
Write-Host "  XAML Err : $($xamlErrors.Count)"
Write-Host ""

if ($errorLines.Count -gt 0) {
    Write-Host "--- ERRORS / WARNINGS ---" -ForegroundColor Yellow
    $errorLines | ForEach-Object { Write-Host $_ }
    Write-Host ""
}

Write-Host "  Summary  : $latestLog"
Write-Host "  Full log : $fullLog"
Write-Host "==========================================" -ForegroundColor $color

exit $exitCode
