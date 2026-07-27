using MentorOS.Models.Enums;

namespace MentorOS.Contracts.Bookmarks;

public record BookmarkDto(
    int Id,
    EntityKind EntityKind,
    int EntityId,
    string? Note,
    string DisplayTitle,
    DateTime CreatedUtc);

public record CreateBookmarkRequest(EntityKind EntityKind, int EntityId, string? Note);
