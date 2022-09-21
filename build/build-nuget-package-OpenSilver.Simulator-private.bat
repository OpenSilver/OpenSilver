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

rem Define the "%PackageVersion%" variable:
set /p PackageVersion="%ESC%[92mOpenSilver version:%ESC%[0m 1.1.0-private-"

rem Get the current date and time:
for /F "tokens=2" %%i in ('date /t') do set currentdate=%%i
set currenttime=%time%

rem Create a Version.txt file with the date:
md temp
@echo OpenSilver.Simulator 1.1.0-private-%PackageVersion% (%currentdate% %currenttime%)> temp/Version.txt

echo. 
echo %ESC%[95mRestoring NuGet packages%ESC%[0m
echo. 
nuget restore ../src/OpenSilver.sln -v quiet

echo. 
echo %ESC%[95mBuilding and packaging %ESC%[0mOpenSilver.Simulator %ESC%[0m
echo. 
msbuild slnf/OpenSilver.Simulator.slnf -p:Configuration=SL -clp:ErrorsOnly
nuget.exe pack nuspec\OpenSilver.Simulator.nuspec -OutputDirectory "output/OpenSilver" -BasePath "%batDirectory%" -Properties "PackageVersion=1.1.0-private-%PackageVersion%;RepositoryUrl=https://github.com/OpenSilver/OpenSilver"

explorer "output\OpenSilver"

pause
