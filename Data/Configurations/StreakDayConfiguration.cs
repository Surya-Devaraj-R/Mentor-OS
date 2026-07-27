using MentorOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorOS.Data.Configurations;

public class StreakDayConfiguration : IEntityTypeConfiguration<StreakDay>
{
    public void Configure(EntityTypeBuilder<StreakDay> builder)
    {
        builder.HasIndex(s => s.ActivityDate).IsUnique();
    }
}
