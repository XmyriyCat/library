using Library.Data.Models;

namespace Library.Data.Repositories.Contracts;

public interface IUserBookRepository
{
    Task<UserBook?> GetAsync(Guid userId, Guid bookId, CancellationToken token = default);

    Task<UserBook> CreateAsync(UserBook userBook, CancellationToken token = default);

    Task<bool> DeleteAsync(UserBook userBook, CancellationToken token = default);
}