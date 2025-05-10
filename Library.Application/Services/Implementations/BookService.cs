using System.Linq.Expressions;
using FluentValidation;
using Library.Application.Exceptions;
using Library.Application.Services.Contracts;
using Library.Contracts.Models;
using Library.Contracts.Requests.Book;
using Library.Contracts.Responses.Book;
using Library.Data.Models;
using Library.Data.UnitOfWork;
using MapsterMapper;

namespace Library.Application.Services.Implementations;

public class BookService : IBookService
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IMapper _mapper;
    private readonly IValidator<Book> _bookValidator;
    private readonly IValidator<BooksRequest> _pagedRequestValidator;

    public BookService(IRepositoryWrapper repositoryWrapper, IMapper mapper,
        IValidator<Book> bookValidator, IValidator<BooksRequest> pagedRequestValidator)
    {
        _repositoryWrapper = repositoryWrapper;
        _mapper = mapper;
        _bookValidator = bookValidator;
        _pagedRequestValidator = pagedRequestValidator;
    }

    public async Task<BookResponse> CreateAsync(CreateBookRequest request, CancellationToken token = default)
    {
        var book = _mapper.Map<Book>(request);

        await _bookValidator.ValidateAndThrowAsync(book, token);

        var author = await _repositoryWrapper.Authors.GetByIdAsync(request.AuthorId, token);

        if (author is null)
        {
            throw new AuthorIdNotFoundException($"Author is not found. Author Id: '{request.AuthorId}'");
        }

        book.Author = author;

        var createdBook = await _repositoryWrapper.Books.CreateAsync(book, token);

        await _repositoryWrapper.SaveChangesAsync(token);

        var response = _mapper.Map<BookResponse>(createdBook);

        return response;
    }

    public async Task<BookResponse?> GetByIdOrIsbnAsync(string idOrIsbn, CancellationToken token = default)
    {
        var result = Guid.TryParse(idOrIsbn, out var id);

        return result ? await GetByIdAsync(id, token) : await GetByIsbnAsync(idOrIsbn, token);
    }

    public async Task<BookResponse?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        var result = await _repositoryWrapper.Books.GetByIdAsync(id, token);

        if (result is null)
        {
            return null;
        }

        var response = _mapper.Map<BookResponse>(result);

        return response;
    }

    public async Task<BookResponse?> GetByIsbnAsync(string isbn, CancellationToken token = default)
    {
        var result = await _repositoryWrapper.Books.GetByIsbnAsync(isbn, token);

        if (result is null)
        {
            return null;
        }

        var response = _mapper.Map<BookResponse>(result);

        return response;
    }

    public async Task<BooksResponse> GetAllAsync(BooksRequest request, CancellationToken token = default)
    {
        await _pagedRequestValidator.ValidateAndThrowAsync(request, token);

        var options = _mapper.Map<GetAllBooksOptions>(request);

        Func<IQueryable<Book>, IOrderedQueryable<Book>>? orderBy = ConfigureBooksOrdering(options);

        Expression<Func<Book, bool>>? filterPredication = ConfigureBooksFiltering(options);

        var result = await _repositoryWrapper.Books.GetAllPaginationAsync
        (
            options.Page,
            options.PageSize,
            orderBy,
            filterPredication,
            token
        );

        var response = _mapper.Map<BooksResponse>(result);

        response.Page = request.Page;
        response.PageSize = request.PageSize;
        response.TotalItems = await _repositoryWrapper.Books.CountAsync(token);

        return response;
    }

    private Func<IQueryable<Book>, IOrderedQueryable<Book>>? ConfigureBooksOrdering(GetAllBooksOptions options)
    {
        Func<IQueryable<Book>, IOrderedQueryable<Book>>? orderByGenre = book => book.OrderBy(b => b.Genre);
        Func<IQueryable<Book>, IOrderedQueryable<Book>>? orderByAuthor = book => book.OrderBy(b => b.Author);

        switch (options.SortField?.ToLower())
        {
            case "genre":
                return orderByGenre;
                break;
            case "author":
                return orderByAuthor;
                break;
        }

        return null;
    }

    private Expression<Func<Book, bool>>? ConfigureBooksFiltering(GetAllBooksOptions options)
    {
        // Expression<Func<Book, bool>> filterByGenre = book =>
        //     book.Genre.Equals(options.Genre, StringComparison.OrdinalIgnoreCase);
        //
        // Expression<Func<Book, bool>> filterByAuthor = book =>
        //     book.Author.Name.Equals(options.Author, StringComparison.OrdinalIgnoreCase);
        
        Expression<Func<Book, bool>> filterByGenre = book => book.Genre.ToLower() == options.Genre!.ToLower();
        
        Expression<Func<Book, bool>> filterByAuthor = book => book.Author.Name.ToLower() == options.Author!.ToLower();

        if (options.Genre != null)
        {
            return filterByGenre;
        }

        if (options.Author != null)
        {
            return filterByAuthor;
        }

        return null;
    }

    public Task<BookResponse> UpdateAsync(UpdateBookRequest request, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountAsync(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AnyAsync(Expression<Func<Book, bool>> predicate, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}