using MentorOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorOS.Data.Configurations;

public class CapstoneChecklistItemConfiguration : IEntityTypeConfiguration<CapstoneChecklistItem>
{
    public void Configure(EntityTypeBuilder<CapstoneChecklistItem> builder)
    {
        builder.Property(i => i.Description).HasMaxLength(500).IsRequired();

        builder.HasOne(i => i.CapstoneProject)
            .WithMany(c => c.ChecklistItems)
            .HasForeignKey(i => i.CapstoneProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
