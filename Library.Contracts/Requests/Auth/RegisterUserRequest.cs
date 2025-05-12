namespace Library.Contracts.Requests.Auth;

public class RegisterUserRequest
{
    public string Email { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }
}