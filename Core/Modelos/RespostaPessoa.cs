namespace Prumo.Core.Modelos;

public class RespostaPessoa : Entidade
{
    private List<Resposta> _respostas = [];
    public int PesquisaId { get; set; }
    public string NomePessoa { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DataAvaliacao { get; set; } = DateTime.UtcNow;
    public IReadOnlyCollection<Resposta> Respostas => _respostas;

    protected RespostaPessoa()
    {
    }

    public RespostaPessoa(int pesquisaId, string nomePessoa, string? email = null)
    {
        PesquisaId = pesquisaId;
        NomePessoa = nomePessoa;
        Email = email ?? string.Empty;
    }

    public RespostaPessoa(string nomePessoa, string? email = null)
    {
        NomePessoa = nomePessoa;
        Email = email ?? string.Empty;
    }

    public void AdicionarResposta(Resposta resposta)
    {
        ArgumentNullException.ThrowIfNull(resposta);
        if (_respostas.All(g => g.Id != resposta.Id) || resposta.Id == 0)
        {
            _respostas.Add(resposta);
        }
    }

    public Dictionary<string, double> ObterPercentuaisGrupos(List<GrupoQuestao> grupos)
    {
        var result = new Dictionary<string, double>();

        foreach (var grupo in grupos)
        {
            var respostasGrupo = new List<int>();

            foreach (var questao in grupo.Questoes)
            {
                var resposta = Respostas.FirstOrDefault(r => r.QuestaoId == questao.Id);
                respostasGrupo.Add(resposta?.ValorBruto ?? 0);
            }

            result[grupo.Categoria] = grupo.CalcularPercentual(respostasGrupo);
        }

        return result;
    }
}