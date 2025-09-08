namespace PesquisaX.Shared.Comandos;

public class RespostaPessoaCrud
{
    public string PesquisaId { get; set; } = string.Empty;
    public string NomePessoa { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DataAvaliacao { get; set; } 
    public List<RespostaCrud> Respostas { get; set; } = [];
}