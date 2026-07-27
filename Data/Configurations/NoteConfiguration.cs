using MentorOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MentorOS.Data.Configurations;

public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.Property(n => n.Title).HasMaxLength(200);

        builder.HasOne(n => n.Lesson)
            .WithMany()
            .HasForeignKey(n => n.LessonId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
