using MentorOS.Contracts.Progress;
using MentorOS.Services;

namespace MentorOS.Endpoints;

public static class ProgressEndpoints
{
    public static void MapProgressEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/progress");

        group.MapGet("/summary", async (ProgressService progress) =>
            Results.Ok(await progress.GetSummaryAsync()));

        group.MapPost("/complete", async (CompleteEntityRequest request, ProgressService progress) =>
        {
            await progress.SetCompletionAsync(request.EntityKind, request.EntityId, request.Completed);
            return Results.NoContent();
        });
    }
}
