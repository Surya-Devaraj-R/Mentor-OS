using MentorOS.Contracts.Checklists;
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

public record LessonPrerequisiteDto(string Slug, string Title);

public record LessonReferenceLinkDto(string Title, string Url, LinkType LinkType);

public record QuizOptionDto(int Id, string Text, bool IsCorrect);

public record QuizQuestionDto(int Id, string QuestionText, string Explanation, IReadOnlyList<QuizOptionDto> Options);

public record LessonDetailDto(
    int Id,
    string Slug,
    string Title,
    string Summary,
    int? EstimatedMinutes,
    bool IsCompleted,
    int? BookmarkId,
    IReadOnlyList<string> Objectives,
    IReadOnlyList<LessonPrerequisiteDto> Prerequisites,
    IReadOnlyList<LessonContentBlockDto> ContentBlocks,
    IReadOnlyList<QuizQuestionDto> Quiz,
    IReadOnlyList<ChecklistItemDto> Checklist,
    IReadOnlyList<LessonReferenceLinkDto> ReferenceLinks);
