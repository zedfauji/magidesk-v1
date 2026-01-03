# Run Documentation Portal Locally
# This script sets up and runs the MkDocs documentation portal locally

Write-Host "Setting up Magidesk POS Documentation Portal..." -ForegroundColor Green

# Check if Python is installed
try {
    $pythonVersion = python --version 2>$null
    Write-Host "Python found: $pythonVersion" -ForegroundColor Green
} catch {
    Write-Host "Error: Python is not installed or not in PATH" -ForegroundColor Red
    Write-Host "Please install Python 3.8 or higher from https://python.org" -ForegroundColor Yellow
    exit 1
}

# Check if pip is available
try {
    $pipVersion = pip --version 2>$null
    Write-Host "Pip found: $pipVersion" -ForegroundColor Green
} catch {
    Write-Host "Error: Pip is not installed or not in PATH" -ForegroundColor Red
    exit 1
}

# Install requirements
Write-Host "Installing Python dependencies..." -ForegroundColor Yellow
pip install -r requirements.txt

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Failed to install dependencies" -ForegroundColor Red
    exit 1
}

# Build and serve documentation
Write-Host "Starting documentation server..." -ForegroundColor Green
Write-Host "Documentation will be available at: http://127.0.0.1:8000" -ForegroundColor Cyan
Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Yellow
Write-Host ""

# Start MkDocs development server
mkdocs serve --dev-addr=0.0.0.0:8000