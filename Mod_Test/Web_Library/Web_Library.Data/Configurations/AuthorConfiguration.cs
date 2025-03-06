using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Web_Library.Models;

namespace Web_Library.Data.Configurations
{
    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.BirthDate)
                .IsRequired();

            builder.Property(a => a.Country)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasMany(a => a.Books)
                .WithOne()
                .HasForeignKey(b => b.AuthorID)
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
