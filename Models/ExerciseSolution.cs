namespace MentorOS.Models;

public class ExerciseSolution
{
    public int Id { get; set; }
    public int ExerciseId { get; set; }
    public Exercise? Exercise { get; set; }
    public string ApproachTitle { get; set; } = "";
    public string Explanation { get; set; } = "";
    public string SolutionCode { get; set; } = "";
    public string Language { get; set; } = "";
    public string? TimeComplexity { get; set; }
    public string? SpaceComplexity { get; set; }
    public int SortOrder { get; set; }
}
