using MentorOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorOS.Data.Configurations;

public class InterviewQuestionTagConfiguration : IEntityTypeConfiguration<InterviewQuestionTag>
{
    public void Configure(EntityTypeBuilder<InterviewQuestionTag> builder)
    {
        builder.HasKey(qt => new { qt.InterviewQuestionId, qt.TagId });

        builder.HasOne(qt => qt.InterviewQuestion)
            .WithMany(q => q.QuestionTags)
            .HasForeignKey(qt => qt.InterviewQuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(qt => qt.Tag)
            .WithMany()
            .HasForeignKey(qt => qt.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
