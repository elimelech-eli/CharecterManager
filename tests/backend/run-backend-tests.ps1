$ErrorActionPreference = "Stop"

$backendRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path $backendRoot "..\..")
$reportsRoot = Join-Path $backendRoot "Reports"

function Invoke-BackendTest {
    param(
        [string]$ProjectPath,
        [string]$SuiteResultsRoot,
        [string]$TrxFileName
    )

    dotnet test $ProjectPath `
        --collect:"XPlat Code Coverage" `
        --logger "trx;LogFileName=$TrxFileName" `
        --results-directory $SuiteResultsRoot

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet test failed for $ProjectPath with exit code $LASTEXITCODE"
    }
}

if (Test-Path -LiteralPath $reportsRoot) {
    Remove-Item -LiteralPath $reportsRoot -Recurse -Force
}

New-Item -ItemType Directory -Force -Path (Join-Path $reportsRoot "Unit") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $reportsRoot "E2E") | Out-Null

Invoke-BackendTest `
    -ProjectPath (Join-Path $backendRoot "Unit\CharacterManager.Backend.Unit.Tests.csproj") `
    -SuiteResultsRoot (Join-Path $reportsRoot "Unit") `
    -TrxFileName "backend-unit.trx"

Invoke-BackendTest `
    -ProjectPath (Join-Path $backendRoot "E2E\CharacterManager.Backend.E2E.Tests.csproj") `
    -SuiteResultsRoot (Join-Path $reportsRoot "E2E") `
    -TrxFileName "backend-e2e.trx"

& (Join-Path $backendRoot "generate-html-report.ps1") -ReportsRoot $reportsRoot

Write-Host "Backend test results, coverage reports, and HTML report written under $reportsRoot"
