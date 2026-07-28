using MentorOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorOS.Data.Configurations;

public class LearningPathProjectConfiguration : IEntityTypeConfiguration<LearningPathProject>
{
    public void Configure(EntityTypeBuilder<LearningPathProject> builder)
    {
        builder.Property(p => p.Title).HasMaxLength(200).IsRequired();
        builder.Property(p => p.ArchitectureDiagramFormat).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(p => p.TopicId).IsUnique();

        builder.HasOne(p => p.Topic)
            .WithMany()
            .HasForeignKey(p => p.TopicId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
