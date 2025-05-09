using System;

namespace Library.Data.Models;

public class UserBook
{
    public Guid UserId { get; set; }
    
    public virtual User User { get; set; }
    
    public Guid BookId { get; set; }
    
    public virtual Book Book { get; set; }
    
    public DateTime TakenDate { get; set; }
    
    public DateTime ReturnDate { get; set; }
}