# Script de Build do PesquisaX com Versionamento Automático
# PowerShell script para builds locais e CI/CD

param(
    [string]$Configuration = "Debug",
    [string]$Output = "./publish",
    [string]$GitCommitHash = "",
    [string]$BuildNumber = "",
    [switch]$IncludeCommitHash,
    [switch]$Publish,
    [switch]$Clean
)

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "    PesquisaX Build Script v1.0     " -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Função para obter informações do Git
function Get-GitInfo {
    try {
        $gitCommit = ""
        $gitBranch = ""
        
        if (Test-Path ".git") {
            $gitCommit = git rev-parse HEAD 2>$null
            $gitBranch = git rev-parse --abbrev-ref HEAD 2>$null
        }
        
        return @{
            Commit = $gitCommit
            Branch = $gitBranch
            HasGit = -not [string]::IsNullOrEmpty($gitCommit)
        }
    }
    catch {
        return @{
            Commit = ""
            Branch = ""
            HasGit = $false
        }
    }
}

# Obter informações do Git
$gitInfo = Get-GitInfo

# Definir variáveis de ambiente para o build
if ([string]::IsNullOrEmpty($GitCommitHash) -and $gitInfo.HasGit) {
    $GitCommitHash = $gitInfo.Commit
}

if ([string]::IsNullOrEmpty($BuildNumber)) {
    $BuildNumber = (Get-Date).ToString("yyyyMMddHHmm")
}

# Mostrar informações do build
Write-Host "Configurações do Build:" -ForegroundColor Yellow
Write-Host "  Configuration: $Configuration" -ForegroundColor White
Write-Host "  Build Number: $BuildNumber" -ForegroundColor White
Write-Host "  Git Commit: $(if($GitCommitHash) { $GitCommitHash.Substring(0, [Math]::Min(8, $GitCommitHash.Length)) } else { 'N/A' })" -ForegroundColor White
Write-Host "  Git Branch: $(if($gitInfo.Branch) { $gitInfo.Branch } else { 'N/A' })" -ForegroundColor White
Write-Host ""

# Definir variáveis de ambiente para que o AppVersion possa acessá-las
if ($GitCommitHash) {
    $env:GIT_COMMIT_HASH = $GitCommitHash
}
if ($BuildNumber) {
    $env:BUILD_NUMBER = $BuildNumber
}
$env:ASPNETCORE_ENVIRONMENT = if ($Configuration -eq "Release") { "Production" } else { "Development" }

try {
    # Limpeza (se solicitado)
    if ($Clean) {
        Write-Host "🧹 Limpando artifacts anteriores..." -ForegroundColor Blue
        if (Test-Path "bin") { Remove-Item "bin" -Recurse -Force }
        if (Test-Path "obj") { Remove-Item "obj" -Recurse -Force }
        if (Test-Path $Output) { Remove-Item $Output -Recurse -Force }
        Write-Host "✅ Limpeza concluída" -ForegroundColor Green
        Write-Host ""
    }

    # Restore de dependências
    Write-Host "📦 Restaurando dependências..." -ForegroundColor Blue
    dotnet restore
    if ($LASTEXITCODE -ne 0) {
        throw "Falha no restore das dependências"
    }
    Write-Host "✅ Dependências restauradas" -ForegroundColor Green
    Write-Host ""

    # Build da solução
    Write-Host "🔨 Compilando solução..." -ForegroundColor Blue
    dotnet build --configuration $Configuration --no-restore
    if ($LASTEXITCODE -ne 0) {
        throw "Falha na compilação"
    }
    Write-Host "✅ Compilação concluída" -ForegroundColor Green
    Write-Host ""

    # Executar testes (se existirem)
    $testProjects = Get-ChildItem -Path . -Recurse -Name "*.Test*.csproj" -o -Name "*Tests.csproj"
    if ($testProjects.Count -gt 0) {
        Write-Host "🧪 Executando testes..." -ForegroundColor Blue
        dotnet test --configuration $Configuration --no-build --verbosity minimal
        if ($LASTEXITCODE -ne 0) {
            Write-Warning "⚠️ Alguns testes falharam, mas continuando o build..."
        }
        else {
            Write-Host "✅ Todos os testes passaram" -ForegroundColor Green
        }
        Write-Host ""
    }

    # Publicação (se solicitado)
    if ($Publish) {
        Write-Host "📂 Publicando aplicação..." -ForegroundColor Blue
        
        # Publicar API
        Write-Host "   📡 Publicando API..." -ForegroundColor Cyan
        dotnet publish Api/Api.csproj --configuration $Configuration --output "$Output/api" --no-build
        if ($LASTEXITCODE -ne 0) {
            throw "Falha na publicação da API"
        }

        # Publicar Web
        Write-Host "   🌐 Publicando Web..." -ForegroundColor Cyan
        dotnet publish Web/Web.csproj --configuration $Configuration --output "$Output/web" --no-build
        if ($LASTEXITCODE -ne 0) {
            throw "Falha na publicação do Web"
        }

        Write-Host "✅ Publicação concluída em: $Output" -ForegroundColor Green
        Write-Host ""
    }

    # Exibir informações da versão compilada
    Write-Host "📋 Informações da Build:" -ForegroundColor Yellow
    Write-Host "   Versão: $(dotnet run --project Shared --configuration $Configuration --property:EmitCompilerGeneratedFiles=false --property:BuildingInsideVisualStudio=false --verbosity:quiet -- --version 2>$null)" -ForegroundColor White
    Write-Host ""

    Write-Host "🎉 Build concluído com sucesso!" -ForegroundColor Green
    
} catch {
    Write-Host ""
    Write-Host "❌ Erro durante o build:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
} finally {
    # Limpar variáveis de ambiente
    if ($env:GIT_COMMIT_HASH) { Remove-Item Env:GIT_COMMIT_HASH -ErrorAction SilentlyContinue }
    if ($env:BUILD_NUMBER) { Remove-Item Env:BUILD_NUMBER -ErrorAction SilentlyContinue }
}

Write-Host ""
Write-Host "Para executar a aplicação:" -ForegroundColor Cyan
Write-Host "  API: dotnet run --project Api" -ForegroundColor White
Write-Host "  Web: dotnet run --project Web" -ForegroundColor White
Write-Host ""