using MentorOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorOS.Data.Configurations;

public class CompletionRecordConfiguration : IEntityTypeConfiguration<CompletionRecord>
{
    public void Configure(EntityTypeBuilder<CompletionRecord> builder)
    {
        builder.Property(c => c.EntityKind).HasConversion<string>().HasMaxLength(40);
        builder.HasIndex(c => new { c.EntityKind, c.EntityId }).IsUnique();
    }
}
