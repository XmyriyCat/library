using Library.Data.Models;
using Library.Data.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.Repositories.Implementations;

public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
{
    public AuthorRepository(IdentityDataContext dataContext) : base(dataContext)
    {
    }

    public Task<IEnumerable<Book>> GetAllBooksAsync(Guid id, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
    
    public override async Task<Author?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        var result = await DataContext.Authors
            .Include(x => x.Books)
            .FirstOrDefaultAsync(x => x.Id == id, token);

        return result;
    }
}