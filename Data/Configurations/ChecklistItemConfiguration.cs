using MentorOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorOS.Data.Configurations;

public class ChecklistItemConfiguration : IEntityTypeConfiguration<ChecklistItem>
{
    public void Configure(EntityTypeBuilder<ChecklistItem> builder)
    {
        builder.Property(c => c.OwnerKind).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.Description).HasMaxLength(500).IsRequired();
        builder.HasIndex(c => new { c.OwnerKind, c.OwnerId });
    }
}
