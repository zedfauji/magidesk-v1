# Setup Test Database Script
# This script initializes the test database schema by running the Magidesk application once

$ErrorActionPreference = "Stop"

Write-Host "Setting up test database schema..." -ForegroundColor Cyan

# Set environment variable to use test database
$env:ConnectionStrings__DefaultConnection = "Host=localhost;Port=5432;Database=magidesk_test;Username=postgres;Password=postgres"

# Build the application
Write-Host "Building Magidesk.Presentation..." -ForegroundColor Yellow
dotnet build src/Magidesk.Presentation/Magidesk.Presentation.csproj -c Release --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit 1
}

Write-Host "Database schema should be created when the application starts." -ForegroundColor Green
Write-Host "Please run the Magidesk application once to initialize the schema." -ForegroundColor Green
Write-Host ""
Write-Host "After that, you can run the E2E tests with:" -ForegroundColor Cyan
Write-Host "  dotnet test src/Magidesk.Tests.E2E/Magidesk.Tests.E2E.csproj --filter Category=FinancialSafety" -ForegroundColor White
