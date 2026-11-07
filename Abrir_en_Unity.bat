@echo off
title Abriendo Proyecto Unity - Alcubierre
echo ============================================================
echo   Iniciando Simulación en Unity 6 (6000.3.24f1)
echo ============================================================
echo Buscando ejecutable de Unity...

set "UNITY_EXE=C:\Program Files\Unity\Hub\Editor\6000.3.24f1\Editor\Unity.exe"

if exist "%UNITY_EXE%" (
    echo [OK] Unity Editor encontrado. Abriendo el proyecto...
    start "" "%UNITY_EXE%" -projectPath "%~dp0"
) else (
    echo [AVISO] No se encontro Unity en la ruta por defecto.
    echo Abriendo Unity Hub...
    start "" "C:\Program Files\Unity\Hub\Unity Hub.exe"
)
exit
