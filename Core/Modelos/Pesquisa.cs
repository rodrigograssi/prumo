using System.Transactions;

namespace PesquisaX.Core.Modelos;

public class Pesquisa: Entidade
{
    private List<GrupoQuestao> _gruposQuestoes = [];
    private List<RespostaPessoa> _respostas = [];
    public string Nome { get; private set; } = string.Empty;
    public DateTime DataCriacao { get; init; } = DateTime.UtcNow;
    public IReadOnlyCollection<GrupoQuestao> GruposQuestoes => _gruposQuestoes; 
    public List<RespostaPessoa> RespostasPessoa => _respostas;
    
    
    protected Pesquisa(){}

    public Pesquisa(string nome)
    {
        Nome = nome;
        DataCriacao = DateTime.UtcNow;
    }
    
    public int ObterTotalQuestoes()
    {
        return GruposQuestoes.Sum(g => g.Questoes.Count);
    }
    
    public int ObterTotalAplicacoes()
    {
        return RespostasPessoa.Count;
    }

    public void AdicionarGrupoQuestao(GrupoQuestao grupoQuestao)
    {
        ArgumentNullException.ThrowIfNull(grupoQuestao);
        if (_gruposQuestoes.All(g => g.Id != grupoQuestao.Id) || grupoQuestao.Id == 0)
        {
            grupoQuestao.DefinirOrdem(_gruposQuestoes.Count);
            _gruposQuestoes.Add(grupoQuestao);
        }
    }
    
    public void AdicionarRespostaPessoa(RespostaPessoa respostaPessoa)
    {
        ArgumentNullException.ThrowIfNull(respostaPessoa);
        if (_respostas.All(g => g.Id != respostaPessoa.Id))
        {
            _respostas.Add(respostaPessoa);
        }
    }

    public void Alterar(string nome)
    {
        Nome = nome;
    }

    public void RemoveGrupo(GrupoQuestao grupo)
    {
        var grupoExistente = GruposQuestoes.FirstOrDefault(g => g.Id == grupo.Id);
        if (grupoExistente != null)
            _gruposQuestoes.Remove(grupoExistente);
    }
}