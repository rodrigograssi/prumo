#!/bin/bash

# Script de Deploy do PesquisaX para Docker Swarm
# Configurado para usar Traefik como Proxy Reverso

set -e

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configurações
STACK_NAME="pesquisax"
COMPOSE_FILE="docker-compose.yml"
ENV_FILE=".env"

# Funções auxiliares
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Verificar se está no diretório correto
if [ ! -f "$COMPOSE_FILE" ]; then
    log_error "Arquivo $COMPOSE_FILE não encontrado!"
    log_error "Execute este script a partir do diretório Scripts/"
    exit 1
fi

# Banner
echo -e "${BLUE}"
echo "======================================"
echo "    PesquisaX Docker Swarm Deploy    "
echo "======================================"
echo -e "${NC}"

# Verificar se Docker Swarm está ativo
if ! docker info --format '{{.Swarm.LocalNodeState}}' | grep -q active; then
    log_error "Docker Swarm não está ativo!"
    log_info "Inicialize o swarm com: docker swarm init"
    exit 1
fi

# Verificar se a rede traefik existe
if ! docker network ls --format '{{.Name}}' | grep -q '^traefik$'; then
    log_warning "Rede 'traefik' não encontrada. Criando..."
    docker network create --driver overlay traefik
    log_success "Rede traefik criada com sucesso"
fi

# Obter informações de build
BUILD_NUMBER=${BUILD_NUMBER:-$(date +%Y%m%d%H%M%S)}
GIT_COMMIT_HASH=${GIT_COMMIT_HASH:-$(git rev-parse HEAD 2>/dev/null || echo "manual-deploy")}

log_info "Build Number: $BUILD_NUMBER"
log_info "Git Commit: ${GIT_COMMIT_HASH:0:8}"

# Exportar variáveis para docker-compose
export BUILD_NUMBER
export GIT_COMMIT_HASH

# Verificar se os secrets existem
check_secret() {
    local secret_name=$1
    if ! docker secret ls --format '{{.Name}}' | grep -q "^${secret_name}$"; then
        log_error "Secret '${secret_name}' não encontrado!"
        log_info "Crie o secret com: docker secret create ${secret_name} /caminho/para/arquivo"
        return 1
    fi
    return 0
}

log_info "Verificando secrets..."
secrets_ok=true

if ! check_secret "pesquisax_postgres_password"; then
    secrets_ok=false
    log_info "Exemplo: echo 'senha_super_segura' | docker secret create pesquisax_postgres_password -"
fi

if ! check_secret "pesquisax_jwt_secret"; then
    secrets_ok=false
    log_info "Exemplo: openssl rand -base64 32 | docker secret create pesquisax_jwt_secret -"
fi

if [ "$secrets_ok" = false ]; then
    log_error "Secrets faltando. Configure-os antes de prosseguir."
    exit 1
fi

# Função para build das imagens
build_images() {
    log_info "Fazendo build das imagens Docker..."
    
    # Build da API
    log_info "Building API image..."
    docker build -f ../Api/Dockerfile \
        --build-arg BUILD_CONFIGURATION=Release \
        --build-arg BUILD_NUMBER="$BUILD_NUMBER" \
        --build-arg GIT_COMMIT_HASH="$GIT_COMMIT_HASH" \
        -t pesquisax/api:latest \
        -t "pesquisax/api:$BUILD_NUMBER" \
        ..
    
    # Build do Web
    log_info "Building Web image..."
    docker build -f ../Web/Dockerfile \
        --build-arg BUILD_CONFIGURATION=Release \
        --build-arg BUILD_NUMBER="$BUILD_NUMBER" \
        --build-arg GIT_COMMIT_HASH="$GIT_COMMIT_HASH" \
        -t pesquisax/web:latest \
        -t "pesquisax/web:$BUILD_NUMBER" \
        ..
    
    log_success "Imagens buildadas com sucesso!"
}

