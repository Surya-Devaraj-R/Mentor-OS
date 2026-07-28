using MentorOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorOS.Data.Configurations;

public class InterviewQuestionCompanyConfiguration : IEntityTypeConfiguration<InterviewQuestionCompany>
{
    public void Configure(EntityTypeBuilder<InterviewQuestionCompany> builder)
    {
        builder.HasKey(qc => new { qc.InterviewQuestionId, qc.CompanyId });

        builder.HasOne(qc => qc.InterviewQuestion)
            .WithMany(q => q.QuestionCompanies)
            .HasForeignKey(qc => qc.InterviewQuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(qc => qc.Company)
            .WithMany(c => c.QuestionCompanies)
            .HasForeignKey(qc => qc.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
