@echo off

IF NOT EXIST "nuspec/OpenSilver.nuspec" (
echo Wrong working directory. Please navigate to the folder that contains the BAT file before executing it.
PAUSE
EXIT
)



rem Define the escape character for colored text
for /F %%a in ('"prompt $E$S & echo on & for %%b in (1) do rem"') do set "ESC=%%a"

rem Define the "%PackageVersion%" variable:
set /p PackageVersion="%ESC%[92mOpenSilver version:%ESC%[0m 1.0.0-alpha-"

rem Get the current date and time:
for /F "tokens=2" %%i in ('date /t') do set currentdate=%%i
set currenttime=%time%

rem Create a Version.txt file with the date:
md temp
@echo OpenSilver 1.0.0-alpha-%PackageVersion% (%currentdate% %currenttime%)> temp/Version.txt

echo. 
echo %ESC%[95mRestoring NuGet packages%ESC%[0m
echo. 
nuget restore ../src/OpenSilver.sln -v quiet

echo. 
echo %ESC%[95mBuilding %ESC%[0mSL.WorkInProgress %ESC%[95mconfiguration%ESC%[0m
echo. 
msbuild slnf/OpenSilver.slnf -p:Configuration=SL.WorkInProgress -clp:ErrorsOnly -restore
echo. 
echo %ESC%[95mPacking %ESC%[0mOpenSilver.WorkInProgress %ESC%[95mNuGet package%ESC%[0m
echo. 
nuget.exe pack nuspec\OpenSilver.nuspec -OutputDirectory "output/OpenSilver" -Properties "PackageId=OpenSilver.WorkInProgress;PackageVersion=1.0.0-alpha-%PackageVersion%;Configuration=SL.WorkInProgress;Target=OpenSilver"

explorer "output\OpenSilver"

pause
