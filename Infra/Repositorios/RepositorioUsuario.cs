using Microsoft.EntityFrameworkCore;
using PesquisaX.Infra.Contexto;
using PesquisaX.Shared;

namespace PesquisaX.Infra.Repositorios;

public class RepositorioUsuario : RepositorioBase<Usuario>, IRepositorioUsuario
{
    public RepositorioUsuario(PesquisaXContexto contexto) : base(contexto)
    {
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        return await _contexto.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> EmailExisteAsync(string email)
    {
        return await _contexto.Usuarios
            .AnyAsync(u => u.Email == email);
    }

    public async Task<bool> ExistemUsuariosAsync()
    {
        return await _contexto.Usuarios.AnyAsync();
    }

    public async Task<int> ContarUsuariosAsync()
    {
        return await _contexto.Usuarios.CountAsync();
    }

    public async Task<List<Usuario>> ListarUsuariosAsync(int page = 1, int pageSize = 10)
    {
        return await _contexto.Usuarios
            .OrderBy(u => u.Nome)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Usuario?> ObterPorIdAsync(int id)
    {
        return await _contexto.Usuarios.FindAsync(id);
    }

    public async Task ExcluirAsync(int id)
    {
        var usuario = await ObterPorIdAsync(id);
        if (usuario != null)
        {
            _contexto.Usuarios.Remove(usuario);
            await _contexto.SaveChangesAsync();
        }
    }
}