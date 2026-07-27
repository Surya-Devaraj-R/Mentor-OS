namespace MentorOS.Contracts.Notes;

public record NoteDto(
    int Id,
    int? LessonId,
    string? LessonTitle,
    string? Title,
    string Body,
    DateTime CreatedUtc,
    DateTime UpdatedUtc);

public record CreateNoteRequest(int? LessonId, string? Title, string Body);

public record UpdateNoteRequest(string? Title, string Body);
