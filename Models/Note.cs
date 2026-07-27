namespace MentorOS.Models;

public class Note
{
    public int Id { get; set; }
    public int? LessonId { get; set; }
    public Lesson? Lesson { get; set; }
    public string? Title { get; set; }
    public string Body { get; set; } = "";
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }
}
