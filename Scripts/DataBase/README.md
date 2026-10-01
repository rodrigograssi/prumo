# Scripts do Banco de Dados PostgreSQL - Prumo

Esta pasta contém os arquivos necessários para configurar e executar o banco de dados PostgreSQL usando Docker.

## Arquivos Incluídos

- **Dockerfile**: Define a imagem PostgreSQL customizada
- **docker-compose.yml**: Configuração para executar o container
- **init-db.sql**: Script de inicialização do banco
- **postgresql.conf**: Configurações personalizadas do PostgreSQL
- **pg_hba.conf**: Configurações de autenticação

## Como Usar

### 1. Executar com Docker Compose (Recomendado)

```bash
cd Scripts
docker-compose up -d
```

### 2. Executar com Docker Build

```bash
cd Scripts
docker build -t prumo-postgres .
docker run -d --name prumo-db -p 5432:5432 prumo-postgres
```

## Configurações do Banco

- **Database**: prumo
- **Usuário**: prumo_user
- **Senha**: prumo_2024!
- **Porta**: 5432
- **Timezone**: UTC
- **Locale**: C.UTF-8

## String de Conexão

```
Host=localhost;Database=prumo;Username=prumo_user;Password=prumo_2024!;Port=5432
```

## Comandos Úteis

### Verificar se o container está rodando
```bash
docker ps
```

### Ver logs do container
```bash
docker logs prumo-postgres
```

### Conectar ao banco via psql
```bash
docker exec -it prumo-postgres psql -U prumo_user -d prumo
```

### Parar o container
```bash
docker-compose down
```

### Parar e remover volumes (CUIDADO: apaga dados)
```bash
docker-compose down -v
```

## Backup e Restore

### Fazer backup
```bash
docker exec prumo-postgres pg_dump -U prumo_user prumo > backup.sql
```

### Restaurar backup
```bash
docker exec -i prumo-postgres psql -U prumo_user -d prumo < backup.sql
```

## Monitoramento

O container inclui um healthcheck que verifica se o PostgreSQL está respondendo. Você pode verificar o status com:

```bash
docker inspect prumo-postgres | grep Health
```