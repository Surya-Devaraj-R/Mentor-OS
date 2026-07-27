using MentorOS.Models;

namespace MentorOS.Data.Seed;

// The 7 top-level learning tracks. Modules and lessons are added underneath
// these incrementally (Phase 1+) using the same seed-if-empty pattern.
public static class CurriculumSeedData
{
    public static List<Topic> BuildTopics()
    {
        var now = DateTime.UtcNow;

        return
        [
            new Topic
            {
                Slug = "csharp",
                Title = "C#",
                Description = "Language fundamentals, OOP, and modern C# features.",
                IconKey = "course",
                SortOrder = 1,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new Topic
            {
                Slug = "dotnet",
                Title = ".NET",
                Description = "Runtime, ASP.NET Core, EF Core, and the wider .NET ecosystem.",
                IconKey = "system",
                SortOrder = 2,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new Topic
            {
                Slug = "dsa",
                Title = "Data Structures & Algorithms",
                Description = "Core data structures, algorithmic patterns, and complexity analysis.",
                IconKey = "practice",
                SortOrder = 3,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new Topic
            {
                Slug = "system-design",
                Title = "System Design",
                Description = "Designing large-scale, reliable distributed systems.",
                IconKey = "system",
                SortOrder = 4,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new Topic
            {
                Slug = "sql",
                Title = "SQL",
                Description = "Relational modeling, query design, and performance tuning.",
                IconKey = "course",
                SortOrder = 5,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new Topic
            {
                Slug = "cloud",
                Title = "Cloud",
                Description = "Cloud-native architecture, deployment, and operations.",
                IconKey = "system",
                SortOrder = 6,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new Topic
            {
                Slug = "interview-prep",
                Title = "Interview Prep",
                Description = "Behavioral, system design, and mock-interview readiness.",
                IconKey = "practice",
                SortOrder = 7,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
        ];
    }
}
