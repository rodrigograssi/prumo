namespace PesquisaX.Shared.Views;

public class PesquisaView
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string NomeResultadoDesejado { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public List<GrupoQuestaoView> GruposQuestoes { get; set; } = [];
    public List<RespostaPessoaView> RespostasPessoa { get; set; } = [];
}