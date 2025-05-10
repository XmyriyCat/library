using Library.Application.Services.Contracts;
using Library.Contracts.Requests.Book;
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

    [HttpGet(ApiEndpoints.Book.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] BooksRequest request, CancellationToken cancellationToken)
    {
        var result = await _bookService.GetAllAsync(request, cancellationToken);
        
        return Ok(result);
    }

    [HttpGet(ApiEndpoints.Book.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrIsbn, CancellationToken token)
    {
        var response = await _bookService.GetByIdOrIsbnAsync(idOrIsbn, token);

        if (response is null)
        {
            return NotFound();
        }

        return Ok(response);
    }
    
    [HttpPost(ApiEndpoints.Book.Create)]
    public async Task<IActionResult> Create([FromBody] CreateBookRequest request, CancellationToken token)
    {
        var result = await _bookService.CreateAsync(request, token);
        
        return Ok(result);
    }
}