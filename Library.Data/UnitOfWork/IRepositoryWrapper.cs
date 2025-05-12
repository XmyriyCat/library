using Library.Data.Repositories.Contracts;

namespace Library.Data.UnitOfWork;

public interface IRepositoryWrapper
{
    public IBookRepository Books { get; }
    
    public IAuthorRepository Authors { get; }
    
    public IRefreshTokenRepository RefreshTokens { get; }
    
    public Task SaveChangesAsync(CancellationToken token = default);
}