using MentorOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorOS.Data.Configurations;

public class ModuleConfiguration : IEntityTypeConfiguration<Module>
{
    public void Configure(EntityTypeBuilder<Module> builder)
    {
        builder.Property(m => m.Slug).HasMaxLength(80).IsRequired();
        builder.Property(m => m.Title).HasMaxLength(200).IsRequired();
        builder.HasIndex(m => m.Slug).IsUnique();

        builder.HasOne(m => m.Topic)
            .WithMany()
            .HasForeignKey(m => m.TopicId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
