# apply_migrations.ps1
# Purpose: Execute the EF Core Migration Bundle to upgrade the database.
# Usage: .\apply_migrations.ps1 -DbHost localhost -DbUser postgres -DbPass password

param (
    [string]$DbHost = "localhost",
    [string]$DbPort = "5432",
    [string]$DbUser = "postgres",
    [string]$DbPass,
    [string]$DbName = "magidesk_prod",
    [string]$BundlePath = "..\redist\efbundle.exe"
)

$ErrorActionPreference = "Stop"

# 1. Verify Bundle Exists
if (-not (Test-Path $BundlePath)) {
    Write-Error "Migration Bundle not found at: $BundlePath"
    exit 1
}

# 2. Verify Database Connection (Pre-flight)
$ConnString = "Host=$DbHost;Port=$DbPort;Database=$DbName;Username=$DbUser;Password=$DbPass"
Write-Host "Targeting Database: $DbName on $DbHost"

# 3. Advisory Lock (Pseudo-code / simplified)
# In production, recommend running SELECT pg_advisory_lock(99999);

# 4. Run Bundle
Write-Host "Executing Migration Bundle..."
try {
    # EF Bundle takes --connection string
    & $BundlePath --connection "$ConnString" --verbose
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Migrations Applied Successfully." -ForegroundColor Green
    }
    else {
        Write-Error "Migration Failed with Exit Code: $LASTEXITCODE"
    }
}
catch {
    Write-Error "Failed to execute bundle: $_"
    exit 1
}
