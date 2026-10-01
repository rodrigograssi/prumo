# Scripts do PostgreSQL - Prumo

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
docker build -t prumo-postgres .
docker run -d --name prumo-db -p 5432:5432 prumo-postgres
```

## Configurações

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

### Verificar container
```bash
docker ps
```

### Ver logs
```bash
docker logs prumo-postgres
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