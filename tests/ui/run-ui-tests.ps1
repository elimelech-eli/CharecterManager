$ErrorActionPreference = "Stop"

$uiRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path $uiRoot "..\..")
$desktopRoot = Join-Path $repoRoot "apps\desktop"
$reportsRoot = Join-Path $uiRoot "Reports"

if (Test-Path -LiteralPath $reportsRoot) {
    Remove-Item -LiteralPath $reportsRoot -Recurse -Force
}

New-Item -ItemType Directory -Force -Path (Join-Path $reportsRoot "Unit") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $reportsRoot "E2E") | Out-Null

Push-Location $desktopRoot
try {
    npm.cmd run test:ui:unit
    if ($LASTEXITCODE -ne 0) {
        throw "UI unit tests failed with exit code $LASTEXITCODE"
    }

    npm.cmd run test:ui:e2e
    if ($LASTEXITCODE -ne 0) {
        throw "UI E2E tests failed with exit code $LASTEXITCODE"
    }
}
finally {
    Pop-Location
}

Write-Host "UI test results written under $reportsRoot"
