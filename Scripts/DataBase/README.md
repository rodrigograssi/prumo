# Scripts do Banco de Dados PostgreSQL - PesquisaX

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
docker build -t pesquisax-postgres .
docker run -d --name pesquisax-db -p 5432:5432 pesquisax-postgres
```

## Configurações do Banco

- **Database**: pesquisax
- **Usuário**: pesquisax_user
- **Senha**: pesquisax_2024!
- **Porta**: 5432
- **Timezone**: UTC
- **Locale**: C.UTF-8

## String de Conexão

```
Host=localhost;Database=pesquisax;Username=pesquisax_user;Password=pesquisax_2024!;Port=5432
```

## Comandos Úteis

### Verificar se o container está rodando
```bash
docker ps
```

### Ver logs do container
```bash
docker logs pesquisax-postgres
```

### Conectar ao banco via psql
```bash
docker exec -it pesquisax-postgres psql -U pesquisax_user -d pesquisax
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
docker exec pesquisax-postgres pg_dump -U pesquisax_user pesquisax > backup.sql
```

### Restaurar backup
```bash
docker exec -i pesquisax-postgres psql -U pesquisax_user -d pesquisax < backup.sql
```

## Monitoramento

O container inclui um healthcheck que verifica se o PostgreSQL está respondendo. Você pode verificar o status com:

```bash
docker inspect pesquisax-postgres | grep Health
```