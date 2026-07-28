using MentorOS.Models.Enums;

namespace MentorOS.Models;

// A topic-level "production project roadmap" — distinct from the existing
// module-level CapstoneProject "mini project". The architecture diagram
// reuses the exact same StructuredSteps/AsciiArt body format and frontend
// renderers already built for lesson diagrams.
public class LearningPathProject
{
    public int Id { get; set; }
    public int TopicId { get; set; }
    public Topic? Topic { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string PortfolioGuidance { get; set; } = "";
    public string ArchitectureDiagramBody { get; set; } = "";
    public DiagramFormat ArchitectureDiagramFormat { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }

    public List<ProjectMilestone> Milestones { get; set; } = [];
    // Implementation checklist is polymorphic (ChecklistItem.OwnerKind =
    // Project, OwnerId = this.Id) — no direct EF navigation.
}
