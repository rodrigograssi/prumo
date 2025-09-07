#!/bin/bash

# Quick Start Script para PesquisaX Docker Swarm Deploy
# Este script configura tudo automaticamente para um deploy rápido

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
STACK_NAME="pesquisax"
DOMAIN_WEB="pesquisax.local"
DOMAIN_API="api.pesquisax.local"
DOMAIN_DB="db.pesquisax.local"

# Banner
echo -e "${PURPLE}"
cat << "EOF"
██████╗ ███████╗███████╗ ██████╗ ██╗   ██╗██╗███████╗ █████╗ ██╗  ██╗
██╔══██╗██╔════╝██╔════╝██╔═══██╗██║   ██║██║██╔════╝██╔══██╗╚██╗██╔╝
██████╔╝█████╗  ███████╗██║   ██║██║   ██║██║███████╗███████║ ╚███╔╝ 
██╔═══╝ ██╔══╝  ╚════██║██║▄▄ ██║██║   ██║██║╚════██║██╔══██║ ██╔██╗ 
██║     ███████╗███████║╚██████╔╝╚██████╔╝██║███████║██║  ██║██╔╝ ██╗
╚═╝     ╚══════╝╚══════╝ ╚══▀▀═╝  ╚═════╝ ╚═╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝
EOF
echo -e "${NC}"
echo -e "${CYAN}🚀 Quick Start para Docker Swarm Deploy${NC}"
echo -e "${CYAN}==========================================${NC}\n"

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
    echo -e "\n${PURPLE}[STEP $1/7]${NC} $2"
    echo -e "${PURPLE}$(printf '=%.0s' {1..50})${NC}"
}

# Verificar pré-requisitos
check_prerequisites() {
    log_step 1 "Verificando pré-requisitos"
    
    # Docker
    if ! command -v docker &> /dev/null; then
        log_error "Docker não está instalado!"
        exit 1
    fi
    log_success "Docker instalado"
    
    # Docker Swarm
    if ! docker info --format '{{.Swarm.LocalNodeState}}' | grep -q active; then
        log_warning "Docker Swarm não está ativo. Inicializando..."
        docker swarm init --advertise-addr $(hostname -I | awk '{print $1}')
        log_success "Docker Swarm inicializado"
    else
        log_success "Docker Swarm ativo"
    fi
    
    # Git (opcional)
    if ! command -v git &> /dev/null; then
        log_warning "Git não está instalado (opcional para hash de commits)"
    else
        log_success "Git instalado"
    fi
    
    # OpenSSL
    if ! command -v openssl &> /dev/null; then
        log_error "OpenSSL não está instalado!"
        exit 1
    fi
    log_success "OpenSSL instalado"
}

# Configurar rede Traefik
setup_traefik_network() {
    log_step 2 "Configurando rede do Traefik"
    
    if ! docker network ls --format '{{.Name}}' | grep -q '^traefik$'; then
        log_info "Criando rede 'traefik'..."
        docker network create --driver overlay traefik
        log_success "Rede 'traefik' criada"
    else
        log_success "Rede 'traefik' já existe"
    fi
}

# Configurar hosts locais
setup_local_hosts() {
    log_step 3 "Configurando hosts locais"
    
    HOSTS_FILE="/etc/hosts"
    if [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "win32" ]]; then
        HOSTS_FILE="/c/Windows/System32/drivers/etc/hosts"
    fi
    
    # Verificar se precisa de sudo
    if [[ ! -w "$HOSTS_FILE" ]]; then
        log_warning "Permissões necessárias para editar $HOSTS_FILE"
        log_info "Executando com sudo..."
        SUDO_CMD="sudo"
    else
        SUDO_CMD=""
    fi
    
    # Adicionar entradas se não existirem
    for domain in "$DOMAIN_WEB" "$DOMAIN_API" "$DOMAIN_DB"; do
        if ! grep -q "$domain" "$HOSTS_FILE" 2>/dev/null; then
            echo "127.0.0.1 $domain" | $SUDO_CMD tee -a "$HOSTS_FILE" > /dev/null
            log_success "Adicionado $domain ao hosts"
        else
            log_success "$domain já configurado no hosts"
        fi
    done
    
    log_info "Hosts configurados:"
    echo -e "  🌐 ${CYAN}https://$DOMAIN_WEB${NC}"
    echo -e "  🔌 ${CYAN}https://$DOMAIN_API${NC}"
    echo -e "  🗄️  ${CYAN}https://$DOMAIN_DB${NC}"
}

# Configurar Docker Secrets
setup_secrets() {
    log_step 4 "Configurando Docker Secrets"
    
    # PostgreSQL password
    if ! docker secret ls --format '{{.Name}}' | grep -q "^pesquisax_postgres_password$"; then
        log_info "Criando secret do PostgreSQL..."
        POSTGRES_PASS=$(openssl rand -base64 32 | tr -d "=+/" | cut -c1-25)
        echo -n "$POSTGRES_PASS" | docker secret create pesquisax_postgres_password -
        log_success "Secret do PostgreSQL criado"
        echo -e "  ${YELLOW}Senha gerada: $POSTGRES_PASS${NC}"
    else
        log_success "Secret do PostgreSQL já existe"
    fi
    
    # JWT secret
    if ! docker secret ls --format '{{.Name}}' | grep -q "^pesquisax_jwt_secret$"; then
        log_info "Criando JWT secret..."
        JWT_SECRET=$(openssl rand -base64 64 | tr -d "=+/" | cut -c1-64)
        echo -n "$JWT_SECRET" | docker secret create pesquisax_jwt_secret -
        log_success "JWT secret criado"
        echo -e "  ${YELLOW}Secret gerado: ${JWT_SECRET:0:20}...${NC}"
    else
        log_success "JWT secret já existe"
    fi
}

