using Prumo.Shared.Comandos;
using Prumo.Shared.DTO.Requestes;
using Prumo.Shared.DTO.Responses;
using Prumo.Shared.Views;

namespace Prumo.Web.Services;

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