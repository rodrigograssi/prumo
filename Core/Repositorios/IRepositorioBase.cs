using System.Linq.Expressions;

namespace PesquisaX.Core.Repositorios;

public interface IRepositorioBase<T> where T : class
{
    Task<IEnumerable<T>> ObterTodosAsync();
    Task<T?> ObterPorIdAsync(Guid id);
    Task<T?> ObterPorAsync(Expression<Func<T, bool>> predicado);
    Task<IEnumerable<T>> ObterVariosAsync(Expression<Func<T, bool>> predicado);
    Task<T> AdicionarAsync(T entidade);
    Task AtualizarAsync(T entidade);
    Task ExcluirAsync(Guid id);
    Task<bool> ExisteAsync(Guid id);
    Task<int> ContarAsync();
    Task<int> ContarAsync(Expression<Func<T, bool>> predicado);
}