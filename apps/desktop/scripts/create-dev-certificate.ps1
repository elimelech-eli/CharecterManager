$ErrorActionPreference = "Stop"

$subject = "CN=CharacterManager Dev"
$desktopRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$certDir = Join-Path $desktopRoot ".certificates"
$pfxPath = Join-Path $certDir "character-manager-dev-signing.pfx"
$envPath = Join-Path $certDir "dev-signing.env"
$cerPath = Join-Path $certDir "character-manager-dev-signing.cer"

New-Item -ItemType Directory -Force -Path $certDir | Out-Null

$existing = Get-ChildItem Cert:\CurrentUser\My |
  Where-Object { $_.Subject -eq $subject -and $_.EnhancedKeyUsageList.FriendlyName -contains "Code Signing" } |
  Sort-Object NotAfter -Descending |
  Select-Object -First 1

if ($null -eq $existing) {
  $cert = New-SelfSignedCertificate `
    -Subject $subject `
    -Type CodeSigningCert `
    -CertStoreLocation Cert:\CurrentUser\My `
    -KeyAlgorithm RSA `
    -KeyLength 3072 `
    -HashAlgorithm SHA256 `
    -KeyUsage DigitalSignature `
    -NotAfter (Get-Date).AddYears(2)
} else {
  $cert = $existing
}

$password = -join ((48..57 + 65..90 + 97..122) | Get-Random -Count 48 | ForEach-Object { [char]$_ })
$securePassword = ConvertTo-SecureString $password -AsPlainText -Force

Export-PfxCertificate -Cert $cert -FilePath $pfxPath -Password $securePassword -Force | Out-Null
Export-Certificate -Cert $cert -FilePath $cerPath -Force | Out-Null

Import-Certificate -FilePath $cerPath -CertStoreLocation Cert:\CurrentUser\Root | Out-Null
Import-Certificate -FilePath $cerPath -CertStoreLocation Cert:\CurrentUser\TrustedPublisher | Out-Null

@"
CSC_LINK=$pfxPath
CSC_KEY_PASSWORD=$password
CHARACTER_MANAGER_DEV_CERT_THUMBPRINT=$($cert.Thumbprint)
"@ | Set-Content -Path $envPath -Encoding utf8

Write-Host "Created/trusted local development signing certificate:"
Write-Host "  Subject: $subject"
Write-Host "  Thumbprint: $($cert.Thumbprint)"
Write-Host "  PFX: $pfxPath"
Write-Host "  Env: $envPath"
Write-Host ""
Write-Host "This certificate is for local development only. Do not use it for public releases."
