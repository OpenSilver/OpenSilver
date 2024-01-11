@ECHO off

REM Define the escape character for colored text
FOR /F %%a IN ('"prompt $E$S & echo on & for %%b in (1) do rem"') DO SET "ESC=%%a"

SET CFG=Release
SET BUILD_DIR=%~dp0
SET SRC_DIR=%~dp0..\src

REM Reading version information from version_info.txt file
FOR /F "delims== tokens=1,2" %%G IN (%BUILD_DIR%\version_info.txt) DO SET %%G=%%H

SET COMPILER_DIR=%SRC_DIR%\packages\OpenSilver.%STABLE_VERSION%

ECHO. 
ECHO %ESC%[95mCopying targets.%ESC%[0m
ECHO.

COPY "%SRC_DIR%\Targets\OpenSilver.targets" "%COMPILER_DIR%\build"
COPY "%SRC_DIR%\Targets\OpenSilver.FSharpSupport.targets" "%COMPILER_DIR%\build"
COPY "%SRC_DIR%\Targets\OpenSilver.Common.targets" "%COMPILER_DIR%\build"
COPY "%SRC_DIR%\Targets\OpenSilver.GenerateAssemblyInfo.targets" "%COMPILER_DIR%\build"

taskkill /f /im "msbuild.exe" 1>NUL 2>NUL

ECHO. 
ECHO %ESC%[95mRestoring NuGet packages%ESC%[0m
%BUILD_DIR%\nuget restore %SRC_DIR%\OpenSilver.sln -v quiet

ECHO. 
ECHO %ESC%[95mBuilding %ESC%[0m%CFG% %ESC%[95mconfiguration%ESC%[0m
ECHO.
msbuild %BUILD_DIR%\slnf\Compiler.slnf -p:Configuration=%CFG% -clp:ErrorsOnly -restore

taskkill /f /im "msbuild.exe" 1>NUL 2>NUL

ECHO. 
ECHO %ESC%[95mCopying Compiler DLLs.%ESC%[0m
ECHO.

CALL :copyDll Compiler OpenSilver.Compiler
CALL :copyDll Compiler.Common OpenSilver.Compiler.Common
CALL :copyDll Compiler.ProgressDialog OpenSilver.Compiler.ProgressDialog
CALL :copyDll Compiler.TypeScriptDefToCSharp OpenSilver.Compiler.TypeScriptDefToCSharp
CALL :copyDll Compiler.ResourcesExtractor OpenSilver.Compiler.Resources
CALL :copyDll Compiler Mono.Cecil
CALL :copyDll Compiler Mono.Cecil.Mdb
CALL :copyDll Compiler Mono.Cecil.Pdb
CALL :copyDll Compiler Mono.Cecil.Rocks

EXIT /B 0
:copyDll
DEL "%COMPILER_DIR%\tools\%~2.dll"
COPY "%SRC_DIR%\Compiler\%~1\bin\%CFG%\net461\%~2.dll" "%COMPILER_DIR%\tools"
EXIT /B 0