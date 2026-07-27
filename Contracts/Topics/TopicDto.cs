namespace MentorOS.Contracts.Topics;

public record TopicDto(int Id, string Slug, string Title, string Description, string IconKey, int SortOrder);
