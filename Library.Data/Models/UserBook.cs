namespace Library.Data.Models;

public class UserBook
{
    public Guid UserId { get; set; }
    
    public User User { get; set; }
    
    public Guid BookId { get; set; }
    
    public Book Book { get; set; }
    
    public DateTime TakenDate { get; set; }
    
    public DateTime ReturnDate { get; set; }
}