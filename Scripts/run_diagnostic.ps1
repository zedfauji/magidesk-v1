$exePath = "C:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\bin\Debug\net8.0-windows10.0.19041.0\Magidesk.Presentation.exe"
$logPath = "C:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\debug_output.txt"
$errLogPath = "C:\Users\giris\Documents\Code\Redesign-POS\Windows Based POS\Magidesk\debug_error.txt"

Write-Host "Starting application: $exePath"
Write-Host "Output redirected to: $logPath and $errLogPath"

Start-Process -FilePath $exePath -RedirectStandardOutput $logPath -RedirectStandardError $errLogPath -Wait
Write-Host "Application exited."
