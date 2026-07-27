using MentorOS.Models.Enums;

namespace MentorOS.Models;

public class LessonContentBlock
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public Lesson? Lesson { get; set; }
    public BlockType BlockType { get; set; }
    public int SortOrder { get; set; }
    public string? Title { get; set; }
    public BodyFormat BodyFormat { get; set; }
    public string Body { get; set; } = "";
    public string? Language { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }
}
