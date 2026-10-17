@echo off
setlocal
pushd "%~dp0"
set "PAWN_PROJECT_ROOT=%CD%\PawnShopManagement"
set "PAWN_CHECK_OUTPUT=%CD%\Ket-qua-kiem-tra"
dotnet run --project "Verification\Checks.csproj"
set "PAWN_EXIT=%ERRORLEVEL%"
popd
pause
exit /b %PAWN_EXIT%
