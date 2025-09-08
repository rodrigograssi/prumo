using PesquisaX.Core.Modelos;
using PesquisaX.Shared.Views;

namespace PesquisaX.Web.Servicos;

public interface IServicoPesquisa
{
    Task<List<PesquisaListaView>> ObterTodasPesquisasAsync();
    Task<Pesquisa?> ObterPesquisaPorIdAsync(Guid id);
    Task CriarPesquisaAsync(Pesquisa pesquisa);
    Task AlterarPesquisaAsync(Pesquisa pesquisa);
    Task ExcluirPesquisaAsync(Guid id);
    Task<RespostaPessoa> SalvarResultadoPessoaAsync(Guid pesquisaId, RespostaPessoa resposta);
    Task<List<RespostaPessoa>> ObterResultadosPesquisaAsync(Guid pesquisaId);
    Task<RespostaPessoa?> ObterResultadoPessoaAsync(Guid pesquisaId, Guid pessoaId);
    Task<string> ExportarDadosAsync();
    Task ImportarDadosAsync(string dadosJson);
}