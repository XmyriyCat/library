namespace Library.Contracts.Responses.User;

public class UserResponse : BaseDto<UserResponse, Data.Models.User>
{
    public Guid Id { get; set; }
    
    public string UserName { get; set; }

    public string Email { get; set; }
}