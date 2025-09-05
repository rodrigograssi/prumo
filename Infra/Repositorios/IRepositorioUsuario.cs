using PesquisaX.Shared;

namespace PesquisaX.Infra.Repositorios;

public interface IRepositorioUsuario : IRepositorioBase<Usuario>
{
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<bool> EmailExisteAsync(string email);
    Task<bool> ExistemUsuariosAsync();
    Task<int> ContarUsuariosAsync();
    Task<List<Usuario>> ListarUsuariosAsync(int page = 1, int pageSize = 10);
    Task<Usuario?> ObterPorIdAsync(int id);
    Task ExcluirAsync(int id);
}