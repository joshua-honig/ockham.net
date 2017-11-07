if($(gv PSSessionProject -ErrorAction SilentlyContinue) -eq $null) {
    New-Variable -Name PSSessionProject -Visibility Public -Scope Global
}

if([string]::IsNullOrEmpty($global:PSSessionProject )) {
    $global:PSSessionProject = [System.IO.Path]::Combine($env:TEMP, 'PSSession', ([Guid]::NewGuid().ToString('n')))
}

[System.Reflection.Assembly]::LoadWithPartialName('System.IO.Compression') | Out-Null

function Get-SessionProject {
    if(!(Test-Path $global:PSSessionProject)) { 
        mkdir $global:PSSessionProject | Out-Null 
        [System.Diagnostics.Process]::GetCurrentProcess().ID > "$global:PSSessionProject\.processid"
    }
    $global:PSSessionProject
}

function Clean-SessionProjects {

    $deleteDirs = New-Object System.Collections.Generic.List[string]
    foreach($sessionDir in [System.IO.Directory]::GetDirectories([System.IO.Path]::Combine($env:TEMP, 'PSSession'))) {
        if($sessionDir -eq $global:PSSessionProject) { continue; }
        $procIDFile = "$sessionDir\.processid"
        if(Test-Path $procIDFile) {
            $processID = [System.IO.File]::ReadAllText($procIDFile).Trim()
            $process = Get-Process -Id $processID -ErrorAction SilentlyContinue
            if($process -ne $null) {
                if($process.Name -like 'powershell*') { continue; }
            }
        }

        $deleteDirs.Add($sessionDir)
    }

    foreach($sessionDir in $deleteDirs) {
        try { Remove-Item $sessionDir -Recurse -Force } catch { }
    }  
}

function Confirm-Nuget {
    $nugetDir  = [System.IO.Path]::Combine($env:LOCALAPPDATA, 'Nuget', 'bin')
    $nugetPath = [System.IO.Path]::Combine($nugetDir, 'nuget.exe')

    # Make the bin directory
    if(![System.IO.Directory]::Exists($nugetDir)) {
        mkdir $nugetDir | Out-Null
        Write-Host "Created directory $nugetDir"
    }

    # Download nuget.exe
    if(![System.IO.File]::Exists($nugetPath)) {
        $wc = New-Object System.Net.WebClient
        $wc.DownloadFile('https://dist.nuget.org/win-x86-commandline/latest/nuget.exe', $nugetPath)
        $wc.Dispose()
        Write-Host "Downloaded nuget.exe to $nugetDir"
    }

    # Add the path to the user's PATH variable 
    if(@($env:Path.Split(';') | ?{ $_ -eq $nugetDir }).count -eq 0) {
        $userPath = [System.Environment]::GetEnvironmentVariable('PATH', 'User')
        $userPath += ';' + $nugetDir
        [System.Environment]::SetEnvironmentVariable('PATH', $userPath, 'User') 
        . Refresh-EnvVars

        Write-Host "Added $nugetDir to user's PATH variable"
    } 
} 
 
function Import-Assembly {
    param([string]$Path, [switch]$CopyLocal)

    if(!(Test-Path $Path)) {
        throw (New-Object System.IO.FileNotFoundException)
        return
    }

    if($CopyLocal) {
        $binPath = Join-Path $(Get-SessionProject) bin
        $sourceDir = Split-Path $Path -Parent

        if(!(Test-Path $binPath)) { mkdir $binPath | Out-Null }
         
        $asmFiles = @(dir $sourceDir -Filter '*.dll') + @(dir $sourceDir -Filter '*.exe')
        foreach($asmFile in $asmFiles) {
            $targetPath = Join-Path $binPath $asmFile.Name
            if(!(Test-Path $targetPath)) {
                Copy-Item $asmFile.FullName $targetPath
            } 
        }

        $Path = Join-Path $binPath $(Split-Path $Path -Leaf)
    }
    
    return [System.Reflection.Assembly]::LoadFrom($Path)
}


function Get-Nuspec {
    param([string]$NupkgPath)

    if(!(Test-Path $NupkgPath)) { 
        Write-Warning "Path '$NupkgPath' does not exist"
        return
    }

    [System.IO.FileStream]$fs = $null
    [System.IO.Compression.ZipArchive]$arch = $null 
    [System.IO.Stream]$entryStream = $null
    [System.IO.StreamReader]$sr = $null
    [string]$content = $null
    try {
        $fs = New-Object System.IO.FileStream $NupkgPath, Open
        $arch = New-Object System.IO.Compression.ZipArchive $fs, Read
        $nuspecEntry = $arch.Entries | ? { $_.Name -like "*.nuspec" } | select -First 1
        if($nuspecEntry -ne $null) { 
            $entryStream = $nuspecEntry.Open()
            $sr = New-Object System.IO.StreamReader $entryStream
            $content = $sr.ReadToEnd() 
        }
    } finally { 
        if($sr -ne $null) { try { $sr.Dispose() } catch { } }
        if($entryStream -ne $null) { try { $entryStream.Dispose() } catch { } }
        if($arch -ne $null) { try { $arch.Dispose() } catch { } }
        if($fs -ne $null) { try { $fs.Dispose() } catch { } }
    }

    return [xml]$content
}

