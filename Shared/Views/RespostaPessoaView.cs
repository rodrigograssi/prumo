namespace PesquisaX.Shared.Views;

public class RespostaPessoaView
{
    public string Id { get; set; } = string.Empty;
    public string PesquisaId { get; set; } = string.Empty;
    public string NomePessoa { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DataAvaliacao { get; set; } 
    
    public List<RespostaView> Respostas { get; set; } = [];
    public Dictionary<string, double> PercentuaisGrupos { get; set; } = [];

    public double MediaGeral => PercentuaisGrupos.Values.Average();
}