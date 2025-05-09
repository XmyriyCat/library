using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.Repositories;

public abstract class GenericRepository<T> : IRepository<T> where T : class
{
    protected readonly IdentityDataContext DataContext;

    protected GenericRepository(IdentityDataContext dataContext)
    {
        DataContext = dataContext;
    }

    public virtual async Task<T> CreateAsync(T item, CancellationToken token = default)
    {
        await using var transaction = await DataContext.Database.BeginTransactionAsync(token);

        var result = await DataContext.Set<T>().AddAsync(item, token);

        await transaction.CommitAsync(token);

        return result.Entity;
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await DataContext.Set<T>().FindAsync([id], token);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken token = default)
    {
        return await DataContext.Set<T>().ToListAsync(token);
    }

    public virtual async Task<IEnumerable<T>> GetAllPaginationAsync(int page = 1, int pageSize = 10, 
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, CancellationToken token = default)
    {
        if (page <= 0)
        {
            throw new Exception($"The page should be greater than zero. Page = {page}");
        }

        if (pageSize <= 0)
        {
            throw new Exception($"The page size should be greater than zero. Page size = {pageSize}");
        }

        var itemsQuery = DataContext.Set<T>()
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        if (orderBy is not null)
        {
            orderBy(itemsQuery);
        }

        return await itemsQuery.ToListAsync(token);
    }

    public virtual async Task<T> UpdateAsync(T item, CancellationToken token = default)
    {
        await using var transaction = await DataContext.Database.BeginTransactionAsync(token);

        var result = DataContext.Set<T>().Update(item);

        await transaction.CommitAsync(token);

        return result.Entity;
    }

    public virtual async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        await using var transaction = await DataContext.Database.BeginTransactionAsync(token);

        var item = await GetByIdAsync(id, token);

        if (item is null)
        {
            return false;
        }
        
        var result = DataContext.Set<T>().Remove(item);

        await transaction.CommitAsync(token);

        return result.State is EntityState.Deleted;
    }

    public virtual async Task<int> CountAsync(CancellationToken token = default)
    {
        return await DataContext.Set<T>().CountAsync(token);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken token = default)
    {
        return await DataContext.Set<T>().AnyAsync(predicate, token);
    }
}