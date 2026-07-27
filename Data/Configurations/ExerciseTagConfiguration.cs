using MentorOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorOS.Data.Configurations;

public class ExerciseTagConfiguration : IEntityTypeConfiguration<ExerciseTag>
{
    public void Configure(EntityTypeBuilder<ExerciseTag> builder)
    {
        builder.HasKey(et => new { et.ExerciseId, et.TagId });

        builder.HasOne(et => et.Exercise)
            .WithMany(e => e.ExerciseTags)
            .HasForeignKey(et => et.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(et => et.Tag)
            .WithMany()
            .HasForeignKey(et => et.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
