using MentorOS.Models.Enums;

namespace MentorOS.Models;

// Used both for lesson-embedded practice and standalone interview-bank
// problems (LessonId null + IsInterviewChallenge true), rather than two
// parallel models.
public class Exercise
{
    public int Id { get; set; }
    public int? LessonId { get; set; }
    public Lesson? Lesson { get; set; }
    public string Slug { get; set; } = "";
    public string Title { get; set; } = "";
    public string Prompt { get; set; } = "";
    public DifficultyLevel DifficultyLevel { get; set; }
    public ExerciseType ExerciseType { get; set; }
    public string? StarterCode { get; set; }
    public string? Language { get; set; }
    public bool IsInterviewChallenge { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }

    public List<ExerciseSolution> Solutions { get; set; } = [];
    public List<ExerciseSubmission> Submissions { get; set; } = [];
    public List<ExerciseTag> ExerciseTags { get; set; } = [];
}
