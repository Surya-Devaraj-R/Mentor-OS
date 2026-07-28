using MentorOS.Models.Enums;

namespace MentorOS.Models;

// Polymorphic, mirroring the CompletionRecord/Bookmark pattern already used
// elsewhere in this codebase: one shared table instead of three near-
// identical ones (Capstone/Lesson/Project completion checklists). Trade-off:
// no DB-enforced FK to the owner (validated at the API layer instead) — an
// acceptable trade for a solo app where owners are effectively never
// hard-deleted outside a full reseed.
public class ChecklistItem
{
    public int Id { get; set; }
    public ChecklistOwnerKind OwnerKind { get; set; }
    public int OwnerId { get; set; }
    public string Description { get; set; } = "";
    public int SortOrder { get; set; }
}
