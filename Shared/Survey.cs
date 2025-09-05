namespace PesquisaX.Shared.Modelos;

public class Pesquisa
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public List<GrupoQuestao> GruposQuestoes { get; set; } = new();
    public List<ResultadoPessoa> Resultados { get; set; } = new();
    
    public int ObterTotalQuestoes()
    {
        return GruposQuestoes.Sum(g => g.Questoes.Count);
    }
    
    public int ObterTotalAplicacoes()
    {
        return Resultados.Count;
    }
}