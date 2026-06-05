param(
    [string]$ReportsRoot = (Join-Path (Split-Path -Parent $MyInvocation.MyCommand.Path) "Reports"),
    [string]$OutputPath = (Join-Path $ReportsRoot "backend-test-report.html")
)

$ErrorActionPreference = "Stop"

function ConvertTo-HtmlText {
    param([AllowNull()][object]$Value)

    if ($null -eq $Value) {
        return ""
    }

    return [System.Net.WebUtility]::HtmlEncode([string]$Value)
}

function ConvertTo-Percent {
    param([double]$Value)

    return "{0:N2}%" -f ($Value * 100)
}

function ConvertFrom-TrxDuration {
    param([AllowNull()][string]$Duration)

    if ([string]::IsNullOrWhiteSpace($Duration)) {
        return 0.0
    }

    try {
        return [TimeSpan]::Parse($Duration).TotalMilliseconds
    }
    catch {
        return 0.0
    }
}

function Get-TrxSuite {
    param([System.IO.FileInfo]$TrxFile)

    [xml]$doc = Get-Content -LiteralPath $TrxFile.FullName -Raw
    $ns = New-Object System.Xml.XmlNamespaceManager($doc.NameTable)
    $ns.AddNamespace("trx", "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")

    $results = @($doc.SelectNodes("//trx:Results/trx:UnitTestResult", $ns))
    $definitions = @($doc.SelectNodes("//trx:TestDefinitions/trx:UnitTest", $ns))
    $definitionById = @{}
    foreach ($definition in $definitions) {
        $definitionById[$definition.id] = $definition
    }

    $counters = $doc.SelectSingleNode("//trx:ResultSummary/trx:Counters", $ns)
    $times = $doc.SelectSingleNode("//trx:Times", $ns)
    $suiteName = Split-Path -Leaf (Split-Path -Parent $TrxFile.FullName)
    if ($suiteName -eq "Unit" -or $suiteName -eq "E2E") {
        $suiteName = $suiteName
    }
    else {
        $suiteName = [System.IO.Path]::GetFileNameWithoutExtension($TrxFile.Name)
    }

    $testResults = foreach ($result in $results) {
        $definition = $definitionById[$result.testId]
        $method = $definition.TestMethod
        $className = if ($method) { [string]$method.className } else { "" }
        $methodName = if ($method) { [string]$method.name } else { [string]$result.testName }
        $message = $result.Output.ErrorInfo.Message
        $stackTrace = $result.Output.ErrorInfo.StackTrace

        [pscustomobject]@{
            Suite = $suiteName
            Name = [string]$result.testName
            ClassName = $className
            MethodName = $methodName
            Outcome = [string]$result.outcome
            DurationMs = ConvertFrom-TrxDuration ([string]$result.duration)
            Message = if ($message) { [string]$message } else { "" }
            StackTrace = if ($stackTrace) { [string]$stackTrace } else { "" }
        }
    }

    $computedTotal = $testResults.Count
    $computedPassed = @($testResults | Where-Object { $_.Outcome -eq "Passed" }).Count
    $computedFailed = @($testResults | Where-Object { $_.Outcome -eq "Failed" }).Count
    $computedSkipped = @($testResults | Where-Object { $_.Outcome -in @("NotExecuted", "Skipped") }).Count

    [pscustomobject]@{
        Name = $suiteName
        TrxPath = $TrxFile.FullName
        StartedAt = if ($times) { [string]$times.start } else { "" }
        FinishedAt = if ($times) { [string]$times.finish } else { "" }
        Total = if ($counters -and $counters.total) { [int]$counters.total } else { $computedTotal }
        Passed = if ($counters -and $counters.passed) { [int]$counters.passed } else { $computedPassed }
        Failed = if ($counters -and $counters.failed) { [int]$counters.failed } else { $computedFailed }
        Skipped = if ($counters -and $counters.notExecuted) { [int]$counters.notExecuted } else { $computedSkipped }
        DurationMs = ($testResults | Measure-Object -Property DurationMs -Sum).Sum
        Results = @($testResults | Sort-Object Outcome, ClassName, MethodName)
    }
}

function Get-CoverageBySuite {
    param([string]$ReportsRoot)

    $coverageBySuite = @{}
    foreach ($suiteName in @("Unit", "E2E")) {
        $suitePath = Join-Path $ReportsRoot $suiteName
        if (-not (Test-Path -LiteralPath $suitePath)) {
            continue
        }

        $coverageFile = Get-ChildItem -LiteralPath $suitePath -Recurse -Filter "coverage.cobertura.xml" |
            Sort-Object LastWriteTimeUtc -Descending |
            Select-Object -First 1

        if ($coverageFile) {
            [xml]$coverage = Get-Content -LiteralPath $coverageFile.FullName -Raw
            $coverageBySuite[$suiteName] = [pscustomobject]@{
                Path = $coverageFile.FullName
                LineRate = [double]$coverage.coverage.'line-rate'
                BranchRate = [double]$coverage.coverage.'branch-rate'
                LinesCovered = [int]$coverage.coverage.'lines-covered'
                LinesValid = [int]$coverage.coverage.'lines-valid'
                BranchesCovered = [int]$coverage.coverage.'branches-covered'
                BranchesValid = [int]$coverage.coverage.'branches-valid'
            }
        }
    }

    return $coverageBySuite
}

