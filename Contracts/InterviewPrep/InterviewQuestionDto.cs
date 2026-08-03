using MentorOS.Models.Enums;

namespace MentorOS.Contracts.InterviewPrep;

public record InterviewQuestionDto(
    int Id,
    QuestionType QuestionType,
    string Title,
    string PromptText,
    string? SuggestedApproach,
    string? SampleAnswer,
    string? DiagramBody,
    DiagramFormat? DiagramFormat,
    int SortOrder,
    bool IsCompleted,
    IReadOnlyList<string> Companies);

public record CompanyDto(int Id, string Name, string Slug, string? OverviewBody);
