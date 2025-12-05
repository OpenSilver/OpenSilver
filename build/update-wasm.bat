@ECHO off

SETLOCAL

REM Define the escape character for colored text
FOR /F %%a IN ('"prompt $E$S & echo on & for %%b in (1) do rem"') DO SET "ESC=%%a"

SET CFG=Release
SET BUILD_DIR=%~dp0
SET SRC_DIR=%~dp0..\src

REM Reading version information from version_info.txt file
FOR /F "delims== tokens=1,2" %%G IN (%BUILD_DIR%\version_info.txt) DO SET %%G=%%H

SET WASM_DIR=%SRC_DIR%\packages\OpenSilver.WebAssembly.%STABLE_VERSION%

FOR /F "delims=" %%a IN ('powershell -Command "[guid]::NewGuid().ToString('N')"') DO SET BUILD_UUID=%%a

taskkill /f /im "msbuild.exe" 1>NUL 2>NUL

ECHO. 
ECHO %ESC%[95mDeleting files.%ESC%[0m
ECHO.

DEL /Q %WASM_DIR%\build\*
DEL /Q %WASM_DIR%\tools\*
DEL /Q %WASM_DIR%\content\js\*
DEL /Q %WASM_DIR%\content\css\*
DEL /Q %WASM_DIR%\lib\net8.0\*
DEL /Q %WASM_DIR%\lib\net9.0\*
DEL /Q %WASM_DIR%\lib\net10.0\*

ECHO. 
ECHO %ESC%[95mBuilding %ESC%[0mOpenSilver.WebAssembly %ESC%[95min %ESC%[0m%CFG% %ESC%[95mconfiguration%ESC%[0m
ECHO.
msbuild %BUILD_DIR%\slnf\OpenSilver.WebAssembly.slnf -p:Configuration=%CFG%;OpenSilverBuildUUID=%BUILD_UUID% -clp:ErrorsOnly -restore

ECHO. 
ECHO %ESC%[95mCopying targets.%ESC%[0m
ECHO.

COPY "%SRC_DIR%\Targets\OpenSilver.WebAssembly.targets" "%WASM_DIR%\build"
COPY "%SRC_DIR%\Compiler\Compiler.SharedBuildTasks\bin\%CFG%\netstandard2.0\OpenSilver.SharedBuildTasks.Config.targets" "%WASM_DIR%\build"

ECHO. 
ECHO %ESC%[95mCopying Compiler DLLs.%ESC%[0m
ECHO.

COPY "%SRC_DIR%\Compiler\Compiler.SharedBuildTasks\bin\%CFG%\netstandard2.0\OpenSilver.Compiler.SharedBuildTasks.%BUILD_UUID%.dll" "%WASM_DIR%\tools"
COPY "%SRC_DIR%\Compiler\Compiler.SharedBuildTasks\bin\%CFG%\netstandard2.0\Mono.Cecil.dll" "%WASM_DIR%\tools"
COPY "%SRC_DIR%\Compiler\Compiler.SharedBuildTasks\bin\%CFG%\netstandard2.0\Mono.Cecil.Rocks.dll" "%WASM_DIR%\tools"

ECHO. 
ECHO %ESC%[95mCopying JS and CSS files.%ESC%[0m
ECHO.

COPY "%SRC_DIR%\Runtime\Scripts\cshtml5.js" "%WASM_DIR%\content\js"
COPY "%SRC_DIR%\Runtime\Scripts\FileSaver.min.js" "%WASM_DIR%\content\js"
COPY "%SRC_DIR%\Runtime\Scripts\htmlToImage.js" "%WASM_DIR%\content\js"
COPY "%SRC_DIR%\Runtime\Scripts\OpenSilver.js" "%WASM_DIR%\content\js"
COPY "%SRC_DIR%\Runtime\Scripts\quill.min.js" "%WASM_DIR%\content\js"
COPY "%SRC_DIR%\Runtime\Scripts\cshtml5.css" "%WASM_DIR%\content\css"
COPY "%SRC_DIR%\Runtime\Scripts\quill.core.css" "%WASM_DIR%\content\css"

ECHO. 
ECHO %ESC%[95mCopying Wasm DLLs.%ESC%[0m
ECHO.

COPY "%SRC_DIR%\WebAssembly\OpenSilver.WebAssembly\bin\%CFG%\net8.0\OpenSilver.WebAssembly.dll" "%WASM_DIR%\lib\net8.0"
COPY "%SRC_DIR%\WebAssembly\OpenSilver.WebAssembly\bin\%CFG%\net9.0\OpenSilver.WebAssembly.dll" "%WASM_DIR%\lib\net9.0"
COPY "%SRC_DIR%\WebAssembly\OpenSilver.WebAssembly\bin\%CFG%\net10.0\OpenSilver.WebAssembly.dll" "%WASM_DIR%\lib\net10.0"

ENDLOCAL