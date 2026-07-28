using MentorOS.Models.Enums;

namespace MentorOS.Models;

// A real Url field so the frontend renders a genuine <a href> — the
// mini-markdown renderer deliberately has no link syntax.
public class LessonReferenceLink
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public Lesson? Lesson { get; set; }
    public string Title { get; set; } = "";
    public string Url { get; set; } = "";
    public LinkType LinkType { get; set; }
    public int SortOrder { get; set; }
}
