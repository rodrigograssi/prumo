@echo off
REM Script para Build e Export das Imagens Docker do Prumo (Windows)
REM Cria as imagens localmente e exporta para arquivos .tar para transferir para o servidor

setlocal enabledelayedexpansion

REM Configurações
set "BUILD_NUMBER=%date:~-4,4%%date:~-10,2%%date:~-7,2%%time:~0,2%%time:~3,2%%time:~6,2%"
set "BUILD_NUMBER=%BUILD_NUMBER: =0%"

REM Obter hash do git (se disponível)
for /f "tokens=*" %%i in ('git rev-parse HEAD 2^>nul') do set "GIT_COMMIT_HASH=%%i"
if "!GIT_COMMIT_HASH!"=="" set "GIT_COMMIT_HASH=local-build"

set "EXPORT_DIR=docker-images"

echo ==========================================
echo     Prumo Docker Build ^& Export     
echo ==========================================
echo.

echo [INFO] Build Number: !BUILD_NUMBER!
echo [INFO] Git Commit: !GIT_COMMIT_HASH:~0,8!
echo [INFO] Export Directory: !EXPORT_DIR!
echo.

REM Verificar se Docker está disponível
docker --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Docker nao esta instalado ou nao esta no PATH!
    pause
    exit /b 1
)

REM Passo 1: Criar diretório de export
echo [STEP 1/6] Preparando diretorio de export
if not exist "!EXPORT_DIR!" mkdir "!EXPORT_DIR!"
echo [OK] Diretorio !EXPORT_DIR! criado
echo.

REM Passo 2: Build da API
echo [STEP 2/6] Building Prumo API
echo [INFO] Construindo imagem da API...

REM Mudar para diretório raiz do projeto
cd ..

docker build -f Api\Dockerfile ^
    --build-arg BUILD_CONFIGURATION=Release ^
    --build-arg BUILD_NUMBER=!BUILD_NUMBER! ^
    --build-arg GIT_COMMIT_HASH=!GIT_COMMIT_HASH! ^
    -t prumo/api:latest ^
    -t prumo/api:!BUILD_NUMBER! ^
    -t prumo/api:production ^
    .

REM Voltar para diretório Scripts
cd Scripts

if errorlevel 1 (
    echo [ERROR] Falha no build da API!
    pause
    exit /b 1
)

echo [OK] Imagem da API construida com sucesso!
docker images | findstr prumo/api
echo.

REM Passo 3: Build do Web
echo [STEP 3/6] Building Prumo Web
echo [INFO] Construindo imagem do Web...

REM Mudar para diretório raiz do projeto
cd ..

docker build -f Web\Dockerfile ^
    --build-arg BUILD_CONFIGURATION=Release ^
    --build-arg BUILD_NUMBER=!BUILD_NUMBER! ^
    --build-arg GIT_COMMIT_HASH=!GIT_COMMIT_HASH! ^
    -t prumo/web:latest ^
    -t prumo/web:!BUILD_NUMBER! ^
    -t prumo/web:production ^
    .

REM Voltar para diretório Scripts
cd Scripts

if errorlevel 1 (
    echo [ERROR] Falha no build do Web!
    pause
    exit /b 1
)

echo [OK] Imagem do Web construida com sucesso!
docker images | findstr prumo/web
echo.

REM Passo 4: Export da API
echo [STEP 4/6] Exportando imagem da API
set "API_TAR_FILE=!EXPORT_DIR!\prumo-api-!BUILD_NUMBER!.tar"

echo [INFO] Exportando prumo/api:latest para !API_TAR_FILE!...
docker save prumo/api:latest > "!API_TAR_FILE!"

if errorlevel 1 (
    echo [ERROR] Falha na exportacao da API!
    pause
    exit /b 1
)

echo [OK] API exportada: !API_TAR_FILE!
echo.

REM Passo 5: Export do Web
echo [STEP 5/6] Exportando imagem do Web
set "WEB_TAR_FILE=!EXPORT_DIR!\prumo-web-!BUILD_NUMBER!.tar"

