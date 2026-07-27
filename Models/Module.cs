namespace MentorOS.Models;

public class Module
{
    public int Id { get; set; }
    public int TopicId { get; set; }
    public Topic? Topic { get; set; }
    public string Slug { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public int SortOrder { get; set; }
    public int? EstimatedMinutes { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }

    public List<Lesson> Lessons { get; set; } = [];
    public CapstoneProject? Capstone { get; set; }
}
