using MentorOS.Models.Enums;

namespace MentorOS.Models;

// A scheduling concern ("did I do my scheduled task today"), distinct from
// CompletionRecord's permanent mastery record. Marking a plan item done can
// additionally trigger a CompletionRecord write for its target entity, but
// the two tables answer different questions.
public class DailyPlanItem
{
    public int Id { get; set; }
    public DateOnly PlanDate { get; set; }
    public EntityKind EntityKind { get; set; }
    public int? EntityId { get; set; }
    public string? CustomTitle { get; set; }
    public bool IsDone { get; set; }
    public DateTime? DoneUtc { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedUtc { get; set; }
}
