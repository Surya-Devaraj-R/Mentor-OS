namespace MentorOS.Models;

public class CapstoneChecklistItem
{
    public int Id { get; set; }
    public int CapstoneProjectId { get; set; }
    public CapstoneProject? CapstoneProject { get; set; }
    public string Description { get; set; } = "";
    public int SortOrder { get; set; }
}
