@echo off
rem define the escape character for colored text
for /F %%a in ('"prompt $E$S & echo on & for %%b in (1) do rem"') do set "ESC=%%a"

set /p PackageVersion="%ESC%[92mCSHTML5 version:%ESC%[0m 2.0.0-alpha"

echo. 
echo %ESC%[95mRestoring NuGet packages%ESC%[0m
echo. 
nuget restore ../src/CSHTML5.sln -v quiet
nuget restore ../src/VSExtension/VSExtension.CSHTML5.sln -MSBuildVersion 14 -v quiet

echo. 
echo %ESC%[95mBuilding %ESC%[0mMigration %ESC%[95mconfiguration%ESC%[0m
echo. 
msbuild ../src/CSHTML5.sln -p:Configuration=Migration -clp:ErrorsOnly
echo. 
echo %ESC%[95mPacking %ESC%[0mCSHTML5.Migration %ESC%[95mNuGet package%ESC%[0m
echo. 
nuget.exe pack nuspec\CSHTML5.nuspec -OutputDirectory "output/CSHTML5" -Properties "PackageId=CSHTML5.Migration;Target=CSHTML5.Migration;PackageVersion=2.0.0-alpha%PackageVersion%;Configuration=Migration;AssembliesPrefix=CSHTML5.Migration;CompilerPrefix=SLMigration.CSharpXamlForHtml5"

echo. 
echo %ESC%[95mBuilding %ESC%[0mDebug %ESC%[95mconfiguration%ESC%[0m
echo. 
msbuild ../src/CSHTML5.sln -p:Configuration=Debug -clp:ErrorsOnly
echo. 
echo %ESC%[95mPacking %ESC%[0mCSHTML5 %ESC%[95mNuGet package%ESC%[0m
echo. 
nuget.exe pack nuspec\CSHTML5.nuspec -OutputDirectory "output/CSHTML5" -Properties "PackageId=CSHTML5;;Target=CSHTML5;PackageVersion=2.0.0-alpha%PackageVersion%;Configuration=Debug;AssembliesPrefix=CSHTML5;CompilerPrefix=CSharpXamlForHtml5"

rem echo. 
rem echo %ESC%[95mBuilding %ESC%[0mVSIX %ESC%[0m
rem echo. 
rem msbuild ../src/VSExtension/VSExtension.CSHTML5.sln -p:Configuration=Release -p:VisualStudioVersion=14.0
rem echo. 
rem echo %ESC%[95mCopying %ESC%[0mCSHTML5.vsix %ESC%[95mto output folder%ESC%[0m
rem echo. 
rem xcopy ..\src\VSExtension\CSHTML5.Vsix\bin\CSHTML5\Release\CSHTML5.vsix output\CSHTML5\ /Y

explorer "output\CSHTML5"

pause
