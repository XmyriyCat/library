using Library.Contracts.Requests.UserBook;
using Library.Contracts.Responses.Book;
using Library.Contracts.Responses.User;

namespace Library.Application.Services.Contracts;

public interface IUserBookService
{
    public Task<BooksResponse> GetAllBorrowedBooksAsync(Guid userId, BorrowedBooksRequest request, CancellationToken token = default);
    
    public Task<UserBookResponse?> CreateBorrowedBookAsync(Guid userId, Guid bookId, CancellationToken token = default);
    
    public Task<bool> DeleteBorrowedBookAsync(Guid userId, Guid bookId, CancellationToken token = default);
}