using PesquisaX.Shared.Comandos;
using PesquisaX.Shared.Views;

namespace PesquisaX.Web.Servicos;

public interface IServicoPesquisa
{
    Task<List<PesquisaListaView>> ObterTodasPesquisasAsync();
    Task<PesquisaView?> ObterPesquisaPorIdAsync(Guid id);
    Task CriarPesquisaAsync(PesquisaCrud pesquisa);
    Task AlterarPesquisaAsync(PesquisaCrud pesquisa);
    Task ExcluirPesquisaAsync(Guid id);
    Task<RespostaPessoaView> SalvarResultadoPessoaAsync(Guid pesquisaId, RespostaPessoaView resposta);
    Task<List<RespostaPessoaView>> ObterResultadosPesquisaAsync(Guid pesquisaId);
    Task<RespostaPessoaView?> ObterResultadoPessoaAsync(Guid pesquisaId, Guid pessoaId);
    Task<string> ExportarDadosAsync();
    Task ImportarDadosAsync(string dadosJson);
}