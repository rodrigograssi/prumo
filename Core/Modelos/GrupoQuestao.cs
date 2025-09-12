namespace PesquisaX.Core.Modelos;

public class GrupoQuestao: Entidade
{
    private readonly List<Questao> _questoes = [];
    public int PesquisaId { get; init; }
    public string Nome { get; private set; } = string.Empty;
    public string Categoria { get; private set; } = string.Empty;
    public int Ordem { get; private set; }
    public string Cor { get; private set; } = "#FF6B6B";
    public IReadOnlyCollection<Questao> Questoes => _questoes;

    protected GrupoQuestao(){}

    public GrupoQuestao(string nome, string categoria, string cor)
    {
        Nome = nome;
        Categoria = categoria;
        Cor = cor;
    }
    
    public GrupoQuestao(int pesquisaId, string nome, string categoria, int ordem, string cor)
    {
        PesquisaId = pesquisaId;
        Nome = nome;
        Categoria = categoria;
        Ordem = ordem;
        Cor = cor;
    }

    public void AdicionarQuestao(Questao questao)
    {
        ArgumentNullException.ThrowIfNull(questao);
        if (_questoes.All(g => g.Id != questao.Id))
        {
            questao.DefinirOrdem(_questoes.Count);
            _questoes.Add(questao);
        }
    }

    public void DefinirOrdem(int ordem)
    {
        Ordem = ordem;  
    }
    
    public double ObterPesoQuestao()
    {
        return (Questoes.Count > 0 ? 1.0 / Questoes.Count : 0);
    }
    
    public double CalcularPercentual(List<int> respostasBrutas)
    {
        if (_questoes.Count == 0 || respostasBrutas.Count != _questoes.Count)
            return 0;
            
        var peso = ObterPesoQuestao();
        var respostasProcessadas = new List<int>();
        
        for (int i = 0; i < _questoes.Count; i++)
        {
            respostasProcessadas.Add(_questoes[i].ProcessarResposta(respostasBrutas[i]));
        }
        
        var total = respostasProcessadas.Sum() * peso * 10;
        return total;
    }

    public void Alterar(string nome, string categoria, string cor)
    {
        Nome = nome;
        Categoria = categoria;
        Cor = cor;   
    }

    public void RemoveQuestao(Questao questao)
    {
        var questaoExistente = Questoes.FirstOrDefault(g => g.Id == questao.Id);
        if (questaoExistente != null)
            _questoes.Remove(questaoExistente);   
    }
}