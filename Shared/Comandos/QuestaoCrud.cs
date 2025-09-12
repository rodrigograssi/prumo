namespace PesquisaX.Shared.Comandos;

public class QuestaoCrud
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public int GrupoDeQuestaoId { get; set; }
    public string Texto { get; set; } = string.Empty;
    public bool EInvertida { get; set; }
    public int Ordem { get; set; }
}