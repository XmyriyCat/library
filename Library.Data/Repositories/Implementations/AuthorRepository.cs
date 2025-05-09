using Library.Data.Models;
using Library.Data.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.Repositories.Implementations;

public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
{
    public AuthorRepository(IdentityDataContext dataContext) : base(dataContext)
    {
    }

    public async Task<IEnumerable<Book>> GetAllBooksAsync(Guid authorId, CancellationToken token = default)
    {
        return await DataContext.Books
            .Include(x => x.Author)
            .Include(x => x.UserBooks)
            .Where(x => x.Author.Id == authorId)
            .ToListAsync(token);
    }
}