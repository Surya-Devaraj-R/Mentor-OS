using MentorOS.Models.Enums;

namespace MentorOS.Models;

public class Bookmark
{
    public int Id { get; set; }
    public EntityKind EntityKind { get; set; }
    public int EntityId { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedUtc { get; set; }
}
