# Stage 1: Build da aplicação Blazor WebAssembly
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG BUILD_NUMBER=docker
ARG GIT_COMMIT_HASH=docker-build

WORKDIR /src

# Copiar arquivos de projeto para cache das camadas
COPY ["Web/PesquisaX.Web.csproj", "Web/"]
COPY ["Shared/PesquisaX.Shared.csproj", "Shared/"]

# Restore das dependências
RUN dotnet restore "Web/PesquisaX.Web.csproj"

# Copiar todo o código fonte
COPY . .

# Definir variáveis de ambiente para versionamento automático
ENV BUILD_NUMBER=$BUILD_NUMBER
ENV GIT_COMMIT_HASH=$GIT_COMMIT_HASH

# Build da aplicação Blazor WebAssembly
WORKDIR "/src/Web"
RUN dotnet publish "PesquisaX.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-restore

# Stage 2: Imagem final leve com nginx
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Remover arquivos padrão do nginx
RUN rm -rf /usr/share/nginx/html/*

# Copiar os arquivos publicados do Blazor WebAssembly
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html

# Copiar configuração customizada do nginx
COPY Web/nginx.conf /etc/nginx/conf.d/default.conf

# Instalar wget para o healthcheck
RUN apk add --no-cache wget

# Healthcheck para verificar se o nginx está funcionando
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD wget --quiet --tries=1 --spider http://localhost:8081/ || exit 1

# Expõe porta 8081
EXPOSE 8081

# Comando para rodar o nginx em foreground
CMD ["nginx", "-g", "daemon off;"]
