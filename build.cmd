@echo off
setlocal
powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File "%~dp0build.ps1" %*
set "buildExitCode=%ERRORLEVEL%"
echo.
if "%buildExitCode%"=="0" (
    echo Build completed successfully.
) else (
    echo Build failed. See the error above.
)
pause
exit /b %buildExitCode%
