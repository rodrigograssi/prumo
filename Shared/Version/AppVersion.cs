using System.Reflection;

namespace PesquisaX.Shared.Version;

/// <summary>
/// Informações de versão da aplicação PesquisaX com versionamento automático
/// </summary>
public static class AppVersion
{
    /// <summary>
    /// Versão atual da aplicação (Semantic Versioning)
    /// </summary>
    public const string Current = "1.0.3";
    
    /// <summary>
    /// Nome da aplicação
    /// </summary>
    public const string AppName = "PesquisaX";
    
    /// <summary>
    /// Nome completo da aplicação
    /// </summary>
    public const string FullName = "PesquisaX - Sistema de Pesquisas";
    
    /// <summary>
    /// Data de release da versão atual
    /// </summary>
    public static readonly DateTime ReleaseDate = new DateTime(2025, 01, 06);
    
    /// <summary>
    /// Ambiente da aplicação (automático baseado na compilação)
    /// </summary>
    public static string Environment => GetEnvironment();
    
    /// <summary>
    /// Build number (automático baseado na data de compilação)
    /// </summary>
    public static string BuildNumber => GetBuildNumber();
    
    /// <summary>
    /// Commit hash (obtido automaticamente do Git ou CI/CD)
    /// </summary>
    public static string CommitHash => GetCommitHash();
    
    /// <summary>
    /// Data/hora de compilação da aplicação
    /// </summary>
    public static DateTime CompileTime => GetCompileTime();
    
    /// <summary>
    /// Versão completa incluindo build
    /// </summary>
    public static string FullVersion => $"{Current}.{BuildNumber}";
    
    /// <summary>
    /// Informações detalhadas da versão para exibição
    /// </summary>
    public static string DetailedVersion => 
        string.IsNullOrEmpty(CommitHash) || CommitHash == "dev" 
            ? $"v{FullVersion} ({Environment})" 
            : $"v{FullVersion}-{CommitHash[..Math.Min(7, CommitHash.Length)]} ({Environment})";
    
    /// <summary>
    /// Informações completas de build para debug
    /// </summary>
    public static string BuildInfo => 
        $"{AppName} v{Current} | Build {BuildNumber} | {Environment} | Compiled {CompileTime:yyyy-MM-dd HH:mm:ss}";
    
    /// <summary>
    /// Copyright da aplicação
    /// </summary>
    public static string Copyright => $"© {ReleaseDate.Year} PesquisaX - Todos os direitos reservados";
    
    /// <summary>
    /// Indica se é uma build de desenvolvimento
    /// </summary>
    public static bool IsDevelopment => Environment == "Development";
    
    /// <summary>
    /// Obtém o ambiente atual baseado na compilação
    /// </summary>
    private static string GetEnvironment()
    {
        // Verifica variáveis de ambiente de CI/CD primeiro
        var ciEnvironment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") 
                           ?? System.Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        
        if (!string.IsNullOrEmpty(ciEnvironment))
            return ciEnvironment;
        
#if DEBUG
        return "Development";
#else
        return "Production";
#endif
    }
    
    /// <summary>
    /// Gera build number baseado na data/hora de compilação
    /// </summary>
    private static string GetBuildNumber()
    {
        try
        {
            // Verifica se há build number do CI/CD
            var ciBuildNumber = System.Environment.GetEnvironmentVariable("BUILD_NUMBER") 
                               ?? System.Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER")
                               ?? System.Environment.GetEnvironmentVariable("CI_PIPELINE_ID");
            
            if (!string.IsNullOrEmpty(ciBuildNumber))
                return ciBuildNumber;
            
            // Gera baseado na data de compilação (formato: yyyyMMddHHmm)
            return GetCompileTime().ToString("yyyyMMddHHmm");
        }
        catch
        {
            return "local";
        }
    }
    
    /// <summary>
    /// Obtém o hash do commit atual
    /// </summary>
    private static string GetCommitHash()
    {
        try
        {
            // 1. Variáveis de ambiente de CI/CD
            var ciCommit = System.Environment.GetEnvironmentVariable("GIT_COMMIT_HASH")
                          ?? System.Environment.GetEnvironmentVariable("BUILD_SOURCEVERSION")
                          ?? System.Environment.GetEnvironmentVariable("GITHUB_SHA")
                          ?? System.Environment.GetEnvironmentVariable("CI_COMMIT_SHA");
            
            if (!string.IsNullOrEmpty(ciCommit))
                return ciCommit;
            
            // 2. Assembly metadata (configurado durante o build)
            var assembly = Assembly.GetExecutingAssembly();
            var commitMetadata = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                .FirstOrDefault(attr => attr.Key == "GitCommitHash");
            
            if (commitMetadata?.Value != null && !string.IsNullOrEmpty(commitMetadata.Value))
                return commitMetadata.Value;
            
            // 3. Tentar ler do Git local (desenvolvimento)
            return TryGetLocalGitCommit();
        }
        catch
        {
            return "dev";
        }
    }
    
    /// <summary>
    /// Obtém a data/hora de compilação
    /// </summary>
    private static DateTime GetCompileTime()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var location = assembly.Location;
            
            if (File.Exists(location))
            {
                return File.GetLastWriteTime(location);
            }
        }
        catch
        {
            // Fallback
        }
        
        return DateTime.Now;
    }
    
    /// <summary>
    /// Tenta obter o commit hash do Git local
    /// </summary>
    private static string TryGetLocalGitCommit()
    {
        try
        {
            var gitDir = FindGitDirectory();
            if (gitDir == null) return "dev";
            
            var headFile = Path.Combine(gitDir, "HEAD");
            if (!File.Exists(headFile)) return "dev";
            
            var head = File.ReadAllText(headFile).Trim();
            
            // Se HEAD aponta para uma ref (branch)
            if (head.StartsWith("ref: "))
            {
                var refPath = head[5..]; // Remove "ref: "
                var refFile = Path.Combine(gitDir, refPath);
                
                if (File.Exists(refFile))
                {
                    return File.ReadAllText(refFile).Trim();
                }
            }
            // Se HEAD contém diretamente o hash (detached HEAD)
            else if (head.Length >= 7)
            {
                return head;
            }
        }
        catch
        {
            // Ignora erros
        }
        
        return "dev";
    }
    
    /// <summary>
    /// Encontra o diretório .git a partir do diretório atual
    /// </summary>
    private static string? FindGitDirectory()
    {
        var currentDir = AppContext.BaseDirectory;
        
        // Procura até 5 níveis acima
        for (int i = 0; i < 5 && currentDir != null; i++)
        {
            var gitDir = Path.Combine(currentDir, ".git");
            if (Directory.Exists(gitDir))
                return gitDir;
            
            currentDir = Directory.GetParent(currentDir)?.FullName;
        }
        
        return null;
    }
}