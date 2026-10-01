namespace Prumo.Shared.Views;

public class QuestaoView
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public string GrupoDeQuestaoId { get; set; } = string.Empty;
    public string Texto { get; set; } = string.Empty;
    public bool EInvertida { get; set; }
    public int Ordem { get; set; }
}