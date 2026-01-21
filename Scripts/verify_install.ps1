# verify_install.ps1
# Purpose: Post-installation health check
# Target Path: C:\Program Files\Magidesk POS\

$InstallDir = "C:\Program Files\Magidesk POS"
$LogDir = "C:\ProgramData\Magidesk\Logs"

Write-Host "Verifying Magidesk Installation..."

# 1. Check Files
if (Test-Path "$InstallDir\Magidesk.Presentation.exe") {
    Write-Host "[OK] Executable found." -ForegroundColor Green
}
else {
    Write-Error "[FAIL] Executable missing in $InstallDir"
}

# 2. Check Permissions
if (Test-Path $LogDir) {
    # Simple write check
    try {
        "test" | Out-File "$LogDir\write_test.tmp"
        Remove-Item "$LogDir\write_test.tmp"
        Write-Host "[OK] Log directory is writable." -ForegroundColor Green
    }
    catch {
        Write-Error "[FAIL] Cannot write to Log directory ($LogDir)"
    }
}
else {
    Write-Warning "Log directory does not exist yet."
}

# 3. Service Check (if applicable)
# Get-Service "MagideskUpdateService" ...
