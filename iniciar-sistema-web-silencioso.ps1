# Inicia o backend e o frontend do Sistema de Estacionamento sem abrir nenhuma
# janela de terminal, e abre o navegador quando os dois estiverem no ar.
# Os logs ficam em .\logs, caso seja preciso investigar algum erro.

$ErrorActionPreference = 'SilentlyContinue'

$raiz = $PSScriptRoot
$logDir = Join-Path $raiz 'logs'
New-Item -ItemType Directory -Force -Path $logDir | Out-Null

$backendPort = 5080
$frontendPort = 5173

function Porta-EmUso($porta) {
    return [bool](Get-NetTCPConnection -LocalPort $porta -State Listen -ErrorAction SilentlyContinue)
}

if (-not (Porta-EmUso $backendPort)) {
    Start-Process -FilePath 'dotnet' `
        -ArgumentList "run --urls http://localhost:$backendPort" `
        -WorkingDirectory (Join-Path $raiz 'backend\EstacionamentoDIO.Api') `
        -WindowStyle Hidden `
        -RedirectStandardOutput (Join-Path $logDir 'backend.log') `
        -RedirectStandardError (Join-Path $logDir 'backend.err.log')
}

if (-not (Porta-EmUso $frontendPort)) {
    Start-Process -FilePath 'cmd.exe' `
        -ArgumentList "/c npm run dev -- --port $frontendPort --strictPort" `
        -WorkingDirectory (Join-Path $raiz 'frontend') `
        -WindowStyle Hidden `
        -RedirectStandardOutput (Join-Path $logDir 'frontend.log') `
        -RedirectStandardError (Join-Path $logDir 'frontend.err.log')
}

# Espera os dois responderem antes de abrir o navegador (até ~40s).
for ($i = 0; $i -lt 20; $i++) {
    $backendOk = Porta-EmUso $backendPort
    $frontendOk = Porta-EmUso $frontendPort
    if ($backendOk -and $frontendOk) { break }
    Start-Sleep -Seconds 2
}

Start-Process "http://localhost:$frontendPort"
