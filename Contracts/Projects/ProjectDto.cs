using MentorOS.Contracts.Checklists;
using MentorOS.Models.Enums;

namespace MentorOS.Contracts.Projects;

public record ProjectMilestoneDto(string Title, string Description, int SortOrder);

public record ProjectDetailDto(
    string Title,
    string Description,
    string PortfolioGuidance,
    string ArchitectureDiagramBody,
    DiagramFormat ArchitectureDiagramFormat,
    IReadOnlyList<ProjectMilestoneDto> Milestones,
    IReadOnlyList<ChecklistItemDto> Checklist);
