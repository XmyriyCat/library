using Library.Contracts.Models;
using Library.Contracts.Requests.Auth;

namespace Library.Application.Services.Contracts;

public interface IAuthService
{
    public Task<AuthorizationResult> RegisterAsync(RegisterUserRequest request, CancellationToken token = default);
    
    public Task<AuthorizationResult?> LoginAsync(LoginUserRequest request, CancellationToken token = default);
    
    public Task<AuthorizationResult?> RefreshAsync(string refreshToken, CancellationToken token = default);
}