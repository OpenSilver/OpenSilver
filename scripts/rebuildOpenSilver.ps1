param (
    $OS_VERSION = $(if ($null -eq $env:OS_VERSION) { "1.2.0" } else { $env:OS_VERSION }),
    $OS_CONFIGURATION = $(if ($null -eq $env:OS_CONFIGURATION) { "SL" } else { $env:OS_CONFIGURATION }),
    $OS_NAME = $(if ($null -eq $env:OS_NAME) { "OpenSilver" } else { $env:OS_NAME }),
    $OS_SIMULATOR_NAME = $(if ($null -eq $env:OS_SIMULATOR_NAME) { "OpenSilver.Simulator" } else { $env:OS_SIMULATOR_NAME }),
    $OS_SIMULATOR_VERSION = $(if ($null -eq $env:OS_SIMULATOR_VERSION) { "1.2.0" } else { $env:OS_SIMULATOR_VERSION }),
    $OS_BUILD_VERSION=$(if ($null -eq $env:OS_BUILD_VERSION) { "1.2.0" } else { $env:OS_BUILD_VERSION }),
    $MSBUILD = "msbuild.exe",
    $NUGET_CACHE = "$env:USERPROFILE\.nuget\packages",
    $COPY_PDB = $false,
    $UPDATE_LOCAL_CACHE = $false,
    $BUILD_SIMULATOR = $false
)

if ($true -eq $COPY_PDB) {
    $BUILD_PARAMS = "/p:DebugSymbols=true /p:DebugType=Full /p:Optimize=false"
}
else {
    # DEFAULT RELEASE BUILD
    $BUILD_PARAMS = "/p:DebugSymbols=false /p:DebugType=None /p:Optimize=true"
    (Get-Content -path ..\build\nuspec\OpenSilver.nuspec -Raw -Encoding UTF8) `
        -replace '<file src(.+)\.pdb" />', '<!-- replaced -->' `
        | Out-File -FilePath ..\build\nuspec\OpenSilver.nuspec -Encoding UTF8
}

