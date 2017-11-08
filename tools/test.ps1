param([string]$ProjectDirectory, [string]$Configuration, [switch]$Unit, [switch]$API, [switch]$Build, [switch]$Clean)

if([string]::IsNullOrEmpty($ProjectDirectory)) {
    $ProjectDirectory = [System.IO.Directory]::GetCurrentDirectory()
}

if([string]::IsNullOrEmpty($Configuration)) {
    $Configuration = 'Release'
}

$startTime = [datetime]::Now
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
 
if($Build -or $Clean) {
    Write-Banner "Cleaning"
    Invoke-Clean $refDir  -Configuration $Configuration
    Invoke-Clean $srcDir  -Configuration $Configuration
    Invoke-Clean $testDir -Configuration $Configuration
}

if($Build) {
    Write-Banner "Rebuilding"
    Set-CurrentDirectory $solutionDir    
    dotnet restore

    Invoke-Build $refDir  -Configuration $Configuration  
    Invoke-Build $srcDir  -Configuration $Configuration  
    Invoke-Build $testDir -Configuration $Configuration 
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
    $resultName = $(if($fail) { 'Failed' } else { 'Passed' })
    $outFile = "$resultsDir\result.$($resultName.ToLower()).htm"

    $projectName = $(dir "$solutionDir\*.sln" | select -First 1).BaseName

    "Project    : $projectName" > $outFile
    "Result     : $resultName" >> $outFile
    "Start Time : $($startTime.ToString('ddd yyyy-MM-dd HH:mm:ss'))" >> $outFile
    "Elapsed    : $($sw.Elapsed.TotalSeconds.ToString('f3')) s" >> $outFile

    $results | ft >> $outFile
}