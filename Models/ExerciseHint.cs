namespace MentorOS.Models;

// Progressive, one-at-a-time reveal — SortOrder 1 is the vaguest nudge,
// higher SortOrder gets progressively more specific.
public class ExerciseHint
{
    public int Id { get; set; }
    public int ExerciseId { get; set; }
    public Exercise? Exercise { get; set; }
    public string Text { get; set; } = "";
    public int SortOrder { get; set; }
}
