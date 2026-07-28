namespace MentorOS.Models;

public class CapstoneProject
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public Module? Module { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Requirements { get; set; } = "";
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }

    // Checklist items are polymorphic (ChecklistItem.OwnerKind = Capstone,
    // OwnerId = this.Id) — no direct EF navigation, queried explicitly.
}
