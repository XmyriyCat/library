using Microsoft.AspNetCore.Identity;

namespace Library.Data.Models;

public class User : IdentityUser<Guid>
{
    public override Guid Id { get; set; }
    
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
    
    public virtual ICollection<UserBook> UserBooks { get; set; } = new List<UserBook>();
}