using MentorOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorOS.Data.Configurations;

public class LessonPrerequisiteConfiguration : IEntityTypeConfiguration<LessonPrerequisite>
{
    public void Configure(EntityTypeBuilder<LessonPrerequisite> builder)
    {
        builder.HasOne(p => p.Lesson)
            .WithMany(l => l.Prerequisites)
            .HasForeignKey(p => p.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        // Restrict, not Cascade: SQLite/EF Core rejects multiple cascade
        // paths through the same table (Lesson -> LessonPrerequisite twice).
        builder.HasOne(p => p.PrerequisiteLesson)
            .WithMany()
            .HasForeignKey(p => p.PrerequisiteLessonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
