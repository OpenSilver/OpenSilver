@echo off

IF NOT EXIST "slnf/Compiler.slnf" (
echo Wrong working directory. Please navigate to the folder that contains the BAT file before executing it.
PAUSE
EXIT
)

rem Define the escape character for colored text
for /F %%a in ('"prompt $E$S & echo on & for %%b in (1) do rem"') do set "ESC=%%a"

rem Reading version information from version_info.txt file
for /f "delims== tokens=1,2" %%G in (version_info.txt) do set %%G=%%H

set config=Release
set repoPath=%~dp0..
set sourceBase=%repoPath%\src\Compiler
set destDir=%repoPath%\src\packages\OpenSilver.%STABLE_VERSION%\tools

taskkill /f /im "msbuild.exe" >NUL

echo. 
echo %ESC%[95mRestoring NuGet packages%ESC%[0m
nuget restore ../src/OpenSilver.sln -v quiet
echo. 
echo %ESC%[95mBuilding %ESC%[0mSL %ESC%[95mconfiguration%ESC%[0m
echo.
msbuild slnf/Compiler.slnf -p:Configuration=SL -clp:ErrorsOnly -restore

taskkill /f /im "msbuild.exe" >NUL

echo. 
echo %ESC%[95mCopying Compiler DLLs and targets.%ESC%[0m
echo.

CALL :copyDll Compiler OpenSilver.Compiler
CALL :copyDll Compiler.Common OpenSilver.Compiler.Common
CALL :copyDll Compiler.ProgressDialog OpenSilver.Compiler.ProgressDialog
CALL :copyDll Compiler.TypeScriptDefToCSharp OpenSilver.Compiler.TypeScriptDefToCSharp
CALL :copyDll Compiler.ResourcesExtractor OpenSilver.Compiler.Resources
copy %repoPath%\src\Targets\OpenSilver.targets %repoPath%\src\packages\OpenSilver.%STABLE_VERSION%\build >NUL
copy %repoPath%\src\Targets\OpenSilver.Common.targets %repoPath%\src\packages\OpenSilver.%STABLE_VERSION%\build >NUL

EXIT /B 0
:copyDll
del %destDir%\%~2.dll >NUL
copy %sourceBase%\%~1\bin\OpenSilver\%config%\net461\%~2.dll %destDir% >NUL
EXIT /B 0