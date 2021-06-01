@echo off

IF NOT EXIST "nuspec/OpenSilver.nuspec" (
echo Wrong working directory. Please navigate to the folder that contains the BAT file before executing it.
PAUSE
EXIT
)



rem Define the escape character for colored text
for /F %%a in ('"prompt $E$S & echo on & for %%b in (1) do rem"') do set "ESC=%%a"

rem Define the "%PackageVersion%" variable:
set /p PackageVersion="%ESC%[92mOpenSilver version:%ESC%[0m 1.0.0-private-"

rem Get the current date and time:
for /F "tokens=2" %%i in ('date /t') do set currentdate=%%i
set currenttime=%time%

rem Create a Version.txt file with the date:
md temp
@echo OpenSilver.Simulator 1.0.0-private-%PackageVersion% (%currentdate% %currenttime%)> temp/Version.txt

echo. 
echo %ESC%[95mRestoring NuGet packages%ESC%[0m
echo. 
nuget restore ../src/OpenSilver.sln

echo. 
echo %ESC%[95mBuilding and packaging %ESC%[0mOpenSilver.Simulator %ESC%[0m
echo. 
msbuild -t:pack slnf/OpenSilver.Simulator.slnf -p:Configuration=SL -p:PackageOutputPath=%~dp0output/OpenSilver -p:NuspecFile=%~dp0nuspec/OpenSilver.Simulator.nuspec -p:NuspecBasePath=%~dp0 -p:NuspecProperties=PackageVersion=1.0.0-private-%PackageVersion%

explorer "output\OpenSilver"

pause
