using System.Reflection;
using Library.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Library.Data;

public class IdentityDataContext : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, RefreshToken>
{
    public IdentityDataContext(DbContextOptions<IdentityDataContext> options) : base(options)
    {
        
    }
    
    public DbSet<Author> Authors { get; set; }
    
    public DbSet<Book> Books { get; set; }
    
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    public DbSet<UserBook> UserBooks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // When overriding OnModelCreating method, you should first call the base method implementation that the migration was created correctly
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}