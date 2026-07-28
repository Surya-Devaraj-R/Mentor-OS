namespace MentorOS.Models;

// "What You'll Learn" bullet list — a fixed-position header list, not part of
// the arbitrarily-orderable LessonContentBlock stream.
public class LessonObjective
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public Lesson? Lesson { get; set; }
    public string Text { get; set; } = "";
    public int SortOrder { get; set; }
}
