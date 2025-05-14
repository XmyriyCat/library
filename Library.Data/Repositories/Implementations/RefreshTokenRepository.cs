using Library.Data.Models;
using Library.Data.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.Repositories.Implementations;

public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(IdentityDataContext dataContext) : base(dataContext)
    {
    }

    public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken, CancellationToken token = default)
    {
        var storedRefreshToken = await DataContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.Value == refreshToken, token);

        if (storedRefreshToken is null)
        {
            return null;
        }

        var user = await DataContext.Users.FirstOrDefaultAsync(x => x.Id == storedRefreshToken.UserId, token);

        return user;
    }

    public async Task<RefreshToken?> GetTokenByValue(string refreshTokenValue, CancellationToken token = default)
    {
        return await DataContext.RefreshTokens.FirstOrDefaultAsync(x => x.Value == refreshTokenValue, token);
    }

    public async Task SetAuthenticationTokenAsync(User user, string loginProvider, string tokenName, string tokenValue,
        DateTime creationDate, DateTime expiresDate, CancellationToken token = default)
    {
        var storedRefreshToken = await DataContext.RefreshTokens
            .FirstOrDefaultAsync(x =>
                x.UserId == user.Id &&
                x.LoginProvider == loginProvider &&
                x.Name == tokenName, cancellationToken: token);

        if (storedRefreshToken is not null)
        {
            DataContext.RefreshTokens.Remove(storedRefreshToken);
        }

        var refreshToken = new RefreshToken()
        {
            UserId = user.Id,
            LoginProvider = loginProvider,
            Name = tokenName,
            CreationDate = creationDate,
            ExpirationDate = expiresDate,
            Value = tokenValue
        };

        await DataContext.RefreshTokens.AddAsync(refreshToken, token);

        await DataContext.SaveChangesAsync(token);
    }

    public async Task<bool> VerifyUserTokenAsync(User user, string refreshTokenValue, string loginProvider,
        string tokenName, CancellationToken token = default)
    {
        var storedRefreshToken = await DataContext.RefreshTokens
            .FirstOrDefaultAsync(x =>
                x.UserId == user.Id &&
                x.LoginProvider == loginProvider &&
                x.Name == tokenName, cancellationToken: token);

        return storedRefreshToken is not null;
    }
}