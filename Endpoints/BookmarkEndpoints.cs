using MentorOS.Contracts.Bookmarks;
using MentorOS.Data;
using MentorOS.Models;
using MentorOS.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace MentorOS.Endpoints;

public static class BookmarkEndpoints
{
    public static void MapBookmarkEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/bookmarks");

        group.MapGet("/", async (EntityKind? entityKind, AppDbContext db) =>
        {
            var query = db.Bookmarks.AsQueryable();
            if (entityKind is not null)
            {
                query = query.Where(b => b.EntityKind == entityKind);
            }

            var bookmarks = await query.OrderByDescending(b => b.CreatedUtc).ToListAsync();
            var dtos = new List<BookmarkDto>();
            foreach (var bookmark in bookmarks)
            {
                dtos.Add(await ToDtoAsync(bookmark, db));
            }

            return Results.Ok(dtos);
        });

        group.MapPost("/", async (CreateBookmarkRequest request, AppDbContext db) =>
        {
            var exists = await db.Bookmarks.AnyAsync(b => b.EntityKind == request.EntityKind && b.EntityId == request.EntityId);
            if (exists)
            {
                return Results.BadRequest(new { message = "This item is already bookmarked." });
            }

            var bookmark = new Bookmark
            {
                EntityKind = request.EntityKind,
                EntityId = request.EntityId,
                Note = request.Note,
                CreatedUtc = DateTime.UtcNow,
            };
            db.Bookmarks.Add(bookmark);
            await db.SaveChangesAsync();

            return Results.Created($"/api/bookmarks/{bookmark.Id}", await ToDtoAsync(bookmark, db));
        });

        group.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
        {
            var bookmark = await db.Bookmarks.FindAsync(id);
            if (bookmark is null) return Results.NotFound();

            db.Bookmarks.Remove(bookmark);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }

    private static async Task<BookmarkDto> ToDtoAsync(Bookmark bookmark, AppDbContext db)
    {
        var displayTitle = bookmark.EntityKind switch
        {
            EntityKind.Topic => (await db.Topics.FindAsync(bookmark.EntityId))?.Title,
            EntityKind.Module => (await db.Modules.FindAsync(bookmark.EntityId))?.Title,
            EntityKind.Lesson => (await db.Lessons.FindAsync(bookmark.EntityId))?.Title,
            EntityKind.Resource => (await db.Resources.FindAsync(bookmark.EntityId))?.Title,
            _ => null,
        } ?? $"{bookmark.EntityKind} #{bookmark.EntityId}";

        return new BookmarkDto(bookmark.Id, bookmark.EntityKind, bookmark.EntityId, bookmark.Note, displayTitle, bookmark.CreatedUtc);
    }
}
