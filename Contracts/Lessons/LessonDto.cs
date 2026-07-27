using MentorOS.Models.Enums;

namespace MentorOS.Contracts.Lessons;

public record LessonContentBlockDto(
    int Id,
    BlockType BlockType,
    string? Title,
    BodyFormat BodyFormat,
    string Body,
    string? Language,
    int SortOrder);

public record LessonDetailDto(
    int Id,
    string Slug,
    string Title,
    string Summary,
    int? EstimatedMinutes,
    bool IsCompleted,
    int? BookmarkId,
    IReadOnlyList<LessonContentBlockDto> ContentBlocks);
