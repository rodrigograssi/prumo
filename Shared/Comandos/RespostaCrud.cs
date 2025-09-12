namespace PesquisaX.Shared.Comandos;

public class RespostaCrud
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public int ResultadoPessoaId { get; set; }
    public int QuestaoId { get; set; } 
    public int ValorBruto { get; set; }
    public int ValorProcessado { get; set; }
}