namespace PesquisaX.Shared.Version;

/// <summary>
/// Tipos de mudanças no changelog
/// </summary>
public enum ChangeType
{
    /// <summary>
    /// Nova funcionalidade
    /// </summary>
    Feature,
    
    /// <summary>
    /// Melhoria em funcionalidade existente
    /// </summary>
    Enhancement,
    
    /// <summary>
    /// Correção de bug
    /// </summary>
    Fix,
    
    /// <summary>
    /// Mudança que pode quebrar compatibilidade
    /// </summary>
    Breaking,
    
    /// <summary>
    /// Mudanças internas/técnicas
    /// </summary>
    Internal,
    
    /// <summary>
    /// Remoção de funcionalidade
    /// </summary>
    Removed,
    
    /// <summary>
    /// Alteração de segurança
    /// </summary>
    Security
}

/// <summary>
/// Item individual do changelog
/// </summary>
public class ChangeLogItem
{
    /// <summary>
    /// Tipo da mudança
    /// </summary>
    public ChangeType Type { get; set; }
    
    /// <summary>
    /// Descrição da mudança
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Issue ou ticket relacionado (opcional)
    /// </summary>
    public string? IssueNumber { get; set; }
    
    /// <summary>
    /// Autor da mudança
    /// </summary>
    public string? Author { get; set; }
}

/// <summary>
/// Versão com suas mudanças
/// </summary>
public class VersionChanges
{
    /// <summary>
    /// Número da versão
    /// </summary>
    public string Version { get; set; } = string.Empty;
    
    /// <summary>
    /// Data de release
    /// </summary>
    public DateTime ReleaseDate { get; set; }
    
    /// <summary>
    /// Lista de mudanças desta versão
    /// </summary>
    public List<ChangeLogItem> Changes { get; set; } = new();
    
    /// <summary>
    /// Indica se é uma versão não lançada (em desenvolvimento)
    /// </summary>
    public bool IsUnreleased { get; set; } = false;
}