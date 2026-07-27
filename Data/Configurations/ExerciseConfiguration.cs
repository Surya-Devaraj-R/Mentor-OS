using MentorOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorOS.Data.Configurations;

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.Property(e => e.Slug).HasMaxLength(80).IsRequired();
        builder.Property(e => e.Title).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Language).HasMaxLength(40);
        builder.Property(e => e.DifficultyLevel).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.ExerciseType).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(e => e.Slug).IsUnique();

        builder.HasOne(e => e.Lesson)
            .WithMany()
            .HasForeignKey(e => e.LessonId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
