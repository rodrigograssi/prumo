#!/bin/bash

# Script para configurar Docker Secrets para o Prumo
# Execute este script antes do primeiro deploy

set -e

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

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

# Banner
echo -e "${BLUE}"
echo "=========================================="
echo "    Prumo Docker Secrets Setup    "
echo "=========================================="
echo -e "${NC}"

# Verificar se Docker Swarm está ativo
if ! docker info --format '{{.Swarm.LocalNodeState}}' | grep -q active; then
    log_error "Docker Swarm não está ativo!"
    log_info "Inicialize o swarm com: docker swarm init"
    exit 1
fi

# Função para verificar se secret já existe
secret_exists() {
    docker secret ls --format '{{.Name}}' | grep -q "^$1$"
}

# Função para criar secret
create_secret() {
    local secret_name=$1
    local secret_value=$2
    local description=$3
    
    if secret_exists "$secret_name"; then
        log_warning "Secret '$secret_name' já existe. Pulando..."
        return 0
    fi
    
    log_info "Criando secret '$secret_name'..."
    echo -n "$secret_value" | docker secret create "$secret_name" -
    log_success "Secret '$secret_name' criado com sucesso!"
}

# Função para gerar senha segura
generate_password() {
    openssl rand -base64 32 | tr -d "=+/" | cut -c1-25
}

# Função para gerar JWT secret
generate_jwt_secret() {
    openssl rand -base64 64 | tr -d "=+/" | cut -c1-64
}

# 1. Configurar senha do PostgreSQL
log_info "Configurando senha do PostgreSQL..."

if [ -n "$POSTGRES_PASSWORD" ]; then
    # Usar senha fornecida via variável de ambiente
    POSTGRES_PASS="$POSTGRES_PASSWORD"
    log_info "Usando senha do PostgreSQL fornecida via variável de ambiente"
else
    # Gerar senha aleatória
    POSTGRES_PASS=$(generate_password)
    log_info "Senha do PostgreSQL gerada automaticamente"
fi

create_secret "prumo_postgres_password" "$POSTGRES_PASS" "Senha do banco PostgreSQL"

# 2. Configurar JWT Secret
log_info "Configurando JWT Secret..."

if [ -n "$JWT_SECRET" ]; then
    # Usar secret fornecido via variável de ambiente
    JWT_SEC="$JWT_SECRET"
    log_info "Usando JWT Secret fornecido via variável de ambiente"
else
    # Gerar JWT secret aleatório
    JWT_SEC=$(generate_jwt_secret)
    log_info "JWT Secret gerado automaticamente"
fi

create_secret "prumo_jwt_secret" "$JWT_SEC" "Chave secreta para JWT"

# 3. Salvar informações em arquivo seguro (opcional)
SECRETS_FILE="secrets-backup.txt"
if [ "$SAVE_SECRETS" = "true" ]; then
    log_info "Salvando secrets em arquivo '$SECRETS_FILE'..."
    
    cat > "$SECRETS_FILE" << EOF
# Prumo Secrets Backup
# Generated on: $(date)
# ⚠️ MANTENHA ESTE ARQUIVO SEGURO E PRIVADO! ⚠️

POSTGRES_PASSWORD=$POSTGRES_PASS
JWT_SECRET=$JWT_SEC

# Para recriar os secrets:
# echo -n "$POSTGRES_PASS" | docker secret create prumo_postgres_password -
# echo -n "$JWT_SEC" | docker secret create prumo_jwt_secret -
EOF
    
    chmod 600 "$SECRETS_FILE"
    log_success "Secrets salvos em '$SECRETS_FILE'"
    log_warning "⚠️ Mantenha o arquivo '$SECRETS_FILE' seguro e privado!"
fi

# 4. Verificar secrets criados
log_info "Verificando secrets criados..."
echo -e "\n${BLUE}Secrets do Prumo:${NC}"
docker secret ls --filter name=prumo --format "table {{.Name}}\t{{.CreatedAt}}\t{{.UpdatedAt}}"

# 5. Mostrar próximos passos
echo -e "\n${GREEN}✅ Secrets configurados com sucesso!${NC}"
echo -e "\n${BLUE}Próximos passos:${NC}"
echo -e "  1. Execute o deploy: ${YELLOW}./deploy.sh${NC}"
echo -e "  2. Verifique se os serviços estão rodando: ${YELLOW}docker stack services prumo${NC}"
echo -e "  3. Acesse a aplicação em: ${YELLOW}https://prumo.local${NC}"

# 6. Informações importantes
echo -e "\n${YELLOW}⚠️ Informações Importantes:${NC}"
echo -e "  • Os secrets são criptografados e distribuídos apenas para os nós que precisam"
echo -e "  • Para remover um secret: ${YELLOW}docker secret rm <nome_do_secret>${NC}"
echo -e "  • Para listar todos os secrets: ${YELLOW}docker secret ls${NC}"
echo -e "  • Secrets não podem ser modificados, apenas removidos e recriados"

if [ "$SAVE_SECRETS" = "true" ]; then
    echo -e "  • Backup dos secrets salvo em: ${YELLOW}$SECRETS_FILE${NC}"
    echo -e "  • ${RED}IMPORTANTE: Mova este arquivo para um local seguro!${NC}"
fi

echo -e "\n${BLUE}Para mais informações sobre Docker Secrets:${NC}"
echo -e "  https://docs.docker.com/engine/swarm/secrets/"