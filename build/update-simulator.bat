@ECHO off

SETLOCAL

REM Define the escape character for colored text
FOR /F %%a IN ('"prompt $E$S & echo on & for %%b in (1) do rem"') DO SET "ESC=%%a"

SET CFG=Release
SET BUILD_DIR=%~dp0
SET SRC_DIR=%~dp0..\src

REM Reading version information from version_info.txt file
FOR /F "delims== tokens=1,2" %%G IN (%BUILD_DIR%\version_info.txt) DO SET %%G=%%H

SET SIMULATOR_DIR=%SRC_DIR%\packages\OpenSilver.Simulator.%STABLE_VERSION%

FOR /F "delims=" %%a IN ('powershell -Command "[guid]::NewGuid().ToString('N')"') DO SET BUILD_UUID=%%a

taskkill /f /im "msbuild.exe" 1>NUL 2>NUL

ECHO. 
ECHO %ESC%[95mDeleting files.%ESC%[0m
ECHO.

DEL /Q %SIMULATOR_DIR%\build\*
DEL /Q %SIMULATOR_DIR%\tools\*
DEL /Q %SIMULATOR_DIR%\contentFiles\any\any\js_css\*
DEL /Q %SIMULATOR_DIR%\contentFiles\any\any\*
DEL /Q %SIMULATOR_DIR%\lib\net8.0-windows7.0\*
DEL /Q %SIMULATOR_DIR%\lib\net9.0-windows7.0\*
DEL /Q %SIMULATOR_DIR%\lib\net10.0-windows7.0\*

ECHO. 
ECHO %ESC%[95mBuilding %ESC%[0mOpenSilver.Simulator %ESC%[95min %ESC%[0m%CFG% %ESC%[95mconfiguration%ESC%[0m
ECHO.
msbuild %BUILD_DIR%\slnf\OpenSilver.Simulator.slnf -p:Configuration=%CFG%;OpenSilverBuildUUID=%BUILD_UUID% -clp:ErrorsOnly -restore

ECHO. 
ECHO %ESC%[95mCopying targets.%ESC%[0m
ECHO.

COPY "%SRC_DIR%\Targets\OpenSilver.Simulator.targets" "%SIMULATOR_DIR%\build"
COPY "%SRC_DIR%\Compiler\Compiler.SharedBuildTasks\bin\%CFG%\netstandard2.0\OpenSilver.SharedBuildTasks.Config.targets" "%SIMULATOR_DIR%\build"

ECHO. 
ECHO %ESC%[95mCopying Compiler DLLs.%ESC%[0m
ECHO.

COPY "%SRC_DIR%\Compiler\Compiler.SharedBuildTasks\bin\%CFG%\netstandard2.0\OpenSilver.Compiler.SharedBuildTasks.%BUILD_UUID%.dll" "%SIMULATOR_DIR%\tools"
COPY "%SRC_DIR%\Compiler\Compiler.SharedBuildTasks\bin\%CFG%\netstandard2.0\Mono.Cecil.dll" "%SIMULATOR_DIR%\tools"

ECHO. 
ECHO %ESC%[95mCopying Content files.%ESC%[0m
ECHO.

COPY "%SRC_DIR%\Runtime\Scripts\cshtml5.css" "%SIMULATOR_DIR%\contentFiles\any\any\js_css"
COPY "%SRC_DIR%\Runtime\Scripts\cshtml5.js" "%SIMULATOR_DIR%\contentFiles\any\any\js_css"
COPY "%SRC_DIR%\Runtime\Scripts\filesaver.min.js" "%SIMULATOR_DIR%\contentFiles\any\any\js_css"
COPY "%SRC_DIR%\Runtime\Scripts\opensilver.js" "%SIMULATOR_DIR%\contentFiles\any\any\js_css"
COPY "%SRC_DIR%\Runtime\Scripts\quill.core.css" "%SIMULATOR_DIR%\contentFiles\any\any\js_css"
COPY "%SRC_DIR%\Runtime\Scripts\quill.min.js" "%SIMULATOR_DIR%\contentFiles\any\any\js_css"
COPY "%SRC_DIR%\Simulator\Simulator\simulator_root_opensilver.html" "%SIMULATOR_DIR%\contentFiles\any\any\simulator_root.html"
COPY "%SRC_DIR%\Simulator\Simulator\interop_debug_root_opensilver.html" "%SIMULATOR_DIR%\contentFiles\any\any\interop_debug_root.html"

ECHO. 
ECHO %ESC%[95mCopying Simulator DLLs.%ESC%[0m
ECHO.

COPY "%SRC_DIR%\Simulator\Simulator\bin\%CFG%\net8.0-windows\OpenSilver.Simulator.dll" "%SIMULATOR_DIR%\lib\net8.0-windows7.0"
COPY "%SRC_DIR%\Simulator\Simulator\bin\%CFG%\net9.0-windows\OpenSilver.Simulator.dll" "%SIMULATOR_DIR%\lib\net9.0-windows7.0"
COPY "%SRC_DIR%\Simulator\Simulator\bin\%CFG%\net10.0-windows\OpenSilver.Simulator.dll" "%SIMULATOR_DIR%\lib\net10.0-windows7.0"

ENDLOCAL