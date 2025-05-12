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

    public UserBookController(IUserBookService userBookService)
    {
        _userBookService = userBookService;
    }

    [Authorize]
    [HttpGet(ApiEndpoints.UserBook.GetAll)]
    public async Task<IActionResult> GetAllBorrowedBooks([FromQuery] BorrowedBooksRequest request,
        CancellationToken token)
    {
        var userId = HttpContext.GetUserId();

        if (userId is null)
        {
            return BadRequest();
        }

        var response = await _userBookService.GetAllBorrowedBooksAsync(userId!.Value, request, token);

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
            return BadRequest();
        }

        var response = await _userBookService.CreateBorrowedBookAsync(userId!.Value, request.BookId, token);

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
            return BadRequest();
        }

        var isDeleted = await _userBookService.DeleteBorrowedBookAsync(userId!.Value, bookId, token);

        if (!isDeleted)
        {
            return NotFound();
        }

        return Ok();
    }
}