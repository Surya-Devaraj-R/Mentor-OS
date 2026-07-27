using MentorOS.Models.Enums;

namespace MentorOS.Models;

// Self-assessment only, no execution: the user's own submitted attempt text,
// plus how they judged themselves against the shown solutions.
public class ExerciseSubmission
{
    public int Id { get; set; }
    public int ExerciseId { get; set; }
    public Exercise? Exercise { get; set; }
    public string SubmittedCode { get; set; } = "";
    public string? Notes { get; set; }
    public SelfAssessment SelfAssessment { get; set; }
    public int AttemptNumber { get; set; }
    public DateTime SubmittedUtc { get; set; }
}
