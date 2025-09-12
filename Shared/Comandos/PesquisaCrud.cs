namespace PesquisaX.Shared.Comandos;

public class PesquisaCrud
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public List<GrupoQuestaoCrud> GruposQuestoes { get; set; } = [];
    public List<RespostaPessoaCrud> RespostasPessoa { get; set; } = [];
}