# Initialize Test Database
# Runs Magidesk once to create the schema in the test database

$ErrorActionPreference = "Stop"

Write-Host "Initializing test database schema..." -ForegroundColor Cyan

# Create a temporary database configuration file for the test database
$testDbConfig = @"
{
  "DatabaseConfiguration": {
    "Host": "localhost",
    "Port": 5432,
    "Database": "magidesk_test",
    "Username": "postgres",
    "Password": "postgres"
  }
}
"@

$configPath = "src/Magidesk.Presentation/bin/Release/net8.0-windows10.0.19041.0/win-x64/database-config.json"
$configDir = Split-Path -Parent $configPath

# Create directory if it doesn't exist
if (!(Test-Path $configDir)) {
    New-Item -ItemType Directory -Path $configDir -Force | Out-Null
}

# Write test database configuration
$testDbConfig | Out-File -FilePath $configPath -Encoding UTF8

Write-Host "Test database configuration created at: $configPath" -ForegroundColor Green
Write-Host ""
Write-Host "Now run the Magidesk application once to initialize the schema." -ForegroundColor Yellow
Write-Host "The application will create all necessary tables automatically." -ForegroundColor Yellow
Write-Host ""
Write-Host "After the application starts successfully, you can close it and run:" -ForegroundColor Cyan
Write-Host "  dotnet test src/Magidesk.Tests.E2E/Magidesk.Tests.E2E.csproj --filter Category=FinancialSafety" -ForegroundColor White
