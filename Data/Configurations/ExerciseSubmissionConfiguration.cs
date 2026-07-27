using MentorOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorOS.Data.Configurations;

public class ExerciseSubmissionConfiguration : IEntityTypeConfiguration<ExerciseSubmission>
{
    public void Configure(EntityTypeBuilder<ExerciseSubmission> builder)
    {
        builder.Property(s => s.SelfAssessment).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(s => s.Exercise)
            .WithMany(e => e.Submissions)
            .HasForeignKey(s => s.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
