using Library.Data.Models;
using Library.Data.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.Repositories.Implementations;

public class UserBookRepository : IUserBookRepository
{
    private readonly IdentityDataContext _dataContext;

    public UserBookRepository(IdentityDataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<UserBook?> GetAsync(Guid userId, Guid bookId, CancellationToken token = default)
    {
        var userBook = await _dataContext.UserBooks.FirstOrDefaultAsync(x =>
            x.UserId == userId &&
            x.BookId == bookId, token);

        return userBook;
    }

    public async Task<UserBook> CreateAsync(UserBook userBook, CancellationToken token = default)
    {
        await using var transaction = await _dataContext.Database.BeginTransactionAsync(token);

        var result = await _dataContext.UserBooks.AddAsync(userBook, token);

        await transaction.CommitAsync(token);

        await _dataContext.SaveChangesAsync(token);

        return result.Entity;
    }

    public async Task<bool> DeleteAsync(UserBook userBook, CancellationToken token = default)
    {
        await using var transaction = await _dataContext.Database.BeginTransactionAsync(token);

        var storedUserBook = await GetAsync(userBook.UserId, userBook.BookId, token);

        if (storedUserBook is null)
        {
            return false;
        }

        var result = _dataContext.UserBooks.Remove(storedUserBook);

        await transaction.CommitAsync(token);

        await _dataContext.SaveChangesAsync(token);

        return true;
    }
}