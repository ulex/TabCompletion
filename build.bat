@echo off
SET PATH=%PATH%;C:\Windows\Microsoft.NET\Framework64\v4.0.30319

powershell -ExecutionPolicy bypass .\build.ps1 || goto :error
pause
goto :EOF

:error
echo "Failded with error #%errorlevel%.
pause
exit /b %errorlevel%
pause
