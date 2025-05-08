using Library.Data.Repositories.Contracts;

namespace Library.Data.UnitOfWork;

public class RepositoryWrapper : IRepositoryWrapper
{
    private readonly IdentityDataContext _context;

    public RepositoryWrapper(IdentityDataContext context, IBookRepository books, IAuthorRepository authors)
    {
        _context = context;
        Books = books;
        Authors = authors;
    }

    public IBookRepository Books { get; }

    public IAuthorRepository Authors { get; }

    public async Task SaveChangesAsync(CancellationToken token = default)
    {
        await _context.SaveChangesAsync(token);
    }
}