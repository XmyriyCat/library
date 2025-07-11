using System.Linq.Expressions;
using Library.Data.Exceptions;
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
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, token);

        return result;
    }

    public override async Task<IEnumerable<Book>> GetAllAsync(CancellationToken token = default)
    {
        return await DataContext.Books
            .Include(x => x.Author)
            .Include(x => x.UserBook)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync(token);
    }

    public async Task<Book?> GetByIsbnAsync(string isbn, CancellationToken token = default)
    {
        var result = await DataContext.Books
            .Include(x => x.Author)
            .Include(x => x.UserBook)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Isbn.ToLower() == isbn.ToLower(), token);

        return result;
    }

    public async Task<IEnumerable<Book>> GetAllPaginationAsync(int page = 1, int pageSize = 10,
        Expression<Func<Book, bool>>? filterPredication = null, CancellationToken token = default)
    {
        if (page <= 0)
        {
            throw new DbPageException($"The page should be greater than zero. Page = {page}");
        }

        if (pageSize <= 0)
        {
            throw new DbPageException($"The page size should be greater than zero. Page size = {pageSize}");
        }

        IQueryable<Book> itemsQuery;

        if (filterPredication is not null)
        {
            itemsQuery = DataContext.Books
                .Include(x => x.Author)
                .Include(x => x.UserBook)
                .ThenInclude(x => x.User)
                .Where(filterPredication)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }
        else
        {
            itemsQuery = DataContext.Books
                .Include(x => x.Author)
                .Include(x => x.UserBook)
                .ThenInclude(x => x.User)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        return await itemsQuery
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync(token);
    }

    public async Task<IEnumerable<Book>> GetAllBorrowedBooksAsync(Guid userId, int page = 1, int pageSize = 10,
        CancellationToken token = default)
    {
        return await DataContext.Books
            .Where(x => x.UserBook.UserId == userId)
            .Include(x => x.Author)
            .Include(x => x.UserBook)
            .ThenInclude(x => x.User)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync(token);
    }

    public async Task<int> CountBorrowedUserBooksAsync(Guid userId, CancellationToken token = default)
    {
        return await DataContext.Books
            .Where(x => x.UserBook.UserId == userId)
            .CountAsync(token);
    }

    public async Task<int> CountAsync(Expression<Func<Book, bool>>? filterPredication = null, CancellationToken token = default)
    {
        if (filterPredication is not null)
        {
            return await DataContext.Books
                .Where(filterPredication)
                .CountAsync(token);
        }

        return await CountAsync(token);
    }

    public override async Task<IEnumerable<Book>> GetAllPaginationAsync(int page = 1, int pageSize = 10,
        Func<IQueryable<Book>, IOrderedQueryable<Book>>? orderBy = null, CancellationToken token = default)
    {
        if (page <= 0)
        {
            throw new DbPageException($"The page should be greater than zero. Page = {page}");
        }

        if (pageSize <= 0)
        {
            throw new DbPageException($"The page size should be greater than zero. Page size = {pageSize}");
        }

        var itemsQuery = DataContext.Books
            .Include(x => x.Author)
            .Include(x => x.UserBook)
            .ThenInclude(x => x.User)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        if (orderBy is not null)
        {
            orderBy(itemsQuery);
        }

        return await itemsQuery
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync(token);
    }
}