using PesquisaX.Shared.DTO.Requestes;
using PesquisaX.Shared.DTO.Responses;
using PesquisaX.Shared.Views;

namespace PesquisaX.Web.Services;

public interface IAuthService
{
    // Task<LoginResponse> LoginAsync(LoginRequest request);
    // Task<LoginResponse> RegistroAsync(RegistroRequest request);
    Task LogoutAsync();
    // Task<bool> EstaAutenticadoAsync();
    Task<UsuarioView?> ObterUsuarioAtualAsync();
    // string? ObterToken();
    // Task<bool> ExistemUsuariosAsync();
    // Task InicializarAsync();
}