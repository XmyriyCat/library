using Library.Application.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    // [HttpGet(ApiEndpoints.Book.GetAll)]
    // public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    // {
    //     var books = await _bookService.GetAllAsync(cancellationToken);
    //     
    //     return Ok(books);
    // }
}