# Build das imagens
build_images() {
    log_step 5 "Fazendo build das imagens Docker"
    
    BUILD_NUMBER=$(date +%Y%m%d%H%M%S)
    GIT_COMMIT=$(git rev-parse HEAD 2>/dev/null || echo "quick-start-$(date +%s)")
    
    export BUILD_NUMBER
    export GIT_COMMIT_HASH="$GIT_COMMIT"
    
    log_info "Build Number: $BUILD_NUMBER"
    log_info "Git Commit: ${GIT_COMMIT:0:8}"
    
    # API
    log_info "Building PesquisaX API..."
    docker build -f ../Api/Dockerfile \
        --build-arg BUILD_CONFIGURATION=Release \
        --build-arg BUILD_NUMBER="$BUILD_NUMBER" \
        --build-arg GIT_COMMIT_HASH="$GIT_COMMIT" \
        -t pesquisax/api:latest \
        -t "pesquisax/api:$BUILD_NUMBER" \
        .. > /dev/null 2>&1
    log_success "API image criada"
    
    # Web
    log_info "Building PesquisaX Web..."
    docker build -f ../Web/Dockerfile \
        --build-arg BUILD_CONFIGURATION=Release \
        --build-arg BUILD_NUMBER="$BUILD_NUMBER" \
        --build-arg GIT_COMMIT_HASH="$GIT_COMMIT" \
        -t pesquisax/web:latest \
        -t "pesquisax/web:$BUILD_NUMBER" \
        .. > /dev/null 2>&1
    log_success "Web image criada"
}

# Deploy do stack
deploy_stack() {
    log_step 6 "Fazendo deploy do stack"
    
    log_info "Deployando stack '$STACK_NAME'..."
    docker stack deploy \
        --compose-file docker-compose.yml \
        --with-registry-auth \
        "$STACK_NAME"
    
    log_success "Stack deployado com sucesso!"
    
    # Aguardar serviços iniciarem
    log_info "Aguardando serviços iniciarem..."
    sleep 15
}

# Verificar status e mostrar resultados
show_results() {
    log_step 7 "Verificando deployment e exibindo resultados"
    
    # Status dos serviços
    log_info "Status dos serviços:"
    docker stack services "$STACK_NAME" --format "table {{.Name}}\t{{.Replicas}}\t{{.Image}}"
    
    echo -e "\n${GREEN}🎉 Deploy do PesquisaX concluído com sucesso!${NC}\n"
    
    # URLs de acesso
    echo -e "${CYAN}📱 URLs de Acesso:${NC}"
    echo -e "  🌐 Frontend:    ${YELLOW}https://$DOMAIN_WEB${NC}"
    echo -e "  🔌 API:         ${YELLOW}https://$DOMAIN_API${NC}"
    echo -e "  📋 Swagger:     ${YELLOW}https://$DOMAIN_API/swagger${NC}"
    echo -e "  ❤️  Health:      ${YELLOW}https://$DOMAIN_API/health${NC}"
    echo -e "  🗄️  DB Admin:    ${YELLOW}https://$DOMAIN_DB${NC}"
    
    # Comandos úteis
    echo -e "\n${CYAN}🛠️  Comandos Úteis:${NC}"
    echo -e "  Ver logs da API:        ${YELLOW}docker service logs ${STACK_NAME}_api -f${NC}"
    echo -e "  Ver logs do Web:        ${YELLOW}docker service logs ${STACK_NAME}_web -f${NC}"
    echo -e "  Ver status:             ${YELLOW}docker stack services $STACK_NAME${NC}"
    echo -e "  Atualizar aplicação:    ${YELLOW}./deploy.sh update${NC}"
    echo -e "  Remover stack:          ${YELLOW}docker stack rm $STACK_NAME${NC}"
    
    # Notas importantes
    echo -e "\n${YELLOW}⚠️  Notas Importantes:${NC}"
    echo -e "  • Os domínios foram configurados para funcionar localmente"
    echo -e "  • Para produção, altere os domínios no docker-compose.yml"
    echo -e "  • Os secrets foram gerados automaticamente"
    echo -e "  • Aguarde alguns segundos para todos os serviços ficarem prontos"
    
    # Teste básico
    echo -e "\n${BLUE}🧪 Testando conectividade...${NC}"
    sleep 5
    
    if curl -s -k "https://$DOMAIN_API/health" > /dev/null; then
        log_success "API respondendo corretamente!"
    else
        log_warning "API ainda não está respondendo. Aguarde mais alguns segundos."
    fi
}

# Executar todos os passos
main() {
    echo -e "${BLUE}Iniciando quick start do PesquisaX...${NC}\n"
    
    check_prerequisites
    setup_traefik_network
    setup_local_hosts
    setup_secrets
    build_images
    deploy_stack
    show_results
    
    echo -e "\n${GREEN}🚀 Quick start concluído! Acesse: https://$DOMAIN_WEB${NC}"
}

# Verificar se está no diretório correto
if [ ! -f "docker-compose.yml" ]; then
    log_error "Execute este script a partir do diretório Scripts/"
    exit 1
fi

# Executar
main