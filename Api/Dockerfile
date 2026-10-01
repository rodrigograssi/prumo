# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG BUILD_NUMBER=docker
ARG GIT_COMMIT_HASH=docker-build

WORKDIR /src

COPY ["Api/Prumo.Api.csproj", "Api/"]
COPY ["Shared/Prumo.Shared.csproj", "Shared/"]
COPY ["Infra/Prumo.Infra.csproj", "Infra/"]

RUN dotnet restore "Api/Prumo.Api.csproj"
COPY . .

ENV BUILD_NUMBER=$BUILD_NUMBER
ENV GIT_COMMIT_HASH=$GIT_COMMIT_HASH
ENV ASPNETCORE_ENVIRONMENT=Production

WORKDIR "/src/Api"
RUN dotnet publish "Prumo.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

RUN adduser --disabled-password --home /app --gecos '' appuser && \
    chown -R appuser:appuser /app
USER appuser

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Prumo.Api.dll"]
