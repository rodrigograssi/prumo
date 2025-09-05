namespace PesquisaX.Shared.Modelos;

public class Questao
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Texto { get; set; } = string.Empty;
    public bool EInvertida { get; set; } = false;
    
    public int ProcessarResposta(int respostaBruta)
    {
        return EInvertida ? (10 - respostaBruta) : respostaBruta;
    }
}