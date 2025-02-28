using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Web_Library.Models;

namespace Web_Library.Data.Configurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.ISBN)
                .IsRequired()
                .HasMaxLength(13);

            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b => b.Genre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.Description)
                .IsRequired(false);

            builder.Property(b => b.Quantity)
                .HasDefaultValue(0);

            builder.Property(b => b.IsNotified)
                .HasDefaultValue(false);

            builder.HasOne<Genre>()
                .WithMany()
                .HasForeignKey(b => b.GenreID);
        }
    }
}
