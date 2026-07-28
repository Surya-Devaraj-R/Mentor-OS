using MentorOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorOS.Data.Configurations;

public class ExerciseHintConfiguration : IEntityTypeConfiguration<ExerciseHint>
{
    public void Configure(EntityTypeBuilder<ExerciseHint> builder)
    {
        builder.HasOne(h => h.Exercise)
            .WithMany(e => e.Hints)
            .HasForeignKey(h => h.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
