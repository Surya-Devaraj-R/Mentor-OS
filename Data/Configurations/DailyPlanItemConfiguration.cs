using MentorOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorOS.Data.Configurations;

public class DailyPlanItemConfiguration : IEntityTypeConfiguration<DailyPlanItem>
{
    public void Configure(EntityTypeBuilder<DailyPlanItem> builder)
    {
        builder.Property(p => p.EntityKind).HasConversion<string>().HasMaxLength(40);
        builder.Property(p => p.CustomTitle).HasMaxLength(200);
        builder.HasIndex(p => p.PlanDate);
    }
}
