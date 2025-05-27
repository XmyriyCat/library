using Library.Api.Extensions;
using Library.Application.Services.Contracts;
using Library.Contracts.Requests.UserBook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
public class UserBookController : ControllerBase
{
    private readonly IUserBookService _userBookService;
    private readonly ILogger<UserBookController> _logger;

    public UserBookController(IUserBookService userBookService, ILogger<UserBookController> logger)
    {
        _userBookService = userBookService;
        _logger = logger;
    }

    [Authorize]
    [HttpGet(ApiEndpoints.UserBook.GetAll)]
    public async Task<IActionResult> GetAllBorrowedBooks([FromQuery] BorrowedBooksRequest request,
        CancellationToken token)
    {
        var userId = HttpContext.GetUserId();

        if (userId is null)
        {
            _logger.LogWarning("GetAllBorrowedBooks called but user ID not found in context.");

            return BadRequest();
        }

        _logger.LogInformation("GetAllBorrowedBooks request for UserId={UserId} with  parameters: " +
                               "PageNumber={Page}, PageSize={PageSize}", userId, request.Page, request.PageSize);

        var response = await _userBookService.GetAllBorrowedBooksAsync(userId!.Value, request, token);

        _logger.LogInformation("Returned {Count} borrowed books for UserId={UserId}",
            response.TotalItems, userId);

        return Ok(response);
    }

    [Authorize]
    [HttpPost(ApiEndpoints.UserBook.Create)]
    public async Task<IActionResult> CreateBorrowedBook([FromBody] CreateBorrowedBookRequest request,
        CancellationToken token)
    {
        var userId = HttpContext.GetUserId();

        if (userId is null)
        {
            _logger.LogWarning("CreateBorrowedBook called but user ID not found in context.");

            return BadRequest();
        }

        _logger.LogInformation("CreateBorrowedBook request for UserId={UserId}, BookId={BookId}",
            userId, request.BookId);

        var response = await _userBookService.CreateBorrowedBookAsync(userId!.Value, request.BookId, token);

        _logger.LogInformation("Borrowed book created for UserId={UserId}, BookId={BookId}",
            userId, request.BookId);

        return Ok(response);
    }

    [Authorize]
    [HttpDelete(ApiEndpoints.UserBook.Delete)]
    public async Task<IActionResult> DeleteBorrowedBook([FromRoute] Guid bookId,
        CancellationToken token)
    {
        var userId = HttpContext.GetUserId();

        if (userId is null)
        {
            _logger.LogWarning("DeleteBorrowedBook called but user ID not found in context.");

            return BadRequest();
        }

        _logger.LogInformation("DeleteBorrowedBook request for UserId={UserId}, BookId={BookId}",
            userId, bookId);

        var isDeleted = await _userBookService.DeleteBorrowedBookAsync(userId!.Value, bookId, token);

        if (!isDeleted)
        {
            _logger.LogWarning("DeleteBorrowedBook failed: BookId={BookId} not found for UserId={UserId}",
                bookId, userId);

            return NotFound();
        }

        _logger.LogInformation("Deleted borrowed book BookId={BookId} for UserId={UserId}",
            bookId, userId);

        return Ok();
    }
}