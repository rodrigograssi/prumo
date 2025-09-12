using PesquisaX.Core.Modelos;
using PesquisaX.Shared.Comandos;
using PesquisaX.Shared.DTO;

namespace PesquisaX.Core.Interfaces;

public interface IPesquisaHandler
{
    Task<RetornoPadrao<Pesquisa>> Criar(PesquisaCrud pesquisa);
    Task<RetornoPadrao> Alterar(PesquisaCrud pesquisa);
    Task<RetornoPadrao> Excluir(Guid pesquisaId);
    Task<RetornoPadrao> CriarGrupoDeQuestao(GrupoQuestaoCrud comando);
    Task<RetornoPadrao> AlterarGrupoDeQuestao(GrupoQuestaoCrud comando);
    Task<RetornoPadrao> ExcluirGrupoDeQuestao(GrupoQuestaoCrud comando);
    Task<RetornoPadrao> CriarQuestao(QuestaoCrud comando);
    Task<RetornoPadrao> AlterarQuestao(QuestaoCrud comando);
    Task<RetornoPadrao> ExcluirQuestao(QuestaoCrud comando);
    Task<RetornoPadrao<RespostaPessoa>> CriarResposta(RespostaPessoaCrud comando);
}