using PesquisaX.Core.Modelos;
using PesquisaX.Shared.Views;

namespace PesquisaX.Core.Repositorios;

public interface IPesquisaRepositorio
{
    Task AlterarAsync(Pesquisa pesquisa);
    Task CriarAsync(Pesquisa comando);
    Task<Pesquisa?> SelecionaAsync(Guid id);
    Task<Pesquisa?> SelecionaAsync(int id);
    Task ExcluirAsync(Pesquisa pesquisa);
    Task ExcluirGrupoQuestoes(GrupoQuestao grupo);
    Task<GrupoQuestao?> SelecionaGrupoQuestaoAsync(Guid id);
    Task<GrupoQuestao?> SelecionaGrupoQuestaoAsync(int id);
    Task GravarGrupoDeQuestaoAsync(GrupoQuestao grupo);
    Task<Questao?> SelecionaQuestaoAsync(Guid parse);
    Task GravarQuestaoAsync(Questao questao);
    Task ExcluiQuestaoAsync(Questao questao);
    Task GravarRespostaPessoaAsync(RespostaPessoa respostaPessoa);
    Task<Pesquisa?> ObterPesquisaCompletaAsync(Guid id);
    Task<IEnumerable<Pesquisa>> ObterPesquisasComResultadosAsync();
    Task<RespostaPessoa> AdicionarResultadoAsync(RespostaPessoa resposta);
    Task<IEnumerable<RespostaPessoa>> ObterResultadosPesquisaAsync(int pesquisaId);
    Task<RespostaPessoa?> ObterResultadoPessoaAsync(Guid pesquisaId, Guid pessoaId);
    Task<List<Pesquisa>?> ObterTodasPequisaAsync();
    Task<List<PesquisaListaView>?> ObterPequisaListView();
    Task<int> ObtemPesquisaId(Guid pesquisaId);
    Task<List<GrupoQuestao>> ObtemGruposDePergunta(int pesquisaId);
    Task<List<MediaDeResultadosDosGruposView>> ObtemMediaDosResultadosPorGrupoView(Guid personId);
    Task<List<Questao>> ObterQuestoes(Guid pesquisaId);
    Task ExcluirResultadoAsync(RespostaPessoa resultado);
}