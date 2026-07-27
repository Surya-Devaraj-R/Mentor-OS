using MentorOS.Contracts.Modules;
using MentorOS.Contracts.Topics;
using MentorOS.Data;
using Microsoft.EntityFrameworkCore;

namespace MentorOS.Endpoints;

public static class TopicEndpoints
{
    public static void MapTopicEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/topics");

        group.MapGet("/", async (AppDbContext db) =>
        {
            var topics = await db.Topics
                .OrderBy(t => t.SortOrder)
                .Select(t => new TopicDto(t.Id, t.Slug, t.Title, t.Description, t.IconKey, t.SortOrder))
                .ToListAsync();

            return Results.Ok(topics);
        });

        group.MapGet("/{slug}", async (string slug, AppDbContext db) =>
        {
            var topic = await db.Topics.FirstOrDefaultAsync(t => t.Slug == slug);

            return topic is null
                ? Results.NotFound()
                : Results.Ok(new TopicDto(topic.Id, topic.Slug, topic.Title, topic.Description, topic.IconKey, topic.SortOrder));
        });

        group.MapGet("/{slug}/modules", async (string slug, AppDbContext db) =>
        {
            var topic = await db.Topics.FirstOrDefaultAsync(t => t.Slug == slug);
            if (topic is null) return Results.NotFound();

            var modules = await db.Modules
                .Where(m => m.TopicId == topic.Id)
                .OrderBy(m => m.SortOrder)
                .Select(m => new ModuleSummaryDto(m.Id, m.Slug, m.Title, m.Description, m.SortOrder, m.EstimatedMinutes))
                .ToListAsync();

            return Results.Ok(modules);
        });
    }
}
