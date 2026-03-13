# build-installer.ps1
# Magidesk POS Installer Build Script
# This script builds the complete installer package including all prerequisites

param(
    [string]$Configuration = "Release",
    [string]$Platform = "x64"
)

$ErrorActionPreference = "Stop"
$SolutionDir = $PSScriptRoot
$SolutionFile = "$SolutionDir\src\Magidesk.sln"

Write-Host "=== Magidesk Installer Build ===" -ForegroundColor Cyan
Write-Host "Configuration: $Configuration" -ForegroundColor Gray
Write-Host "Platform: $Platform" -ForegroundColor Gray
Write-Host ""

# ============================================================================
# Step 1: Build Solution
# ============================================================================
Write-Host "[1/1] Building solution..." -ForegroundColor Yellow

if (-not (Test-Path $SolutionFile)) {
    Write-Error "Solution file not found: $SolutionFile"
    exit 1
}

# Build only the Presentation project (main application) using dotnet build
# We use dotnet build here instead of build-xaml.ps1 because:
# 1. The installer only needs the Presentation project output
# 2. build-xaml.ps1 builds the entire solution including test projects
# 3. Test projects have architecture mismatches that don't affect the installer
$PresentationProject = "$SolutionDir\src\Magidesk.Presentation\Magidesk.Presentation.csproj"

if (-not (Test-Path $PresentationProject)) {
    Write-Error "Presentation project not found: $PresentationProject"
    exit 1
}

Write-Host "  Running: dotnet build $PresentationProject --configuration $Configuration" -ForegroundColor Gray

dotnet build $PresentationProject `
    --configuration $Configuration `
    --verbosity minimal

