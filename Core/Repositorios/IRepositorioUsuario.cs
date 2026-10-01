using Prumo.Core;

namespace Prumo.Core.Repositorios;

public interface IRepositorioUsuario : IRepositorioBase<Usuario>
{
    Task CriarAsync(Usuario usuario);
    Task AlterarAsync(Usuario usuario);
    Task<Usuario?> ObterPeloToken(string token);
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<bool> EmailExisteAsync(string email);
    Task<bool> ExistemUsuariosAsync();
    Task<int> ContarUsuariosAsync();
    Task<List<Usuario>> ListarUsuariosAsync(int page = 1, int pageSize = 10);
    Task<Usuario?> ObterPorIdAsync(int id);
    Task ExcluirAsync(int id);
    
    
}