# Função para deploy do stack
deploy_stack() {
    log_info "Fazendo deploy do stack '$STACK_NAME'..."
    
    docker stack deploy \
        --compose-file "$COMPOSE_FILE" \
        --with-registry-auth \
        "$STACK_NAME"
    
    log_success "Stack deployado com sucesso!"
}

# Função para verificar status do deploy
check_deployment() {
    log_info "Verificando status do deployment..."
    
    # Aguardar um pouco para os serviços iniciarem
    sleep 10
    
    # Verificar status dos serviços
    echo -e "\n${BLUE}Status dos Serviços:${NC}"
    docker stack services "$STACK_NAME"
    
    echo -e "\n${BLUE}Logs dos Serviços (últimas 10 linhas):${NC}"
    
    # API logs
    echo -e "\n${YELLOW}=== API Logs ===${NC}"
    docker service logs --tail 10 "${STACK_NAME}_api" 2>/dev/null || log_warning "Não foi possível obter logs da API"
    
    # Web logs
    echo -e "\n${YELLOW}=== Web Logs ===${NC}"
    docker service logs --tail 10 "${STACK_NAME}_web" 2>/dev/null || log_warning "Não foi possível obter logs do Web"
    
    # Database logs
    echo -e "\n${YELLOW}=== Database Logs ===${NC}"
    docker service logs --tail 10 "${STACK_NAME}_database" 2>/dev/null || log_warning "Não foi possível obter logs do Database"
}

# Função para mostrar URLs de acesso
show_access_urls() {
    echo -e "\n${GREEN}🎉 Deploy Concluído!${NC}"
    echo -e "\n${BLUE}URLs de Acesso:${NC}"
    echo -e "  🌐 Frontend: https://pesquisax.local"
    echo -e "  🔌 API: https://api.pesquisax.local"
    echo -e "  🗄️  Adminer: https://db.pesquisax.local"
    echo -e "  📊 Swagger: https://api.pesquisax.local/swagger"
    
    echo -e "\n${YELLOW}⚠️  Certifique-se de que os domínios estão configurados no DNS ou /etc/hosts${NC}"
    echo -e "\n${BLUE}Comandos Úteis:${NC}"
    echo -e "  docker stack ls"
    echo -e "  docker stack services $STACK_NAME"
    echo -e "  docker service logs ${STACK_NAME}_api -f"
    echo -e "  docker stack rm $STACK_NAME"
}

# Menu de opções
case "${1:-deploy}" in
    "build")
        build_images
        ;;
    "deploy")
        build_images
        deploy_stack
        check_deployment
        show_access_urls
        ;;
    "update")
        log_info "Atualizando stack existente..."
        build_images
        deploy_stack
        check_deployment
        ;;
    "remove")
        log_warning "Removendo stack '$STACK_NAME'..."
        docker stack rm "$STACK_NAME"
        log_success "Stack removido com sucesso!"
        ;;
    "status")
        log_info "Status do stack '$STACK_NAME':"
        docker stack services "$STACK_NAME"
        ;;
    "logs")
        SERVICE=${2:-api}
        log_info "Logs do serviço '${STACK_NAME}_${SERVICE}':"
        docker service logs "${STACK_NAME}_${SERVICE}" -f
        ;;
    "help"|"-h"|"--help")
        echo "Uso: $0 [comando]"
        echo ""
        echo "Comandos disponíveis:"
        echo "  build    - Apenas fazer build das imagens"
        echo "  deploy   - Build + Deploy completo (padrão)"
        echo "  update   - Atualizar stack existente"
        echo "  remove   - Remover stack"
        echo "  status   - Ver status dos serviços"
        echo "  logs     - Ver logs de um serviço (logs [api|web|database])"
        echo "  help     - Mostrar esta ajuda"
        ;;
    *)
        log_error "Comando desconhecido: $1"
        log_info "Use '$0 help' para ver os comandos disponíveis"
        exit 1
        ;;
esac