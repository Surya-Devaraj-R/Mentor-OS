using MentorOS.Contracts.Search;
using MentorOS.Data;
using Microsoft.EntityFrameworkCore;

namespace MentorOS.Services;

// Deliberately EF Contains()/LIKE-based, not SQLite FTS5 — FTS5 is
// SQLite-specific and wouldn't carry over to SQL Server/Postgres without a
// rewrite. Plain LIKE is fully portable and adequate at a single user's
// content volume.
public class SearchService(AppDbContext db)
{
    public async Task<SearchResponseDto> SearchAsync(string query)
    {
        var term = query.Trim();
        if (string.IsNullOrEmpty(term))
        {
            return new SearchResponseDto(query, []);
        }

        var groups = new List<SearchGroupDto>();

        var topics = await db.Topics
            .Where(t => t.Title.Contains(term) || t.Description.Contains(term))
            .Select(t => new SearchResultItemDto("Topic", t.Id, t.Title, t.Description, $"#/roadmap/{t.Slug}"))
            .ToListAsync();
        AddGroup(groups, "Topic", topics);

        var modules = await db.Modules
            .Include(m => m.Topic)
            .Where(m => m.Title.Contains(term) || m.Description.Contains(term))
            .Select(m => new SearchResultItemDto("Module", m.Id, m.Title, m.Description, $"#/roadmap/{m.Topic!.Slug}/{m.Slug}"))
            .ToListAsync();
        AddGroup(groups, "Module", modules);

        var lessonsById = (await db.Lessons
                .Where(l => l.Title.Contains(term) || l.Summary.Contains(term))
                .Select(l => new SearchResultItemDto("Lesson", l.Id, l.Title, l.Summary, $"#/lesson/{l.Slug}"))
                .ToListAsync())
            .ToDictionary(l => l.EntityId);

        // Also surface lessons whose CONTENT (not just title/summary) matches,
        // mapped back to the owning lesson.
        var lessonsFromContent = await db.LessonContentBlocks
            .Include(b => b.Lesson)
            .Where(b => b.Body.Contains(term))
            .Select(b => new { b.Lesson!.Id, b.Lesson.Title, b.Lesson.Summary, b.Lesson.Slug })
            .ToListAsync();
        foreach (var match in lessonsFromContent)
        {
            lessonsById.TryAdd(match.Id, new SearchResultItemDto("Lesson", match.Id, match.Title, match.Summary, $"#/lesson/{match.Slug}"));
        }
        AddGroup(groups, "Lesson", lessonsById.Values.ToList());

        var exercises = await db.Exercises
            .Where(e => e.Title.Contains(term) || e.Prompt.Contains(term))
            .Select(e => new SearchResultItemDto("Exercise", e.Id, e.Title, e.Prompt, $"#/practice/{e.Slug}"))
            .ToListAsync();
        AddGroup(groups, "Exercise", exercises);

        var interviewQuestions = await db.InterviewQuestions
            .Where(q => q.Title.Contains(term) || q.PromptText.Contains(term))
            .Select(q => new SearchResultItemDto("InterviewQuestion", q.Id, q.Title, q.PromptText, "#/interview-prep"))
            .ToListAsync();
        AddGroup(groups, "InterviewQuestion", interviewQuestions);

        var notes = await db.Notes
            .Where(n => n.Body.Contains(term) || (n.Title != null && n.Title.Contains(term)))
            .Select(n => new SearchResultItemDto("Note", n.Id, n.Title ?? "Untitled note", n.Body, "#/notes"))
            .ToListAsync();
        AddGroup(groups, "Note", notes);

        var resources = await db.Resources
            .Where(r => r.Title.Contains(term) || r.Label.Contains(term))
            .Select(r => new SearchResultItemDto("Resource", r.Id, r.Title, r.Label, "#/resources"))
            .ToListAsync();
        AddGroup(groups, "Resource", resources);

        return new SearchResponseDto(query, groups);
    }

    private static void AddGroup(List<SearchGroupDto> groups, string entityType, List<SearchResultItemDto> items)
    {
        if (items.Count > 0) groups.Add(new SearchGroupDto(entityType, items));
    }
}
