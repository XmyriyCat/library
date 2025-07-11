using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Library.Data.Models;

public class Role : IdentityRole<Guid>
{
    public override Guid Id { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}