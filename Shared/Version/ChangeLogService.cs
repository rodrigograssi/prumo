namespace PesquisaX.Shared.Version;

/// <summary>
/// Serviço para gerenciar e obter informações do changelog
/// </summary>
public static class ChangeLogService
{
    /// <summary>
    /// Obtém o changelog completo da aplicação
    /// </summary>
    public static List<VersionChanges> GetChangeLog()
    {
        return new List<VersionChanges>
        {
            new VersionChanges
            {
                Version = "1.0.2",
                ReleaseDate = DateTime.Now,
                Changes = new List<ChangeLogItem>
                {
                    new ChangeLogItem
                    {
                        Type = ChangeType.Feature,
                        Description = "Incluído a opção de exibir/ocultar o resultado atual",
                        Author = "Rodrigo"
                    },
                    new ChangeLogItem
                    {
                        Type = ChangeType.Feature,
                        Description = "Incluído de exportar o resultado para PDF",
                        Author = "Rodrigo"
                    },
                    new ChangeLogItem
                    {
                        Type = ChangeType.Feature,
                        Description = "Incluído processo de exportar e importar pesquisa",
                        Author = "Rodrigo"
                    },
                    new ChangeLogItem
                    {
                        Type = ChangeType.Fix,
                        Description = "Corrigido a exclusão de resultado",
                        Author = "Rodrigo"
                    },
                    new ChangeLogItem
                    {
                        Type = ChangeType.Security,
                        Description = "Melhorias no processo de autenticação",
                        Author = "Rodrigo"
                    }
                }
            },
            // Versão atual em desenvolvimento
            new VersionChanges
            {
                Version = "1.0.1",
                ReleaseDate = DateTime.Now,
                Changes = new List<ChangeLogItem>
                {
                    new ChangeLogItem
                    {
                        Type = ChangeType.Feature,
                        Description = "Sistema de controle de versão e changelog implementado",
                        Author = "Sistema"
                    },
                    new ChangeLogItem
                    {
                        Type = ChangeType.Security,
                        Description = "Autenticação JWT implementada em todos os endpoints protegidos",
                        Author = "Sistema"
                    },
                    new ChangeLogItem
                    {
                        Type = ChangeType.Enhancement,
                        Description = "Melhorias no fluxo de login com verificação automática de token",
                        Author = "Sistema"
                    }
                }
            },
            
            // Versão 1.0.0 - Release inicial
            new VersionChanges
            {
                Version = "1.0.0",
                ReleaseDate = new DateTime(2025, 01, 06),
                Changes = new List<ChangeLogItem>
                {
                    new ChangeLogItem
                    {
                        Type = ChangeType.Feature,
                        Description = "Sistema completo de criação e aplicação de pesquisas",
                        Author = "Sistema"
                    },
                    new ChangeLogItem
                    {
                        Type = ChangeType.Feature,
                        Description = "Visualização de resultados em radar charts lado a lado",
                        Author = "Sistema"
                    },
                    new ChangeLogItem
                    {
                        Type = ChangeType.Feature,
                        Description = "Sistema de grupos de questões com pesos automáticos",
                        Author = "Sistema"
                    },
                    new ChangeLogItem
                    {
                        Type = ChangeType.Feature,
                        Description = "Questões invertidas para pontuação reversa",
                        Author = "Sistema"
                    },
                    new ChangeLogItem
                    {
                        Type = ChangeType.Feature,
                        Description = "Interface Blazor WebAssembly com MudBlazor",
                        Author = "Sistema"
                    },
                    new ChangeLogItem
                    {
                        Type = ChangeType.Feature,
                        Description = "API ASP.NET Core com PostgreSQL",
                        Author = "Sistema"
                    },
                    new ChangeLogItem
                    {
                        Type = ChangeType.Feature,
                        Description = "Sistema de autenticação com JWT",
                        Author = "Sistema"
                    },
                    new ChangeLogItem
                    {
                        Type = ChangeType.Feature,
                        Description = "PWA (Progressive Web App) com Service Worker",
                        Author = "Sistema"
                    },
                    new ChangeLogItem
                    {
                        Type = ChangeType.Feature,
                        Description = "Suporte a temas claro e escuro",
                        Author = "Sistema"
                    },
                    new ChangeLogItem
                    {
                        Type = ChangeType.Feature,
                        Description = "Gestão completa de usuários",
                        Author = "Sistema"
                    }
                }
            }
        };
    }
    
    /// <summary>
    /// Obtém as mudanças da versão mais recente
    /// </summary>
    public static VersionChanges? GetLatestChanges()
    {
        return GetChangeLog().FirstOrDefault();
    }
    
    /// <summary>
    /// Obtém mudanças de uma versão específica
    /// </summary>
    public static VersionChanges? GetChangesForVersion(string version)
    {
        return GetChangeLog().FirstOrDefault(v => v.Version == version);
    }
    
    /// <summary>
    /// Obtém apenas as versões lançadas (não em desenvolvimento)
    /// </summary>
    public static List<VersionChanges> GetReleasedVersions()
    {
        return GetChangeLog().Where(v => !v.IsUnreleased).ToList();
    }
    
    /// <summary>
    /// Obtém estatísticas do changelog
    /// </summary>
    public static ChangeLogStats GetStats()
    {
        var allChanges = GetChangeLog();
        var totalChanges = allChanges.SelectMany(v => v.Changes).ToList();
        
        return new ChangeLogStats
        {
            TotalVersions = allChanges.Count,
            TotalChanges = totalChanges.Count,
            ReleasedVersions = allChanges.Count(v => !v.IsUnreleased),
            Features = totalChanges.Count(c => c.Type == ChangeType.Feature),
            Fixes = totalChanges.Count(c => c.Type == ChangeType.Fix),
            Enhancements = totalChanges.Count(c => c.Type == ChangeType.Enhancement)
        };
    }
}

/// <summary>
/// Estatísticas do changelog
/// </summary>
public class ChangeLogStats
{
    public int TotalVersions { get; set; }
    public int TotalChanges { get; set; }
    public int ReleasedVersions { get; set; }
    public int Features { get; set; }
    public int Fixes { get; set; }
    public int Enhancements { get; set; }
}