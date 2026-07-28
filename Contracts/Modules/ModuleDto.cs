using MentorOS.Contracts.Checklists;

namespace MentorOS.Contracts.Modules;

public record ModuleSummaryDto(
    int Id,
    string Slug,
    string Title,
    string Description,
    int SortOrder,
    int? EstimatedMinutes);

public record LessonSummaryDto(
    int Id,
    string Slug,
    string Title,
    string Summary,
    int SortOrder,
    int? EstimatedMinutes,
    bool IsCompleted);

public record CapstoneSummaryDto(
    string Title,
    string Description,
    string Requirements,
    IReadOnlyList<ChecklistItemDto> ChecklistItems);

public record ModuleDetailDto(
    int Id,
    string Slug,
    string Title,
    string Description,
    int? EstimatedMinutes,
    IReadOnlyList<LessonSummaryDto> Lessons,
    CapstoneSummaryDto? Capstone);
