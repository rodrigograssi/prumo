namespace PesquisaX.Shared.Modelos;

public class Resposta: Entidade
{
    public Guid ResultadoPessoaId { get; set; }
    public Guid QuestaoId { get; set; }
    public int ValorBruto { get; set; }
    public int ValorProcessado { get; set; }
}