using Microsoft.EntityFrameworkCore;
using Prumo.Core;
using Prumo.Core.Repositorios;
using Prumo.Infra.Contexto;

namespace Prumo.Infra.Repositorios;

public class RepositorioUsuario(PrumoContexto db) : RepositorioBase<Usuario>(db), IRepositorioUsuario
{
    public async Task GravarAsync(Usuario usuario)
    {
       
    }

    public async Task CriarAsync(Usuario usuario)
    {
        await Db.AddAsync(usuario);
        await Db.SaveChangesAsync();
    }

    public async Task AlterarAsync(Usuario usuario)
    {
        Db.Update(usuario);
        await Db.SaveChangesAsync();
    }

    public async Task<Usuario?> ObterPeloToken(string token)
    {
        var usuario = await Db.Usuarios.FirstOrDefaultAsync(u => u.Sistema.Token == token);
        return usuario;
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        return await Db.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> EmailExisteAsync(string email)
    {
        return await Db.Usuarios
            .AnyAsync(u => u.Email == email);
    }

    public async Task<bool> ExistemUsuariosAsync()
    {
        return await Db.Usuarios.AnyAsync();
    }

    public async Task<int> ContarUsuariosAsync()
    {
        return await Db.Usuarios.CountAsync();
    }

    public async Task<List<Usuario>> ListarUsuariosAsync(int page = 1, int pageSize = 10)
    {
        return await Db.Usuarios
            .OrderBy(u => u.Nome)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Usuario?> ObterPorIdAsync(int id)
    {
        return await Db.Usuarios.FindAsync(id);
    }

    public async Task ExcluirAsync(int id)
    {
        var usuario = await ObterPorIdAsync(id);
        if (usuario != null)
        {
            Db.Usuarios.Remove(usuario);
            await Db.SaveChangesAsync();
        }
    }
}