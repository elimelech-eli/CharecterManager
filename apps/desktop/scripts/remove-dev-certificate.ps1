$ErrorActionPreference = "Stop"

$subject = "CN=CharacterManager Dev"
$desktopRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$certDir = Join-Path $desktopRoot ".certificates"

$stores = @(
  "Cert:\CurrentUser\My",
  "Cert:\CurrentUser\Root",
  "Cert:\CurrentUser\TrustedPublisher"
)

foreach ($store in $stores) {
  Get-ChildItem $store |
    Where-Object { $_.Subject -eq $subject } |
    Remove-Item -Force
}

if (Test-Path $certDir) {
  Remove-Item -Path $certDir -Recurse -Force
}

Write-Host "Removed local CharacterManager development signing certificate and local certificate files."
