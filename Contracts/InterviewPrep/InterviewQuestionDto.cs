using MentorOS.Models.Enums;

namespace MentorOS.Contracts.InterviewPrep;

public record InterviewQuestionDto(
    int Id,
    QuestionType QuestionType,
    string Title,
    string PromptText,
    string? SuggestedApproach,
    string? SampleAnswer,
    int SortOrder,
    bool IsCompleted);
