using Library.Data.Models;
using Library.Data.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.Repositories.Implementations;

public class BookRepository : GenericRepository<Book>, IBookRepository
{
    public BookRepository(IdentityDataContext dataContext) : base(dataContext)
    {
    }

    public override async Task<Book?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        var result = await DataContext.Books
            .Include(x => x.Author)
            .Include(x => x.UserBook)
            .FirstOrDefaultAsync(x => x.Id == id, token);

        return result;
    }

    public override async Task<IEnumerable<Book>> GetAllAsync(CancellationToken token = default)
    {
        return await DataContext.Books
            .Include(x => x.Author)
            .Include(x => x.UserBook)
            .ToListAsync(token);
    }

    public async Task<Book?> GetByIsbnAsync(string isbn, CancellationToken token = default)
    {
        var result = await DataContext.Books
            .Include(x => x.Author)
            .Include(x => x.UserBook)
            .FirstOrDefaultAsync(x => x.Isbn.ToLower() == isbn.ToLower(), token);

        return result;
    }
}