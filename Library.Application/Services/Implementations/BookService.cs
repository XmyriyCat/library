using System.Linq.Expressions;
using Library.Application.Services.Contracts;
using Library.Data.Models;
using Library.Data.UnitOfWork;

namespace Library.Application.Services.Implementations;

public class BookService : IBookService
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    // TODO add validator

    public BookService(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Book> CreateAsync(Book item, CancellationToken token = default)
    {
        return await _repositoryWrapper.Books.CreateAsync(item, token);
    }

    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await _repositoryWrapper.Books.GetByIdAsync(id, token);
    }

    public async Task<Book?> GetByIsbnAsync(string isbn, CancellationToken token = default)
    {
        return await _repositoryWrapper.Books.GetByIsbnAsync(isbn, token);
    }

    public async Task<IEnumerable<Book>> GetAllAsync(CancellationToken token = default)
    {
        return await _repositoryWrapper.Books.GetAllAsync(token);
    }

    public async Task<Book> UpdateAsync(Book item, CancellationToken token = default)
    {
        return await _repositoryWrapper.Books.UpdateAsync(item, token);
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        return await _repositoryWrapper.Books.DeleteByIdAsync(id, token);
    }

    public async Task<int> CountAsync(CancellationToken token = default)
    {
        return await _repositoryWrapper.Books.CountAsync(token);
    }

    public async Task<bool> AnyAsync(Expression<Func<Book, bool>> predicate, CancellationToken token = default)
    {
        return await _repositoryWrapper.Books.AnyAsync(predicate, token);
    }
}