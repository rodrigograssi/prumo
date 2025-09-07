# Sistema de Versionamento Automático - PesquisaX

Este documento descreve como o sistema de versionamento automático funciona no PesquisaX.

## 📋 Visão Geral

O sistema de versionamento automático foi implementado para:
- Gerar automaticamente builds numbers únicos
- Capturar hashes de commits do Git
- Detectar o ambiente de execução (Development/Production)
- Funcionar tanto em desenvolvimento local quanto em CI/CD
- Exibir informações detalhadas de versão na interface

## 🔧 Componentes

### 1. AppVersion.cs - Classe Central de Versionamento
Localizada em: `Shared/Version/AppVersion.cs`

**Funcionalidades:**
- **Versão Semântica**: Mantém a versão principal (ex: 1.0.0)
- **Build Number Automático**: Gerado baseado na data/hora de compilação ou CI/CD
- **Hash do Commit**: Obtido automaticamente do Git local ou CI/CD
- **Detecção de Ambiente**: Development/Production baseado na compilação
- **Informações de Build**: Timestamp de compilação e detalhes técnicos

**Propriedades Principais:**
```csharp
public static string Current        // Versão principal (1.0.0)
public static string BuildNumber    // Build automático (202501061430)
public static string CommitHash     // Hash do commit (abc1234...)
public static string Environment    // Development/Production
public static DateTime CompileTime  // Data/hora de compilação
public static string FullVersion    // Versão completa (1.0.0.202501061430)
public static string DetailedVersion // Para UI (v1.0.0.202501061430-abc1234 (Development))
```

### 2. Scripts de Build

#### PowerShell Script (`build.ps1`)
Script para builds locais com suporte a:
- Detecção automática de informações do Git
- Configuração de variáveis de ambiente
- Build, teste e publicação automatizados
- Limpeza de artifacts

**Uso:**
```powershell
# Build simples
.\build.ps1

# Build com publicação
.\build.ps1 -Publish -Configuration Release

# Build com limpeza
.\build.ps1 -Clean -Publish
```

#### GitHub Actions (`.github/workflows/build-and-deploy.yml`)
Workflow completo de CI/CD com:
- Build automático em múltiplas branches
- Execução de testes
- Geração de artifacts
- Deploy automático para staging/production
- Criação automática de tags de versão

## 🚀 Como Funciona

### Desenvolvimento Local
1. **Build Number**: Gerado baseado na data/hora de compilação (formato: `yyyyMMddHHmm`)
2. **Commit Hash**: Lido diretamente do `.git/HEAD` e refs locais
3. **Ambiente**: Definido pela compilação (#DEBUG vs #RELEASE)

### CI/CD (GitHub Actions, Azure DevOps, etc.)
1. **Build Number**: Obtido das variáveis de ambiente do CI:
   - `BUILD_NUMBER` (Jenkins)
   - `GITHUB_RUN_NUMBER` (GitHub Actions)
   - `CI_PIPELINE_ID` (GitLab)

2. **Commit Hash**: Obtido das variáveis de ambiente:
   - `GITHUB_SHA` (GitHub Actions)
   - `BUILD_SOURCEVERSION` (Azure DevOps)
   - `CI_COMMIT_SHA` (GitLab)

3. **Ambiente**: Definido por:
   - `ASPNETCORE_ENVIRONMENT`
   - `DOTNET_ENVIRONMENT`

## 📱 Interface do Usuário

### Footer Principal
Localização: `Layout/MainLayout.razor` (linha 64-85)
- Exibe copyright e versão detalhada
- Botão para acessar o changelog
- Informações fixas na parte inferior

### Página de Changelog  
Localização: `Pages/ChangeLog.razor`
- Exibe a versão atual em destaque
- Timeline de todas as versões
- Estatísticas de mudanças
- Atualização automática das informações

## 🔍 Variáveis de Ambiente Suportadas

### CI/CD Build Numbers:
- `BUILD_NUMBER` - Número sequencial do build
- `GITHUB_RUN_NUMBER` - Número do run no GitHub Actions
- `CI_PIPELINE_ID` - ID do pipeline no GitLab

### Commit Hashes:
- `GIT_COMMIT_HASH` - Hash personalizado
- `GITHUB_SHA` - SHA do commit no GitHub
- `BUILD_SOURCEVERSION` - Versão source no Azure DevOps
- `CI_COMMIT_SHA` - SHA do commit no GitLab

### Ambientes:
- `ASPNETCORE_ENVIRONMENT` - Ambiente ASP.NET Core
- `DOTNET_ENVIRONMENT` - Ambiente .NET

## 📝 Exemplos de Saída

### Desenvolvimento Local:
- **BuildNumber**: `202501061430` (ano-mês-dia-hora-minuto)
- **CommitHash**: `abc1234def5678` (obtido do Git local)
- **Environment**: `Development`
- **DetailedVersion**: `v1.0.0.202501061430-abc1234 (Development)`

### CI/CD Production:
- **BuildNumber**: `42` (número do build do CI)
- **CommitHash**: `abc1234def5678901234567890abcdef12345678`
- **Environment**: `Production`
- **DetailedVersion**: `v1.0.0.42-abc1234 (Production)`

## 🛠️ Configuração para Novos Ambientes

### Azure DevOps
```yaml
variables:
  BUILD_NUMBER: $(Build.BuildNumber)
  BUILD_SOURCEVERSION: $(Build.SourceVersion)
  ASPNETCORE_ENVIRONMENT: Production
```

### GitLab CI
```yaml
variables:
  CI_PIPELINE_ID: $CI_PIPELINE_ID
  CI_COMMIT_SHA: $CI_COMMIT_SHA
  ASPNETCORE_ENVIRONMENT: Production
```

### Docker Build
```dockerfile
ARG BUILD_NUMBER=local
ARG GIT_COMMIT_HASH=dev
ARG ASPNETCORE_ENVIRONMENT=Production

ENV BUILD_NUMBER=$BUILD_NUMBER
ENV GIT_COMMIT_HASH=$GIT_COMMIT_HASH
ENV ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT
```

## 🔧 Manutenção

### Atualizando a Versão Principal
Edite `Shared/Version/AppVersion.cs`:
```csharp
public const string Current = "1.1.0"; // Nova versão
```

### Adicionando Novo Changelog
Edite `Shared/Version/ChangeLogService.cs` e adicione nova entrada:
```csharp
new VersionChanges
{
    Version = "1.1.0",
    ReleaseDate = new DateTime(2025, 02, 01),
    Changes = new List<ChangeLogItem> { /* suas mudanças */ }
}
```

## 📊 Monitoramento

### Logs de Build
O sistema gera logs detalhados incluindo:
- Informações de compilação
- Detecção de Git
- Variáveis de ambiente detectadas
- Timestamps de build

### Debug de Versioning
Use a propriedade `BuildInfo` para debugging:
```csharp
Console.WriteLine(AppVersion.BuildInfo);
// Output: PesquisaX v1.0.0 | Build 202501061430 | Development | Compiled 2025-01-06 14:30:15
```

## ✅ Benefícios

1. **Rastreabilidade**: Cada build é único e rastreável
2. **Automação**: Zero intervenção manual necessária
3. **Flexibilidade**: Funciona em qualquer ambiente
4. **Transparência**: Usuários veem exatamente qual versão estão usando
5. **Debug**: Facilita identificação de problemas em produção
6. **CI/CD Ready**: Integração perfeita com pipelines automatizados