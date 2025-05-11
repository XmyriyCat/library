using System.Linq.Expressions;
using Library.Contracts.Models;
using Library.Contracts.Requests.Book;
using Library.Contracts.Responses.Book;
using Library.Data.Models;

namespace Library.Application.Services.Contracts;

public interface IBookService
{
    Task<BookResponse> CreateAsync(CreateBookRequest request, CancellationToken token = default);

    Task<BookResponse?> GetByIdOrIsbnAsync(string idOrIsbn, CancellationToken token = default);

    Task<BookResponse?> GetByIdAsync(Guid id, CancellationToken token = default);

    Task<BookResponse?> GetByIsbnAsync(string isbn, CancellationToken token = default);

    Task<BooksResponse> GetAllAsync(BooksRequest request, CancellationToken token = default);

    Task<BookResponse> UpdateAsync(Guid bookId, UpdateBookRequest request, CancellationToken token = default);

    Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);

    Task<int> CountAsync(CancellationToken token = default);

    Task<bool> AnyAsync(Expression<Func<Book, bool>> predicate, CancellationToken token = default);

    Task<ImageResult?> GetBookImageByIdOrIsbnAsync(string idOrIsbn, CancellationToken token = default);
}