if (-not (Test-Path -LiteralPath $ReportsRoot)) {
    throw "Reports root does not exist: $ReportsRoot"
}

$trxFiles = @(Get-ChildItem -LiteralPath $ReportsRoot -Recurse -Filter "*.trx" | Sort-Object FullName)
if ($trxFiles.Count -eq 0) {
    throw "No TRX files found under $ReportsRoot"
}

$suites = @($trxFiles | ForEach-Object { Get-TrxSuite $_ })
$coverageBySuite = Get-CoverageBySuite $ReportsRoot
$allResults = @($suites | ForEach-Object { $_.Results })
$total = ($suites | Measure-Object -Property Total -Sum).Sum
$passed = ($suites | Measure-Object -Property Passed -Sum).Sum
$failed = ($suites | Measure-Object -Property Failed -Sum).Sum
$skipped = ($suites | Measure-Object -Property Skipped -Sum).Sum
$durationMs = ($suites | Measure-Object -Property DurationMs -Sum).Sum
$generatedAt = Get-Date -Format "yyyy-MM-dd HH:mm:ss zzz"
$statusClass = if ($failed -gt 0) { "failed" } else { "passed" }
$statusText = if ($failed -gt 0) { "Failed" } else { "Passed" }

$suiteCards = foreach ($suite in $suites) {
    $coverage = $coverageBySuite[$suite.Name]
    $coverageMarkup = if ($coverage) {
        @"
        <div class="coverage">
          <span>Line coverage <strong>$(ConvertTo-Percent $coverage.LineRate)</strong></span>
          <span>Branch coverage <strong>$(ConvertTo-Percent $coverage.BranchRate)</strong></span>
        </div>
"@
    }
    else {
        '<div class="coverage muted">No coverage file found for this suite.</div>'
    }

    @"
    <section class="suite-card">
      <div class="suite-header">
        <h2>$(ConvertTo-HtmlText $suite.Name)</h2>
        <span class="pill $([string]$(if ($suite.Failed -gt 0) { "failed" } else { "passed" }))">$([string]$(if ($suite.Failed -gt 0) { "Failed" } else { "Passed" }))</span>
      </div>
      <div class="metrics compact">
        <div><span>Total</span><strong>$($suite.Total)</strong></div>
        <div><span>Passed</span><strong>$($suite.Passed)</strong></div>
        <div><span>Failed</span><strong>$($suite.Failed)</strong></div>
        <div><span>Skipped</span><strong>$($suite.Skipped)</strong></div>
        <div><span>Duration</span><strong>$("{0:N0} ms" -f $suite.DurationMs)</strong></div>
      </div>
      $coverageMarkup
      <p class="path">$(ConvertTo-HtmlText $suite.TrxPath)</p>
    </section>
"@
}

$rows = foreach ($result in $allResults | Sort-Object Suite, Outcome, ClassName, MethodName) {
    $outcome = $result.Outcome.ToLowerInvariant()
    $messageMarkup = if ([string]::IsNullOrWhiteSpace($result.Message)) {
        ""
    }
    else {
        "<details><summary>Failure detail</summary><pre>$(ConvertTo-HtmlText $result.Message)`n$(ConvertTo-HtmlText $result.StackTrace)</pre></details>"
    }

    @"
      <tr class="$outcome">
        <td><span class="pill $outcome">$(ConvertTo-HtmlText $result.Outcome)</span></td>
        <td>$(ConvertTo-HtmlText $result.Suite)</td>
        <td>
          <div class="test-name">$(ConvertTo-HtmlText $result.MethodName)</div>
          <div class="class-name">$(ConvertTo-HtmlText $result.ClassName)</div>
          $messageMarkup
        </td>
        <td class="numeric">$("{0:N2}" -f $result.DurationMs)</td>
      </tr>
"@
}

