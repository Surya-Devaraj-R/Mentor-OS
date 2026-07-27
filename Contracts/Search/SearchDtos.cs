namespace MentorOS.Contracts.Search;

public record SearchResultItemDto(string EntityType, int EntityId, string Title, string Snippet, string NavigateHash);

public record SearchGroupDto(string EntityType, IReadOnlyList<SearchResultItemDto> Items);

public record SearchResponseDto(string Query, IReadOnlyList<SearchGroupDto> Groups);
