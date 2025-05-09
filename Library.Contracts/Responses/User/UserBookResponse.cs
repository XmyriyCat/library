namespace Library.Contracts.Responses.User;

public class UserBookResponse
{
    public UserResponse User { get; set; }
    
    public DateTime TakenDate { get; set; }
    
    public DateTime ReturnDate { get; set; }
}