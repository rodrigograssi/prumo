namespace PesquisaX.Shared.Modelos;

public class GrupoQuestao: Entidade
{
    public string Nome { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string Cor { get; set; } = "#FF6B6B";
    public List<Questao> Questoes { get; set; } = [];
    
    public Guid PesquisaId { get; set; }
    
    public double ObterPesoQuestao()
    {
        return Questoes.Count > 0 ? 1.0 / Questoes.Count : 0;
    }
    
    public double CalcularPercentual(List<int> respostasBrutas)
    {
        if (Questoes.Count == 0 || respostasBrutas.Count != Questoes.Count)
            return 0;
            
        var peso = ObterPesoQuestao();
        var respostasProcessadas = new List<int>();
        
        for (int i = 0; i < Questoes.Count; i++)
        {
            respostasProcessadas.Add(Questoes[i].ProcessarResposta(respostasBrutas[i]));
        }
        
        var total = respostasProcessadas.Sum() * peso * 10;
        return total;
    }
}