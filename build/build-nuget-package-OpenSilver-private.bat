@echo off

IF NOT EXIST "nuspec/OpenSilver.nuspec" (
echo Wrong working directory. Please navigate to the folder that contains the BAT file before executing it.
PAUSE
EXIT
)

rem Define the escape character for colored text
for /F %%a in ('"prompt $E$S & echo on & for %%b in (1) do rem"') do set "ESC=%%a"

rem Reading version information from version_info.txt file
for /f "delims== tokens=1,2" %%G in (version_info.txt) do set %%G=%%H

rem Define the "%PackageVersion%" variable:
set /p PackageVersion="%ESC%[92mOpenSilver version:%ESC%[0m %CURRENT_VERSION_PREFIX%"

rem Get the current date and time:
for /F "tokens=2" %%i in ('date /t') do set currentdate=%%i
set currenttime=%time%

rem Create a Version.txt file with the date:
md temp
@echo OpenSilver %CURRENT_VERSION_PREFIX%%PackageVersion% (%currentdate% %currenttime%)> temp/Version.txt

taskkill /f /im "msbuild.exe" >NUL

echo. 
echo %ESC%[95mRestoring NuGet packages%ESC%[0m
echo. 
nuget restore ../src/OpenSilver.sln -v quiet

echo. 
echo %ESC%[95mBuilding %ESC%[0mRelease %ESC%[95mconfiguration%ESC%[0m
echo. 
msbuild slnf/OpenSilver.slnf -p:Configuration=Release -clp:ErrorsOnly -restore

echo. 
echo %ESC%[95mPacking %ESC%[0mOpenSilver %ESC%[95mNuGet package%ESC%[0m
echo. 
nuget.exe pack nuspec\OpenSilver.nuspec -OutputDirectory "output/OpenSilver" -Properties "PackageVersion=%CURRENT_VERSION_PREFIX%%PackageVersion%;Configuration=Release;RepositoryUrl=https://github.com/OpenSilver/OpenSilver"

explorer "output\OpenSilver"

pause
