@echo off
setlocal
pushd "%~dp0"
dotnet publish "PawnShopManagement\PawnShopManagement.csproj" -c Release -r win-x64 --self-contained true -o "Ban-chay\Application"
if errorlevel 1 goto failed
echo Run Ban-chay\Application\PawnShopManagement.exe --desktop to test your new build.
echo Ready-to-run folder: Ban-chay. Zip its CONTENTS to distribute.
popd
pause
exit /b 0
:failed
echo Build failed. Developers need .NET SDK 10 and Internet. End users should download the published Windows ZIP from Releases.
popd
pause
exit /b 1
