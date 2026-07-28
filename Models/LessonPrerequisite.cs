namespace MentorOS.Models;

// Self-referencing: a lesson can point at zero or more lessons recommended
// beforehand. Purely informational in this pass — no gating/enforcement.
public class LessonPrerequisite
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public Lesson? Lesson { get; set; }
    public int PrerequisiteLessonId { get; set; }
    public Lesson? PrerequisiteLesson { get; set; }
}
