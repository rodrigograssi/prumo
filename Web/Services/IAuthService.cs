using PesquisaX.Shared;

namespace PesquisaX.Web.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> RegistroAsync(RegistroRequest request);
    Task LogoutAsync();
    Task<bool> EstaAutenticadoAsync();
    Task<Usuario?> ObterUsuarioAtualAsync();
    string? ObterToken();
    Task<bool> ExistemUsuariosAsync();
    Task InicializarAsync();
}