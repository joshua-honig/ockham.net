param([string]$ProjectDirectory, [string]$Configuration, [switch]$Unit, [switch]$API, [switch]$Build)

if([string]::IsNullOrEmpty($ProjectDirectory)) {
    $ProjectDirectory = [System.IO.Directory]::GetCurrentDirectory()
}

if([string]::IsNullOrEmpty($Configuration)) {
    $Configuration = 'Release'
}

$sw = [System.Diagnostics.Stopwatch]::StartNew()

$toolsDir = $PSScriptRoot

Import-Module "$toolsDir\TestUtils.psm1"

$initialDir  = Get-CurrentDirectory 
$solutionDir = Split-Path $ProjectDirectory -Parent
$refDir      = Join-Path $solutionDir ref
$srcDir      = Join-Path $solutionDir src
$testDir     = Join-Path $solutionDir test

Write-Banner "Ockham Test Script"
Write-Host " Paths:"
Write-Host "  Tools       : $toolsDir"
Write-Host "  Solution    : $solutionDir"
Write-Host "    Reference : $refDir"
Write-Host "    Source    : $srcDir"
Write-Host "    Test      : $testDir"
Write-Bar 
 
if($Build) {
    Write-Banner "Rebuilding"
    Set-CurrentDirectory $solutionDir    
    dotnet restore

    Invoke-Build $refDir -Configuration $Configuration -Clean
    Invoke-Build $srcDir -Configuration $Configuration -Clean
    Invoke-Build $testDir -Configuration $Configuration -Clean
}
  
 
if($Unit) {
    Write-Banner "Running Unit Tests"
    Set-Location $ProjectDirectory
    [Environment]::CurrentDirectory = $ProjectDirectory
    Remove-Item "$ProjectDirectory\output\tests\*.xml" -Force -ErrorAction SilentlyContinue
    dotnet xunit -xml "output\tests\xunit.xml"
}

if($API) { 
    Write-Banner "Checking API surface"
    Test-API -SolutionDirectory $solutionDir -ToolsPath $toolsDir -Configuration $Configuration -OutputPath "$solutionDir\test\output\api"
}

$sw.Stop()

if($Unit -or $API) { 
    Write-Banner "Generating summary"
    $results = Get-TestSummary -SolutionDirectory $solutionDir

    $resultsDir = [System.IO.Path]::Combine($testDir, 'output', 'results')
    Remove-Item "$resultsDir\result.*" -Force -ErrorAction SilentlyContinue

    $fail = (@($results | ? { $_.Result -eq $false }).Count -gt 0)
    $resultsName = $(if($fail) { 'failed' } else { 'passed' })
    $outFile = "$resultsDir\result.$resultsName.htm"

    $results | ft > $outFile
}