@ECHO off

SETLOCAL

SET BUILD_DIR=%~dp0

rem Define the escape character for colored text
FOR /F %%a IN ('"prompt $E$S & echo on & for %%b in (1) do rem"') DO SET "ESC=%%a"

rem Define the "%PackageVersion%" variable:
IF "%~1" == "" (
	SET /p PackageVersion="%ESC%[92mOpenSilver version:%ESC%[0m "
) ELSE (
	SET PackageVersion=%1
)

rem Get the current date and time:
FOR /F "tokens=2" %%i IN ('date /t') DO SET currentdate=%%i
SET currenttime=%time%

FOR /F "delims=" %%a IN ('powershell -Command "[guid]::NewGuid().ToString('N')"') DO SET BUILD_UUID=%%a

rem Create a Version.txt file with the date:
MD %BUILD_DIR%\temp
ECHO OpenSilver %PackageVersion% (%currentdate% %currenttime%)> %BUILD_DIR%\temp\Version.txt

ECHO. 
ECHO %ESC%[95mBuilding %ESC%[0mRelease %ESC%[95mconfiguration%ESC%[0m
ECHO. 
msbuild %BUILD_DIR%\slnf\OpenSilver.slnf -p:Configuration=Release;OpenSilverBuildUUID=%BUILD_UUID% -clp:ErrorsOnly -restore

ECHO. 
ECHO %ESC%[95mPacking %ESC%[0mOpenSilver %ESC%[95mNuGet package%ESC%[0m
ECHO. 
%BUILD_DIR%\nuget.exe pack %BUILD_DIR%\nuspec\OpenSilver.nuspec -OutputDirectory "%BUILD_DIR%\output\OpenSilver" -Properties "PackageVersion=%PackageVersion%;Configuration=Release;OpenSilverBuildUUID=%BUILD_UUID%;RepositoryUrl=https://github.com/OpenSilver/OpenSilver"

ENDLOCAL