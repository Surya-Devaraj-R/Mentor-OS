using MentorOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorOS.Data.Configurations;

public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.Property(r => r.Slug).HasMaxLength(80).IsRequired();
        builder.Property(r => r.Title).HasMaxLength(200).IsRequired();
        builder.Property(r => r.Label).HasMaxLength(200).IsRequired();
        builder.Property(r => r.Url).HasMaxLength(500).IsRequired();
        builder.Property(r => r.IconKey).HasMaxLength(40).IsRequired();
        builder.Property(r => r.LegacySectionTitle).HasMaxLength(200).IsRequired();
        builder.HasIndex(r => r.Slug).IsUnique();

        builder.HasOne(r => r.Topic)
            .WithMany()
            .HasForeignKey(r => r.TopicId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
