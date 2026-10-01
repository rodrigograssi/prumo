namespace Prumo.Shared.Views;

public class GrupoQuestaoView
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public string PesquisaId { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public string Cor { get; set; } = "#FF6B6B";
    public List<QuestaoView> Questoes { get; set; } = [];
}