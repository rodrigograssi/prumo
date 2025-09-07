# Guia de Export e Import de Imagens Docker - PesquisaX

Este guia explica como criar as imagens Docker na sua máquina local e transferir para o servidor de produção.

## 🎯 Objetivo

Fazer o build das imagens Docker localmente e exportar para arquivos `.tar` que podem ser transferidos para o servidor e importados lá.

## 📋 Pré-requisitos

### Na sua máquina (desenvolvimento):
- Docker Desktop instalado e rodando
- Git (opcional, para hash de commits)
- Bash ou PowerShell/CMD

### No servidor (produção):
- Docker Engine instalado
- Docker Swarm configurado
- Traefik rodando

## 🚀 Processo Completo

### Passo 1: Build e Export Local

**No Windows (PowerShell/CMD):**
```cmd
cd Scripts
build-and-export.bat
```

**No Linux/Mac/WSL:**
```bash
cd Scripts
./build-and-export.sh
```

### Passo 2: Transferir para o Servidor

**Usando SCP:**
```bash
# Compactar primeiro (opcional)
tar -czf pesquisax-images.tar.gz docker-images/

# Transferir
scp -r docker-images/ user@servidor:/home/user/
# ou
scp pesquisax-images.tar.gz user@servidor:/home/user/
```

**Usando FTP/SFTP:**
- Faça upload de toda a pasta `docker-images/`

**Usando Pendrive/Rede:**
- Copie a pasta `docker-images/` para o servidor

### Passo 3: Import no Servidor

```bash
# No servidor
cd docker-images/

# Importar imagens
chmod +x import-images.sh
./import-images.sh

# Verificar se importou
docker images | grep pesquisax
```

### Passo 4: Deploy no Servidor

```bash
# 1. Configurar secrets (primeira vez apenas)
./setup-secrets.sh

# 2. Fazer deploy
docker stack deploy -c docker-compose.yml pesquisax

# 3. Verificar status
docker stack services pesquisax
```

## 📁 Arquivos Gerados

O script cria os seguintes arquivos na pasta `docker-images/`:

```
docker-images/
├── pesquisax-api-YYYYMMDDHHMMSS.tar     # Imagem da API (~200-500MB)
├── pesquisax-web-YYYYMMDDHHMMSS.tar     # Imagem do Web (~200-400MB)  
├── import-images.sh                      # Script de import para o servidor
└── version-info.txt                      # Informações da build
```

## 🔧 Comandos Detalhados

### Build Manual das Imagens

Se quiser fazer o build manualmente:

```bash
# API
docker build -f Api/Dockerfile \
    --build-arg BUILD_CONFIGURATION=Release \
    --build-arg BUILD_NUMBER=$(date +%Y%m%d%H%M%S) \
    --build-arg GIT_COMMIT_HASH=$(git rev-parse HEAD) \
    -t pesquisax/api:latest \
    .

# Web  
docker build -f Web/Dockerfile \
    --build-arg BUILD_CONFIGURATION=Release \
    --build-arg BUILD_NUMBER=$(date +%Y%m%d%H%M%S) \
    --build-arg GIT_COMMIT_HASH=$(git rev-parse HEAD) \
    -t pesquisax/web:latest \
    .
```

### Export Manual das Imagens

```bash
# Criar diretório
mkdir docker-images

# Exportar API
docker save pesquisax/api:latest > docker-images/pesquisax-api.tar

# Exportar Web
docker save pesquisax/web:latest > docker-images/pesquisax-web.tar
```

### Import Manual no Servidor

```bash
# Importar API
docker load < pesquisax-api.tar

# Importar Web
docker load < pesquisax-web.tar

# Verificar
docker images | grep pesquisax
```

## 🔍 Verificações

### Verificar Imagens Criadas

```bash
# Listar imagens locais
docker images | grep pesquisax

# Verificar tamanho das imagens
docker images --format "table {{.Repository}}\t{{.Tag}}\t{{.Size}}" | grep pesquisax
```

### Verificar Arquivos Exportados

```bash
# Listar arquivos
ls -lah docker-images/

# Verificar tamanhos
du -sh docker-images/*
```

### Testar Imagens no Servidor

```bash
# Após import, testar se a API funciona
docker run --rm -p 8080:8080 pesquisax/api:latest &
curl http://localhost:8080/health

# Testar se o Web funciona  
docker run --rm -p 8081:8080 pesquisax/web:latest &
curl http://localhost:8081/
```

## 📊 Tamanhos Típicos

- **API**: ~300-500MB (depende das dependências)
- **Web**: ~200-400MB (Blazor WebAssembly)
- **Total compactado**: ~200-400MB (.tar.gz)

## 🚨 Troubleshooting

### Erro: Docker não encontrado

```bash
# Windows
# Instalar Docker Desktop

# Linux
sudo apt update
sudo apt install docker.io
sudo systemctl start docker
```

### Erro: Permissão negada

```bash
# Adicionar usuário ao grupo docker
sudo usermod -aG docker $USER
# Fazer logout e login novamente
```

### Erro: Arquivo muito grande para transferir

```bash
# Compactar primeiro
tar -czf pesquisax-images.tar.gz docker-images/

# Ou usar docker registry (alternativa)
docker tag pesquisax/api:latest registry.com/pesquisax/api
docker push registry.com/pesquisax/api
```

### Erro: Import falha no servidor

```bash
# Verificar integridade do arquivo
md5sum pesquisax-api.tar  # local
md5sum pesquisax-api.tar  # servidor (devem ser iguais)

# Verificar espaço em disco
df -h

# Verificar se Docker está rodando
docker --version
sudo systemctl status docker
```

## 🔄 Alternativas

### 1. Docker Registry

```bash
# Setup registry privado
docker run -d -p 5000:5000 --name registry registry:2

# Push images
docker tag pesquisax/api:latest localhost:5000/pesquisax/api
docker push localhost:5000/pesquisax/api

# Pull no servidor
docker pull servidor:5000/pesquisax/api
```

### 2. GitHub Container Registry

```bash
# Login
echo $PAT | docker login ghcr.io -u USERNAME --password-stdin

# Tag e push
docker tag pesquisax/api:latest ghcr.io/username/pesquisax/api
docker push ghcr.io/username/pesquisax/api
```

### 3. Build direto no servidor

```bash
# Fazer git clone no servidor
git clone https://github.com/user/pesquisax.git
cd pesquisax/Scripts

# Build direto no servidor
./build-and-export.sh  # mas sem export, direto deploy
```

## 💡 Dicas

1. **Compacte sempre**: Use `.tar.gz` para reduzir tamanho
2. **Versione as imagens**: Use tags com build number
3. **Teste local primeiro**: Rode `docker run` local antes de transferir  
4. **Monitore recursos**: Verifique espaço em disco antes do import
5. **Backup antes de atualizar**: Sempre tenha uma versão funcionando de backup

---

**Próximos passos**: Após import bem-sucedido, configure os Docker Secrets e faça o deploy conforme o [DEPLOY_README.md](./DEPLOY_README.md)