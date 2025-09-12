namespace PesquisaX.Shared.Views;

public class RespostaPessoaView
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public string PesquisaId { get; set; } = string.Empty;
    public string NomePessoa { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DataAvaliacao { get; set; } 
    
    public List<RespostaView> Respostas { get; set; } = [];
    public Dictionary<string, double> PercentuaisGrupos { get; set; } = [];
    public double MediaGeral { get; set; }
};
