using MentorOS.Models.Enums;

namespace MentorOS.Models;

// Centralized "this thing is done" record for every completable entity type,
// instead of a boolean column scattered across Topic/Lesson/Exercise/Resource/etc.
// Trade-off: no DB-enforced FK to the target entity (validated at the API layer
// instead) in exchange for one uniform query path for progress/streak/dashboard reads.
public class CompletionRecord
{
    public int Id { get; set; }
    public EntityKind EntityKind { get; set; }
    public int EntityId { get; set; }
    public DateTime CompletedUtc { get; set; }
}
