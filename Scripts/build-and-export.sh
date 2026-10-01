#!/bin/bash

# Script para Build e Export das Imagens Docker do Prumo
# Cria as imagens localmente e exporta para arquivos .tar para transferir para o servidor

set -e

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Configurações
BUILD_NUMBER=${BUILD_NUMBER:-$(date +%Y%m%d%H%M%S)}
GIT_COMMIT_HASH=${GIT_COMMIT_HASH:-$(git rev-parse HEAD 2>/dev/null || echo "local-build")}
EXPORT_DIR="./docker-images"

# Funções auxiliares
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[✅ OK]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[⚠️  WARN]${NC} $1"
}

log_error() {
    echo -e "${RED}[❌ ERROR]${NC} $1"
}

log_step() {
    echo -e "\n${PURPLE}[STEP $1/6]${NC} $2"
    echo -e "${PURPLE}$(printf '=%.0s' {1..60})${NC}"
}

# Banner
echo -e "${CYAN}"
echo "=========================================="
echo "    Prumo Docker Build & Export     "
echo "=========================================="
echo -e "${NC}"

# Verificar se está no diretório correto
if [ ! -f "build-and-export.sh" ]; then
    log_error "Execute este script a partir do diretório Scripts/"
    exit 1
fi

# Informações do build
log_info "Build Number: $BUILD_NUMBER"
log_info "Git Commit: ${GIT_COMMIT_HASH:0:8}"
log_info "Export Directory: $EXPORT_DIR"

# Passo 1: Criar diretório de export
log_step 1 "Preparando diretório de export"
mkdir -p "$EXPORT_DIR"
log_success "Diretório $EXPORT_DIR criado"

# Passo 2: Build da API
log_step 2 "Building Prumo API"
log_info "Construindo imagem da API..."

# Mudar para diretório raiz do projeto
cd ..

docker build -f Api/Dockerfile \
    --build-arg BUILD_CONFIGURATION=Release \
    --build-arg BUILD_NUMBER="$BUILD_NUMBER" \
    --build-arg GIT_COMMIT_HASH="$GIT_COMMIT_HASH" \
    -t prumo/api:latest \
    -t "prumo/api:$BUILD_NUMBER" \
    -t prumo/api:production \
    .

# Voltar para diretório Scripts
cd Scripts

log_success "Imagem da API construída com sucesso!"
docker images | grep prumo/api

# Passo 3: Build do Web
log_step 3 "Building Prumo Web"
log_info "Construindo imagem do Web..."

# Mudar para diretório raiz do projeto
cd ..

docker build -f Web/Dockerfile \
    --build-arg BUILD_CONFIGURATION=Release \
    --build-arg BUILD_NUMBER="$BUILD_NUMBER" \
    --build-arg GIT_COMMIT_HASH="$GIT_COMMIT_HASH" \
    -t prumo/web:latest \
    -t "prumo/web:$BUILD_NUMBER" \
    -t prumo/web:production \
    .

# Voltar para diretório Scripts
cd Scripts

log_success "Imagem do Web construída com sucesso!"
docker images | grep prumo/web

# Passo 4: Export da API
log_step 4 "Exportando imagem da API"
API_TAR_FILE="$EXPORT_DIR/prumo-api-$BUILD_NUMBER.tar"

log_info "Exportando prumo/api:latest para $API_TAR_FILE..."
docker save prumo/api:latest > "$API_TAR_FILE"

# Verificar tamanho do arquivo
API_SIZE=$(du -h "$API_TAR_FILE" | cut -f1)
log_success "API exportada: $API_TAR_FILE ($API_SIZE)"

# Passo 5: Export do Web
log_step 5 "Exportando imagem do Web"
WEB_TAR_FILE="$EXPORT_DIR/prumo-web-$BUILD_NUMBER.tar"

log_info "Exportando prumo/web:latest para $WEB_TAR_FILE..."
docker save prumo/web:latest > "$WEB_TAR_FILE"

# Verificar tamanho do arquivo
WEB_SIZE=$(du -h "$WEB_TAR_FILE" | cut -f1)
log_success "Web exportada: $WEB_TAR_FILE ($WEB_SIZE)"

# Passo 6: Criar script de import para o servidor
log_step 6 "Criando script de import"
IMPORT_SCRIPT="$EXPORT_DIR/import-images.sh"

cat > "$IMPORT_SCRIPT" << EOF
#!/bin/bash

# Script para importar imagens Docker do Prumo no servidor
# Execute este script no servidor de destino

set -e

echo "======================================"
echo "    Prumo Docker Images Import   "
echo "======================================"

# Cores
GREEN='\033[0;32m'
BLUE='\033[0;34m'
NC='\033[0m'

echo -e "\${BLUE}[INFO]\${NC} Importando imagens Docker do Prumo..."

