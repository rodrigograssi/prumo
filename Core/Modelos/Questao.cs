namespace Prumo.Core.Modelos;

public class Questao:Entidade
{
    public int GrupoDeQuestaoId { get; init; }
    public string Texto { get; private set; } = string.Empty;
    public bool EInvertida { get; private set; }
    public int Ordem { get; private set; }
    
    protected Questao(){}

    public Questao(string texto, bool eInvertida)
    {
        Texto = texto;
        EInvertida = eInvertida;
    }

    public void DefinirOrdem(int ordem)
    {
        Ordem = ordem;   
    }
    
    public int ProcessarResposta(int respostaBruta)
    {
        return EInvertida ? (10 - respostaBruta) : respostaBruta;
    }

    public void Alterar(string texto, bool eInvertida, int ordem)
    {
        Texto = texto;
        EInvertida = eInvertida;
        Ordem = ordem;  
    }
}