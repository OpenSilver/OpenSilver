@echo off
rem define the escape character for colored text
for /F %%a in ('"prompt $E$S & echo on & for %%b in (1) do rem"') do set "ESC=%%a"

set /p PackageVersion="%ESC%[92mOpenSilver version:%ESC%[0m 1.0.0-alpha-"

echo. 
echo %ESC%[95mRestoring NuGet packages%ESC%[0m
echo. 
nuget restore ../src/OpenSilver.sln
nuget restore ../src/VSExtension/VSExtension.OpenSilver.sln -MSBuildVersion 14

echo. 
echo %ESC%[95mBuilding %ESC%[0mSL %ESC%[95mconfiguration%ESC%[0m
echo. 
msbuild ../src/OpenSilver.sln -p:Configuration=SL -restore
echo. 
echo %ESC%[95mPacking %ESC%[0mOpenSilver %ESC%[95mNuGet package%ESC%[0m
echo. 
nuget.exe pack data\OpenSilver.nuspec -OutputDirectory "output/OpenSilver" -Properties "PackageId=OpenSilver;PackageVersion=1.0.0-alpha-%PackageVersion%;Configuration=SL;AssembliesPrefix=OpenSilver"

echo. 
echo %ESC%[95mBuilding %ESC%[0mSL.WorkInProgress %ESC%[95mconfiguration%ESC%[0m
echo. 
msbuild ../src/OpenSilver.sln -p:Configuration=SL.WorkInProgress
echo. 
echo %ESC%[95mPacking %ESC%[0mOpenSilver.WorkInProgress %ESC%[95mNuGet package%ESC%[0m
echo. 
nuget.exe pack data\OpenSilver.nuspec -OutputDirectory "output/OpenSilver" -Properties "PackageId=OpenSilver.WorkInProgress;PackageVersion=1.0.0-alpha-%PackageVersion%;Configuration=SL.WorkInProgress;AssembliesPrefix=OpenSilver"

echo. 
echo %ESC%[95mBuilding %ESC%[0mUWP %ESC%[95mconfiguration%ESC%[0m
echo. 
msbuild ../src/OpenSilver.sln -p:Configuration=UWP
echo. 
echo %ESC%[95mPacking %ESC%[0mOpenSilver.UWPCompatible %ESC%[95mNuGet package%ESC%[0m
echo. 
nuget.exe pack data\OpenSilver.nuspec -OutputDirectory "output/OpenSilver" -Properties "PackageId=OpenSilver.UWPCompatible;PackageVersion=1.0.0-alpha-%PackageVersion%;Configuration=UWP;AssembliesPrefix=OpenSilver.UWPCompatible"

rem echo. 
rem echo %ESC%[95mBuilding %ESC%[0mVSIX %ESC%[0m
rem echo. 
rem msbuild ../src/VSExtension/VSExtension.OpenSilver.sln -p:Configuration=Release -p:VisualStudioVersion=14.0
rem echo. 
rem echo %ESC%[95mCopying %ESC%[0mOpenSilver.vsix %ESC%[95mto output folder%ESC%[0m
rem echo. 
rem xcopy ..\src\VSExtension\OpenSilver.VSIX\bin\OpenSilver\Release\OpenSilver.vsix output\OpenSilver\ /Y

explorer "output\OpenSilver"

pause