# Importar API
if [ -f "prumo-api-$BUILD_NUMBER.tar" ]; then
    echo -e "\${BLUE}[INFO]\${NC} Importando prumo/api..."
    docker load < prumo-api-$BUILD_NUMBER.tar
    echo -e "\${GREEN}[OK]\${NC} API importada com sucesso!"
else
    echo -e "\${RED}[ERROR]\${NC} Arquivo prumo-api-$BUILD_NUMBER.tar não encontrado!"
    exit 1
fi

# Importar Web
if [ -f "prumo-web-$BUILD_NUMBER.tar" ]; then
    echo -e "\${BLUE}[INFO]\${NC} Importando prumo/web..."
    docker load < prumo-web-$BUILD_NUMBER.tar
    echo -e "\${GREEN}[OK]\${NC} Web importado com sucesso!"
else
    echo -e "\${RED}[ERROR]\${NC} Arquivo prumo-web-$BUILD_NUMBER.tar não encontrado!"
    exit 1
fi

echo ""
echo -e "\${GREEN}✅ Todas as imagens foram importadas com sucesso!\${NC}"
echo ""
echo "Imagens disponíveis:"
docker images | grep prumo

echo ""
echo -e "\${BLUE}Próximos passos:\${NC}"
echo "1. Configure os Docker Secrets:"
echo "   ./setup-secrets.sh"
echo ""
echo "2. Faça o deploy:"
echo "   docker stack deploy -c docker-compose.yml prumo"
echo ""
echo "3. Verifique o status:"
echo "   docker stack services prumo"
EOF

chmod +x "$IMPORT_SCRIPT"
log_success "Script de import criado: $IMPORT_SCRIPT"

# Criar arquivo de informações da versão
VERSION_INFO="$EXPORT_DIR/version-info.txt"
cat > "$VERSION_INFO" << EOF
Prumo Docker Images Export Information
==========================================

Build Date: $(date)
Build Number: $BUILD_NUMBER
Git Commit: $GIT_COMMIT_HASH
Environment: Production

Files Generated:
- prumo-api-$BUILD_NUMBER.tar ($API_SIZE)
- prumo-web-$BUILD_NUMBER.tar ($WEB_SIZE)
- import-images.sh (Import script for server)
- version-info.txt (This file)

Docker Images Created:
- prumo/api:latest
- prumo/api:$BUILD_NUMBER
- prumo/api:production
- prumo/web:latest
- prumo/web:$BUILD_NUMBER
- prumo/web:production

Server Import Instructions:
1. Copy all files from $EXPORT_DIR to your server
2. Run: chmod +x import-images.sh
3. Run: ./import-images.sh
4. Configure Docker Secrets and deploy

URLs (after deploy):
- Frontend: https://prumo.thaishiroki.com.br
- API: https://api-prumo.thaishiroki.com.br
- Health Check: https://api-prumo.thaishiroki.com.br/health
- DB Admin: https://db-prumo.thaishiroki.com.br
EOF

log_success "Informações da versão salvas: $VERSION_INFO"

# Resumo final
echo -e "\n${GREEN}🎉 Build e Export Concluído com Sucesso!${NC}\n"

echo -e "${CYAN}📦 Arquivos Gerados:${NC}"
ls -lah "$EXPORT_DIR/"

echo -e "\n${CYAN}📋 Resumo:${NC}"
echo -e "  📁 Diretório: ${YELLOW}$EXPORT_DIR${NC}"
echo -e "  🐳 API Image: ${YELLOW}prumo-api-$BUILD_NUMBER.tar${NC} ($API_SIZE)"
echo -e "  🌐 Web Image: ${YELLOW}prumo-web-$BUILD_NUMBER.tar${NC} ($WEB_SIZE)"
echo -e "  📜 Import Script: ${YELLOW}import-images.sh${NC}"
echo -e "  📝 Version Info: ${YELLOW}version-info.txt${NC}"

echo -e "\n${CYAN}🚀 Para usar no servidor:${NC}"
echo -e "  1. ${YELLOW}Copie todo o diretório '$EXPORT_DIR' para seu servidor${NC}"
echo -e "  2. ${YELLOW}cd $EXPORT_DIR${NC}"
echo -e "  3. ${YELLOW}./import-images.sh${NC}"
echo -e "  4. ${YELLOW}Configure Docker Secrets e faça o deploy${NC}"

echo -e "\n${CYAN}📊 Tamanho Total:${NC}"
TOTAL_SIZE=$(du -sh "$EXPORT_DIR" | cut -f1)
echo -e "  ${YELLOW}$TOTAL_SIZE${NC}"

echo -e "\n${BLUE}Comando para compactar (opcional):${NC}"
echo -e "  ${YELLOW}tar -czf prumo-images-$BUILD_NUMBER.tar.gz $EXPORT_DIR${NC}"