function Get-FrameworkNames {
    param([string]$FrameworkMoniker)

    $names = @($FrameworkMoniker)
    if($FrameworkMoniker -match '^net\d+') {
        $version = $FrameworkMoniker.Substring(3)
        $expandedVersion = $version[0] + '.' + $version.Substring(1)
        $names += @('.NETFramework' + $expandedVersion)
    }

    if($FrameworkMoniker -match '^netstandard') {
        $version = $FrameworkMoniker.Substring(11)
        $names += @('.NETStandard' + $expandedVersion)
    }

    return [string[]]$names
}

function Load-Package {
    param([string]$PackageID)

    $packageRootDir = Join-Path $(Get-SessionProject) packages
    $packageDir = Join-Path $packageRootDir $PackageID 

    $libDir = $null
    $fmk    = $null 

    foreach($fmkItem in @('net46', 'net45', 'net40', 'netstandard2.0', 'netstandard1.3', 'netstandard1.0')) {
        $testPath = "$packageDir\lib\$fmkItem"
        if(Test-Path $testPath) {
            $libDir = $testPath
            $fmk = $fmkItem
            break
        }
    }
     
    if(($libDir -ne $null) -and (Test-Path $libDir)) {
        
        $asmPath = $null
        if(Test-Path "$libDir\$PackageID.dll") {
            $asmPath = "$libDir\$PackageID.dll"
        } elseif(Test-Path "$libDir\$PackageID.exe") {
            $asmPath = "$libDir\$PackageID.exe"
        }

        if(Test-Path $asmPath) {  
            # Found the local assembly. 
            # Now also check nuspec for dependencies 
            $nupkg = dir $packageDir -Filter "*.nupkg" | select -First 1
            if($nupkg -ne $null) {
                $nuspec = Get-Nuspec $nupkg.FullName

                $deps = $nuspec.package.metadata.dependencies
                $depGroup = $null
                if($deps -ne $null) {
                    if($deps.dependency -ne $null)        {
                        $depGroup = $deps
                    } elseif ($deps.group -ne $null) {
                        foreach($fmkName in (Get-FrameworkNames $fmk)) {
                            $group = $deps.group | ? { $_.targetFramework -eq $fmkName } | select -First 1
                            if($group -ne $null) {
                                $depGroup = $group
                                break
                            }
                        } 
                    }
                }

                if($depGroup -ne $null) {
                    foreach($dep in $depGroup.dependency) {
                        $depID = $dep.id
                        Write-Host "Loading required package $depID"
                        Load-Package $depID
                    }
                }
            } 

            return Import-Assembly $asmPath -CopyLocal
        }
    }
}

function Import-Package {

    param([String]$PackageID, [String]$Version, [String]$Source)

    if([string]::IsNullOrEmpty($PackageID)) { Write-Warning 'Missing required paramter PackageID'; return }
    if([string]::IsNullOrEmpty($Source)) { $Source = 'https://api.nuget.org/v3/index.json' } 
    
    $packageRootDir = Join-Path $(Get-SessionProject) packages

    $args = [string[]]@(
        'install',
        $PackageID,
        '-OutputDirectory',
        $packageRootDir,
        '-ExcludeVersion',
        '-Source',
        $Source
    )

    if(![string]::IsNullOrEmpty($Version)) {
        $args += [string[]]@('-Version', $Version)
    }

    &nuget $args | Write-Host 

    Load-Package $PackageID
}

<#
function Get-Repository  {

    param([String]$Uri)

    $repo = New-Object Ockham.NuGet.Repository $Uri
    $repo.Logger = New-Object Ockham.NuGet.Logging.EventLogger
    Register-ObjectEvent $repo.Logger LogDataReceived -Action {
        param($sender, $e) 

        switch($e.Level) {
            'Debug'       { Write-Debug $e.Data }
            'Verbose'     { Write-Verbose $e.Data }
            'Warning'     { Write-Warning $e.Data }
            'Error'       { Write-Error $e.Data }
            'Information' { Write-Host $e.Data }
            'Minimal'     { Write-Host $e.Data } 
        }
    } 

    $repo
}
#>

. Clean-SessionProjects

Export-ModuleMember -Function Import-Assembly, Import-Package, Confirm-Nuget, Get-SessionProject