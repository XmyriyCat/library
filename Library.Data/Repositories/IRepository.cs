using System.Linq.Expressions;

namespace Library.Data.Repositories;

public interface IRepository<T> where T : class
{
    Task<T> CreateAsync(T item, CancellationToken token = default);

    Task<T?> GetByIdAsync(Guid id, CancellationToken token = default);

    Task<IEnumerable<T>> GetAllAsync(CancellationToken token = default);

    Task<IEnumerable<T>> GetAllPaginationAsync(int page = 1, int pageSize = 10,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, CancellationToken token = default);

    Task<T> UpdateAsync(T item, CancellationToken token = default);

    Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);

    Task<int> CountAsync(CancellationToken token = default);

    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken token = default);
}