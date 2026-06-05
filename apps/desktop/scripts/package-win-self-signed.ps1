$ErrorActionPreference = "Stop"

$desktopRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$envPath = Join-Path $desktopRoot ".certificates\dev-signing.env"

function Invoke-Checked {
  param(
    [Parameter(Mandatory = $true)]
    [string] $FilePath,
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]] $Arguments
  )

  & $FilePath @Arguments
  if ($LASTEXITCODE -ne 0) {
    throw "Command failed with exit code ${LASTEXITCODE}: $FilePath $($Arguments -join ' ')"
  }
}

if (-not (Test-Path $envPath)) {
  & (Join-Path $PSScriptRoot "create-dev-certificate.ps1")
}

Get-Content $envPath | ForEach-Object {
  if ($_ -match "^\s*([^#][^=]+)=(.*)$") {
    [Environment]::SetEnvironmentVariable($matches[1].Trim(), $matches[2].Trim(), "Process")
  }
}

if (-not $env:CSC_LINK -or -not $env:CSC_KEY_PASSWORD) {
  throw "Development signing environment was not loaded correctly."
}

$pfxPath = $env:CSC_LINK
$pfxPassword = $env:CSC_KEY_PASSWORD

# Keep electron-builder out of its winCodeSign helper locally. PowerShell signs the output below.
Remove-Item Env:\CSC_LINK -ErrorAction SilentlyContinue
Remove-Item Env:\CSC_KEY_PASSWORD -ErrorAction SilentlyContinue
Remove-Item Env:\WIN_CSC_LINK -ErrorAction SilentlyContinue
Remove-Item Env:\WIN_CSC_KEY_PASSWORD -ErrorAction SilentlyContinue
Set-Item Env:\CSC_IDENTITY_AUTO_DISCOVERY "false"

Invoke-Checked npm run build
Invoke-Checked npm run clean:backend
Invoke-Checked npm run publish:backend:framework
Invoke-Checked npm run copy:backend:framework
Invoke-Checked npx electron-builder --config electron-builder.self-signed.yml --win nsis

$thumbprint = $env:CHARACTER_MANAGER_DEV_CERT_THUMBPRINT
if (-not $thumbprint) {
  throw "Development signing certificate thumbprint was not loaded."
}

$cert = Get-ChildItem Cert:\CurrentUser\My |
  Where-Object { $_.Thumbprint -eq $thumbprint } |
  Select-Object -First 1

if ($null -eq $cert) {
  throw "Development signing certificate was not found in Cert:\CurrentUser\My."
}

$signTargets = @()
$appExe = Join-Path $desktopRoot "release\win-unpacked\CharacterManager.exe"
if (Test-Path $appExe) {
  $signTargets += $appExe
}

$installerTargets = Get-ChildItem (Join-Path $desktopRoot "release") -Filter "CharacterManager-*-DevSigned-Setup-win-x64.exe" -ErrorAction SilentlyContinue
$signTargets += $installerTargets.FullName

if ($signTargets.Count -eq 0) {
  throw "No executable artifacts were found to sign."
}

foreach ($target in $signTargets) {
  $signature = Set-AuthenticodeSignature -FilePath $target -Certificate $cert -HashAlgorithm SHA256
  if ($signature.Status -ne "Valid") {
    throw "Signing failed for $target. Status: $($signature.Status). Message: $($signature.StatusMessage)"
  }

  Write-Host "Signed $target with local development certificate."
}

Write-Host "Used local development PFX: $pfxPath"
