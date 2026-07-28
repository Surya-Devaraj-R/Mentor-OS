namespace MentorOS.Models;

// Single-correct-answer multiple choice.
public class QuizOption
{
    public int Id { get; set; }
    public int QuizQuestionId { get; set; }
    public QuizQuestion? QuizQuestion { get; set; }
    public string Text { get; set; } = "";
    public bool IsCorrect { get; set; }
    public int SortOrder { get; set; }
}
