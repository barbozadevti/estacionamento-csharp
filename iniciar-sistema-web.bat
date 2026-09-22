@echo off
title Sistema de Estacionamento - Web
set RAIZ=%~dp0

echo Verificando se o backend ja esta rodando...
netstat -ano | findstr ":5080" | findstr "LISTENING" >nul
if not errorlevel 1 goto backend_ok
echo Iniciando backend (API)...
start "Estacionamento - API" cmd /k "cd /d "%RAIZ%backend\EstacionamentoDIO.Api" && dotnet run --urls http://localhost:5080"
:backend_ok

echo Verificando se o frontend ja esta rodando...
netstat -ano | findstr ":5173" | findstr "LISTENING" >nul
if not errorlevel 1 goto frontend_ok
echo Iniciando frontend...
start "Estacionamento - Frontend" cmd /k "cd /d "%RAIZ%frontend" && npm run dev -- --port 5173 --strictPort"
:frontend_ok

echo Aguardando os servidores subirem...
timeout /t 8 /nobreak > nul

start http://localhost:5173

echo.
echo Sistema iniciado:
echo   Frontend: http://localhost:5173
echo   API (Swagger): http://localhost:5080/swagger
echo.
echo Para encerrar o sistema, feche as janelas de terminal "Estacionamento - API"
echo e "Estacionamento - Frontend".
pause
