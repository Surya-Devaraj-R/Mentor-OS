using MentorOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorOS.Data.Configurations;

public class QuizOptionConfiguration : IEntityTypeConfiguration<QuizOption>
{
    public void Configure(EntityTypeBuilder<QuizOption> builder)
    {
        builder.HasOne(o => o.QuizQuestion)
            .WithMany(q => q.Options)
            .HasForeignKey(o => o.QuizQuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
