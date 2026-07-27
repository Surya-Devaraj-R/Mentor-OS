using MentorOS.Models;

namespace MentorOS.Data.Seed;

// Verbatim migration of the 8 links that used to live in wwwroot/app.js's
// RESOURCE_SECTIONS array (id -> Slug, icon -> IconKey, section title ->
// LegacySectionTitle preserved as-is). TopicId is a best-fit mapping onto
// the new 7-topic taxonomy.
public static class ResourceSeedData
{
    private record SeedEntry(
        string Slug,
        string TopicSlug,
        string LegacySectionTitle,
        string Title,
        string Label,
        string Url,
        string IconKey,
        int SortOrder);

    public static List<Resource> BuildResources(IReadOnlyDictionary<string, int> topicIdBySlug)
    {
        var now = DateTime.UtcNow;

        var entries = new List<SeedEntry>
        {
            new("foundational-csharp", "csharp", "C# & .NET Core Mastery",
                "Foundational C#", "Official Microsoft Certification Path",
                "https://www.freecodecamp.org/learn/foundational-c-sharp-with-microsoft", "course", 1),
            new("iamtimcorey", "csharp", "C# & .NET Core Mastery",
                "IAmTimCorey", "Deep-Dive C# Architecture",
                "https://youtube.com", "video", 2),
            new("official-dotnet-docs", "dotnet", "C# & .NET Core Mastery",
                "Official .NET Docs", "Core Syntax Reference",
                "https://learn.microsoft.com/en-us/dotnet/core/get-started", "course", 3),
            new("neetcode-150", "dsa", "Data Structures & Algorithms",
                "NeetCode 150", "Core Algorithmic Tree",
                "https://neetcode.io/roadmap", "practice", 4),
            new("abdul-bari", "dsa", "Data Structures & Algorithms",
                "Abdul Bari", "Master Big O & Logic",
                "https://youtube.com", "video", 5),
            new("leetcode", "dsa", "Data Structures & Algorithms",
                "LeetCode", "Daily Active Coding",
                "https://leetcode.com", "practice", 6),
            new("pramp", "interview-prep", "System Design & Interview Preparation",
                "Pramp", "Practice Coding Out Loud",
                "https://pramp.com", "practice", 7),
            new("bytebytego", "system-design", "System Design & Interview Preparation",
                "ByteByteGo", "High-Scale Systems",
                "https://bytebytego.com", "system", 8),
        };

        return entries.Select(e => new Resource
        {
            Slug = e.Slug,
            TopicId = topicIdBySlug.GetValueOrDefault(e.TopicSlug),
            LegacySectionTitle = e.LegacySectionTitle,
            Title = e.Title,
            Label = e.Label,
            Url = e.Url,
            IconKey = e.IconKey,
            SortOrder = e.SortOrder,
            CreatedUtc = now,
        }).ToList();
    }
}
