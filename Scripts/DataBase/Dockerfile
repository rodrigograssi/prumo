# Dockerfile para PostgreSQL - Sistema Prumo
FROM postgres:16

# Definir variáveis de ambiente para configuração do banco
ENV POSTGRES_DB=prumo
ENV POSTGRES_USER=prumo_user
ENV POSTGRES_PASSWORD=prumo_2024!
ENV POSTGRES_INITDB_ARGS="--auth-host=scram-sha-256 --auth-local=scram-sha-256"

# Configurar locale usando padrões disponíveis na imagem
ENV LANG=C.UTF-8
ENV LC_COLLATE=C.UTF-8
ENV LC_CTYPE=C.UTF-8

# Expor porta padrão do PostgreSQL
EXPOSE 5432

# Volume para persistir dados
VOLUME ["/var/lib/postgresql/data"]

# Script de inicialização será executado quando o container for criado
COPY init-db.sql /docker-entrypoint-initdb.d/