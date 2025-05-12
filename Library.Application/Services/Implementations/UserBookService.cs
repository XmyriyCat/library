using FluentValidation;
using Library.Application.Exceptions;
using Library.Application.Services.Contracts;
using Library.Contracts.Requests.UserBook;
using Library.Contracts.Responses.Book;
using Library.Contracts.Responses.User;
using Library.Data.Models;
using Library.Data.UnitOfWork;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Library.Application.Services.Implementations;

public class UserBookService : IUserBookService
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    private readonly IValidator<BorrowedBooksRequest> _userBookValidator;

    public UserBookService(IRepositoryWrapper repositoryWrapper, IMapper mapper, IConfiguration config,
        IValidator<BorrowedBooksRequest> userBookValidator, UserManager<User> userManager)
    {
        _repositoryWrapper = repositoryWrapper;
        _userManager = userManager;
        _config = config;
        _userBookValidator = userBookValidator;
        _mapper = mapper;
    }

    public async Task<BooksResponse> GetAllBorrowedBooksAsync(Guid userId, BorrowedBooksRequest request,
        CancellationToken token = default)
    {
        await _userBookValidator.ValidateAndThrowAsync(request, token);

        var result = await _repositoryWrapper.Books
            .GetAllBorrowedBooksAsync(userId, request.Page, request.PageSize, token);

        var response = _mapper.Map<BooksResponse>(result);

        response.Page = request.Page;
        response.PageSize = request.PageSize;
        response.TotalItems = await _repositoryWrapper.Books.CountBorrowedUserBooksAsync(userId, token);

        return response;
    }

    public async Task<UserBookResponse?> CreateBorrowedBookAsync(Guid userId, Guid bookId,
        CancellationToken token = default)
    {
        await CheckExistsAndThrowAsync(userId, bookId, token);

        var bookUsageHours = int.Parse(_config["BorrowTimeHours:Book"]!);

        var userBook = new UserBook()
        {
            UserId = userId,
            BookId = bookId,
            TakenDate = DateTime.UtcNow,
            ReturnDate = DateTime.UtcNow.AddHours(bookUsageHours)
        };

        await _repositoryWrapper.UserBooks.CreateAsync(userBook, token);

        var response = _mapper.Map<UserBookResponse>(userBook);

        return response;
    }

    public async Task<bool> DeleteBorrowedBookAsync(Guid userId, Guid bookId, CancellationToken token = default)
    {
        await CheckExistsAndThrowAsync(userId, bookId, token);

        var userBook = await _repositoryWrapper.UserBooks.GetAsync(userId, bookId, token);

        if (userBook is null)
        {
            return false;
        }

        var result = await _repositoryWrapper.UserBooks.DeleteAsync(userBook, token);

        return result;
    }

    private async Task CheckExistsAndThrowAsync(Guid userId, Guid bookId, CancellationToken token)
    {
        var userExists = await UserExistsAsync(userId, token);

        if (!userExists)
        {
            throw new UserNotFoundException("User is not found.");
        }

        var bookExists = await BookExistsAsync(bookId, token);

        if (!bookExists)
        {
            throw new BookNotFoundException("Book is not found.");
        }
    }

    private async Task<bool> UserExistsAsync(Guid userId, CancellationToken token = default)
    {
        var userExists = await _userManager.FindByIdAsync(userId.ToString());

        return userExists is not null;
    }

    private async Task<bool> BookExistsAsync(Guid bookId, CancellationToken token = default)
    {
        var bookExists = await _repositoryWrapper.Books.GetByIdAsync(bookId, token);

        return bookExists is not null;
    }
}