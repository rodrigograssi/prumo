namespace PesquisaX.Core.Modelos;

public class Resposta: Entidade
{
    public int ResultadoPessoaId { get; init; }
    public int QuestaoId { get; init; }
    public int ValorBruto { get; private set; }
    public int ValorProcessado { get; private set; }
    
    protected Resposta(){}

    public Resposta(int valorBruto, int valorProcessado)
    {
        ValorBruto = valorBruto;
        ValorProcessado = valorProcessado;
    }

    public Resposta(Questao questao, int valorBruto)
    {
        QuestaoId = questao.Id;
        ValorBruto = valorBruto;
        ValorProcessado = questao.ProcessarResposta(valorBruto);
    }
}