using Library.Api.Variables;
using Library.Application.Services.Contracts;
using Library.Contracts.Requests.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly ILogger<BooksController> _logger;

    public BooksController(IBookService bookService, ILogger<BooksController> logger)
    {
        _bookService = bookService;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Book.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] BooksRequest request, CancellationToken token)
    {
        _logger.LogInformation(
            "GetAll books request received with parameters: PageNumber={PageNumber}, PageSize={PageSize}, Title={Title}, Genre={Genre}, Author={Author}",
            request.Page, request.PageSize, request.Title, request.Genre, request.Author);

        var result = await _bookService.GetAllAsync(request, token);

        _logger.LogInformation("GetAll books request processed successfully. Returned {Count} items.",
            result.TotalItems);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Book.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrIsbn, CancellationToken token)
    {
        _logger.LogInformation("Received request to get book by id or ISBN: {IdOrIsbn}", idOrIsbn);

        var response = await _bookService.GetByIdOrIsbnAsync(idOrIsbn, token);

        if (response is null)
        {
            _logger.LogWarning("Book not found with id or ISBN: {IdOrIsbn}", idOrIsbn);

            return NotFound();
        }

        _logger.LogInformation("Book found and returned for id or ISBN: {IdOrIsbn}", idOrIsbn);

        return Ok(response);
    }

    [Authorize(AuthConstants.ManagerPolicyName)]
    [HttpPost(ApiEndpoints.Book.Create)]
    public async Task<IActionResult> Create([FromForm] CreateBookRequest request, CancellationToken token)
    {
        _logger.LogInformation("Create book request received: Title={Title}, AuthorId={AuthorId}",
            request.Title, request.AuthorId);

        var result = await _bookService.CreateAsync(request, token);

        _logger.LogInformation("Book created successfully with Id={BookId}", result.Id);

        return Ok(result);
    }

    [Authorize(AuthConstants.ManagerPolicyName)]
    [HttpPut(ApiEndpoints.Book.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromForm] UpdateBookRequest request,
        CancellationToken token)
    {
        _logger.LogInformation("Update book request received for Id={BookId}", id);

        var bookExists = await _bookService.AnyAsync(x => x.Id == id, token);

        if (bookExists is false)
        {
            _logger.LogWarning("Update failed: Book not found with Id={BookId}", id);

            return NotFound();
        }

        var response = await _bookService.UpdateAsync(id, request, token);

        _logger.LogInformation("Book updated successfully with Id={BookId}", id);

        return Ok(response);
    }

    [Authorize(AuthConstants.AdminPolicyName)]
    [HttpDelete(ApiEndpoints.Book.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        _logger.LogInformation("Delete book request received for Id={BookId}", id);
        
        var isDeleted = await _bookService.DeleteByIdAsync(id, token);

        if (!isDeleted)
        {
            _logger.LogWarning("Delete failed: Book not found with Id={BookId}", id);
            
            return NotFound();
        }

        _logger.LogInformation("Book deleted successfully with Id={BookId}", id);
        
        return Ok();
    }

    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Book.GetImage)]
    public async Task<IActionResult> GetImage([FromRoute] string idOrIsbn, CancellationToken token)
    {
        _logger.LogInformation("GetImage request received for book Id/ISBN: {IdOrIsbn}", idOrIsbn);
        
        var bookImage = await _bookService.GetBookImageByIdOrIsbnAsync(idOrIsbn, token);

        if (bookImage?.ImageBytes is null || bookImage?.MimeType is null)
        {
            _logger.LogWarning("GetImage failed: Image not found for Id/ISBN: {IdOrIsbn}", idOrIsbn);
            
            return NotFound();
        }
        
        _logger.LogInformation("GetImage succeeded for book Id/ISBN: {IdOrIsbn}", idOrIsbn);

        return File(bookImage.ImageBytes, bookImage.MimeType);
    }
}