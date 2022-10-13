:: Note: this batch file is here so that the right version of DISM is called regardless of whether we are running on a 32-bit or 64-bit system. For more information, please refer to: http://stackoverflow.com/questions/5936719/calling-dism-exe-from-system-diagnostics-process-fails

echo off
if exist %WINDIR%\SysNative\dism.exe (
    echo Launching WINDIR\SysNative\dism.exe %*
    %WINDIR%\SysNative\dism.exe %*
) else (
    echo Launching dism.exe %*
    dism.exe %*
)

