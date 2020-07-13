@echo off
rem define the escape character for colored text
for /F %%a in ('"prompt $E$S & echo on & for %%b in (1) do rem"') do set "ESC=%%a"

set /p PackageVersion="%ESC%[92mCSHTML5 version:%ESC%[0m 2.0.0-alpha"

echo. 
echo %ESC%[95mBuilding %ESC%[0mMigration %ESC%[95mconfiguration%ESC%[0m
echo. 
msbuild ../src/CSHTML5.sln -p:Configuration=Migration -restore
echo. 
echo %ESC%[95mPacking %ESC%[0mCSHTML5.Migration %ESC%[95mNuGet package%ESC%[0m
echo. 
nuget.exe pack data\CSHTML5.nuspec -OutputDirectory "output" -Properties "PackageId=CSHTML5.Migration;PackageVersion=2.0.0-alpha%PackageVersion%;Configuration=Migration;AssembliesPrefix=CSHTML5.Migration"

echo. 
echo %ESC%[95mBuilding %ESC%[0mMigration.WorkInProgress %ESC%[95mconfiguration%ESC%[0m
echo. 
msbuild ../src/CSHTML5.sln -p:Configuration=Migration.WorkInProgress -restore
echo. 
echo %ESC%[95mPacking %ESC%[0mCSHTML5.Migration.WorkInProgress %ESC%[95mNuGet package%ESC%[0m
echo. 
nuget.exe pack data\CSHTML5.nuspec -OutputDirectory "output" -Properties "PackageId=CSHTML5.Migration.WorkInProgress;PackageVersion=2.0.0-alpha%PackageVersion%;Configuration=Migration.WorkInProgress;AssembliesPrefix=CSHTML5.Migration"

echo. 
echo %ESC%[95mBuilding %ESC%[0mDebug %ESC%[95mconfiguration%ESC%[0m
echo. 
msbuild ../src/CSHTML5.sln -p:Configuration=Debug -restore
echo. 
echo %ESC%[95mPacking %ESC%[0mCSHTML5 %ESC%[95mNuGet package%ESC%[0m
echo. 
nuget.exe pack data\CSHTML5.nuspec -OutputDirectory "output" -Properties "PackageId=CSHTML5;PackageVersion=2.0.0-alpha%PackageVersion%;Configuration=Debug;AssembliesPrefix=CSHTML5"

pause