using Prumo.Shared.DTO.Requestes;
using Prumo.Shared.DTO.Responses;
using Prumo.Shared.Views;

namespace Prumo.Web.Services;

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