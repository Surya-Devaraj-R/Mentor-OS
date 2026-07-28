using MentorOS.Models.Enums;

namespace MentorOS.Data.Seed;

// Checklist items are polymorphic (no real FK), so unlike everything else in
// the seed graph they can't be nested directly into an object initializer —
// their OwnerId is only known after the owner is saved. OwnerSlug means:
// LessonSlug when Kind=Lesson, ModuleSlug when Kind=Capstone (1:1 with
// Module), TopicSlug when Kind=Project (1:1 with Topic).
public record ChecklistSeed(ChecklistOwnerKind Kind, string OwnerSlug, List<string> Descriptions);
