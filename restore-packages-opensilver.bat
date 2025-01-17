%~dp0\build\nuget.exe install OpenSilver -Version 3.1.0 -OutputDirectory %~dp0\src\packages -Source https://api.nuget.org/v3/index.json
dotnet workload restore %~dp0\src\OpenSilver.sln