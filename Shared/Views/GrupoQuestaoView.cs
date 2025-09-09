namespace PesquisaX.Shared.Views;

public class GrupoQuestaoView
{
    public string Id { get; set; }
    public string PesquisaId { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public string Cor { get; set; } = "#FF6B6B";
    public List<QuestaoView> Questoes { get; set; } = [];
}