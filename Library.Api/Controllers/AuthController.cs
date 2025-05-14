using Library.Application.Services.Contracts;
using Library.Contracts.Requests.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost(ApiEndpoints.Auth.Register)]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken token)
    {
        var result = await _authService.RegisterAsync(request, token);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost(ApiEndpoints.Auth.Login)]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request, CancellationToken token)
    {
        var result = await _authService.LoginAsync(request, token);

        if (result is null)
        {
            return Unauthorized("Login or Password is wrong.");
        }

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost(ApiEndpoints.Auth.Refresh)]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken, CancellationToken token)
    {
        var result = await _authService.RefreshAsync(refreshToken, token);

        if (result is null)
        {
            return Unauthorized("The refresh token has expired or is invalid.");
        }
        
        return Ok(result);
    }
}