namespace Library.Contracts.Requests.Auth;

public class LoginUserRequest
{
    public string Email { get; set; }
    
    public string Password { get; set; }
}