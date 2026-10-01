#!/bin/bash

# Script para Build e Push das Imagens Docker do Prumo para DockerHub
# Execute este script da raiz do projeto (onde estão as pastas Api, Web)

set -e

# Cores
GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

echo -e "${BLUE}============================================${NC}"
echo -e "${BLUE}   Prumo Docker Build & Push DockerHub${NC}"
echo -e "${BLUE}============================================${NC}"
echo

# Verificar se estamos no diretório correto
if [ ! -f "Api/Prumo.Api.csproj" ]; then
    echo -e "${RED}[ERROR] Execute este script da raiz do projeto (onde está a pasta Api)${NC}"
    echo -e "${RED}[ERROR] Diretório atual: $(pwd)${NC}"
    exit 1
fi

if [ ! -f "Web/Prumo.Web.csproj" ]; then
    echo -e "${RED}[ERROR] Execute este script da raiz do projeto (onde está a pasta Web)${NC}"
    echo -e "${RED}[ERROR] Diretório atual: $(pwd)${NC}"
    exit 1
fi

# Configurações DockerHub
DOCKERHUB_USER="rodrigoeajt"
IMAGE_TAG="latest"

# Configurações de build
BUILD_NUMBER=$(date +%Y%m%d%H%M%S)
GIT_COMMIT_HASH=$(git rev-parse HEAD 2>/dev/null || echo "local-build")

echo -e "${BLUE}[INFO]${NC} DockerHub User: $DOCKERHUB_USER"
echo -e "${BLUE}[INFO]${NC} Build Number: $BUILD_NUMBER"
echo -e "${BLUE}[INFO]${NC} Git Commit: ${GIT_COMMIT_HASH:0:8}"
echo -e "${BLUE}[INFO]${NC} Diretório: $(pwd)"
echo

# Build da API
echo -e "${BLUE}[STEP 1/4] Building Prumo API${NC}"
echo -e "${BLUE}[INFO]${NC} Construindo imagem da API para DockerHub..."

docker build -f Api/Dockerfile \
    --build-arg BUILD_CONFIGURATION=Release \
    --build-arg BUILD_NUMBER="$BUILD_NUMBER" \
    --build-arg GIT_COMMIT_HASH="$GIT_COMMIT_HASH" \
    -t "$DOCKERHUB_USER/prumo-api:$IMAGE_TAG" \
    -t "$DOCKERHUB_USER/prumo-api:$BUILD_NUMBER" \
    .

echo -e "${GREEN}[OK]${NC} Imagem da API construída com sucesso!"
echo

# Build do Web
echo -e "${BLUE}[STEP 2/4] Building Prumo Web${NC}"
echo -e "${BLUE}[INFO]${NC} Construindo imagem do Web para DockerHub..."

docker build -f Web/Dockerfile \
    --build-arg BUILD_CONFIGURATION=Release \
    --build-arg BUILD_NUMBER="$BUILD_NUMBER" \
    --build-arg GIT_COMMIT_HASH="$GIT_COMMIT_HASH" \
    -t "$DOCKERHUB_USER/prumo-web:$IMAGE_TAG" \
    -t "$DOCKERHUB_USER/prumo-web:$BUILD_NUMBER" \
    .

echo -e "${GREEN}[OK]${NC} Imagem do Web construída com sucesso!"
echo

# Login no DockerHub
echo -e "${BLUE}[STEP 3/4] Login no DockerHub${NC}"
echo -e "${BLUE}[INFO]${NC} Fazendo login no DockerHub (usuário: $DOCKERHUB_USER)..."
docker login --username "$DOCKERHUB_USER"

echo -e "${GREEN}[OK]${NC} Login no DockerHub realizado com sucesso!"
echo

# Push das imagens
echo -e "${BLUE}[STEP 4/4] Push para DockerHub${NC}"
echo -e "${BLUE}[INFO]${NC} Enviando imagem da API para DockerHub..."
docker push "$DOCKERHUB_USER/prumo-api:$IMAGE_TAG"
docker push "$DOCKERHUB_USER/prumo-api:$BUILD_NUMBER"

echo -e "${BLUE}[INFO]${NC} Enviando imagem do Web para DockerHub..."
docker push "$DOCKERHUB_USER/prumo-web:$IMAGE_TAG"
docker push "$DOCKERHUB_USER/prumo-web:$BUILD_NUMBER"

echo -e "${GREEN}[OK]${NC} Todas as imagens foram enviadas com sucesso!"
echo

# Resumo
echo -e "${GREEN}===========================================${NC}"
echo -e "${GREEN}        Build e Push Concluídos!         ${NC}"
echo -e "${GREEN}===========================================${NC}"
echo

echo -e "${BLUE}Imagens enviadas para DockerHub:${NC}"
echo -e "${YELLOW}- $DOCKERHUB_USER/prumo-api:$IMAGE_TAG${NC}"
echo -e "${YELLOW}- $DOCKERHUB_USER/prumo-api:$BUILD_NUMBER${NC}"
echo -e "${YELLOW}- $DOCKERHUB_USER/prumo-web:$IMAGE_TAG${NC}"
echo -e "${YELLOW}- $DOCKERHUB_USER/prumo-web:$BUILD_NUMBER${NC}"
echo

echo -e "${BLUE}Para usar no servidor, atualize o docker-compose.yml com:${NC}"
echo -e "${YELLOW}  api-test:${NC}"
echo -e "${YELLOW}    image: $DOCKERHUB_USER/prumo-api:$IMAGE_TAG${NC}"
echo -e "${YELLOW}  web-test:${NC}"
echo -e "${YELLOW}    image: $DOCKERHUB_USER/prumo-web:$IMAGE_TAG${NC}"
echo