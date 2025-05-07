using Library.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Data.ModelConfigs;

public class UserBookConfiguration : IEntityTypeConfiguration<UserBook>
{
    public void Configure(EntityTypeBuilder<UserBook> builder)
    {
        builder.HasKey(userBook => new
        {
            userBook.UserId,
            userBook.BookId
        });
        
        builder.Property(x => x.BookId)
            .IsRequired();
        
        builder.Property(x => x.UserId)
            .IsRequired();
        
        builder.Property(x => x.TakenDate)
            .IsRequired();
        
        builder.Property(x => x.ReturnDate)
            .IsRequired();
    }
}