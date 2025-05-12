using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Library.Application.Services.Contracts;
using Library.Data.Models;
using Library.Data.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Library.Application.Services.Implementations;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly UserManager<User> _userManager;
    private readonly IRepositoryWrapper _repositoryWrapper;

    public TokenService(IConfiguration config, UserManager<User> userManager, IRepositoryWrapper repositoryWrapper)
    {
        _config = config;
        _userManager = userManager;
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<string> GenerateAccessTokenAsync(User user, CancellationToken token = default)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);

        var claims = new List<Claim>
        {
            new Claim("sub", user.Id.ToString()),
            new Claim("name", user.UserName!),
        };

        claims.AddRange(userClaims);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenLifetime = int.Parse(_config["Jwt:Lifetime:AccessTokenMinutes"]!);

        var jwtToken = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(tokenLifetime),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }

    public string GenerateRefreshToken()
    {
        // Without dashes
        var guid = Guid.NewGuid().ToString("N");

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        return $"{guid}_{timestamp}";
    }

    public async Task<bool> IsValidToken(string refreshTokenValue, CancellationToken token = default)
    {
        var refreshToken = await _repositoryWrapper.RefreshTokens.GetTokenByValue(refreshTokenValue, token);

        if (refreshToken is null)
        {
            return false;
        }

        return DateTime.UtcNow < refreshToken.ExpirationDate;
    }

    public async Task<bool> VerifyUserTokenAsync(User user, string refreshTokenValue, string loginProvider,
        string tokenName,
        CancellationToken token = default)
    {
        var isValid = await _repositoryWrapper.RefreshTokens
            .VerifyUserTokenAsync(user, refreshTokenValue, loginProvider, tokenName, token);
        
        return isValid;
    }
}