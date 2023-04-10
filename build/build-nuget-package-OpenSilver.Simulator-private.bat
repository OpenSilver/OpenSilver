@echo off

IF NOT EXIST "nuspec/OpenSilver.nuspec" (
echo Wrong working directory. Please navigate to the folder that contains the BAT file before executing it.
PAUSE
EXIT
)



rem Define the escape character for colored text
for /F %%a in ('"prompt $E$S & echo on & for %%b in (1) do rem"') do set "ESC=%%a"

rem Gets the directory where this bat is located, without the trailing "\"
set batDirectory=%~dp0
set batDirectory=%batDirectory:~0,-1%

rem Reading version information from version_info.txt file
for /f "delims== tokens=1,2" %%G in (version_info.txt) do set %%G=%%H

rem Define the "%PackageVersion%" variable:
set /p PackageVersion="%ESC%[92mOpenSilver version:%ESC%[0m %CURRENT_VERSION_PREFIX%"

rem Get the current date and time:
for /F "tokens=2" %%i in ('date /t') do set currentdate=%%i
set currenttime=%time%

rem Create a Version.txt file with the date:
md temp
@echo OpenSilver.Simulator %CURRENT_VERSION_PREFIX%%PackageVersion% (%currentdate% %currenttime%)> temp/Version.txt

taskkill /f /im "msbuild.exe" >NUL

echo. 
echo %ESC%[95mRestoring NuGet packages%ESC%[0m
echo. 
nuget restore ../src/OpenSilver.sln -v quiet

echo. 
echo %ESC%[95mBuilding and packaging %ESC%[0mOpenSilver.Simulator %ESC%[0m
echo. 
msbuild slnf/OpenSilver.Simulator.slnf -p:Configuration=SL -clp:ErrorsOnly
nuget.exe pack nuspec\OpenSilver.Simulator.nuspec -OutputDirectory "output/OpenSilver" -BasePath "%batDirectory%" -Properties "PackageVersion=%CURRENT_VERSION_PREFIX%%PackageVersion%;RepositoryUrl=https://github.com/OpenSilver/OpenSilver"

explorer "output\OpenSilver"

pause
