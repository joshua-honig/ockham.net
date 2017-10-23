# Params
$ProjectDir = 'C:\Source\Git\ockham.net\src\Ockham.Test\test'
$ProjectFile = "$projectDir\Ockham.Test.Tests.csproj"
$PackageID   = 'OpenCover'
 
$thisDir = $PSScriptRoot

Import-Module "$thisDir\..\..\..\..\tools\BuildTools.psm1"

$osDir = Find-PackageDir -ProjectFile $ProjectFile -PackageID $PackageID
if($osDir -eq $null) {
    
}