using System.Linq.Expressions;
using Library.Application.Services.Contracts;
using Library.Data.Models;
using Library.Data.UnitOfWork;

namespace Library.Application.Services.Implementations;

public class AuthorService : IAuthorService
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    // TODO add validator

    public AuthorService(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<Author> CreateAsync(Author item, CancellationToken token = default)
    {
        return await _repositoryWrapper.Authors.CreateAsync(item, token);
    }

    public async Task<Author?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await _repositoryWrapper.Authors.GetByIdAsync(id, token);
    }

    public async Task<IEnumerable<Author>> GetAllAsync(CancellationToken token = default)
    {
        return await _repositoryWrapper.Authors.GetAllAsync(token);
    }

    public async Task<Author> UpdateAsync(Author item, CancellationToken token = default)
    {
        return await _repositoryWrapper.Authors.UpdateAsync(item, token);
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        return await _repositoryWrapper.Authors.DeleteByIdAsync(id, token);
    }

    public async Task<int> CountAsync(CancellationToken token = default)
    {
        return await _repositoryWrapper.Authors.CountAsync(token);
    }

    public async Task<bool> AnyAsync(Expression<Func<Author, bool>> predicate, CancellationToken token = default)
    {
        return await _repositoryWrapper.Authors.AnyAsync(predicate, token);
    }

    public async Task<IEnumerable<Book>> GetAllBooksAsync(Guid authorId, CancellationToken token = default)
    {
        return await _repositoryWrapper.Authors.GetAllBooksAsync(authorId, token);
    }
}