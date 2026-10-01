@echo off
REM Script para Teste Rápido das Imagens Docker do Prumo

echo ==========================================
echo     Teste das Imagens Prumo        
echo ==========================================
echo.

REM Verificar se as imagens existem
echo [INFO] Verificando imagens disponiveis...
docker images | findstr prumo
if errorlevel 1 (
    echo [ERROR] Nenhuma imagem Prumo encontrada!
    echo [ERROR] Execute primeiro: build-images.bat
    pause
    exit /b 1
)
echo.

REM Teste 1: Health check da API
echo [TESTE 1] Testando API standalone...
start /B docker run --rm --name prumo-api-test -p 8001:8080 prumo/api:latest

REM Aguardar API iniciar
echo [INFO] Aguardando API inicializar (30 segundos)...
timeout /t 30 /nobreak >nul

REM Testar health check
echo [INFO] Testando health check...
curl -s http://localhost:8001/health
if errorlevel 1 (
    echo [WARN] Health check falhou - API pode estar ainda inicializando
) else (
    echo [OK] API respondendo corretamente!
)
echo.

REM Parar API test
docker stop prumo-api-test >nul 2>&1

REM Teste 2: Stack completo com Docker Compose
echo [TESTE 2] Testando stack completo...
echo [INFO] Iniciando PostgreSQL + API + Web...

docker-compose -f docker-compose-test.yml up -d

echo [INFO] Aguardando servicos inicializarem (45 segundos)...
timeout /t 45 /nobreak >nul

echo [INFO] Testando endpoints...

REM Testar API
echo [INFO] Testando API (http://localhost:8001/health)...
curl -s http://localhost:8001/health
echo.

REM Testar Web
echo [INFO] Testando Web (http://localhost:8002)...
curl -s -I http://localhost:8002 | findstr "200 OK"
if errorlevel 1 (
    echo [WARN] Web pode estar ainda carregando
) else (
    echo [OK] Web respondendo corretamente!
)
echo.

REM Mostrar status dos containers
echo [INFO] Status dos containers:
docker-compose -f docker-compose-test.yml ps

echo.
echo ==========================================
echo            Teste Concluido              
echo ==========================================
echo.
echo URLs de Teste:
echo - API Health: http://localhost:8001/health
echo - API Swagger: http://localhost:8001/swagger
echo - Frontend: http://localhost:8002
echo - PostgreSQL: localhost:5433 (user: prumo, pass: teste123)
echo.
echo Para parar os testes:
echo   docker-compose -f docker-compose-test.yml down -v
echo.
echo Abrir URLs no navegador? (s/N)
set /p choice=
if /i "%choice%"=="s" (
    start http://localhost:8001/health
    start http://localhost:8001/swagger
    start http://localhost:8002
)

pause