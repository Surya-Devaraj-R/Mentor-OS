using MentorOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorOS.Data.Configurations;

public class BookmarkConfiguration : IEntityTypeConfiguration<Bookmark>
{
    public void Configure(EntityTypeBuilder<Bookmark> builder)
    {
        builder.Property(b => b.EntityKind).HasConversion<string>().HasMaxLength(40);
        builder.HasIndex(b => new { b.EntityKind, b.EntityId }).IsUnique();
    }
}
