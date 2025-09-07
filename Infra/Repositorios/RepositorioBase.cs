using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PesquisaX.Infra.Contexto;

namespace PesquisaX.Infra.Repositorios;

public abstract class RepositorioBase<T> : IRepositorioBase<T> where T : class
{
    protected readonly PesquisaXContexto _contexto;
    protected readonly DbSet<T> _dbSet;

    protected RepositorioBase(PesquisaXContexto contexto)
    {
        _contexto = contexto;
        _dbSet = contexto.Set<T>();
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
        await _contexto.SaveChangesAsync();
        return entidade;
    }

    public virtual async Task AtualizarAsync(T entidade)
    {
        // Desacoplar todas as entidades rastreadas para evitar conflitos
        // _contexto.ChangeTracker.Clear();
        _dbSet.Update(entidade);
        await _contexto.SaveChangesAsync();
    }

    public virtual async Task ExcluirAsync(Guid id)
    {
        var entidade = await ObterPorIdAsync(id);
        if (entidade != null)
        {
            _dbSet.Remove(entidade);
            await _contexto.SaveChangesAsync();
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