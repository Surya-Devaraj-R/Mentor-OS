using MentorOS.Services;

namespace MentorOS.Endpoints;

public static class StreakEndpoints
{
    public static void MapStreakEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/streaks");

        group.MapGet("/current", async (ProgressService progress) =>
            Results.Ok(await progress.GetStreakSummaryAsync()));

        group.MapGet("/calendar", async (DateOnly from, DateOnly to, ProgressService progress) =>
            Results.Ok(await progress.GetStreakCalendarAsync(from, to)));
    }
}
