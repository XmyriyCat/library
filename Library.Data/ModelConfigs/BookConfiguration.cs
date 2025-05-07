using Library.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Data.ModelConfigs;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();
        
        builder.Property(x => x.Isbn)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.Genre)
            .IsRequired()
            .HasMaxLength(30);
        
        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(300);
        
        builder.HasIndex(x => x.Id)
            .IsUnique();
        
        builder.HasIndex(x => x.Isbn)
            .IsUnique();
    }
}