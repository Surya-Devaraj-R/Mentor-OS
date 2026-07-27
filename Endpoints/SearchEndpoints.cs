using MentorOS.Services;

namespace MentorOS.Endpoints;

public static class SearchEndpoints
{
    public static void MapSearchEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/search", async (string? q, SearchService search) =>
            Results.Ok(await search.SearchAsync(q ?? "")));
    }
}
