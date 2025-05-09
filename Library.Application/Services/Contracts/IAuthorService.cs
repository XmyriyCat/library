using System.Linq.Expressions;
using Library.Contracts.Requests;
using Library.Contracts.Requests.Author;
using Library.Contracts.Responses.Author;
using Library.Contracts.Responses.Book;
using Library.Data.Models;

namespace Library.Application.Services.Contracts;

public interface IAuthorService
{
    Task<AuthorResponse> CreateAsync(CreateAuthorRequest request, CancellationToken token = default);

    Task<AuthorResponse?> GetByIdAsync(Guid id, CancellationToken token = default);

    Task<AuthorsResponse> GetAllAsync(PagedRequest request, CancellationToken token = default);

    Task<AuthorResponse> UpdateAsync(Guid authorId, UpdateAuthorRequest request, CancellationToken token = default);

    Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);

    Task<int> CountAsync(CancellationToken token = default);

    Task<bool> AnyAsync(Expression<Func<Author, bool>> predicate, CancellationToken token = default);
    
    Task<IEnumerable<BookResponse>> GetAllBooksAsync(Guid authorId, CancellationToken token = default);
}