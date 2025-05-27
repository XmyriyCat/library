using Library.Api.Variables;
using Library.Application.Services.Contracts;
using Library.Contracts.Requests;
using Library.Contracts.Requests.Author;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;
    private readonly ILogger<AuthorsController> _logger;

    public AuthorsController(IAuthorService authorService, ILogger<AuthorsController> logger)
    {
        _authorService = authorService;
        _logger = logger;
    }

    [Authorize(AuthConstants.ManagerPolicyName)]
    [HttpPost(ApiEndpoints.Author.Create)]
    public async Task<IActionResult> Create([FromBody] CreateAuthorRequest request, CancellationToken token)
    {
        var response = await _authorService.CreateAsync(request, token);

        _logger.LogInformation("Author created successfully. AuthorId: {AuthorId}, Name: {AuthorName}", response.Id,
            response.Name);

        return CreatedAtAction(nameof(Get), new { Id = response.Id }, response);
    }

    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Author.Get)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken token)
    {
        var response = await _authorService.GetByIdAsync(id, token);

        if (response is null)
        {
            _logger.LogInformation("Author not found. AuthorId: {AuthorId}", id);

            return NotFound();
        }

        _logger.LogInformation("Author retrieved successfully. AuthorId: {AuthorId}", id);

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Author.GetAll)]
    public async Task<IActionResult> GetAllAsync([FromQuery] PagedRequest request, CancellationToken token)
    {
        _logger.LogInformation("Fetching all authors with page {Page} and page size {PageSize}", request.Page,
            request.PageSize);

        var response = await _authorService.GetAllAsync(request, token);

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Author.GetBooks)]
    public async Task<IActionResult> GetAllBooks([FromRoute] Guid id, CancellationToken token)
    {
        _logger.LogInformation("Fetching books for author with ID: {AuthorId}", id);

        var response = await _authorService.GetAllBooksAsync(id, token);

        return Ok(response);
    }

    [Authorize(AuthConstants.ManagerPolicyName)]
    [HttpPut(ApiEndpoints.Author.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateAuthorRequest request,
        CancellationToken token)
    {
        _logger.LogInformation("Attempting to update author with ID: {AuthorId}", id);

        var authorExists = await _authorService.AnyAsync(x => x.Id == id, token);

        if (authorExists is false)
        {
            _logger.LogWarning("Author with ID: {AuthorId} not found. Update aborted.", id);

            return NotFound();
        }

        var response = await _authorService.UpdateAsync(id, request, token);

        return Ok(response);
    }

    [Authorize(AuthConstants.AdminPolicyName)]
    [HttpDelete(ApiEndpoints.Author.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        _logger.LogInformation("Attempting to delete author with ID: {AuthorId}", id);

        var isDeleted = await _authorService.DeleteByIdAsync(id, token);

        if (!isDeleted)
        {
            _logger.LogWarning("Author with ID: {AuthorId} not found. Deletion failed.", id);

            return NotFound();
        }

        _logger.LogInformation("Successfully deleted author with ID: {AuthorId}", id);

        return Ok();
    }
}