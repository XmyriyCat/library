using Library.Data.Models;

namespace Library.Data.Repositories.Contracts;

public interface IAuthorRepository : IRepository<Author>
{
    public Task<IEnumerable<Book>> GetAllBooksAsync(Guid id, CancellationToken token = default);
}