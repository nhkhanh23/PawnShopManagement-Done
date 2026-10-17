@echo off
setlocal
if not exist "%~dp0Khoi-dong.ps1" (
    echo Please extract the FULL GitHub ZIP before running.
    pause
    exit /b 2
)
"%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe" -NoProfile -ExecutionPolicy Bypass -File "%~dp0Khoi-dong.ps1" %*
set "PAWN_EXIT=%ERRORLEVEL%"
if not "%PAWN_EXIT%"=="0" pause
exit /b %PAWN_EXIT%
