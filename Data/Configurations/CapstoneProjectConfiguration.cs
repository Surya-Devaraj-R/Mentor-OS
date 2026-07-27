using MentorOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorOS.Data.Configurations;

public class CapstoneProjectConfiguration : IEntityTypeConfiguration<CapstoneProject>
{
    public void Configure(EntityTypeBuilder<CapstoneProject> builder)
    {
        builder.Property(c => c.Title).HasMaxLength(200).IsRequired();
        builder.HasIndex(c => c.ModuleId).IsUnique();

        builder.HasOne(c => c.Module)
            .WithOne(m => m.Capstone)
            .HasForeignKey<CapstoneProject>(c => c.ModuleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