Push-Location ".."
try {

    # Recompile opensilver.compiler and copy into local cache
    Write-Output "a" | .\restore-packages-opensilver.bat
    get-childitem -Include bin -Recurse -force | Remove-Item -Force -Recurse
    get-childitem -Include obj -Recurse -force | Remove-Item -Force -Recurse

    Invoke-Expression "&'$MSBUILD' .\build\slnf\OpenSilver.slnf /t:""Restore;Clean"" /p:Configuration=$OS_CONFIGURATION /consoleloggerparameters:ErrorsOnly"

    .\build\nuget.exe install Mono.Cecil -Version 0.9.5.0 -OutputDirectory src\packages
    .\build\nuget.exe install MahApps.Metro -Version 2.4.9 -OutputDirectory src\packages
    .\build\nuget.exe install Microsoft.Web.WebView2 -Version 1.0.1905-prerelease -OutputDirectory src\packages
    .\build\nuget.exe install Microsoft.Web.WebView2.DevToolsProtocolExtension -Version 1.0.824 -OutputDirectory src\packages
    .\build\nuget.exe install Microsoft.Extensions.ObjectPool -Version 7.0.4 -OutputDirectory src\packages
    .\build\nuget.exe install NuGet.Build.Tasks.Pack -Version 5.9.1 -OutputDirectory src\packages

    taskkill /fi "imagename eq msbuild.exe" /f
    Invoke-Expression "&'$MSBUILD' .\src\Compiler\Compiler\Compiler.OpenSilver.csproj /p:Configuration=$OS_CONFIGURATION /consoleloggerparameters:ErrorsOnly $BUILD_PARAMS"
    Invoke-Expression "&'$MSBUILD' .\src\Compiler\Compiler.ResourcesExtractor\Compiler.ResourcesExtractor.OpenSilver.csproj /p:Configuration=Release /consoleloggerparameters:ErrorsOnly $BUILD_PARAMS"

    # Compiler
    xcopy /y "src\Compiler\Compiler\bin\OpenSilver\$OS_CONFIGURATION\net461\OpenSilver.Compiler.*" "src\packages\$OS_NAME.$OS_BUILD_VERSION\tools\"
    xcopy /y "src\Compiler\Compiler\bin\OpenSilver\$OS_CONFIGURATION\net461\Mono.Cecil.*" "src\packages\$OS_NAME.$OS_BUILD_VERSION\tools\"
    xcopy /y "src\Compiler\Compiler.ResourcesExtractor\bin\OpenSilver\Release\net461\OpenSilver.Compiler.Resources.*" "src\packages\$OS_NAME.$OS_BUILD_VERSION\tools\"

    # Targets
    xcopy /y "src\Targets\$OS_NAME.targets" "src\packages\$OS_NAME.$OS_BUILD_VERSION\build\"
    xcopy /y "src\Targets\OpenSilver.Common.targets" "src\packages\$OS_NAME.$OS_BUILD_VERSION\build\"

    # Build
    Invoke-Expression "&'$MSBUILD' .\build\slnf\OpenSilver.slnf /p:Configuration=$OS_CONFIGURATION /consoleloggerparameters:ErrorsOnly /t:""clean;build"" $BUILD_PARAMS"

    # Packaging 
    Push-Location build
    try {

        $date = (get-date -Format "MM/dd/yyy HH:mm:ss").toString()

        If(!(test-path temp)) {
            mkdir temp
        }
        Write-Output "Opensilver $OS_VERSION ($date)" > temp/Version.txt
        .\nuget.exe pack nuspec\OpenSilver.nuspec -OutputDirectory "output/OpenSilver" -Properties "PackageId=$OS_NAME;PackageVersion=$OS_VERSION;Configuration=$OS_CONFIGURATION;Target=$OS_NAME"

        if ($true -eq $BUILD_SIMULATOR) {
            Invoke-Expression "&'$MSBUILD' .\slnf\OpenSilver.Simulator.slnf /p:Configuration=$OS_CONFIGURATION /consoleloggerparameters:ErrorsOnly /t:""clean;restore;build"""

            Write-Output "Opensilver.Simulator $OS_VERSION ($date)" > temp/Version.txt
            Invoke-Expression "&'$MSBUILD' -t:pack .\slnf\OpenSilver.Simulator.slnf -p:Configuration=SL -p:PackageOutputPath=""$pwd/output/OpenSilver"" -p:NuspecFile=""$pwd/nuspec/OpenSilver.Simulator.nuspec"" -p:NuspecBasePath=""$pwd"" -p:NuspecProperties=PackageVersion=$OS_SIMULATOR_VERSION /consoleloggerparameters:ErrorsOnly"
        }
    }
    finally{
        Pop-Location
    }

    if ($true -eq $UPDATE_LOCAL_CACHE) {

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

            # Removal is optional due to us perhaps compiling a future version
            if (Test-Path $NUGET_CACHE\$OS_NAME\$OS_VERSION) {
                Remove-Item $NUGET_CACHE\$OS_NAME\$OS_VERSION -Force -Recurse -Confirm:$false
            }
            dotnet add package $OS_NAME -v $OS_VERSION -s ..\build\output

            if ($true -eq $BUILD_SIMULATOR) {
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
            xcopy /y "src\Runtime\Runtime\bin\OpenSilver\$OS_CONFIGURATION\netstandard2.0\OpenSilver.*" "$NUGET_CACHE\$OS_NAME\$OS_VERSION\lib\netstandard2.0\"
            xcopy /y "src\Compiler\Compiler\bin\OpenSilver\$OS_CONFIGURATION\net461\OpenSilver.Compiler.*" "$NUGET_CACHE\$OS_NAME\$OS_VERSION\tools\"

            if ($true -eq $BUILD_SIMULATOR) {
                xcopy /y "src\Simulator\Simulator\bin\OpenSilver\Debug\*" "$NUGET_CACHE\$OS_SIMULATOR_NAME\$OS_SIMULATOR_VERSION\lib\net461\"
            }
        }
    }
}
finally {
    Pop-Location
}