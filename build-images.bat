@echo off
REM Script Simples para Build das Imagens Docker do PesquisaX
REM Execute este script da raiz do projeto

setlocal enabledelayedexpansion

echo ==========================================
echo     PesquisaX Docker Build               
echo ==========================================
echo.

REM Configurações
set "BUILD_NUMBER=%date:~-4,4%%date:~-10,2%%date:~-7,2%%time:~0,2%%time:~3,2%%time:~6,2%"
set "BUILD_NUMBER=%BUILD_NUMBER: =0%"

REM Obter hash do git (se disponível)
for /f "tokens=*" %%i in ('git rev-parse HEAD 2^>nul') do set "GIT_COMMIT_HASH=%%i"
if "!GIT_COMMIT_HASH!"=="" set "GIT_COMMIT_HASH=local-build"

echo [INFO] Build Number: !BUILD_NUMBER!
echo [INFO] Git Commit: !GIT_COMMIT_HASH:~0,8!
echo [INFO] Diretorio: %cd%
echo.

REM Verificar se Docker está funcionando
echo [STEP 1/5] Verificando Docker...
docker --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Docker nao esta instalado ou nao esta funcionando!
    pause
    exit /b 1
)
echo [OK] Docker encontrado
echo.

REM Build da API
echo [STEP 2/5] Building PesquisaX API
echo [INFO] Construindo imagem da API...

docker build -f Api\Dockerfile ^
    --build-arg BUILD_CONFIGURATION=Release ^
    --build-arg BUILD_NUMBER=!BUILD_NUMBER! ^
    --build-arg GIT_COMMIT_HASH=!GIT_COMMIT_HASH! ^
    -t pesquisax/api:latest ^
    -t pesquisax/api:!BUILD_NUMBER! ^
    -t pesquisax/api:production ^
    .

if errorlevel 1 (
    echo [ERROR] Falha no build da API!
    echo [ERROR] Verifique se esta executando da raiz do projeto
    pause
    exit /b 1
)

echo [OK] Imagem da API construida com sucesso!
echo.

REM Build do Web
echo [STEP 3/5] Building PesquisaX Web
echo [INFO] Construindo imagem do Web...

docker build -f Web\Dockerfile ^
    --build-arg BUILD_CONFIGURATION=Release ^
    --build-arg BUILD_NUMBER=!BUILD_NUMBER! ^
    --build-arg GIT_COMMIT_HASH=!GIT_COMMIT_HASH! ^
    -t pesquisax/web:latest ^
    -t pesquisax/web:!BUILD_NUMBER! ^
    -t pesquisax/web:production ^
    .

if errorlevel 1 (
    echo [ERROR] Falha no build do Web!
    echo [ERROR] Verifique se esta executando da raiz do projeto
    pause
    exit /b 1
)

echo [OK] Imagem do Web construida com sucesso!
echo.

REM Verificar imagens criadas
echo [STEP 4/5] Verificando imagens criadas
docker images | findstr pesquisax
echo.

REM Criar diretório de export e exportar
echo [STEP 5/5] Exportando imagens
set "EXPORT_DIR=Scripts\docker-images"
if not exist "!EXPORT_DIR!" mkdir "!EXPORT_DIR!"

echo [INFO] Exportando pesquisax/api:latest...
docker save pesquisax/api:latest > "!EXPORT_DIR!\pesquisax-api-!BUILD_NUMBER!.tar"

echo [INFO] Exportando pesquisax/web:latest...
docker save pesquisax/web:latest > "!EXPORT_DIR!\pesquisax-web-!BUILD_NUMBER!.tar"

REM Criar script de import para o servidor
set "IMPORT_SCRIPT=!EXPORT_DIR!\import-images.sh"
(
echo #!/bin/bash
echo echo "=== Importando Imagens Docker do PesquisaX ==="
echo echo "Build Number: !BUILD_NUMBER!"
echo echo "Git Commit: !GIT_COMMIT_HASH:~0,8!"
echo echo ""
echo if [ -f "pesquisax-api-!BUILD_NUMBER!.tar" ]; then
echo     echo "[INFO] Importando pesquisax/api..."
echo     docker load ^< pesquisax-api-!BUILD_NUMBER!.tar
echo     echo "[OK] API importada!"
echo else
echo     echo "[ERROR] Arquivo pesquisax-api-!BUILD_NUMBER!.tar nao encontrado!"
echo     exit 1
echo fi
echo echo ""
echo if [ -f "pesquisax-web-!BUILD_NUMBER!.tar" ]; then
echo     echo "[INFO] Importando pesquisax/web..."
echo     docker load ^< pesquisax-web-!BUILD_NUMBER!.tar
echo     echo "[OK] Web importado!"
echo else
echo     echo "[ERROR] Arquivo pesquisax-web-!BUILD_NUMBER!.tar nao encontrado!"
echo     exit 1
echo fi
echo echo ""
echo echo "=== Importacao Concluida ==="
echo echo "Imagens disponiveis:"
echo docker images ^| grep pesquisax
echo echo ""
echo echo "Proximos passos:"
echo echo "1. Configure Docker Secrets:"
echo echo "   ./setup-secrets.sh"
echo echo "2. Faca o deploy:"
echo echo "   docker stack deploy -c docker-compose.yml pesquisax"
) > "!IMPORT_SCRIPT!"

REM Criar arquivo de informações
set "INFO_FILE=!EXPORT_DIR!\build-info.txt"
(
echo PesquisaX Docker Images - Build !BUILD_NUMBER!
echo =============================================
echo Build Date: %date% %time%
echo Git Commit: !GIT_COMMIT_HASH!
echo Environment: Production
echo.
echo Arquivos:
echo - pesquisax-api-!BUILD_NUMBER!.tar
echo - pesquisax-web-!BUILD_NUMBER!.tar  
echo - import-images.sh
echo - build-info.txt
echo.
echo Para usar no servidor:
echo 1. Copie todos os arquivos para o servidor
echo 2. chmod +x import-images.sh
echo 3. ./import-images.sh
echo.
echo URLs apos deploy:
echo - Frontend: https://pesquisax.thaishiroki.com.br
echo - API: https://api-pesquisax.thaishiroki.com.br
echo - Health: https://api-pesquisax.thaishiroki.com.br/health
echo - DB Admin: https://db-pesquisax.thaishiroki.com.br
) > "!INFO_FILE!"

echo [OK] Imagens exportadas para: !EXPORT_DIR!
echo.

REM Resumo final
echo ==========================================
echo           Build Concluido!              
echo ==========================================
echo.
echo Arquivos criados em !EXPORT_DIR!:
dir "!EXPORT_DIR!" /b
echo.
echo Tamanho total:
for /f "tokens=3" %%a in ('dir "!EXPORT_DIR!" ^| find "bytes"') do echo Tamanho: %%a bytes
echo.
echo Para usar no servidor:
echo 1. Copie a pasta '!EXPORT_DIR!' para o servidor
echo 2. cd docker-images
echo 3. chmod +x import-images.sh  
echo 4. ./import-images.sh
echo 5. Configure Docker Secrets e faca o deploy
echo.
echo URLs finais:
echo - https://pesquisax.thaishiroki.com.br
echo - https://api-pesquisax.thaishiroki.com.br
echo.
pause