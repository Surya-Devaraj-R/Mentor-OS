using MentorOS.Contracts.Lessons;
using MentorOS.Contracts.Progress;
using MentorOS.Data;
using MentorOS.Models.Enums;
using MentorOS.Services;
using Microsoft.EntityFrameworkCore;

namespace MentorOS.Endpoints;

public static class LessonEndpoints
{
    public static void MapLessonEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/lessons");

        group.MapGet("/{slug}", async (string slug, AppDbContext db, ProgressService progress) =>
        {
            var lesson = await db.Lessons
                .Include(l => l.ContentBlocks)
                .FirstOrDefaultAsync(l => l.Slug == slug);

            if (lesson is null) return Results.NotFound();

            var completedIds = await progress.GetCompletedIdsAsync(EntityKind.Lesson);
            var bookmark = await db.Bookmarks
                .FirstOrDefaultAsync(b => b.EntityKind == EntityKind.Lesson && b.EntityId == lesson.Id);

            var blocks = lesson.ContentBlocks
                .OrderBy(b => b.SortOrder)
                .Select(b => new LessonContentBlockDto(
                    b.Id, b.BlockType, b.Title, b.BodyFormat, b.Body, b.Language, b.SortOrder))
                .ToList();

            var dto = new LessonDetailDto(
                lesson.Id, lesson.Slug, lesson.Title, lesson.Summary,
                lesson.EstimatedMinutes, completedIds.Contains(lesson.Id), bookmark?.Id, blocks);

            return Results.Ok(dto);
        });

        group.MapPatch("/{id:int}/complete", async (int id, CompleteRequest request, AppDbContext db, ProgressService progress) =>
        {
            var exists = await db.Lessons.AnyAsync(l => l.Id == id);
            if (!exists) return Results.NotFound();

            await progress.SetCompletionAsync(EntityKind.Lesson, id, request.Completed);
            return Results.NoContent();
        });
    }
}
