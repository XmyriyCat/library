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

    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    [Authorize(AuthConstants.ManagerPolicyName)]
    [HttpPost(ApiEndpoints.Author.Create)]
    public async Task<IActionResult> Create([FromBody] CreateAuthorRequest request, CancellationToken token)
    {
        var response = await _authorService.CreateAsync(request, token);

        return CreatedAtAction(nameof(Get), new { Id = response.Id }, response);
    }

    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Author.Get)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken token)
    {
        var response = await _authorService.GetByIdAsync(id, token);

        return response is null ? NotFound() : Ok(response);
    }

    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Author.GetAll)]
    public async Task<IActionResult> GetAllAsync([FromQuery] PagedRequest request, CancellationToken token)
    {
        var response = await _authorService.GetAllAsync(request, token);

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Author.GetBooks)]
    public async Task<IActionResult> GetAllBooks([FromRoute] Guid id, CancellationToken token)
    {
        var response = await _authorService.GetAllBooksAsync(id, token);
        
        return Ok(response);
    }

    [Authorize(AuthConstants.ManagerPolicyName)]
    [HttpPut(ApiEndpoints.Author.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateAuthorRequest request,
        CancellationToken token)
    {
        var authorExists = await _authorService.AnyAsync(x => x.Id == id, token);

        if (authorExists is false)
        {
            return NotFound();
        }

        var response = await _authorService.UpdateAsync(id, request, token);

        return Ok(response);
    }

    [Authorize(AuthConstants.AdminPolicyName)]
    [HttpDelete(ApiEndpoints.Author.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        var isDeleted = await _authorService.DeleteByIdAsync(id, token);

        if (!isDeleted)
        {
            return NotFound();
        }

        return Ok();
    }
}