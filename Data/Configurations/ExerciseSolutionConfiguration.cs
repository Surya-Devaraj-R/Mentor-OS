using MentorOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorOS.Data.Configurations;

public class ExerciseSolutionConfiguration : IEntityTypeConfiguration<ExerciseSolution>
{
    public void Configure(EntityTypeBuilder<ExerciseSolution> builder)
    {
        builder.Property(s => s.ApproachTitle).HasMaxLength(200).IsRequired();
        builder.Property(s => s.Language).HasMaxLength(40).IsRequired();
        builder.Property(s => s.TimeComplexity).HasMaxLength(40);
        builder.Property(s => s.SpaceComplexity).HasMaxLength(40);

        builder.HasOne(s => s.Exercise)
            .WithMany(e => e.Solutions)
            .HasForeignKey(s => s.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
