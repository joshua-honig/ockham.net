<#
    .SYNOPSIS
    Ensure PS Location and Environment.CurrentDirectory are in sync
#>
function Set-CurrentDirectory {
    param([string]$Path)

    if([string]::IsNullOrEmpty($Path)) {
        $Path = $(Get-Location).Path
    }

    Set-Location $Path
    [Environment]::CurrentDirectory = $Path
}

<#
    .SYNOPSIS
    Ensure PS Location and Environment.CurrentDirectory are in sync
#>
function Get-CurrentDirectory {
    Set-CurrentDirectory
    [Environment]::CurrentDirectory
}

function Write-Bar {
    param([string]$Char = '=', [int]$Length = 120)
    
    if([string]::IsNullOrEmpty($Char)) { $Char = '=' }
    $Char = $Char.Substring(0, 1)
    Write-Host $($Char * $Length)
}

function Write-Banner {
    param([string]$Message, [string]$Char = '=')
     
    Write-Bar $Char 
    Write-Host " $Message"
    Write-Bar $Char
}

function Invoke-Clean {
    param([string]$ProjectDirectory)

    if(!(Test-Path $ProjectDirectory)) { return }

    if(Test-Path $ProjectDirectory) {
        if(Test-Path "$ProjectDirectory\bin") {
            Remove-Item "$ProjectDirectory\bin" -Force -Recurse -ErrorAction SilentlyContinue
        }
        if(Test-Path "$ProjectDirectory\obj") {
            Remove-Item "$ProjectDirectory\obj" -Force -Recurse -ErrorAction SilentlyContinue
        }
    }
}

function Invoke-Build {
    param([string]$ProjectDirectory, [string]$Configuration, [switch]$Clean)
    
    if(!(Test-Path $ProjectDirectory)) { return }

    if($Clean) {
        Invoke-Clean $ProjectDirectory
    }

    if([string]::IsNullOrEmpty($Configuration)) { $Configuration = 'Release' }

    $initialDir = Get-CurrentDirectory
    Set-CurrentDirectory $ProjectDirectory
    dotnet build -c $Configuration

    Set-CurrentDirectory $initialDir
}

function Test-API {
    param([string]$SolutionDirectory, [string]$ToolsPath, [string]$Configuration, [string]$OutputPath, [switch]$Quiet)

    $refDir = Join-Path $SolutionDirectory ref
    $srcDir = Join-Path $SolutionDirectory src

    $genApi = "$ToolsPath\bin\GenAPI\GenAPI.exe"

    function _Warn ($message) {
        if(!$Quiet) { Write-Warning $message }
        return
    }

    if(!(Test-Path $refDir)) {  
        return _Warn "Reference project not found. Expected path: $refDir" 
    }
     
    $refBin = "$refDir\bin\$Configuration"
    if(!(Test-Path $refBin)) {
        return _Warn "Reference project bin output path not found. Expected path: $refBin" 
    }

    if(@(Get-ChildItem $refBin -Filter *.dll).Count -eq 0) {
        $childDir = Get-ChildItem $refBin -Directory | select -First 1
        if(($childDir -eq $null) -or (@(Get-ChildItem $childDir.FullName -Filter *.dll).Count -eq 0)) {
            return _Warn "No assemblies found in reference bin output path $refBin"
        } else {
            $refBin = $childDir.FullName
        }
    }

    if(!(Test-Path $srcDir)) {
        return _Warn "Source project not found. Expected path: $srcDir"
    }

    $outBin = "$srcDir\bin\$Configuration"
    if(!(Test-Path $outBin)) {
        return _Warn "Source project bin output path not found. Expected path: $outBin"
    }

    $outBinPaths = New-Object System.Collections.Generic.List[string]
    if(@(Get-ChildItem $outBin -Filter *.dll).Count -gt 0) {
        # Single output bin dir
        $outBinPaths.Add($outBin)
    } else {
        foreach($childDir in @(Get-ChildItem $outBin -Directory)) {
            if(@(Get-ChildItem $childDir.FullName -Filter *.dll).Count -gt 0) {
                $outBinPaths.Add($childDir.FullName)
            }
        } 
    }
     
    if($outBinPaths.Count -eq 0) {
        return _Warn "No assemblies found in source bin output path $outBin"
    }

    if([string]::IsNullOrEmpty($OutputPath)) {
        $OutputPath = Join-Path $(Join-Path $($env:TEMP) Test) $([Guid]::NewGuid().ToString('n'))
    } else {
        Remove-Item "$OutputPath\API*.cs"
        Remove-Item "$OutputPath\API*.diff"
    }

    if(!(Test-Path $OutputPath)) { mkdir $OutputPath | Out-Null }
     
    Write-Host "Generating API files to $OutputPath"

    $refOut = Join-Path $OutputPath 'API_Ref.cs'

    &$genApi $refBin -out:$refOut -apiOnly

    foreach($binPath in $outBinPaths) {
        $itemOut = Join-Path $OutputPath $('API_' + $(Split-Path $binPath -Leaf) + '.cs')
        $diffOut = [regex]::Replace($itemOut, '.cs$', '.diff')
         
        &$genApi $binPath -out:$itemOut -apiOnly
        git diff --no-index $refOut $itemOut > $diffOut
    }
}

function Get-TestSummary {
    param([string]$SolutionDirectory)

    $apiDir   = [System.IO.Path]::Combine($SolutionDirectory, 'test', 'output', 'api')
    $testsDir = [System.IO.Path]::Combine($SolutionDirectory, 'test', 'output', 'tests')
    $results  = [System.IO.Path]::Combine($SolutionDirectory, 'test', 'output', 'results')
      
    $results = New-Object System.Collections.Generic.List[psobject]
     
    if(Test-Path $apiDir) { 
        foreach($diffFile in (dir $apiDir -Filter '*.diff')) { 
            $itemPassed = $true

            $diffContent = [System.IO.File]::ReadAllText($diffFile.FullName).Trim()
            $itemPassed  = $diffContent.Length -eq 0 

            $results.Add((New-Object psobject -Property @{
                type      = 'API Diff'
                framework = $diffFile.Name.Substring(4).Replace('.diff', '')
                result    = $itemPassed
                file      = $diffFile.FullName
            }))
        }
    }

    if(Test-Path $testsDir) {
        foreach($testXml in (dir $testsDir -Filter *.xml)) {
            $itemPassed = $true

            $xTest = New-Object xml
            $xTest.Load($testXml.FullName)
            $failCount = $xTest.DocumentElement.SelectNodes('//test[@result!="Pass"]').Count
            $itemPassed = ($failCount -eq 0)

            $results.Add((New-Object psobject -Property @{
                Type      = 'xUnit'
                Framework = $testXml.Name.Split('-')[1].Replace('.xml', '')
                Result    = $itemPassed
                File      = $testXml.FullName
            }))
        }
    }

    $results
}