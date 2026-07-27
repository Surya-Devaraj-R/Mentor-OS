namespace MentorOS.Contracts.Resources;

public record ResourceDto(
    int Id,
    string Slug,
    string Title,
    string Label,
    string Url,
    string IconKey,
    string LegacySectionTitle,
    bool IsCompleted);
