namespace PesquisaX.Shared.Views;

public class PesquisaListaView
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int TotalDeGrupos { get; set; }
    public int TotalDePerguntas { get; set; }
    public int TotalDeRespostas { get; set; }
    public DateTime DataCriacao { get; set; }
}