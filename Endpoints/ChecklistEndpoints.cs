using MentorOS.Contracts.Progress;
using MentorOS.Data;
using MentorOS.Models.Enums;
using MentorOS.Services;
using Microsoft.EntityFrameworkCore;

namespace MentorOS.Endpoints;

public static class ChecklistEndpoints
{
    public static void MapChecklistEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/checklist-items");

        group.MapPatch("/{id:int}/complete", async (int id, CompleteRequest request, AppDbContext db, ProgressService progress) =>
        {
            var exists = await db.ChecklistItems.AnyAsync(c => c.Id == id);
            if (!exists) return Results.NotFound();

            await progress.SetCompletionAsync(EntityKind.ChecklistItem, id, request.Completed);
            return Results.NoContent();
        });
    }
}
