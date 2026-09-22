@echo off
title Sistema de Estacionamento - Web
set RAIZ=%~dp0

echo Iniciando backend (API)...
start "Estacionamento - API" cmd /k "cd /d "%RAIZ%backend\EstacionamentoDIO.Api" && dotnet run --urls http://localhost:5080"

echo Iniciando frontend...
start "Estacionamento - Frontend" cmd /k "cd /d "%RAIZ%frontend" && npm run dev -- --port 5173"

echo Aguardando os servidores subirem...
timeout /t 8 /nobreak > nul

start http://localhost:5173

echo.
echo Sistema iniciado:
echo   Frontend: http://localhost:5173
echo   API (Swagger): http://localhost:5080/swagger
echo.
echo Feche as duas janelas de terminal abertas para encerrar o sistema.
pause
