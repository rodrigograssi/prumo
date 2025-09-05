using PesquisaX.Shared.Modelos;

namespace PesquisaX.Infra.Repositorios;

public interface IRepositorioPesquisa : IRepositorioBase<Pesquisa>
{
    Task<Pesquisa?> ObterPesquisaCompletaAsync(Guid id);
    Task<IEnumerable<Pesquisa>> ObterPesquisasComResultadosAsync();
    Task<ResultadoPessoa> AdicionarResultadoAsync(Guid pesquisaId, ResultadoPessoa resultado);
    Task<IEnumerable<ResultadoPessoa>> ObterResultadosPesquisaAsync(Guid pesquisaId);
    Task<ResultadoPessoa?> ObterResultadoPessoaAsync(Guid pesquisaId, Guid pessoaId);
}