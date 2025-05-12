using Library.Data.Models;

namespace Library.Application.Services.Contracts;

public interface ITokenService
{
    public Task<string> GenerateAccessTokenAsync(User user, CancellationToken token = default);

    public string GenerateRefreshToken();

    public Task<bool> IsValidToken(string refreshTokenValue, CancellationToken token = default);

    public Task<bool> VerifyUserTokenAsync(User user, string refreshTokenValue, string loginProvider, string tokenName,
        CancellationToken token = default);
}