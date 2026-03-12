# Download Prerequisites for Magidesk Installer
# This script downloads all required prerequisite installers to the redist/ directory

param(
    [switch]$Force = $false  # Force re-download even if files exist
)

$ErrorActionPreference = "Stop"

# Define download URLs and target filenames
$prerequisites = @(
    @{
        Name = ".NET 8 Desktop Runtime (x64)"
        Url = "https://download.visualstudio.microsoft.com/download/pr/4a5c3a36-e1e6-4d3e-8e8e-a5f6c3e3e3e3/windowsdesktop-runtime-8.0.12-win-x64.exe"
        FileName = "windowsdesktop-runtime-8.0.12-win-x64.exe"
        MinSize = 50MB
        MaxSize = 150MB
    },
    @{
        Name = "Windows App SDK 1.6 Runtime"
        Url = "https://aka.ms/windowsappsdk/1.6/latest/windowsappruntimeinstall-x64.exe"
        FileName = "Microsoft.WindowsAppRuntime.1.6.msix"
        MinSize = 10MB
        MaxSize = 100MB
        Note = "May need manual download and rename from .exe to .msix"
    },
    @{
        Name = "Visual C++ Redistributable (x64)"
        Url = "https://aka.ms/vs/17/release/vc_redist.x64.exe"
        FileName = "VC_redist.x64.exe"
        MinSize = 10MB
        MaxSize = 50MB
    },
    @{
        Name = "PostgreSQL 16 Windows x64 Binaries"
        Url = "https://get.enterprisedb.com/postgresql/postgresql-16.6-1-windows-x64-binaries.zip"
        FileName = "postgresql-16-windows-x64.zip"
        MinSize = 100MB
        MaxSize = 500MB
    }
)

# Determine project root and redist directory
$scriptDir = Split-Path -Parent $PSCommandPath
$projectRoot = Split-Path -Parent (Split-Path -Parent $scriptDir)
$redistDir = Join-Path $projectRoot "redist"

Write-Host "Magidesk Installer - Prerequisite Downloader" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Project Root: $projectRoot"
Write-Host "Redist Directory: $redistDir"
Write-Host ""

# Create redist directory if it doesn't exist
if (-not (Test-Path $redistDir)) {
    Write-Host "Creating redist directory..." -ForegroundColor Yellow
    New-Item -ItemType Directory -Path $redistDir -Force | Out-Null
}


# Download each prerequisite
$downloadResults = @()

foreach ($prereq in $prerequisites) {
    $targetPath = Join-Path $redistDir $prereq.FileName
    $result = @{
        Name = $prereq.Name
        FileName = $prereq.FileName
        Success = $false
        Message = ""
    }
    
    Write-Host "Processing: $($prereq.Name)" -ForegroundColor Cyan
    Write-Host "  Target: $($prereq.FileName)"
    
    # Check if file already exists
    if ((Test-Path $targetPath) -and -not $Force) {
        $fileSize = (Get-Item $targetPath).Length
        $fileSizeMB = [math]::Round($fileSize / 1MB, 2)
        
        if ($fileSize -ge $prereq.MinSize -and $fileSize -le $prereq.MaxSize) {
            Write-Host "  Status: Already exists ($fileSizeMB MB) - Skipping" -ForegroundColor Green
            $result.Success = $true
            $result.Message = "Already exists ($fileSizeMB MB)"
        } else {
            Write-Host "  Status: File exists but size is unexpected ($fileSizeMB MB)" -ForegroundColor Yellow
            Write-Host "  Expected: $([math]::Round($prereq.MinSize / 1MB, 2)) MB - $([math]::Round($prereq.MaxSize / 1MB, 2)) MB"
            $result.Message = "File exists but size is unexpected"
        }
    } else {
        # Download the file
        Write-Host "  Downloading from: $($prereq.Url)" -ForegroundColor Yellow
        
        try {
            # Use Invoke-WebRequest with progress
            $ProgressPreference = 'SilentlyContinue'  # Faster downloads
            Invoke-WebRequest -Uri $prereq.Url -OutFile $targetPath -UseBasicParsing
            $ProgressPreference = 'Continue'
            
            # Verify file size
            $fileSize = (Get-Item $targetPath).Length
            $fileSizeMB = [math]::Round($fileSize / 1MB, 2)
            
            if ($fileSize -ge $prereq.MinSize -and $fileSize -le $prereq.MaxSize) {
                Write-Host "  Status: Downloaded successfully ($fileSizeMB MB)" -ForegroundColor Green
                $result.Success = $true
                $result.Message = "Downloaded successfully ($fileSizeMB MB)"
            } else {
                Write-Host "  Status: Downloaded but size is unexpected ($fileSizeMB MB)" -ForegroundColor Yellow
                Write-Host "  Expected: $([math]::Round($prereq.MinSize / 1MB, 2)) MB - $([math]::Round($prereq.MaxSize / 1MB, 2)) MB"
                $result.Message = "Downloaded but size is unexpected"
            }
        } catch {
            Write-Host "  Status: Download failed - $($_.Exception.Message)" -ForegroundColor Red
            $result.Message = "Download failed: $($_.Exception.Message)"
            
            if ($prereq.Note) {
                Write-Host "  Note: $($prereq.Note)" -ForegroundColor Yellow
            }
        }
    }
    
    $downloadResults += $result
    Write-Host ""
}


# Display summary
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "Download Summary" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

$successCount = ($downloadResults | Where-Object { $_.Success }).Count
$totalCount = $downloadResults.Count

foreach ($result in $downloadResults) {
    $status = if ($result.Success) { "✓" } else { "✗" }
    $color = if ($result.Success) { "Green" } else { "Red" }
    
    Write-Host "$status $($result.Name)" -ForegroundColor $color
    Write-Host "  File: $($result.FileName)"
    Write-Host "  Status: $($result.Message)"
    Write-Host ""
}

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "Results: $successCount/$totalCount prerequisites ready" -ForegroundColor $(if ($successCount -eq $totalCount) { "Green" } else { "Yellow" })
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

if ($successCount -lt $totalCount) {
    Write-Host "Some prerequisites failed to download. Please:" -ForegroundColor Yellow
    Write-Host "  1. Check your internet connection"
    Write-Host "  2. Verify the download URLs are still valid"
    Write-Host "  3. Download missing files manually (see redist/README.md)"
    Write-Host ""
    exit 1
} else {
    Write-Host "All prerequisites are ready for installer build!" -ForegroundColor Green
    Write-Host ""
    exit 0
}
