# Verify Prerequisites for Magidesk Installer
# This script verifies all required prerequisite files are present and valid

$ErrorActionPreference = "Stop"

# Define expected files and their validation criteria
$expectedFiles = @(
    @{
        Pattern = "windowsdesktop-runtime-8.0.*-win-x64.exe"
        Description = ".NET 8 Desktop Runtime (x64)"
        MinSize = 50MB
        MaxSize = 150MB
    },
    @{
        Pattern = "Microsoft.WindowsAppRuntime.1.6.msix"
        Description = "Windows App SDK 1.6 Runtime"
        MinSize = 10MB
        MaxSize = 100MB
    },
    @{
        Pattern = "VC_redist.x64.exe"
        Description = "Visual C++ Redistributable (x64)"
        MinSize = 10MB
        MaxSize = 50MB
    },
    @{
        Pattern = "postgresql-16-windows-x64.zip"
        Description = "PostgreSQL 16 Windows x64 Binaries"
        MinSize = 100MB
        MaxSize = 500MB
    }
)

# Determine project root and redist directory
$scriptDir = Split-Path -Parent $PSCommandPath
$projectRoot = Split-Path -Parent (Split-Path -Parent $scriptDir)
$redistDir = Join-Path $projectRoot "redist"

Write-Host "Magidesk Installer - Prerequisite Verification" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Redist Directory: $redistDir"
Write-Host ""

# Check if redist directory exists
if (-not (Test-Path $redistDir)) {
    Write-Host "ERROR: Redist directory does not exist!" -ForegroundColor Red
    Write-Host "Expected: $redistDir"
    Write-Host ""
    Write-Host "Please run download-prerequisites.ps1 first."
    exit 1
}

# Verify each prerequisite
$verificationResults = @()
$allValid = $true

foreach ($expected in $expectedFiles) {
    $result = @{
        Description = $expected.Description
        Pattern = $expected.Pattern
        Found = $false
        Valid = $false
        Message = ""
        FilePath = ""
    }
    
    Write-Host "Checking: $($expected.Description)" -ForegroundColor Cyan
    Write-Host "  Pattern: $($expected.Pattern)"
    
    # Find matching files
    $matchingFiles = Get-ChildItem -Path $redistDir -Filter $expected.Pattern -ErrorAction SilentlyContinue
    
    if ($matchingFiles.Count -eq 0) {
        Write-Host "  Status: NOT FOUND" -ForegroundColor Red
        $result.Message = "File not found"
        $allValid = $false
    } elseif ($matchingFiles.Count -gt 1) {
        Write-Host "  Status: MULTIPLE FILES FOUND" -ForegroundColor Yellow
        $result.Found = $true
        $result.Message = "Multiple files match pattern: $($matchingFiles.Name -join ', ')"
        Write-Host "  Files: $($matchingFiles.Name -join ', ')"
    } else {
        $file = $matchingFiles[0]
        $result.Found = $true
        $result.FilePath = $file.FullName
        
        $fileSize = $file.Length
        $fileSizeMB = [math]::Round($fileSize / 1MB, 2)
        
        Write-Host "  File: $($file.Name)"
        Write-Host "  Size: $fileSizeMB MB"
        
        # Validate file size
        if ($fileSize -lt $expected.MinSize) {
            Write-Host "  Status: TOO SMALL (expected >= $([math]::Round($expected.MinSize / 1MB, 2)) MB)" -ForegroundColor Red
            $result.Message = "File too small: $fileSizeMB MB"
            $allValid = $false
        } elseif ($fileSize -gt $expected.MaxSize) {
            Write-Host "  Status: TOO LARGE (expected <= $([math]::Round($expected.MaxSize / 1MB, 2)) MB)" -ForegroundColor Yellow
            $result.Valid = $true
            $result.Message = "File larger than expected: $fileSizeMB MB (may be newer version)"
        } else {
            Write-Host "  Status: VALID" -ForegroundColor Green
            $result.Valid = $true
            $result.Message = "Valid ($fileSizeMB MB)"
        }
    }
    
    $verificationResults += $result
    Write-Host ""
}


# Display summary
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "Verification Summary" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""

$validCount = ($verificationResults | Where-Object { $_.Valid }).Count
$totalCount = $verificationResults.Count

foreach ($result in $verificationResults) {
    $status = if ($result.Valid) { "✓" } else { "✗" }
    $color = if ($result.Valid) { "Green" } elseif ($result.Found) { "Yellow" } else { "Red" }
    
    Write-Host "$status $($result.Description)" -ForegroundColor $color
    Write-Host "  Pattern: $($result.Pattern)"
    Write-Host "  Status: $($result.Message)"
    if ($result.FilePath) {
        Write-Host "  Path: $($result.FilePath)"
    }
    Write-Host ""
}

Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "Results: $validCount/$totalCount prerequisites valid" -ForegroundColor $(if ($allValid) { "Green" } else { "Red" })
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""

if (-not $allValid) {
    Write-Host "Verification failed. Please:" -ForegroundColor Yellow
    Write-Host "  1. Download missing prerequisites (see redist/README.md)"
    Write-Host "  2. Run download-prerequisites.ps1 to automate downloads"
    Write-Host "  3. Verify file integrity if sizes are unexpected"
    Write-Host ""
    exit 1
} else {
    Write-Host "All prerequisites verified successfully!" -ForegroundColor Green
    Write-Host "The installer is ready to be built." -ForegroundColor Green
    Write-Host ""
    exit 0
}
