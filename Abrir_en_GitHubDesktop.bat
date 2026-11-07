@echo off
title Abrir en GitHub Desktop - Simulacion Alcubierre
cd /d "%~dp0"
echo Abriendo simulador en GitHub Desktop...
github open "%~dp0"
if %ERRORLEVEL% NEQ 0 (
    start "" "%LOCALAPPDATA%\GitHubDesktop\GitHubDesktop.exe" "%~dp0"
)
exit

