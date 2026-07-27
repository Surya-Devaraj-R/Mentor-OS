using MentorOS.Contracts.Progress;
using MentorOS.Contracts.Resources;
using MentorOS.Data;
using MentorOS.Models.Enums;
using MentorOS.Services;
using Microsoft.EntityFrameworkCore;

namespace MentorOS.Endpoints;

public static class ResourceEndpoints
{
    public static void MapResourceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/resources");

        group.MapGet("/", async (AppDbContext db, ProgressService progress) =>
        {
            var completedIds = await progress.GetCompletedIdsAsync(EntityKind.Resource);
            var resources = await db.Resources.OrderBy(r => r.SortOrder).ToListAsync();

            var dtos = resources.Select(r => new ResourceDto(
                r.Id, r.Slug, r.Title, r.Label, r.Url, r.IconKey,
                r.LegacySectionTitle, completedIds.Contains(r.Id)));

            return Results.Ok(dtos);
        });

        group.MapPatch("/{id:int}/complete", async (int id, CompleteRequest request, AppDbContext db, ProgressService progress) =>
        {
            var exists = await db.Resources.AnyAsync(r => r.Id == id);
            if (!exists) return Results.NotFound();

            await progress.SetCompletionAsync(EntityKind.Resource, id, request.Completed);
            return Results.NoContent();
        });
    }
}
