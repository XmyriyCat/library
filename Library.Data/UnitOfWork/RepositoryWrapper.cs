using Library.Data.Repositories.Contracts;

namespace Library.Data.UnitOfWork;

public class RepositoryWrapper : IRepositoryWrapper
{
    private readonly IdentityDataContext _context;

    public RepositoryWrapper(IdentityDataContext context, IBookRepository books, IAuthorRepository authors,
        IRefreshTokenRepository refreshTokens, IUserBookRepository userBooks)
    {
        _context = context;
        Books = books;
        Authors = authors;
        RefreshTokens = refreshTokens;
        UserBooks = userBooks;
    }

    public IBookRepository Books { get; }

    public IAuthorRepository Authors { get; }

    public IRefreshTokenRepository RefreshTokens { get; }

    public IUserBookRepository UserBooks { get; }

    public async Task SaveChangesAsync(CancellationToken token = default)
    {
        await _context.SaveChangesAsync(token);
    }
}