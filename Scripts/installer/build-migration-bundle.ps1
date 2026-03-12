# Build EF Core Migration Bundle for Magidesk Installer
# This script generates a self-contained migration bundle (efbundle.exe) for database deployment

param(
    [switch]$Force = $false  # Force rebuild even if bundle exists
)

$ErrorActionPreference = "Stop"

# Determine project paths
$scriptDir = Split-Path -Parent $PSCommandPath
$projectRoot = Split-Path -Parent (Split-Path -Parent $scriptDir)
$infrastructureProject = Join-Path $projectRoot "src\Magidesk.Infrastructure\Magidesk.Infrastructure.csproj"
$startupProject = Join-Path $projectRoot "src\Magidesk.Presentation\Magidesk.Presentation.csproj"
$stagingDir = Join-Path $projectRoot "build\installer\staging"
$toolsDir = Join-Path $stagingDir "tools"
$bundleOutput = Join-Path $toolsDir "efbundle.exe"

Write-Host "Magidesk Installer - EF Core Migration Bundle Builder" -ForegroundColor Cyan
Write-Host "======================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Project Root: $projectRoot"
Write-Host "Infrastructure Project: $infrastructureProject"
Write-Host "Startup Project: $startupProject"
Write-Host "Output: $bundleOutput"
Write-Host ""

# Verify projects exist
if (-not (Test-Path $infrastructureProject)) {
    Write-Host "ERROR: Infrastructure project not found!" -ForegroundColor Red
    Write-Host "Expected: $infrastructureProject"
    exit 1
}

if (-not (Test-Path $startupProject)) {
    Write-Host "ERROR: Startup project not found!" -ForegroundColor Red
    Write-Host "Expected: $startupProject"
    exit 1
}

# Create staging directories if they don't exist
if (-not (Test-Path $stagingDir)) {
    Write-Host "Creating staging directory..." -ForegroundColor Yellow
    New-Item -ItemType Directory -Path $stagingDir -Force | Out-Null
}

if (-not (Test-Path $toolsDir)) {
    Write-Host "Creating tools directory..." -ForegroundColor Yellow
    New-Item -ItemType Directory -Path $toolsDir -Force | Out-Null
}

# Check if bundle already exists
if ((Test-Path $bundleOutput) -and -not $Force) {
    $bundleSize = (Get-Item $bundleOutput).Length
    $bundleSizeMB = [math]::Round($bundleSize / 1MB, 2)
    
    Write-Host "Migration bundle already exists ($bundleSizeMB MB)" -ForegroundColor Green
    Write-Host "Use -Force to rebuild"
    Write-Host ""
    exit 0
}

# Check if dotnet ef tool is installed
Write-Host "Checking for dotnet-ef tool..." -ForegroundColor Cyan
$efToolCheck = dotnet tool list --global | Select-String "dotnet-ef"

if (-not $efToolCheck) {
    Write-Host "dotnet-ef tool not found. Installing..." -ForegroundColor Yellow
    try {
        dotnet tool install --global dotnet-ef
        Write-Host "dotnet-ef tool installed successfully" -ForegroundColor Green
    } catch {
        Write-Host "ERROR: Failed to install dotnet-ef tool" -ForegroundColor Red
        Write-Host $_.Exception.Message
        exit 1
    }
} else {
    Write-Host "dotnet-ef tool is installed" -ForegroundColor Green
}
Write-Host ""

# Generate migration bundle
Write-Host "Generating EF Core migration bundle..." -ForegroundColor Cyan
Write-Host "This may take a few minutes..." -ForegroundColor Yellow
Write-Host ""

try {
    # Build the command - using framework-dependent bundle to avoid WinUI runtime issues
    # The installer will ensure .NET 8 runtime is present before running the bundle
    $efCommand = "dotnet ef migrations bundle " +
                 "--project `"$infrastructureProject`" " +
                 "--startup-project `"$startupProject`" " +
                 "--output `"$bundleOutput`" " +
                 "--configuration Release " +
                 "--force " +
                 "--verbose"
    
    Write-Host "Executing: $efCommand" -ForegroundColor Gray
    Write-Host ""
    
    # Execute the command and capture output
    $output = & cmd /c "dotnet ef migrations bundle --project `"$infrastructureProject`" --startup-project `"$startupProject`" --output `"$bundleOutput`" --configuration Release --force --verbose 2>&1"
    $exitCode = $LASTEXITCODE
    
    # Display output
    Write-Host $output
    
    if ($exitCode -ne 0) {
        Write-Host ""
        Write-Host "ERROR: Migration bundle generation failed with exit code $exitCode" -ForegroundColor Red
        Write-Host ""
        Write-Host "Common causes:" -ForegroundColor Yellow
        Write-Host "  - Exit code -1073741189: DLL load failure (WinUI dependencies)" -ForegroundColor Yellow
        Write-Host "  - This is expected for WinUI projects and can be ignored if bundle was created" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Checking if bundle was created despite error..." -ForegroundColor Cyan
        
        # Check if bundle was actually created despite the error
        if (Test-Path $bundleOutput) {
            $bundleSize = (Get-Item $bundleOutput).Length
            if ($bundleSize -gt 1MB) {
                Write-Host "Bundle was created successfully despite error code!" -ForegroundColor Green
                # Continue to verification section
            } else {
                Write-Host "Bundle file exists but is too small ($bundleSize bytes)" -ForegroundColor Red
                exit $exitCode
            }
        } else {
            exit $exitCode
        }
    }
    
    Write-Host ""
    Write-Host "Migration bundle generated successfully!" -ForegroundColor Green
    
    # Verify the bundle was created
    if (Test-Path $bundleOutput) {
        $bundleSize = (Get-Item $bundleOutput).Length
        $bundleSizeMB = [math]::Round($bundleSize / 1MB, 2)
        
        Write-Host ""
        Write-Host "======================================================" -ForegroundColor Cyan
        Write-Host "Bundle Information" -ForegroundColor Cyan
        Write-Host "======================================================" -ForegroundColor Cyan
        Write-Host "Location: $bundleOutput"
        Write-Host "Size: $bundleSizeMB MB"
        Write-Host ""
        
        # Verify bundle is executable
        if ($bundleSize -lt 1MB) {
            Write-Host "WARNING: Bundle size is unusually small ($bundleSizeMB MB)" -ForegroundColor Yellow
            Write-Host "Expected size: 10-50 MB for a self-contained bundle"
            Write-Host ""
        }
        
        Write-Host "The migration bundle is ready for installer packaging!" -ForegroundColor Green
        Write-Host ""
        exit 0
    } else {
        Write-Host "ERROR: Bundle file was not created at expected location" -ForegroundColor Red
        Write-Host "Expected: $bundleOutput"
        exit 1
    }
    
} catch {
    Write-Host ""
    Write-Host "ERROR: Migration bundle generation failed" -ForegroundColor Red
    Write-Host $_.Exception.Message
    Write-Host ""
    Write-Host "Troubleshooting tips:" -ForegroundColor Yellow
    Write-Host "  1. Ensure the solution builds successfully (dotnet build src\Magidesk.sln)"
    Write-Host "  2. Verify EF Core migrations exist in Magidesk.Infrastructure"
    Write-Host "  3. Check that Magidesk.Presentation has a valid DbContext configuration"
    Write-Host "  4. Run 'dotnet ef migrations list' to verify migrations are detected"
    Write-Host ""
    exit 1
}
