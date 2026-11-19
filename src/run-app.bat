@echo off
REM Run TopNotch app - bypass Smart App Control by using dotnet to run the DLL
setlocal

echo Starting TopNotch...
echo.

cd /d "%~dp0"

REM Try running the DLL directly via dotnet
"C:\Program Files\dotnet\dotnet.exe" "bin\Debug\net8.0\TopNotch.dll"
if %ERRORLEVEL% EQU 0 goto :eof

REM If that fails, show error
echo.
echo Error: Could not start app. Smart App Control may be blocking it.
echo.
echo Solutions:
echo 1. Add exclusion: Windows Security → Virus & threat protection → Exclusions
echo 2. Contact IT admin if on managed device
echo 3. Disable Smart App Control temporarily
echo.
pause