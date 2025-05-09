using System.Linq.Expressions;
using Library.Data.Models;

namespace Library.Data.Repositories.Contracts;

public interface IBookRepository : IRepository<Book>
{
    public Task<Book?> GetByIsbnAsync(string isbn, CancellationToken token = default);

     public Task<IEnumerable<Book>> GetAllPaginationAsync(int page = 1, int pageSize = 10,
         Func<IQueryable<Book>, IOrderedQueryable<Book>>? orderBy = null,
         Expression<Func<Book, bool>>? filterPredication = null,
         CancellationToken token = default);
}