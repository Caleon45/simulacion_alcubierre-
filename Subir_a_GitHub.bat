@echo off
title Subir Proyecto a GitHub - Simulacion Alcubierre
echo ====================================================================
echo        SUBIENDO SIMULADOR DE PROPULSION ESPACIAL A GITHUB
echo ====================================================================
echo.
echo Usuario: Caleon45
echo Correo:  camleon@uniboyaca.edu.co
echo Rama:    main
echo Remoto:  https://github.com/Caleon45/simulacion_alcubierre.git
echo.
echo Asegurate de que el repositorio 'simulacion_alcubierre' ya este creado en:
echo https://github.com/new
echo.
echo Presiona cualquier tecla para iniciar 'git push -u origin main'...
pause
echo.
git push -u origin main
echo.
if %ERRORLEVEL% EQU 0 (
    echo [EXITO] Proyecto subido exitosamente a GitHub!
    echo Puedes verlo en: https://github.com/Caleon45/simulacion_alcubierre
) else (
    echo [AVISO] Si dio error 404, crea el repositorio en https://github.com/new
    echo con el nombre 'simulacion_alcubierre' (sin README ni .gitignore) y vuelve a ejecutar este archivo.
)
echo.
pause
