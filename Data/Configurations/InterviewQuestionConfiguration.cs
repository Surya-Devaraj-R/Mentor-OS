using MentorOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorOS.Data.Configurations;

public class InterviewQuestionConfiguration : IEntityTypeConfiguration<InterviewQuestion>
{
    public void Configure(EntityTypeBuilder<InterviewQuestion> builder)
    {
        builder.Property(q => q.QuestionType).HasConversion<string>().HasMaxLength(30);
        builder.Property(q => q.Title).HasMaxLength(200).IsRequired();
        builder.Property(q => q.DiagramFormat).HasConversion<string>().HasMaxLength(20);
    }
}
