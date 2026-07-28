namespace MentorOS.Models.Enums;

// Shared discriminator for the polymorphic CompletionRecord/Bookmark tables —
// one entry per entity type that can be marked complete or bookmarked.
public enum EntityKind
{
    Topic,
    Module,
    Lesson,
    Exercise,

    // Tracks completion of one ChecklistItem row (Capstone/Lesson/Project) —
    // EntityId is the ChecklistItem's own Id, not its OwnerId, so each item
    // toggles independently under this one shared kind.
    ChecklistItem,

    InterviewQuestion,
    Resource,

    // Only valid on DailyPlanItem — a free-text planner entry with no
    // backing entity (CustomTitle is set instead of resolving EntityId).
    Custom,
}
