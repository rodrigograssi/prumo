# Scripts do PostgreSQL - PesquisaX

## Executar PostgreSQL

### Método Simples (Recomendado)

Execute na pasta Scripts:
```bash
docker-compose up -d
```

### Método Personalizado

Se quiser usar configurações personalizadas, execute na pasta DataBase:
```bash
cd DataBase
docker build -t pesquisax-postgres .
docker run -d --name pesquisax-db -p 5432:5432 pesquisax-postgres
```

## Configurações

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

### Verificar container
```bash
docker ps
```

### Ver logs
```bash
docker logs pesquisax-postgres
```

### Parar container
```bash
docker-compose down
```

### Parar e remover dados (CUIDADO!)
```bash
docker-compose down -v
```

## Estrutura das Pastas

- `docker-compose.yml` - Configuração simples para executar
- `DataBase/` - Contém Dockerfile personalizado e scripts
  - `Dockerfile` - Imagem customizada
  - `init-db.sql` - Script de inicialização
  - `postgresql.conf` - Configurações do PostgreSQL
  - `pg_hba.conf` - Configurações de autenticação