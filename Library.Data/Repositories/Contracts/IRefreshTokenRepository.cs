using Library.Data.Models;

namespace Library.Data.Repositories.Contracts;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    public Task<User?> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken token = default);

    public Task<RefreshToken?> GetTokenByValue(string refreshTokenValue, CancellationToken token = default);

    public Task SetAuthenticationTokenAsync(User user, string loginProvider, string tokenName, string tokenValue,
        DateTime creationDate, DateTime expiresDate, CancellationToken token = default);
    
    public Task<bool> VerifyUserTokenAsync(User user, string refreshTokenValue, string loginProvider, string tokenName,
        CancellationToken token = default);
}