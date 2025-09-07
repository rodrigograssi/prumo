@echo off
REM Script para Build e Push das Imagens Docker do PesquisaX para DockerHub
REM Execute este script da raiz do projeto (onde est�o as pastas Api, Web)

REM For�ar o script a rodar no diret�rio onde ele est�
cd /d "%~dp0"

setlocal enabledelayedexpansion

echo ============================================
echo    PesquisaX Docker Build e Push DockerHub
echo ============================================
echo.



REM Configura��es DockerHub
set "DOCKERHUB_USER=rodrigoeajt"
set "IMAGE_TAG=latest"

REM Configura��es de build
set "BUILD_NUMBER=%date:~-4,4%%date:~-10,2%%date:~-7,2%%time:~0,2%%time:~3,2%%time:~6,2%"
set "BUILD_NUMBER=%BUILD_NUMBER: =0%"

REM Obter hash do git (se dispon�vel)
for /f "tokens=*" %%i in ('git rev-parse HEAD 2^>nul') do set "GIT_COMMIT_HASH=%%i"
if "!GIT_COMMIT_HASH!"=="" set "GIT_COMMIT_HASH=local-build"

echo [INFO] DockerHub User: %DOCKERHUB_USER%
echo [INFO] Build Number: !BUILD_NUMBER!
echo [INFO] Git Commit: !GIT_COMMIT_HASH:~0,8!
echo [INFO] Diretorio: %cd%
echo.

REM Verificar se o Docker esta rodando
echo [VERIFICACAO] Testando se Docker esta rodando...
docker version > nul 2>&1
if errorlevel 1 (
    echo [ERROR] Docker nao esta rodando!
    echo [ERROR] Inicie o Docker Desktop e tente novamente.
    pause
    exit /b 1
)
echo [OK] Docker esta funcionando!
echo.

REM Build da API
echo [STEP 1/4] Building PesquisaX API
echo [INFO] Construindo imagem da API para DockerHub...

docker build -f Api\Dockerfile ^
    --build-arg BUILD_CONFIGURATION=Release ^
    --build-arg BUILD_NUMBER=!BUILD_NUMBER! ^
    --build-arg GIT_COMMIT_HASH=!GIT_COMMIT_HASH! ^
    -t %DOCKERHUB_USER%/pesquisax-api:%IMAGE_TAG% ^
    -t %DOCKERHUB_USER%/pesquisax-api:!BUILD_NUMBER! ^
    .

if errorlevel 1 (
    echo [ERROR] Falha no build da API!
    pause
    exit /b 1
)

echo [OK] Imagem da API construida com sucesso!
echo.

REM Build do Web
echo [STEP 2/4] Building PesquisaX Web
echo [INFO] Construindo imagem do Web para DockerHub...

docker build -f Web\Dockerfile ^
    --build-arg BUILD_CONFIGURATION=Release ^
    --build-arg BUILD_NUMBER=!BUILD_NUMBER! ^
    --build-arg GIT_COMMIT_HASH=!GIT_COMMIT_HASH! ^
    -t %DOCKERHUB_USER%/pesquisax-web:%IMAGE_TAG% ^
    -t %DOCKERHUB_USER%/pesquisax-web:!BUILD_NUMBER! ^
    .

if errorlevel 1 (
    echo [ERROR] Falha no build do Web!
    pause
    exit /b 1
)

echo [OK] Imagem do Web construida com sucesso!
echo.

REM Login no DockerHub
echo [STEP 3/4] Login no DockerHub
echo [INFO] Fazendo login no DockerHub (usuario: %DOCKERHUB_USER%)...
docker login --username %DOCKERHUB_USER%

if errorlevel 1 (
    echo [ERROR] Falha no login do DockerHub!
    pause
    exit /b 1
)

echo [OK] Login no DockerHub realizado com sucesso!
echo.

REM Push das imagens
echo [STEP 4/4] Push para DockerHub
echo [INFO] Enviando imagem da API para DockerHub...
docker push %DOCKERHUB_USER%/pesquisax-api:%IMAGE_TAG%
docker push %DOCKERHUB_USER%/pesquisax-api:!BUILD_NUMBER!

if errorlevel 1 (
    echo [ERROR] Falha no push da imagem da API!
    pause
    exit /b 1
)

echo [INFO] Enviando imagem do Web para DockerHub...
docker push %DOCKERHUB_USER%/pesquisax-web:%IMAGE_TAG%
docker push %DOCKERHUB_USER%/pesquisax-web:!BUILD_NUMBER!

if errorlevel 1 (
    echo [ERROR] Falha no push da imagem do Web!
    pause
    exit /b 1
)

echo [OK] Todas as imagens foram enviadas com sucesso!
echo.

REM Resumo
echo ==========================================
echo        Build e Push Concluidos!         
echo ==========================================
echo.
echo Imagens enviadas para DockerHub:
echo - %DOCKERHUB_USER%/pesquisax-api:%IMAGE_TAG%
echo - %DOCKERHUB_USER%/pesquisax-api:!BUILD_NUMBER!
echo - %DOCKERHUB_USER%/pesquisax-web:%IMAGE_TAG%
echo - %DOCKERHUB_USER%/pesquisax-web:!BUILD_NUMBER!
echo.
echo Para usar no servidor, atualize o docker-compose.yml com:
echo   api-test:
echo     image: %DOCKERHUB_USER%/pesquisax-api:%IMAGE_TAG%
echo   web-test:
echo     image: %DOCKERHUB_USER%/pesquisax-web:%IMAGE_TAG%
echo.
pause