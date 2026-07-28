using MentorOS.Models;
using Microsoft.EntityFrameworkCore;

namespace MentorOS.Data.Seed;

// Imperative "seed if empty" runner, called once from Program.cs after
// migrations apply. Preferred over EF's HasData because hand-editing seed
// content (especially large lesson-content bodies in later phases) shouldn't
// require a new migration diff every time.
public static class SeedRunner
{
    public static async Task ApplyAsync(AppDbContext db)
    {
        await SeedTopicsAsync(db);
        await SeedResourcesAsync(db);
        await SeedCurriculumContentAsync(db);
        await SeedExercisesAsync(db);
        await SeedInterviewQuestionsAsync(db);
        await SeedProjectsAsync(db);
    }

    private static async Task SeedTopicsAsync(AppDbContext db)
    {
        if (await db.Topics.AnyAsync()) return;

        db.Topics.AddRange(CurriculumSeedData.BuildTopics());
        await db.SaveChangesAsync();
    }

    private static async Task SeedResourcesAsync(AppDbContext db)
    {
        if (await db.Resources.AnyAsync()) return;

        var topicIdBySlug = await db.Topics.ToDictionaryAsync(t => t.Slug, t => t.Id);
        db.Resources.AddRange(ResourceSeedData.BuildResources(topicIdBySlug));
        await db.SaveChangesAsync();
    }

    private static async Task SeedCurriculumContentAsync(AppDbContext db)
    {
        if (await db.Modules.AnyAsync()) return;

        var topicIdBySlug = await db.Topics.ToDictionaryAsync(t => t.Slug, t => t.Id);
        var (modules, checklistSeeds) = CurriculumContentSeedData.BuildModules(topicIdBySlug);

        // Modules/Lessons/ContentBlocks/Objectives/ReferenceLinks/QuizQuestions/
        // Prerequisites all have real FKs+navigations, so EF Core inserts the
        // whole graph in one pass. ChecklistItem is polymorphic (no real FK),
        // so it's resolved in a second pass below, once real Ids exist.
        db.Modules.AddRange(modules);
        await db.SaveChangesAsync();

        await ApplyChecklistSeedsAsync(db, checklistSeeds);
    }

    private static async Task SeedExercisesAsync(AppDbContext db)
    {
        if (await db.Exercises.AnyAsync()) return;

        var lessonIdBySlug = await db.Lessons.ToDictionaryAsync(l => l.Slug, l => l.Id);
        db.Exercises.AddRange(ExerciseSeedData.BuildExercises(lessonIdBySlug));
        await db.SaveChangesAsync();
    }

    private static async Task SeedInterviewQuestionsAsync(AppDbContext db)
    {
        if (await db.InterviewQuestions.AnyAsync()) return;

        var (questions, companies) = InterviewQuestionSeedData.BuildQuestions();
        db.Companies.AddRange(companies);
        db.InterviewQuestions.AddRange(questions);
        await db.SaveChangesAsync();
    }

    private static async Task SeedProjectsAsync(AppDbContext db)
    {
        if (await db.LearningPathProjects.AnyAsync()) return;

        var topicIdBySlug = await db.Topics.ToDictionaryAsync(t => t.Slug, t => t.Id);
        var (projects, checklistSeeds) = ProjectSeedData.BuildProjects(topicIdBySlug);

        db.LearningPathProjects.AddRange(projects);
        await db.SaveChangesAsync();

        await ApplyChecklistSeedsAsync(db, checklistSeeds);
    }

    private static async Task ApplyChecklistSeedsAsync(AppDbContext db, List<ChecklistSeed> checklistSeeds)
    {
        if (checklistSeeds.Count == 0) return;

        var lessonIdBySlug = await db.Lessons.ToDictionaryAsync(l => l.Slug, l => l.Id);
        var capstoneIdByModuleSlug = await db.CapstoneProjects
            .Include(c => c.Module)
            .ToDictionaryAsync(c => c.Module!.Slug, c => c.Id);
        var projectIdByTopicSlug = await db.LearningPathProjects
            .Include(p => p.Topic)
            .ToDictionaryAsync(p => p.Topic!.Slug, p => p.Id);

        var items = new List<ChecklistItem>();
        foreach (var seed in checklistSeeds)
        {
            var ownerId = seed.Kind switch
            {
                Models.Enums.ChecklistOwnerKind.Lesson => lessonIdBySlug[seed.OwnerSlug],
                Models.Enums.ChecklistOwnerKind.Capstone => capstoneIdByModuleSlug[seed.OwnerSlug],
                Models.Enums.ChecklistOwnerKind.Project => projectIdByTopicSlug[seed.OwnerSlug],
                _ => throw new InvalidOperationException($"Unknown checklist owner kind: {seed.Kind}"),
            };

            items.AddRange(seed.Descriptions.Select((description, i) => new ChecklistItem
            {
                OwnerKind = seed.Kind,
                OwnerId = ownerId,
                Description = description,
                SortOrder = i + 1,
            }));
        }

        db.ChecklistItems.AddRange(items);
        await db.SaveChangesAsync();
    }
}
