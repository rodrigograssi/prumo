namespace PesquisaX.Shared.Views;

public class PesquisaListaView
{
    public string Id { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public int TotalDeGrupos { get; set; }
    public int TotalDePerguntas { get; set; }
    public int TotalDeRespostas { get; set; }
    public DateTime DataCriacao { get; set; }
}