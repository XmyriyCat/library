using System.Security.Claims;
using FluentValidation;
using Library.Application.Exceptions;
using Library.Application.Services.Contracts;
using Library.Contracts.Models;
using Library.Contracts.Requests.Auth;
using Library.Data.Models;
using Library.Data.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Library.Application.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _config;
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IValidator<RegisterUserRequest> _registerValidator;
    private readonly IValidator<LoginUserRequest> _loginValidator;

    public AuthService(UserManager<User> userManager, ITokenService tokenService, IConfiguration config,
        IRepositoryWrapper repositoryWrapper, IValidator<RegisterUserRequest> registerValidator, 
        IValidator<LoginUserRequest> loginValidator)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _config = config;
        _repositoryWrapper = repositoryWrapper;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    public async Task<AuthorizationResult> RegisterAsync(RegisterUserRequest request, CancellationToken token = default)
    {
        await _registerValidator.ValidateAndThrowAsync(request, token);

        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            throw new UserAlreadyExistsException("User with this email already exists.");
        }

        var user = new User
        {
            Email = request.Email,
            UserName = request.UserName
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);

        if (!createResult.Succeeded)
        {
            var errorDescriptions = string.Join("; ", createResult.Errors.Select(e => e.Description));

            throw new UserCreationException($"Failed to create user: {errorDescriptions}");
        }

        await _userManager.AddClaimAsync(user, new Claim(Data.Variables.Claims.Email, user.Email!));

        return await GenerateTokensAsync(user, null, token);
    }

    public async Task<AuthorizationResult?> LoginAsync(LoginUserRequest request, CancellationToken token = default)
    {
        await _loginValidator.ValidateAndThrowAsync(request, token);

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.Email, token);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return null;
        }

        return await GenerateTokensAsync(user, null, token);
    }

    public async Task<AuthorizationResult?> RefreshAsync(string refreshToken, CancellationToken token = default)
    {
        var isValidToken = await _tokenService.IsValidToken(refreshToken, token);

        if (!isValidToken)
        {
            throw new UnauthorizedAccessException("Refresh token is invalid.");
        }

        var user = await _repositoryWrapper.RefreshTokens.GetUserByRefreshTokenAsync(refreshToken, token);

        if (user is null)
        {
            throw new UnauthorizedAccessException("User is not found.");
        }

        var isValid = await _tokenService.VerifyUserTokenAsync(user, refreshToken, _config["Jwt:LoginProvider"]!,
            _config["Jwt:TokenName"]!, token);

        if (!isValid)
        {
            return null;
        }

        return await GenerateTokensAsync(user, refreshToken, token);
    }

    private async Task<AuthorizationResult> GenerateTokensAsync(User user, string? existingRefreshToken = null,
        CancellationToken token = default)
    {
        var accessToken = await _tokenService.GenerateAccessTokenAsync(user, token);

        var refreshTokenValue = _tokenService.GenerateRefreshToken();

        var refreshTokenLifetime = int.Parse(_config["Jwt:Lifetime:RefreshTokenHours"]!);

        var refreshTokenExpires = DateTime.UtcNow.AddHours(refreshTokenLifetime);

        await _repositoryWrapper.RefreshTokens.SetAuthenticationTokenAsync(user, _config["Jwt:LoginProvider"]!,
            _config["Jwt:TokenName"]!, refreshTokenValue, DateTime.Now, refreshTokenExpires, token);

        var authResult = new AuthorizationResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue
        };

        if (existingRefreshToken is not null)
        {
            authResult.RefreshToken = existingRefreshToken;
        }

        return authResult;
    }
}