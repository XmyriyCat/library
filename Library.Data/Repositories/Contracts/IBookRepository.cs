using Library.Data.Models;

namespace Library.Data.Repositories.Contracts;

public interface IBookRepository : IRepository<Book>
{
    Task<Book?> GetByIsbnAsync(string isbn, CancellationToken token = default);
}