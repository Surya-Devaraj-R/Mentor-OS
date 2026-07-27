using MentorOS.Contracts.Planner;
using MentorOS.Data;
using MentorOS.Models;
using MentorOS.Models.Enums;
using MentorOS.Services;
using Microsoft.EntityFrameworkCore;

namespace MentorOS.Endpoints;

public static class PlannerEndpoints
{
    public static void MapPlannerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/planner");

        group.MapGet("/{date}", async (DateOnly date, AppDbContext db) =>
        {
            var items = await db.DailyPlanItems
                .Where(p => p.PlanDate == date)
                .OrderBy(p => p.SortOrder)
                .ToListAsync();

            return Results.Ok(await ResolveDtosAsync(items, db));
        });

        group.MapGet("/", async (DateOnly from, DateOnly to, AppDbContext db) =>
        {
            var items = await db.DailyPlanItems
                .Where(p => p.PlanDate >= from && p.PlanDate <= to)
                .OrderBy(p => p.PlanDate).ThenBy(p => p.SortOrder)
                .ToListAsync();

            return Results.Ok(await ResolveDtosAsync(items, db));
        });

        group.MapPost("/", async (CreatePlanItemRequest request, AppDbContext db) =>
        {
            if (request.EntityKind == EntityKind.Custom && string.IsNullOrWhiteSpace(request.CustomTitle))
            {
                return Results.BadRequest(new { message = "CustomTitle is required for Custom plan items." });
            }
            if (request.EntityKind != EntityKind.Custom && request.EntityId is null)
            {
                return Results.BadRequest(new { message = "EntityId is required for non-Custom plan items." });
            }

            var item = new DailyPlanItem
            {
                PlanDate = request.PlanDate,
                EntityKind = request.EntityKind,
                EntityId = request.EntityId,
                CustomTitle = request.CustomTitle,
                SortOrder = request.SortOrder,
                CreatedUtc = DateTime.UtcNow,
            };
            db.DailyPlanItems.Add(item);
            await db.SaveChangesAsync();

            var dto = (await ResolveDtosAsync([item], db)).Single();
            return Results.Created($"/api/planner/{item.Id}", dto);
        });

        group.MapPatch("/{id:int}/done", async (int id, MarkDoneRequest request, AppDbContext db, ProgressService progress) =>
        {
            var item = await db.DailyPlanItems.FindAsync(id);
            if (item is null) return Results.NotFound();

            item.IsDone = request.Done;
            item.DoneUtc = request.Done ? DateTime.UtcNow : null;
            await db.SaveChangesAsync();

            if (request.Done)
            {
                if (item.EntityKind == EntityKind.Custom)
                {
                    // No backing entity to complete, but it's still real
                    // activity — count it toward today's streak directly.
                    await progress.IncrementStreakAsync(DateOnly.FromDateTime(DateTime.UtcNow));
                }
                else if (item.EntityId is { } entityId)
                {
                    await progress.SetCompletionAsync(item.EntityKind, entityId, true);
                }
            }

            return Results.NoContent();
        });

        group.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
        {
            var item = await db.DailyPlanItems.FindAsync(id);
            if (item is null) return Results.NotFound();

            db.DailyPlanItems.Remove(item);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }

    private static async Task<List<PlanItemDto>> ResolveDtosAsync(List<DailyPlanItem> items, AppDbContext db)
    {
        var result = new List<PlanItemDto>();

        foreach (var item in items)
        {
            var displayTitle = item.EntityKind switch
            {
                EntityKind.Custom => item.CustomTitle ?? "Untitled",
                EntityKind.Lesson => (await db.Lessons.FindAsync(item.EntityId))?.Title ?? "Unknown lesson",
                EntityKind.Resource => (await db.Resources.FindAsync(item.EntityId))?.Title ?? "Unknown resource",
                _ => item.EntityKind.ToString(),
            };

            result.Add(new PlanItemDto(
                item.Id, item.PlanDate, item.EntityKind, item.EntityId,
                item.CustomTitle, item.IsDone, item.SortOrder, displayTitle));
        }

        return result;
    }
}
