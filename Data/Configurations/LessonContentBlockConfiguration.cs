using MentorOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorOS.Data.Configurations;

public class LessonContentBlockConfiguration : IEntityTypeConfiguration<LessonContentBlock>
{
    public void Configure(EntityTypeBuilder<LessonContentBlock> builder)
    {
        builder.Property(b => b.BlockType).HasConversion<string>().HasMaxLength(40);
        builder.Property(b => b.BodyFormat).HasConversion<string>().HasMaxLength(40);
        builder.Property(b => b.Title).HasMaxLength(200);
        builder.Property(b => b.Language).HasMaxLength(40);

        builder.HasOne(b => b.Lesson)
            .WithMany(l => l.ContentBlocks)
            .HasForeignKey(b => b.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
