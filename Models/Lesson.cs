namespace MentorOS.Models;

public class Lesson
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public Module? Module { get; set; }
    public string Slug { get; set; } = "";
    public string Title { get; set; } = "";
    public string Summary { get; set; } = "";
    public int SortOrder { get; set; }
    public int? EstimatedMinutes { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }

    public List<LessonContentBlock> ContentBlocks { get; set; } = [];
}
