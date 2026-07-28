using MentorOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorOS.Data.Configurations;

public class LessonObjectiveConfiguration : IEntityTypeConfiguration<LessonObjective>
{
    public void Configure(EntityTypeBuilder<LessonObjective> builder)
    {
        builder.Property(o => o.Text).HasMaxLength(300).IsRequired();

        builder.HasOne(o => o.Lesson)
            .WithMany(l => l.Objectives)
            .HasForeignKey(o => o.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
