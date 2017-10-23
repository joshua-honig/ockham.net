<#
.SYNOPSIS 
Install a Nuget package into the global package cache
#>
function Install-GlobalPackage {
    param([string]$PackageID, [string]$Version, [string]$Source, [string]$TargetFrameworks)
    
    $tempDir  = "$env:TEMP\Project-$([Guid]::NewGuid().ToString('n'))" 
    mkdir $tempDir | Out-Null

    $tempProj = "$tempDir\project.csproj"
    
    if([string]::IsNullOrEmpty($TargetFrameworks)) { $TargetFrameworks = 'netcoreapp1.0' }

    $projectXml = @"
<Project Sdk="Microsoft.NET.Sdk"> 
  <PropertyGroup>
    <TargetFrameworks>$TargetFrameworks</TargetFrameworks> 
  </PropertyGroup>  
</Project> 
"@;

    $projectXml > $tempProj
    [string[]]$nugetArgs = 'install', $PackageID 

    if(![string]::IsNullOrEmpty($Version)) { $nugetArgs += '-Version', $Version }
    if(![string]::IsNullOrEmpty($Source))  { $nugetcmd  += '-Source'  , $Source  }

    cd $tempDir
    &nuget $nugetArgs

    cd $env:TEMP
    Remove-Item $tempDir -Recurse -Force
}


<#
.SYNOPSIS 
Find the path on disk to the contents of a package referenced by a PackageReference element in a project file
#>
function Find-PackageDir {

    param([string]$ProjectFile, [string]$PackageID)

    if(!(Test-Path $ProjectFile)) { 
        Write-Warning $("Project file $ProjectFile not fonud")
        return
    }

    $xProject = New-Object xml
    $xProject.Load($ProjectFile)
    $xPkgRef = $xProject.SelectSingleNode('//PackageReference[@Include="' + $PackageID + '"]')

    if($xPkgRef -eq $null) { 
        Write-Warning "No PackageReference element found for $PackageID found"
        return
    }
 
    $pkgVersion = $xPkgRef.Version

    $userPackages = "$env:USERPROFILE\.nuget\packages"
    $sysPackages  = "${Env:\ProgramFiles(x86)}\Microsoft SDKs\NugetPackages"
    $pkgDir       = '' 

    foreach($packageDir in @($userPackages, $sysPackages)) {
        $testPath = "$packageDir\$PackageID\$pkgVersion"
        if(Test-Path $testPath) {
            $pkgDir = $testPath
            break
        }
    }

    if(($pkgDir -eq '') -or (!(Test-Path $pkgDir))) {
        Write-Warning "$PackageID package path not found"
        return
    }

    $pkgDir
}
 
Export-ModuleMember -Function Install-GlobalPackage
Export-ModuleMember -Function Find-PackageDir