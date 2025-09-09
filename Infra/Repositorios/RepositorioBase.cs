using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PesquisaX.Core.Repositorios;
using PesquisaX.Infra.Contexto;

namespace PesquisaX.Infra.Repositorios;

public abstract class RepositorioBase<T> : IRepositorioBase<T> where T : class
{
    protected readonly PesquisaXContexto Db;
    protected readonly DbSet<T> _dbSet;

    protected RepositorioBase(PesquisaXContexto db)
    {
        Db = db;
        _dbSet = db.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> ObterTodosAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T?> ObterPorIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<T?> ObterPorAsync(Expression<Func<T, bool>> predicado)
    {
        return await _dbSet.FirstOrDefaultAsync(predicado);
    }

    public virtual async Task<IEnumerable<T>> ObterVariosAsync(Expression<Func<T, bool>> predicado)
    {
        return await _dbSet.Where(predicado).ToListAsync();
    }

    public virtual async Task<T> AdicionarAsync(T entidade)
    {
        await _dbSet.AddAsync(entidade);
        await Db.SaveChangesAsync();
        return entidade;
    }

    public virtual async Task AtualizarAsync(T entidade)
    {
        // Desacoplar todas as entidades rastreadas para evitar conflitos
        // _contexto.ChangeTracker.Clear();
        _dbSet.Update(entidade);
        await Db.SaveChangesAsync();
    }

    public virtual async Task ExcluirAsync(Guid id)
    {
        var entidade = await ObterPorIdAsync(id);
        if (entidade != null)
        {
            _dbSet.Remove(entidade);
            await Db.SaveChangesAsync();
        }
    }

    public virtual async Task<bool> ExisteAsync(Guid id)
    {
        return await _dbSet.FindAsync(id) != null;
    }

    public virtual async Task<int> ContarAsync()
    {
        return await _dbSet.CountAsync();
    }

    public virtual async Task<int> ContarAsync(Expression<Func<T, bool>> predicado)
    {
        return await _dbSet.CountAsync(predicado);
    }
}