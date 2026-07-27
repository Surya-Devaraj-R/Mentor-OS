using MentorOS.Models.Enums;

namespace MentorOS.Contracts.Planner;

public record PlanItemDto(
    int Id,
    DateOnly PlanDate,
    EntityKind EntityKind,
    int? EntityId,
    string? CustomTitle,
    bool IsDone,
    int SortOrder,
    string DisplayTitle);

public record CreatePlanItemRequest(
    DateOnly PlanDate,
    EntityKind EntityKind,
    int? EntityId,
    string? CustomTitle,
    int SortOrder);

public record MarkDoneRequest(bool Done);
