using PesquisaX.Shared.Modelos;

namespace PesquisaX.Web.Servicos;

public interface IServicoPesquisa
{
    Task<List<Pesquisa>> ObterTodasPesquisasAsync();
    Task<Pesquisa?> ObterPesquisaPorIdAsync(Guid id);
    Task CriarPesquisaAsync(Pesquisa pesquisa);
    Task AlterarPesquisaAsync(Pesquisa pesquisa);
    Task ExcluirPesquisaAsync(Guid id);
    Task<ResultadoPessoa> SalvarResultadoPessoaAsync(Guid pesquisaId, ResultadoPessoa resultado);
    Task<List<ResultadoPessoa>> ObterResultadosPesquisaAsync(Guid pesquisaId);
    Task<ResultadoPessoa?> ObterResultadoPessoaAsync(Guid pesquisaId, Guid pessoaId);
    Task<string> ExportarDadosAsync();
    Task ImportarDadosAsync(string dadosJson);
}