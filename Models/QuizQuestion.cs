namespace MentorOS.Models;

// Ungraded, client-side self-check — no QuizAttempt/score persistence.
public class QuizQuestion
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public Lesson? Lesson { get; set; }
    public string QuestionText { get; set; } = "";
    public string Explanation { get; set; } = "";
    public int SortOrder { get; set; }

    public List<QuizOption> Options { get; set; } = [];
}
