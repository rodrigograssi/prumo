# Guia de Deploy - Prumo Docker Swarm

Este guia contém todas as informações necessárias para fazer o deploy do Prumo em um cluster Docker Swarm com Traefik como proxy reverso.

## 📋 Pré-requisitos

### Sistema
- Docker Engine 20.10+ com Docker Swarm ativo
- Git (para obter hash dos commits)
- OpenSSL (para gerar secrets)
- Bash (para executar os scripts)

### Traefik
O Traefik deve estar configurado e rodando no cluster com:
- Rede `traefik` criada: `docker network create --driver overlay traefik`
- Certificados SSL configurados (Let's Encrypt recomendado)
- Entrypoints `web` (80) e `websecure` (443)

## 🚀 Deploy Rápido

### 1. Configurar Secrets
```bash
cd Scripts
./setup-secrets.sh
```

### 2. Deploy da Aplicação
```bash
./deploy.sh
```

### 3. Verificar Status
```bash
./deploy.sh status
```

## 📁 Estrutura dos Arquivos

```
Scripts/
├── docker-compose.yml     # Stack principal do Docker Swarm
├── deploy.sh             # Script de deploy automatizado
├── setup-secrets.sh      # Configuração de secrets
├── .env.example          # Exemplo de variáveis de ambiente
└── DEPLOY_README.md      # Este arquivo
```

## ⚙️ Configuração Detalhada

### Variáveis de Ambiente

Copie `.env.example` para `.env` e ajuste conforme necessário:

```bash
cp .env.example .env
nano .env
```

**Principais variáveis:**
- `DOMAIN_WEB`: Domínio principal (ex: prumo.com)
- `DOMAIN_API`: Domínio da API (ex: api.prumo.com)
- `BUILD_NUMBER`: Número do build (automático)
- `GIT_COMMIT_HASH`: Hash do commit (automático)

### Docker Secrets

O sistema usa Docker Secrets para informações sensíveis:

```bash
# Senha do PostgreSQL
echo "sua_senha_super_segura" | docker secret create prumo_postgres_password -

# JWT Secret
openssl rand -base64 64 | docker secret create prumo_jwt_secret -
```

## 🏗️ Arquitetura do Deploy

### Serviços

1. **database** (PostgreSQL 16)
   - Réplicas: 1 (apenas no manager)
   - Persistência: Volume `postgres_data`
   - Backup: Volume `postgres_backup`

2. **api** (Prumo API)
   - Réplicas: 2 (load balancing)
   - URL: `https://api.prumo.local`
   - Healthcheck: `/health`

3. **web** (Frontend Blazor)
   - Réplicas: 2 (load balancing)
   - URL: `https://prumo.local`
   - SPA routing configurado

4. **db-backup** (Backup automático)
   - Backup a cada 6 horas
   - Retenção de 7 dias

5. **adminer** (Administração DB - opcional)
   - URL: `https://db.prumo.local`

### Redes

- **traefik**: Rede externa para proxy reverso
- **prumo-internal**: Rede interna dos serviços

### Volumes

- **postgres_data**: Dados do PostgreSQL
- **postgres_backup**: Backups automáticos

## 🔧 Scripts de Deploy

### deploy.sh

Script principal de deploy com comandos:

```bash
# Deploy completo (build + deploy)
./deploy.sh

# Apenas build das imagens
./deploy.sh build

# Atualizar stack existente
./deploy.sh update

# Ver status dos serviços
./deploy.sh status

# Ver logs de um serviço
./deploy.sh logs api
./deploy.sh logs web
./deploy.sh logs database

# Remover stack
./deploy.sh remove

# Ajuda
./deploy.sh help
```

### setup-secrets.sh

Configura os Docker Secrets necessários:

```bash
# Configuração interativa
./setup-secrets.sh

# Salvar backup dos secrets (use com cuidado!)
SAVE_SECRETS=true ./setup-secrets.sh

# Usar senhas específicas
POSTGRES_PASSWORD="minha_senha" JWT_SECRET="meu_jwt_secret" ./setup-secrets.sh
```

## 🌐 Configuração de DNS

Para testar localmente, adicione ao `/etc/hosts` (Linux/Mac) ou `C:\Windows\System32\drivers\etc\hosts` (Windows):

```
127.0.0.1 prumo.local
127.0.0.1 api.prumo.local
127.0.0.1 db.prumo.local
```

## 🔍 Monitoramento e Logs

### Verificar Status dos Serviços
```bash
docker stack services prumo
```

### Logs em Tempo Real
```bash
# API
docker service logs prumo_api -f

# Web
docker service logs prumo_web -f

# Database
docker service logs prumo_database -f
```

### Informações dos Containers
```bash
# Listar containers do stack
docker stack ps prumo

# Entrar em um container
docker exec -it $(docker ps -q -f name=prumo_api) bash
```

## 🔒 Segurança

### Configurações Implementadas

1. **Docker Secrets**: Senhas não ficam em variáveis de ambiente
2. **Non-root User**: Containers rodando com usuário não-privilegiado
3. **Resource Limits**: Limites de CPU e memória configurados
4. **Health Checks**: Verificações automáticas de saúde
5. **SSL/TLS**: HTTPS obrigatório via Traefik

### Recomendações Adicionais

- Use senhas fortes geradas automaticamente
- Configure firewall para bloquear portas não utilizadas
- Mantenha Docker e sistema operacional atualizados
- Configure logging centralizado
- Faça backups regulares dos volumes

## 🚨 Troubleshooting

### Serviços não iniciam
```bash
# Verificar logs detalhados
docker service logs prumo_api --details

# Verificar recursos disponíveis
docker node ls
docker system df
```

### Problemas de conectividade
```bash
# Verificar redes
docker network ls
docker network inspect traefik

# Verificar secrets
docker secret ls
```

### Erro de certificados SSL
- Verifique se o Traefik está configurado corretamente
- Confirme se os domínios estão apontando para o servidor
- Verifique logs do Traefik: `docker service logs traefik_traefik -f`

### Database não conecta
```bash
# Verificar se PostgreSQL está rodando
docker service logs prumo_database

# Testar conexão
docker exec -it $(docker ps -q -f name=prumo_database) psql -U prumo -d prumo
```

## 🔄 Atualizações

### Atualizar Aplicação
```bash
# Build nova versão e deploy
./deploy.sh update
```

### Rollback
```bash
# Remover stack atual
docker stack rm prumo

# Fazer checkout do commit anterior
git checkout HEAD~1

# Deploy da versão anterior
./deploy.sh
```

### Backup antes de Atualizar
```bash
# Backup manual do banco
docker exec $(docker ps -q -f name=prumo_database) \
  pg_dump -U prumo prumo > backup_$(date +%Y%m%d_%H%M%S).sql
```

## 📊 Métricas e Performance

### Recursos Utilizados
```bash
# Estatísticas de uso
docker stats --no-stream

# Informações detalhadas dos serviços
docker service inspect prumo_api --pretty
```

### Configurações de Performance

**CPU e Memória** (ajustar conforme hardware):
- API: 0.5 CPU, 512MB RAM
- Web: 0.3 CPU, 256MB RAM  
- Database: 0.5 CPU, 512MB RAM

**Réplicas** (ajustar conforme carga):
- API: 2 réplicas (load balancing)
- Web: 2 réplicas (load balancing)
- Database: 1 réplica (master único)

## 🔗 URLs de Acesso

Após deploy bem-sucedido:

- 🌐 **Frontend**: https://prumo.local
- 🔌 **API**: https://api.prumo.local
- 📋 **Swagger**: https://api.prumo.local/swagger
- 🗄️ **Adminer**: https://db.prumo.local
- 📈 **Health Check**: https://api.prumo.local/health

## 📞 Suporte

Para problemas específicos:

1. Verifique os logs dos serviços
2. Confirme se todos os pré-requisitos estão atendidos
3. Teste a conectividade entre os serviços
4. Verifique a documentação do Traefik para problemas de proxy

---

**Versão do Guia**: 1.0.0  
**Última Atualização**: Janeiro 2025