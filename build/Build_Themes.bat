@ECHO off

SETLOCAL

SET BUILD_DIR=%~dp0
SET SRC_DIR=%~dp0..\src

REM Define the escape character for colored text
FOR /F %%a IN ('"prompt $E$S & echo on & for %%b in (1) do rem"') DO SET "ESC=%%a"

IF "%~1" == "" (
	SET /P OPENSILVER_PKG_VER="%ESC%[92mOpensilver version:%ESC%[0m "
	SET /P PKG_VER="%ESC%[92mPackage version:%ESC%[0m "
) ELSE (
	SET OPENSILVER_PKG_VER=%1
	SET PKG_VER=%2
)

IF "%PKG_VER%" == "" (
	SET PKG_VER=%OPENSILVER_PKG_VER%
)

SET PROPERTIES="PackageVersion=%PKG_VER%;OpenSilverPackageVersion=%OPENSILVER_PKG_VER%;Configuration=Release;RepositoryUrl=https://github.com/OpenSilver/OpenSilver"
SET OUTPUT_DIR="%BUILD_DIR%\output\OpenSilver"

echo. 
echo %ESC%[95mBuilding %ESC%[0mRelease %ESC%[95mconfiguration%ESC%[0m
echo. 
msbuild %BUILD_DIR%\slnf\OpenSilver.Themes.slnf -p:Configuration=Release -clp:ErrorsOnly -restore

echo. 
echo %ESC%[95mPacking %ESC%[0mOpenSilver.Theme.BubbleCreme %ESC%[95mNuGet package%ESC%[0m
echo. 
%BUILD_DIR%\nuget pack %BUILD_DIR%\nuspec\OpenSilver.Theme.BubbleCreme.nuspec -OutputDirectory %OUTPUT_DIR% -Properties %PROPERTIES%

echo. 
echo %ESC%[95mPacking %ESC%[0mOpenSilver.Theme.BureauBlack %ESC%[95mNuGet package%ESC%[0m
echo. 
%BUILD_DIR%\nuget pack %BUILD_DIR%\nuspec\OpenSilver.Theme.BureauBlack.nuspec -OutputDirectory %OUTPUT_DIR% -Properties %PROPERTIES%

echo. 
echo %ESC%[95mPacking %ESC%[0mOpenSilver.Theme.BureauBlue %ESC%[95mNuGet package%ESC%[0m
echo. 
%BUILD_DIR%\nuget pack %BUILD_DIR%\nuspec\OpenSilver.Theme.BureauBlue.nuspec -OutputDirectory %OUTPUT_DIR% -Properties %PROPERTIES%

echo. 
echo %ESC%[95mPacking %ESC%[0mOpenSilver.Theme.ExpressionDark %ESC%[95mNuGet package%ESC%[0m
echo. 
%BUILD_DIR%\nuget pack %BUILD_DIR%\nuspec\OpenSilver.Theme.ExpressionDark.nuspec -OutputDirectory %OUTPUT_DIR% -Properties %PROPERTIES%

echo. 
echo %ESC%[95mPacking %ESC%[0mOpenSilver.Theme.ExpressionLight %ESC%[95mNuGet package%ESC%[0m
echo. 
%BUILD_DIR%\nuget pack %BUILD_DIR%\nuspec\OpenSilver.Theme.ExpressionLight.nuspec -OutputDirectory %OUTPUT_DIR% -Properties %PROPERTIES%

echo. 
echo %ESC%[95mPacking %ESC%[0mOpenSilver.Theme.RainierOrange %ESC%[95mNuGet package%ESC%[0m
echo. 
%BUILD_DIR%\nuget pack %BUILD_DIR%\nuspec\OpenSilver.Theme.RainierOrange.nuspec -OutputDirectory %OUTPUT_DIR% -Properties %PROPERTIES%

echo. 
echo %ESC%[95mPacking %ESC%[0mOpenSilver.Theme.RainierPurple %ESC%[95mNuGet package%ESC%[0m
echo. 
%BUILD_DIR%\nuget pack %BUILD_DIR%\nuspec\OpenSilver.Theme.RainierPurple.nuspec -OutputDirectory %OUTPUT_DIR% -Properties %PROPERTIES%

echo. 
echo %ESC%[95mPacking %ESC%[0mOpenSilver.Theme.ShinyBlue %ESC%[95mNuGet package%ESC%[0m
echo. 
%BUILD_DIR%\nuget pack %BUILD_DIR%\nuspec\OpenSilver.Theme.ShinyBlue.nuspec -OutputDirectory %OUTPUT_DIR% -Properties %PROPERTIES%

echo. 
echo %ESC%[95mPacking %ESC%[0mOpenSilver.Theme.ShinyRed %ESC%[95mNuGet package%ESC%[0m
echo. 
%BUILD_DIR%\nuget pack %BUILD_DIR%\nuspec\OpenSilver.Theme.ShinyRed.nuspec -OutputDirectory %OUTPUT_DIR% -Properties %PROPERTIES%

echo. 
echo %ESC%[95mPacking %ESC%[0mOpenSilver.Theme.SystemColors %ESC%[95mNuGet package%ESC%[0m
echo. 
%BUILD_DIR%\nuget pack %BUILD_DIR%\nuspec\OpenSilver.Theme.SystemColors.nuspec -OutputDirectory %OUTPUT_DIR% -Properties %PROPERTIES%

echo. 
echo %ESC%[95mPacking %ESC%[0mOpenSilver.Theme.TwilightBlue %ESC%[95mNuGet package%ESC%[0m
echo. 
%BUILD_DIR%\nuget pack %BUILD_DIR%\nuspec\OpenSilver.Theme.TwilightBlue.nuspec -OutputDirectory %OUTPUT_DIR% -Properties %PROPERTIES%

echo. 
echo %ESC%[95mPacking %ESC%[0mOpenSilver.Theme.WhistlerBlue %ESC%[95mNuGet package%ESC%[0m
echo. 
%BUILD_DIR%\nuget pack %BUILD_DIR%\nuspec\OpenSilver.Theme.WhistlerBlue.nuspec -OutputDirectory %OUTPUT_DIR% -Properties %PROPERTIES%

ENDLOCAL