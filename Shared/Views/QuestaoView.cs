namespace PesquisaX.Shared.Views;

public class QuestaoView
{
    public string Id { get; set; } = string.Empty;
    public string GrupoDeQuestaoId { get; set; } = string.Empty;
    public string Texto { get; set; } = string.Empty;
    public bool EInvertida { get; set; }
    public int Ordem { get; set; }
}