namespace PesquisaX.Shared.Views;

public class PesquisaView
{
    public string Id { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public List<GrupoQuestaoView> GruposQuestoes { get; set; } = [];
    public List<RespostaPessoaView> RespostasPessoa { get; set; } = [];
}