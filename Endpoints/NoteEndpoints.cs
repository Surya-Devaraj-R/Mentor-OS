using MentorOS.Contracts.Notes;
using MentorOS.Data;
using MentorOS.Models;
using Microsoft.EntityFrameworkCore;

namespace MentorOS.Endpoints;

public static class NoteEndpoints
{
    public static void MapNoteEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/notes");

        group.MapGet("/", async (int? lessonId, string? search, AppDbContext db) =>
        {
            var query = db.Notes.Include(n => n.Lesson).AsQueryable();

            if (lessonId is not null)
            {
                query = query.Where(n => n.LessonId == lessonId);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(n => n.Body.Contains(search) || (n.Title != null && n.Title.Contains(search)));
            }

            var notes = await query.OrderByDescending(n => n.UpdatedUtc).ToListAsync();
            return Results.Ok(notes.Select(ToDto));
        });

        group.MapGet("/{id:int}", async (int id, AppDbContext db) =>
        {
            var note = await db.Notes.Include(n => n.Lesson).FirstOrDefaultAsync(n => n.Id == id);
            return note is null ? Results.NotFound() : Results.Ok(ToDto(note));
        });

        group.MapPost("/", async (CreateNoteRequest request, AppDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(request.Body))
            {
                return Results.BadRequest(new { message = "Body is required." });
            }

            var now = DateTime.UtcNow;
            var note = new Note
            {
                LessonId = request.LessonId,
                Title = request.Title,
                Body = request.Body,
                CreatedUtc = now,
                UpdatedUtc = now,
            };
            db.Notes.Add(note);
            await db.SaveChangesAsync();

            await db.Entry(note).Reference(n => n.Lesson).LoadAsync();
            return Results.Created($"/api/notes/{note.Id}", ToDto(note));
        });

        group.MapPut("/{id:int}", async (int id, UpdateNoteRequest request, AppDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(request.Body))
            {
                return Results.BadRequest(new { message = "Body is required." });
            }

            var note = await db.Notes.Include(n => n.Lesson).FirstOrDefaultAsync(n => n.Id == id);
            if (note is null) return Results.NotFound();

            note.Title = request.Title;
            note.Body = request.Body;
            note.UpdatedUtc = DateTime.UtcNow;
            await db.SaveChangesAsync();

            return Results.Ok(ToDto(note));
        });

        group.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
        {
            var note = await db.Notes.FindAsync(id);
            if (note is null) return Results.NotFound();

            db.Notes.Remove(note);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }

    private static NoteDto ToDto(Note note) =>
        new(note.Id, note.LessonId, note.Lesson?.Title, note.Title, note.Body, note.CreatedUtc, note.UpdatedUtc);
}
