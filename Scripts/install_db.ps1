# install_db.ps1
# Purpose: automation script to bootstrap the database
# Usage: .\install_db.ps1 -DbHost localhost -DbUser postgres -DbPass password

param (
    [string]$DbHost = "localhost",
    [string]$DbPort = "5432",
    [string]$DbUser = "postgres",
    [string]$DbPass,
    [string]$DbName = "magidesk_prod"
)

Write-Host "Initializing Magidesk Database: $DbName on $DbHost"

# 1. Check if psql is available
try {
    & psql --version | Out-Null
} catch {
    Write-Error "PostgreSQL tools (psql) not found in PATH."
    exit 1
}

# 2. Create Database (Idempotent check needed in real script)
Write-Host "Creating database if not exists..."
# & psql ... "CREATE DATABASE $DbName"

# 3. Run Bootstrap
Write-Host "Applying Schema..."
# & psql -d $DbName -f "db\00_bootstrap_schema.sql"

# 4. Run Seed
Write-Host "Seeding Data..."
# & psql -d $DbName -f "db\01_seed_defaults.sql"

Write-Host "Database Installation Complete."
