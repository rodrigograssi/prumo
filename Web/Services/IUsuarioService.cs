using PesquisaX.Core;
using PesquisaX.Shared.DTO.Requestes;
using PesquisaX.Shared.DTO.Responses;

namespace PesquisaX.Web.Services;

public interface IUsuarioService
{
    Task<List<Usuario>> ObterUsuariosAsync(int page = 1, int pageSize = 10);
    Task<Usuario?> ObterUsuarioAsync(int id);
    Task<Usuario> CriarUsuarioAsync(RegistroRequest request);
    Task<Usuario> AtualizarUsuarioAsync(int id, Usuario usuario);
    Task ExcluirUsuarioAsync(int id);
    Task ResetarSenhaAsync(int id, ResetPasswordRequest request);
    Task<int> ContarUsuariosAsync();
}