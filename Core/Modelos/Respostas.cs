namespace PesquisaX.Core.Modelos;

public class Resposta: Entidade
{
    public Guid ResultadoPessoaId { get; init; }
    public Guid QuestaoId { get; init; }
    public int ValorBruto { get; private set; }
    public int ValorProcessado { get; private set; }
    
    protected Resposta(){}

    public Resposta(int valorBruto, int valorProcessado)
    {
        ValorBruto = valorBruto;
        ValorProcessado = valorProcessado;
    }

    public Resposta(Guid questaoId, int valorBruto, int valorProcessado)
    {
        QuestaoId = questaoId;
        ValorBruto = valorBruto;
        ValorProcessado = valorProcessado;
    }
}