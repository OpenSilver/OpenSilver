param (
    $OS_VERSION = $(if ($null -eq $env:OS_VERSION) { "2.0.0" } else { $env:OS_VERSION }),
    $OS_NAME = $(if ($null -eq $env:OS_NAME) { "OpenSilver" } else { $env:OS_NAME }),
    $OS_SIMULATOR_NAME = $(if ($null -eq $env:OS_SIMULATOR_NAME) { "OpenSilver.Simulator" } else { $env:OS_SIMULATOR_NAME }),
    $OS_SIMULATOR_VERSION = $(if ($null -eq $env:OS_SIMULATOR_VERSION) { "2.0.0" } else { $env:OS_SIMULATOR_VERSION }),
    $MSBUILD = "msbuild.exe",
    $NUGET_CACHE = "$env:USERPROFILE\.nuget\packages",
    $COPY_PDB = $false,
    $UPDATE_LOCAL_CACHE = $false,
    $BUILD_SIMULATOR = $false
)

function Green
{
    process { Write-Host $_ -ForegroundColor Green }
}

Write-Output "Getting stable version" | Green

$versionInfoFilePath = "..\build\version_info.txt"

if (Test-Path $versionInfoFilePath) {
    $stableVersionLine = Get-Content $versionInfoFilePath | Where-Object { $_ -match "^STABLE_VERSION=" }

    if ($stableVersionLine) {
        $OS_BUILD_VERSION = $stableVersionLine -replace "^STABLE_VERSION=", ""
        # Output or use the variable as needed
        Write-Output "Stable Version: $OS_BUILD_VERSION"
    } else {
        throw "STABLE_VERSION not found in the file."
    }
} else {
    throw "File not found: $versionInfoFilePath"
}

if ($true -eq $COPY_PDB) {
    $BUILD_PARAMS = "/p:DebugSymbols=true /p:DebugType=Full /p:Optimize=false"
    $OS_CONFIGURATION = "Debug"
}
else {
    # DEFAULT RELEASE BUILD
    $BUILD_PARAMS = "/p:DebugSymbols=false /p:DebugType=None /p:Optimize=true"
    (Get-Content -path ..\build\nuspec\OpenSilver.nuspec -Raw -Encoding UTF8) `
        -replace '<file src(.+)\.pdb" />', '<!-- replaced -->' `
        | Out-File -FilePath ..\build\nuspec\OpenSilver.nuspec -Encoding UTF8
    $OS_CONFIGURATION = "Release"
}

