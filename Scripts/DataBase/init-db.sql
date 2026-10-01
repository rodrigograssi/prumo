-- Script de inicialização do banco de dados Prumo
-- Este script será executado automaticamente quando o container for criado

-- Criar extensões úteis
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";

-- Configurar timezone
SET timezone = 'UTC';

-- Mensagem de confirmação
DO $$
BEGIN
    RAISE NOTICE 'Banco de dados Prumo inicializado com sucesso!';
    RAISE NOTICE 'Database: prumo';
    RAISE NOTICE 'User: prumo_user';
    RAISE NOTICE 'Timezone: UTC';
END $$;