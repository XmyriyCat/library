using System.Linq.Expressions;
using Library.Data.Models;

namespace Library.Application.Services.Contracts;

public interface IAuthorService
{
    Task<Author> CreateAsync(Author item, CancellationToken token = default);

    Task<Author?> GetByIdAsync(Guid id, CancellationToken token = default);

    Task<IEnumerable<Author>> GetAllAsync(CancellationToken token = default);

    Task<Author> UpdateAsync(Author item, CancellationToken token = default);

    Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);

    Task<int> CountAsync(CancellationToken token = default);

    Task<bool> AnyAsync(Expression<Func<Author, bool>> predicate, CancellationToken token = default);
    
    Task<IEnumerable<Book>> GetAllBooksAsync(Guid authorId, CancellationToken token = default);
}