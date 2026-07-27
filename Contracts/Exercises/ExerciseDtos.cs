using MentorOS.Models.Enums;

namespace MentorOS.Contracts.Exercises;

public record ExerciseSummaryDto(
    int Id,
    string Slug,
    string Title,
    DifficultyLevel DifficultyLevel,
    ExerciseType ExerciseType,
    bool IsInterviewChallenge,
    string? Language,
    SelfAssessment? LatestStatus);

public record ExerciseSolutionDto(
    int Id,
    string ApproachTitle,
    string Explanation,
    string SolutionCode,
    string Language,
    string? TimeComplexity,
    string? SpaceComplexity,
    int SortOrder);

public record ExerciseSubmissionDto(
    int Id,
    string SubmittedCode,
    string? Notes,
    SelfAssessment SelfAssessment,
    int AttemptNumber,
    DateTime SubmittedUtc);

public record ExerciseDetailDto(
    int Id,
    string Slug,
    string Title,
    string Prompt,
    DifficultyLevel DifficultyLevel,
    ExerciseType ExerciseType,
    string? StarterCode,
    string? Language,
    bool IsInterviewChallenge,
    IReadOnlyList<ExerciseSolutionDto> Solutions,
    IReadOnlyList<ExerciseSubmissionDto> Submissions);

public record CreateSubmissionRequest(string SubmittedCode, string? Notes, SelfAssessment SelfAssessment);
