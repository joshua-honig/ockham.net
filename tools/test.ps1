param([string]$ProjectDirectory)

if([string]::IsNullOrEmpty($ProjectDirectory)) {
    $ProjectDirectory = [System.IO.Directory]::GetCurrentDirectory()
}

$toolsDir = $PSScriptRoot
Write-Host "ProjectDirectory = $ProjectDirectory"
Write-Host "toolsDir = $toolsDir"