$html = @"
<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>Backend Test Report</title>
  <style>
    :root {
      color-scheme: light;
      --bg: #f6f7f9;
      --panel: #ffffff;
      --text: #17202a;
      --muted: #657180;
      --border: #d8dee6;
      --pass: #137a3f;
      --pass-bg: #e6f4ec;
      --fail: #b42318;
      --fail-bg: #fdecec;
      --skip: #765a00;
      --skip-bg: #fff4cc;
      --accent: #2457c5;
    }

    * { box-sizing: border-box; }
    body {
      margin: 0;
      background: var(--bg);
      color: var(--text);
      font: 14px/1.45 "Segoe UI", Arial, sans-serif;
    }

    header {
      background: var(--panel);
      border-bottom: 1px solid var(--border);
      padding: 28px clamp(20px, 5vw, 56px);
    }

    main {
      padding: 24px clamp(20px, 5vw, 56px) 48px;
    }

    h1, h2 { margin: 0; }
    h1 { font-size: 28px; }
    h2 { font-size: 17px; }

    .subtitle {
      color: var(--muted);
      margin-top: 6px;
    }

    .summary {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
      gap: 12px;
      margin-top: 20px;
    }

    .summary > div, .suite-card {
      background: var(--panel);
      border: 1px solid var(--border);
      border-radius: 8px;
      padding: 16px;
    }

    .summary span, .metrics span {
      color: var(--muted);
      display: block;
      font-size: 12px;
      text-transform: uppercase;
      letter-spacing: .04em;
    }

    .summary strong {
      display: block;
      font-size: 24px;
      margin-top: 4px;
    }

    .suite-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(320px, 1fr));
      gap: 16px;
      margin: 20px 0;
    }

    .suite-header {
      align-items: center;
      display: flex;
      justify-content: space-between;
      gap: 12px;
      margin-bottom: 14px;
    }

    .metrics {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(92px, 1fr));
      gap: 10px;
    }

    .metrics strong {
      display: block;
      font-size: 18px;
      margin-top: 2px;
    }

    .coverage {
      display: flex;
      flex-wrap: wrap;
      gap: 10px 18px;
      margin-top: 14px;
      color: var(--muted);
    }

    .coverage strong { color: var(--text); }
    .path {
      color: var(--muted);
      font-size: 12px;
      overflow-wrap: anywhere;
      margin: 14px 0 0;
    }

    .pill {
      border-radius: 999px;
      display: inline-block;
      font-size: 12px;
      font-weight: 700;
      padding: 3px 9px;
      white-space: nowrap;
    }

    .pill.passed { background: var(--pass-bg); color: var(--pass); }
    .pill.failed { background: var(--fail-bg); color: var(--fail); }
    .pill.notexecuted, .pill.skipped { background: var(--skip-bg); color: var(--skip); }

    .table-wrap {
      background: var(--panel);
      border: 1px solid var(--border);
      border-radius: 8px;
      overflow: auto;
    }

    table {
      border-collapse: collapse;
      width: 100%;
      min-width: 760px;
    }

    th, td {
      border-bottom: 1px solid var(--border);
      padding: 11px 12px;
      text-align: left;
      vertical-align: top;
    }

    th {
      background: #f1f3f6;
      color: #394452;
      font-size: 12px;
      position: sticky;
      top: 0;
      text-transform: uppercase;
      letter-spacing: .04em;
      z-index: 1;
    }

    tr:last-child td { border-bottom: 0; }
    tr.failed td { background: #fff8f8; }
    .numeric { text-align: right; white-space: nowrap; }
    .test-name { font-weight: 650; }
    .class-name {
      color: var(--muted);
      font-size: 12px;
      margin-top: 2px;
      overflow-wrap: anywhere;
    }

    details {
      margin-top: 8px;
    }

    pre {
      background: #111827;
      border-radius: 6px;
      color: #f9fafb;
      overflow: auto;
      padding: 12px;
      white-space: pre-wrap;
    }

    .muted { color: var(--muted); }
  </style>
</head>
<body>
  <header>
    <h1>Backend Test Report</h1>
    <div class="subtitle">Generated $generatedAt from TRX files under $(ConvertTo-HtmlText $ReportsRoot)</div>
    <div class="summary">
      <div><span>Status</span><strong><span class="pill $statusClass">$statusText</span></strong></div>
      <div><span>Total</span><strong>$total</strong></div>
      <div><span>Passed</span><strong>$passed</strong></div>
      <div><span>Failed</span><strong>$failed</strong></div>
      <div><span>Skipped</span><strong>$skipped</strong></div>
      <div><span>Duration</span><strong>$("{0:N0} ms" -f $durationMs)</strong></div>
    </div>
  </header>
  <main>
    <div class="suite-grid">
      $($suiteCards -join "`n")
    </div>
    <div class="table-wrap">
      <table>
        <thead>
          <tr>
            <th>Outcome</th>
            <th>Suite</th>
            <th>Test</th>
            <th class="numeric">Duration ms</th>
          </tr>
        </thead>
        <tbody>
          $($rows -join "`n")
        </tbody>
      </table>
    </div>
  </main>
</body>
</html>
"@

New-Item -ItemType Directory -Force -Path (Split-Path -Parent $OutputPath) | Out-Null
Set-Content -LiteralPath $OutputPath -Value $html -Encoding UTF8
Write-Host "HTML backend test report written to $OutputPath"
