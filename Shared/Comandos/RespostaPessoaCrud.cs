namespace Prumo.Shared.Comandos;

public class RespostaPessoaCrud
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public int PesquisaId { get; set; }
    public string NomePessoa { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DataAvaliacao { get; set; } 
    public List<RespostaCrud> Respostas { get; set; } = [];
    public Dictionary<string, double> PercentuaisGrupos { get; set; } = [];
    public double MediaGeral { get; set; }
}