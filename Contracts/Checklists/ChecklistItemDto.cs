namespace MentorOS.Contracts.Checklists;

// Shared shape for Capstone/Lesson/Project checklists — mirrors the
// ChecklistItem model's polymorphic OwnerKind design.
public record ChecklistItemDto(int Id, string Description, int SortOrder, bool IsCompleted);
