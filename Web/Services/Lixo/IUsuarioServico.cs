using PesquisaX.Shared.Comandos;
using PesquisaX.Shared.DTO.Requestes;
using PesquisaX.Shared.DTO.Responses;
using PesquisaX.Shared.Views;

namespace PesquisaX.Web.Services;

public interface IUsuarioServico
{
    Task<List<UsuarioView>> ObterUsuariosAsync(int page = 1, int pageSize = 10);
    Task<UsuarioView?> ObterUsuarioAsync(string id);
    Task CriarUsuarioAsync(UsuarioCrud request);
    Task AtualizarUsuarioAsync(string id, UsuarioCrud usuario);
    Task ExcluirUsuarioAsync(string id);
    Task ResetarSenhaAsync(string id, ResetPasswordRequest request);
    Task<int> ContarUsuariosAsync();
}