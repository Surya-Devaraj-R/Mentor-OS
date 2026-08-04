using MentorOS.Models;

namespace MentorOS.Data.Seed;

// The 10 top-level learning paths. Interview Prep is deliberately NOT one of
// these — it's its own feature (InterviewQuestion bank), not a learning path.
public static class CurriculumSeedData
{
    public static List<Topic> BuildTopics()
    {
        var now = DateTime.UtcNow;

        return
        [
            new Topic
            {
                Slug = "csharp-basics",
                Title = "C# Basics",
                Description = "Absolute-beginner C#, from zero: variables, loops, methods, classes, and more. Start here if you're brand new to programming.",
                IconKey = "course",
                SortOrder = 0,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
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
                Slug = "git",
                Title = "Git",
                Description = "Version control fundamentals and collaborative workflows.",
                IconKey = "practice",
                SortOrder = 7,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new Topic
            {
                Slug = "devops",
                Title = "DevOps",
                Description = "CI/CD pipelines, automation, and observability.",
                IconKey = "system",
                SortOrder = 8,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new Topic
            {
                Slug = "architecture",
                Title = "Architecture",
                Description = "Software architecture patterns, layering, and design principles.",
                IconKey = "course",
                SortOrder = 9,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new Topic
            {
                Slug = "soft-skills",
                Title = "Soft Skills",
                Description = "Communication, collaboration, and interview readiness beyond the code.",
                IconKey = "practice",
                SortOrder = 10,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new Topic
            {
                Slug = "ai-integration",
                Title = "AI Integration",
                Description = "Building AI-powered features with LLMs, vector search, and agentic protocols.",
                IconKey = "system",
                SortOrder = 11,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new Topic
            {
                Slug = "frontend",
                Title = "Frontend",
                Description = "JavaScript, TypeScript, and React for building modern web frontends that talk to your API.",
                IconKey = "course",
                SortOrder = 12,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
        ];
    }
}
