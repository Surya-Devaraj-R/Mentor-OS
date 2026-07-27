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
        db.Modules.AddRange(CurriculumContentSeedData.BuildModules(topicIdBySlug));
        await db.SaveChangesAsync();
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

        db.InterviewQuestions.AddRange(InterviewQuestionSeedData.BuildQuestions());
        await db.SaveChangesAsync();
    }
}