if ($LASTEXITCODE -ne 0) {
    Write-Error "Presentation project build failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "  Solution build completed successfully" -ForegroundColor Green
Write-Host ""

# ============================================================================
# Step 2: Publish Application
# ============================================================================
Write-Host "[2/6] Publishing application..." -ForegroundColor Yellow

$StagingDir = "$SolutionDir\build\installer\staging"
$AppStagingDir = "$StagingDir\app"

# Create staging directories
if (Test-Path $StagingDir) {
    Remove-Item $StagingDir -Recurse -Force
}
New-Item -ItemType Directory -Path $AppStagingDir -Force | Out-Null

Write-Host "  Running: dotnet publish $PresentationProject" -ForegroundColor Gray

dotnet publish $PresentationProject `
    --configuration $Configuration `
    --runtime win-x64 `
    --self-contained false `
    --output $AppStagingDir `
    --verbosity minimal

if ($LASTEXITCODE -ne 0) {
    Write-Error "Application publish failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "  Application published to: $AppStagingDir" -ForegroundColor Green
Write-Host ""

# ============================================================================
# Step 3: Generate EF Core Migration Bundle
# ============================================================================
Write-Host "[3/6] Generating EF Core migration bundle..." -ForegroundColor Yellow

$MigrationsProject = "$SolutionDir\src\Magidesk.Migrations\Magidesk.Migrations.csproj"
$ToolsDir = "$StagingDir\tools"
$BundlePath = "$ToolsDir\efbundle.exe"

New-Item -ItemType Directory -Path $ToolsDir -Force | Out-Null

Write-Host "  Running: dotnet ef migrations bundle" -ForegroundColor Gray

# Use Magidesk.Migrations as the project (where migrations are stored)
# Use Magidesk.Migrations as startup project (it has the design-time factory)
dotnet ef migrations bundle `
    --project $MigrationsProject `
    --startup-project $MigrationsProject `
    --output $BundlePath `
    --configuration $Configuration `
    --force `
    --verbose

if ($LASTEXITCODE -ne 0) {
    Write-Error "EF Core migration bundle generation failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "  Migration bundle created: $BundlePath" -ForegroundColor Green
Write-Host ""

# ============================================================================
# Step 4: Copy Prerequisites
# ============================================================================
Write-Host "[4/6] Copying prerequisites..." -ForegroundColor Yellow

$RedistDir = "$SolutionDir\redist"
$PrereqDir = "$StagingDir\prerequisites"

New-Item -ItemType Directory -Path $PrereqDir -Force | Out-Null

# Check if redist directory exists
if (-not (Test-Path $RedistDir)) {
    Write-Warning "Redist directory not found: $RedistDir"
    Write-Warning "Prerequisites will need to be downloaded manually to: $RedistDir"
    Write-Warning "See redist/README.md for download instructions"
} else {
    # Copy .NET Runtime
    $NetRuntimePattern = "$RedistDir\windowsdesktop-runtime-*-win-x64.exe"
    $NetRuntimeFiles = Get-ChildItem $NetRuntimePattern -ErrorAction SilentlyContinue
    if ($NetRuntimeFiles) {
        Copy-Item $NetRuntimeFiles[0].FullName "$PrereqDir\windowsdesktop-runtime-8.0-win-x64.exe"
        Write-Host "  Copied: .NET 8 Desktop Runtime" -ForegroundColor Gray
    } else {
        Write-Warning "  .NET Runtime not found in redist/"
    }

    # Copy Windows App SDK
    $WinAppSDKPattern = "$RedistDir\Microsoft.WindowsAppRuntime.*.msix"
    $WinAppSDKFiles = Get-ChildItem $WinAppSDKPattern -ErrorAction SilentlyContinue
    if ($WinAppSDKFiles) {
        Copy-Item $WinAppSDKFiles[0].FullName "$PrereqDir\Microsoft.WindowsAppRuntime.1.6.msix"
        Write-Host "  Copied: Windows App SDK" -ForegroundColor Gray
    } else {
        Write-Warning "  Windows App SDK not found in redist/"
    }

    # Copy VC++ Redistributable
    $VCRedistFile = "$RedistDir\VC_redist.x64.exe"
    if (Test-Path $VCRedistFile) {
        Copy-Item $VCRedistFile "$PrereqDir\VC_redist.x64.exe"
        Write-Host "  Copied: VC++ Redistributable" -ForegroundColor Gray
    } else {
        Write-Warning "  VC++ Redistributable not found in redist/"
    }

    # Copy PostgreSQL ZIP
    $PostgreSQLDir = "$StagingDir\postgresql"
    New-Item -ItemType Directory -Path $PostgreSQLDir -Force | Out-Null
    
    $PostgreSQLPattern = "$RedistDir\postgresql-*-windows-x64.zip"
    $PostgreSQLFiles = Get-ChildItem $PostgreSQLPattern -ErrorAction SilentlyContinue
    if ($PostgreSQLFiles) {
        Copy-Item $PostgreSQLFiles[0].FullName "$PostgreSQLDir\postgresql-16-windows-x64.zip"
        Write-Host "  Copied: PostgreSQL 16" -ForegroundColor Gray
    } else {
        Write-Warning "  PostgreSQL ZIP not found in redist/"
    }
}

Write-Host "  Prerequisites copied" -ForegroundColor Green
Write-Host ""

# ============================================================================
# Step 5: Build WiX Installer
# ============================================================================
Write-Host "[5/6] Building WiX installer..." -ForegroundColor Yellow

$InstallerProject = "$SolutionDir\src\Magidesk.Installer\Magidesk.Installer.wixproj"

if (-not (Test-Path $InstallerProject)) {
    Write-Error "Installer project not found: $InstallerProject"
    exit 1
}

Write-Host "  Running: dotnet build $InstallerProject" -ForegroundColor Gray

dotnet build $InstallerProject `
    --configuration $Configuration `
    --verbosity minimal

if ($LASTEXITCODE -ne 0) {
    Write-Error "WiX installer build failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "  WiX installer built successfully" -ForegroundColor Green
Write-Host ""

# ============================================================================
# Step 6: Verify Installer Output
# ============================================================================
Write-Host "[6/6] Verifying installer output..." -ForegroundColor Yellow

# The project is currently configured to build a Package (MSI), not a Bundle (EXE)
# Look for Magidesk.msi in the x64/Release output directory
$InstallerOutput = "$SolutionDir\src\Magidesk.Installer\bin\x64\$Configuration\Magidesk.msi"

if (-not (Test-Path $InstallerOutput)) {
    Write-Error "Installer not found: $InstallerOutput"
    Write-Error "WiX build may have failed silently"
    exit 1
}

$InstallerSize = (Get-Item $InstallerOutput).Length
$InstallerSizeMB = [math]::Round($InstallerSize / 1MB, 2)

Write-Host "  Installer size: $InstallerSizeMB MB" -ForegroundColor Gray
Write-Host "  Installer location: $InstallerOutput" -ForegroundColor Green
Write-Host ""

# ============================================================================
# Build Complete
# ============================================================================
Write-Host "=== Build Complete ===" -ForegroundColor Green
Write-Host "Installer package: $InstallerOutput" -ForegroundColor Cyan
Write-Host "Installer size: $InstallerSizeMB MB" -ForegroundColor Cyan
Write-Host ""
Write-Host "NOTE: This is an MSI package (not a Bundle/EXE)." -ForegroundColor Yellow
Write-Host "Prerequisites (.NET 8, Windows App SDK, VC++ Redistributable) must be installed separately." -ForegroundColor Yellow
