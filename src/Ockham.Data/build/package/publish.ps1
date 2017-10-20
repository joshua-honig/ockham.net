$thisDir = (Get-Item (Get-Location)).FullName
Write-Host "Current dir: $thisDir"

$xNuspec = New-Object xml
$xNuspec.Load('Ockham.Data.nuspec')
$xPackage = $xNuspec.DocumentElement 
$version = $xPackage.metadata.version.Trim()
$id = $xPackage.metadata.id
 
[void][Reflection.Assembly]::LoadWithPartialName('Microsoft.VisualBasic')
$apiKey = ([string]([Microsoft.VisualBasic.Interaction]::InputBox('Enter the ockham.net nuget.org API key', 'Enter API Key'))).Trim()

nuget pack

$nupkgPath = "$id.$version.nupkg"

nuget push $nupkgPath -ApiKey $apiKey -s nuget.org