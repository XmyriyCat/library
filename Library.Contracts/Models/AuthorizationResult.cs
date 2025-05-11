namespace Library.Contracts.Models;

public class AuthorizationResult
{
    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }
}