Push-Location ".."
try {

    Write-Output "Restoring Packages" | Green
    # Recompile opensilver.compiler and copy into local cache
    Write-Output "a" | .\restore-packages-opensilver.bat
    get-childitem -Include bin -Recurse -force | Remove-Item -Force -Recurse
    get-childitem -Include obj -Recurse -force | Remove-Item -Force -Recurse

    Invoke-Expression "&'$MSBUILD' .\build\slnf\OpenSilver.slnf /t:""Restore;Clean"" /p:Configuration=$OS_CONFIGURATION /consoleloggerparameters:ErrorsOnly"

    Write-Output "Installing Additional Packages" | Green

    .\build\nuget.exe install Mono.Cecil -Version 0.9.5.0 -OutputDirectory src\packages -Verbosity quiet
    .\build\nuget.exe install MahApps.Metro -Version 2.4.9 -OutputDirectory src\packages -Verbosity quiet
    .\build\nuget.exe install Microsoft.Web.WebView2 -Version 1.0.1905-prerelease -OutputDirectory src\packages -Verbosity quiet
    .\build\nuget.exe install Microsoft.Web.WebView2.DevToolsProtocolExtension -Version 1.0.824 -OutputDirectory src\packages -Verbosity quiet
    .\build\nuget.exe install Microsoft.Extensions.ObjectPool -Version 7.0.4 -OutputDirectory src\packages -Verbosity quiet
    .\build\nuget.exe install NuGet.Build.Tasks.Pack -Version 5.9.1 -OutputDirectory src\packages -Verbosity quiet

    Write-Output "Killing MSBuild instances" | Green
    taskkill /fi "imagename eq msbuild.exe" /f
    Write-Output "Building Compiler" | Green
    Invoke-Expression "&'$MSBUILD' .\src\Compiler\Compiler\Compiler.OpenSilver.csproj /p:Configuration=$OS_CONFIGURATION /consoleloggerparameters:ErrorsOnly $BUILD_PARAMS"
    Write-Output "Building ResourcesExtractor" | Green
    Invoke-Expression "&'$MSBUILD' .\src\Compiler\Compiler.ResourcesExtractor\Compiler.ResourcesExtractor.OpenSilver.csproj /p:Configuration=Release /consoleloggerparameters:ErrorsOnly $BUILD_PARAMS"

    # Compiler
    Write-Output "Copying OpenSilver.Compiler.* files to local packages folder" | Green
    xcopy /y "src\Compiler\Compiler\bin\$OS_CONFIGURATION\net461\OpenSilver.Compiler.*" "src\packages\$OS_NAME.$OS_BUILD_VERSION\tools\"
    if ($LASTEXITCODE -ne 0) {
        throw "Xcopy failed with exit code $LASTEXITCODE"
    }

    Write-Output "Copying Mono.Cecil.* files to local packages folder" | Green
    xcopy /y "src\Compiler\Compiler\bin\$OS_CONFIGURATION\net461\Mono.Cecil.*" "src\packages\$OS_NAME.$OS_BUILD_VERSION\tools\"
    if ($LASTEXITCODE -ne 0) {
        throw "Xcopy failed with exit code $LASTEXITCODE"
    }

    Write-Output "Copying OpenSilver.Compiler.Resources.* files to local packages folder" | Green
    xcopy /y "src\Compiler\Compiler.ResourcesExtractor\bin\Release\net461\OpenSilver.Compiler.Resources.*" "src\packages\$OS_NAME.$OS_BUILD_VERSION\tools\"
    if ($LASTEXITCODE -ne 0) {
        throw "Xcopy failed with exit code $LASTEXITCODE"
    }

    # Targets
    Write-Output "Copying $OS_NAME.targets" | Green
    xcopy /y "src\Targets\$OS_NAME.targets" "src\packages\$OS_NAME.$OS_BUILD_VERSION\build\"
    if ($LASTEXITCODE -ne 0) {
        throw "Xcopy failed with exit code $LASTEXITCODE"
    }

    Write-Output "Copying OpenSilver.Common.targets" | Green
    xcopy /y "src\Targets\OpenSilver.Common.targets" "src\packages\$OS_NAME.$OS_BUILD_VERSION\build\"
    if ($LASTEXITCODE -ne 0) {
        throw "Xcopy failed with exit code $LASTEXITCODE"
    }

    # Build
    Write-Output "Building OpenSilver" | Green
    Invoke-Expression "&'$MSBUILD' .\build\slnf\OpenSilver.slnf /p:Configuration=$OS_CONFIGURATION /consoleloggerparameters:ErrorsOnly /t:""clean;build"" $BUILD_PARAMS"
    if ($LASTEXITCODE -ne 0) {
        throw "Msbuild failed with exit code $LASTEXITCODE"
    }

    # Packaging
    Push-Location build
    try {

        $date = (get-date -Format "MM/dd/yyy HH:mm:ss").toString()

        If(!(test-path temp)) {
            mkdir temp
        }

        Write-Output "Packing OpenSilver package" | Green
        Write-Output "Opensilver $OS_VERSION ($date)" > temp/Version.txt
        .\nuget.exe pack nuspec\OpenSilver.nuspec -OutputDirectory "output/OpenSilver" -Properties "PackageId=$OS_NAME;PackageVersion=$OS_VERSION;Configuration=$OS_CONFIGURATION;Target=$OS_NAME"
        if ($LASTEXITCODE -ne 0) {
            throw "Nuget failed with exit code $LASTEXITCODE"
        }

        if ($true -eq $BUILD_SIMULATOR) {
            Write-Output "Building Simulator" | Green
            Invoke-Expression "&'$MSBUILD' .\slnf\OpenSilver.Simulator.slnf /p:Configuration=$OS_CONFIGURATION /consoleloggerparameters:ErrorsOnly /t:""clean;restore;build"""
            if ($LASTEXITCODE -ne 0) {
                throw "Msbuild failed with exit code $LASTEXITCODE"
            }

            Write-Output "Packing Simulator" | Green
            Write-Output "Opensilver.Simulator $OS_VERSION ($date)" > temp/Version.txt
            .\nuget.exe pack nuspec\OpenSilver.Simulator.nuspec -OutputDirectory "output/OpenSilver" -BasePath "./" -Properties "PackageVersion=$OS_VERSION;Configuration=$OS_CONFIGURATION"
            if ($LASTEXITCODE -ne 0) {
                throw "Nuget failed with exit code $LASTEXITCODE"
            }
        }
    }
    finally{
        Pop-Location
    }

    if ($true -eq $UPDATE_LOCAL_CACHE) {
        Write-Output "Updating Local Cache" | Green
        # Force Cache restore
        If(!(test-path cache-temp)) {
            mkdir cache-temp
        }

        Push-Location cache-temp
        try {
            taskkill /fi "imagename eq msbuild.exe" /f
            dotnet new classlib
            dotnet add package $OS_NAME -v $OS_VERSION
            dotnet remove package $OS_NAME

            Write-Output "Installing OpenSilver package" | Green
            # Removal is optional due to us perhaps compiling a future version
            if (Test-Path $NUGET_CACHE\$OS_NAME\$OS_VERSION) {
                Remove-Item $NUGET_CACHE\$OS_NAME\$OS_VERSION -Force -Recurse -Confirm:$false
            }
            dotnet add package $OS_NAME -v $OS_VERSION -s ..\build\output

            if ($true -eq $BUILD_SIMULATOR) {
                Write-Output "Installing Simulator package" | Green
                if (Test-Path $NUGET_CACHE\$OS_SIMULATOR_NAME\$OS_SIMULATOR_VERSION) {
                    Remove-Item $NUGET_CACHE\$OS_SIMULATOR_NAME\$OS_SIMULATOR_VERSION -Force -Recurse -Confirm:$false
                }
                dotnet add package $OS_SIMULATOR_NAME -v $OS_SIMULATOR_VERSION -s ..\build\output
            }
        }
        finally {
            Pop-Location
            Remove-Item cache-temp -Force -Recurse -Confirm:$false
        }

        if ($COPY_PDB -eq $true) {
            Write-Output "Copying pdb files" | Green
            xcopy /y "src\Runtime\Runtime\bin\$OS_CONFIGURATION\netstandard2.0\OpenSilver.*" "$NUGET_CACHE\$OS_NAME\$OS_VERSION\lib\netstandard2.0\"
            if ($LASTEXITCODE -ne 0) {
                throw "Xcopy failed with exit code $LASTEXITCODE"
            }
            xcopy /y "src\Compiler\Compiler\bin\$OS_CONFIGURATION\net461\OpenSilver.Compiler.*" "$NUGET_CACHE\$OS_NAME\$OS_VERSION\tools\"
            if ($LASTEXITCODE -ne 0) {
                throw "Xcopy failed with exit code $LASTEXITCODE"
            }

            if ($true -eq $BUILD_SIMULATOR) {
                xcopy /y "src\Simulator\Simulator\bin\Debug\net462\*" "$NUGET_CACHE\$OS_SIMULATOR_NAME\$OS_SIMULATOR_VERSION\lib\net461\"
                if ($LASTEXITCODE -ne 0) {
                    throw "Xcopy failed with exit code $LASTEXITCODE"
                }
            }
        }
    }
}
finally {
    Pop-Location
}