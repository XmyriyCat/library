using Library.Application.Services.Contracts;
using Library.Contracts.Requests.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost(ApiEndpoints.Auth.Register)]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken token)
    {
        _logger.LogInformation("Registration attempt for user: {Email}", request.Email);

        var result = await _authService.RegisterAsync(request, token);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost(ApiEndpoints.Auth.Login)]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request, CancellationToken token)
    {
        _logger.LogInformation("Login attempt for user: {Email}", request.Email);

        var result = await _authService.LoginAsync(request, token);

        if (result is null)
        {
            _logger.LogWarning("Login failed for user: {Email}", request.Email);

            return Unauthorized("Login or Password is wrong.");
        }

        _logger.LogInformation("User logged in successfully: {Email}", request.Email);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost(ApiEndpoints.Auth.Refresh)]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken, CancellationToken token)
    {
        var result = await _authService.RefreshAsync(refreshToken, token);

        if (result is null)
        {
            _logger.LogWarning("Refresh token failed: invalid or expired. Token: {refreshToken}", refreshToken);

            return Unauthorized("The refresh token has expired or is invalid.");
        }

        _logger.LogInformation("Refresh token succeeded. New tokens issued. Token: {refreshToken}", refreshToken);

        return Ok(result);
    }
}