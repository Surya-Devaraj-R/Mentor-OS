using MentorOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorOS.Data.Configurations;

public class LessonReferenceLinkConfiguration : IEntityTypeConfiguration<LessonReferenceLink>
{
    public void Configure(EntityTypeBuilder<LessonReferenceLink> builder)
    {
        builder.Property(r => r.Title).HasMaxLength(200).IsRequired();
        builder.Property(r => r.Url).HasMaxLength(500).IsRequired();
        builder.Property(r => r.LinkType).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(r => r.Lesson)
            .WithMany(l => l.ReferenceLinks)
            .HasForeignKey(r => r.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
