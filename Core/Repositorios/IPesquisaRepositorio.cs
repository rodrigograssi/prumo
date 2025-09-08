using PesquisaX.Core.Modelos;
using PesquisaX.Shared.Views;

namespace PesquisaX.Core.Repositorios;

public interface IPesquisaRepositorio
{
    Task GravarAsync(Pesquisa pesquisa);
    Task<Pesquisa?> SelecionaAsync(Guid id);
    Task ExcluirAsync(Pesquisa pesquisa);
    Task ExcluirGrupoQuestoes(GrupoQuestao grupo);
    Task<GrupoQuestao?> SelecionaGrupoQuestaoAsync(Guid id);
    Task GravarGrupoDeQuestaoAsync(GrupoQuestao grupo);
    Task<Questao?> SelecionaQuestaoAsync(Guid parse);
    Task GravarQuestaoAsync(Questao questao);
    Task ExcluiQuestaoAsync(Questao questao);
    Task GravarRespostaPessoaAsync(RespostaPessoa respostaPessoa);
    Task<Pesquisa?> ObterPesquisaCompletaAsync(Guid id);
    Task<IEnumerable<Pesquisa>> ObterPesquisasComResultadosAsync();
    Task<RespostaPessoa> AdicionarResultadoAsync(RespostaPessoa resposta);
    Task<IEnumerable<RespostaPessoa>> ObterResultadosPesquisaAsync(Guid pesquisaId);
    Task<RespostaPessoa?> ObterResultadoPessoaAsync(Guid pesquisaId, Guid pessoaId);
    Task<List<Pesquisa>?> ObterTodasPequisaAsync();
    Task<List<PesquisaListaView>?> ObterPequisaListView();
}