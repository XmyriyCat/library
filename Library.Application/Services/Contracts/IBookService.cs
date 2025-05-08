using System.Linq.Expressions;
using Library.Data.Models;

namespace Library.Application.Services.Contracts;

public interface IBookService
{
    Task<Book> CreateAsync(Book item, CancellationToken token = default);

    Task<Book?> GetByIdAsync(Guid id, CancellationToken token = default);
    
    Task<Book?> GetByIsbnAsync(string isbn, CancellationToken token = default);

    Task<IEnumerable<Book>> GetAllAsync(CancellationToken token = default);

    Task<Book> UpdateAsync(Book item, CancellationToken token = default);

    Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);

    Task<int> CountAsync(CancellationToken token = default);

    Task<bool> AnyAsync(Expression<Func<Book, bool>> predicate, CancellationToken token = default);
}