echo [INFO] Exportando prumo/web:latest para !WEB_TAR_FILE!...
docker save prumo/web:latest > "!WEB_TAR_FILE!"

if errorlevel 1 (
    echo [ERROR] Falha na exportacao do Web!
    pause
    exit /b 1
)

echo [OK] Web exportado: !WEB_TAR_FILE!
echo.

REM Passo 6: Criar script de import
echo [STEP 6/6] Criando script de import
set "IMPORT_SCRIPT=!EXPORT_DIR!\import-images.sh"

(
echo #!/bin/bash
echo.
echo # Script para importar imagens Docker do Prumo no servidor
echo # Execute este script no servidor de destino
echo.
echo set -e
echo.
echo echo "======================================"
echo echo "    Prumo Docker Images Import   "
echo echo "======================================"
echo.
echo # Importar API
echo if [ -f "prumo-api-!BUILD_NUMBER!.tar" ]; then
echo     echo "[INFO] Importando prumo/api..."
echo     docker load ^< prumo-api-!BUILD_NUMBER!.tar
echo     echo "[OK] API importada com sucesso!"
echo else
echo     echo "[ERROR] Arquivo prumo-api-!BUILD_NUMBER!.tar nao encontrado!"
echo     exit 1
echo fi
echo.
echo # Importar Web
echo if [ -f "prumo-web-!BUILD_NUMBER!.tar" ]; then
echo     echo "[INFO] Importando prumo/web..."
echo     docker load ^< prumo-web-!BUILD_NUMBER!.tar
echo     echo "[OK] Web importado com sucesso!"
echo else
echo     echo "[ERROR] Arquivo prumo-web-!BUILD_NUMBER!.tar nao encontrado!"
echo     exit 1
echo fi
echo.
echo echo ""
echo echo "Todas as imagens foram importadas com sucesso!"
echo echo ""
echo echo "Imagens disponiveis:"
echo docker images ^| grep prumo
) > "!IMPORT_SCRIPT!"

echo [OK] Script de import criado: !IMPORT_SCRIPT!
echo.

REM Criar arquivo de informações
set "VERSION_INFO=!EXPORT_DIR!\version-info.txt"

(
echo Prumo Docker Images Export Information
echo ==========================================
echo.
echo Build Date: %date% %time%
echo Build Number: !BUILD_NUMBER!
echo Git Commit: !GIT_COMMIT_HASH!
echo Environment: Production
echo.
echo Files Generated:
echo - prumo-api-!BUILD_NUMBER!.tar
echo - prumo-web-!BUILD_NUMBER!.tar  
echo - import-images.sh ^(Import script for server^)
echo - version-info.txt ^(This file^)
echo.
echo Server Import Instructions:
echo 1. Copy all files to your server
echo 2. Run: chmod +x import-images.sh
echo 3. Run: ./import-images.sh
echo 4. Configure Docker Secrets and deploy
echo.
echo URLs ^(after deploy^):
echo - Frontend: https://prumo.thaishiroki.com.br
echo - API: https://api-prumo.thaishiroki.com.br
echo - Health Check: https://api-prumo.thaishiroki.com.br/health
echo - DB Admin: https://db-prumo.thaishiroki.com.br
) > "!VERSION_INFO!"

echo [OK] Informacoes da versao salvas: !VERSION_INFO!
echo.

REM Resumo final
echo ==========================================
echo   Build e Export Concluido com Sucesso!
echo ==========================================
echo.

echo Arquivos Gerados:
dir "!EXPORT_DIR!" /b
echo.

echo Para usar no servidor:
echo 1. Copie todo o diretorio '!EXPORT_DIR!' para seu servidor
echo 2. cd !EXPORT_DIR!
echo 3. ./import-images.sh
echo 4. Configure Docker Secrets e faca o deploy
echo.

echo Pressione qualquer tecla para continuar...
pause >nul