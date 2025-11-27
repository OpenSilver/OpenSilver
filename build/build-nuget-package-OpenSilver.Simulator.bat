@ECHO off

SETLOCAL

SET BUILD_DIR=%~dp0

rem Define the escape character for colored text
FOR /F %%a IN ('"prompt $E$S & echo on & for %%b in (1) do rem"') DO SET "ESC=%%a"

rem Define the "%PackageVersion%" variable:
IF "%~1" == "" (
	SET /P PackageVersion="%ESC%[92mOpenSilver version:%ESC%[0m "
) ELSE (
	SET PackageVersion=%1
)

IF "%~2" == "" (
	SET CFG=Release
) ELSE (
	SET CFG=%2
)

FOR /F "delims=" %%a IN ('powershell -Command "[guid]::NewGuid().ToString('N')"') DO SET BUILD_UUID=%%a

ECHO. 
ECHO %ESC%[95mBuilding %ESC%[0mOpenSilver.Simulator %ESC%[0m
ECHO. 
msbuild %BUILD_DIR%\slnf\OpenSilver.Simulator.slnf -p:Configuration=%CFG%;OpenSilverBuildUUID=%BUILD_UUID% -clp:ErrorsOnly -restore

ECHO. 
ECHO %ESC%[95mPacking %ESC%[0mOpenSilver.Simulator %ESC%[95mNuGet package%ESC%[0m
ECHO. 
%BUILD_DIR%\nuget.exe pack %BUILD_DIR%\nuspec\OpenSilver.Simulator.nuspec -OutputDirectory "%BUILD_DIR%\output\OpenSilver" -Properties "PackageVersion=%PackageVersion%;Configuration=%CFG%;OpenSilverBuildUUID=%BUILD_UUID%;RepositoryUrl=https://github.com/OpenSilver/OpenSilver"

ENDLOCAL