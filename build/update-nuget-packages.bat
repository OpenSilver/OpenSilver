@ECHO off

CALL %~dp0update-compiler.bat
CALL %~dp0update-wasm.bat
CALL %~dp0update-simulator.bat