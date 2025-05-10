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
    public async Task<IActionResult> Create([FromForm] CreateBookRequest request, CancellationToken token)
    {
        var result = await _bookService.CreateAsync(request, token);
        
        return Ok(result);
    }
    
    [HttpPut(ApiEndpoints.Book.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromForm] UpdateBookRequest request,
        CancellationToken token)
    {
        var bookExists = await _bookService.AnyAsync(x => x.Id == id, token);

        if (bookExists is false)
        {
            return NotFound();
        }

        var response = await _bookService.UpdateAsync(id, request, token);

        return Ok(response);
    }

    [HttpDelete(ApiEndpoints.Book.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        var isDeleted = await _bookService.DeleteByIdAsync(id, token);

        if (!isDeleted)
        {
            return NotFound();
        }

        return Ok();
    }
}