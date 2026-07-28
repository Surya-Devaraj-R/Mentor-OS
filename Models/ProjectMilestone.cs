namespace MentorOS.Models;

public class ProjectMilestone
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public LearningPathProject? Project { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public int SortOrder { get; set; }
}
