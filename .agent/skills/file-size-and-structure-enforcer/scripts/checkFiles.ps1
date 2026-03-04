Param(
  [string]$RootPath = "."
)

$limit = 300
$violations = @()

Get-ChildItem -Recurse -Filter *.cs -Path $RootPath |
  ForEach-Object {
    $lineCount = (Get-Content $_.FullName).Count
    if ($lineCount -gt $limit) {
      $violations += "$($_.FullName) has $lineCount lines (limit: $limit)"
    }
  }

if ($violations.Count -gt 0) {
  Write-Output "FAIL"
  $violations | ForEach-Object { Write-Output $_ }
} else {
  Write-Output "PASS"
}
