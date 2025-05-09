using Library.Data.Repositories.Contracts;

namespace Library.Data.UnitOfWork;

public interface IRepositoryWrapper
{
    IBookRepository Books { get; }
    
    IAuthorRepository Authors { get; }
    
    Task SaveChangesAsync(CancellationToken token = default);
}