using MentorOS.Contracts.Checklists;
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
                .Include(l => l.Objectives)
                .Include(l => l.ReferenceLinks)
                .Include(l => l.QuizQuestions).ThenInclude(q => q.Options)
                .Include(l => l.Prerequisites).ThenInclude(p => p.PrerequisiteLesson)
                .FirstOrDefaultAsync(l => l.Slug == slug);

            if (lesson is null) return Results.NotFound();

            var completedLessonIds = await progress.GetCompletedIdsAsync(EntityKind.Lesson);
            var completedChecklistIds = await progress.GetCompletedIdsAsync(EntityKind.ChecklistItem);
            var bookmark = await db.Bookmarks
                .FirstOrDefaultAsync(b => b.EntityKind == EntityKind.Lesson && b.EntityId == lesson.Id);

            var blocks = lesson.ContentBlocks
                .OrderBy(b => b.SortOrder)
                .Select(b => new LessonContentBlockDto(
                    b.Id, b.BlockType, b.Title, b.BodyFormat, b.Body, b.Language, b.SortOrder))
                .ToList();

            var objectives = lesson.Objectives.OrderBy(o => o.SortOrder).Select(o => o.Text).ToList();

            var prerequisites = lesson.Prerequisites
                .Where(p => p.PrerequisiteLesson is not null)
                .Select(p => new LessonPrerequisiteDto(p.PrerequisiteLesson!.Slug, p.PrerequisiteLesson.Title))
                .ToList();

            var referenceLinks = lesson.ReferenceLinks
                .OrderBy(r => r.SortOrder)
                .Select(r => new LessonReferenceLinkDto(r.Title, r.Url, r.LinkType))
                .ToList();

            var quiz = lesson.QuizQuestions
                .OrderBy(q => q.SortOrder)
                .Select(q => new QuizQuestionDto(
                    q.Id, q.QuestionText, q.Explanation,
                    q.Options.OrderBy(o => o.SortOrder).Select(o => new QuizOptionDto(o.Id, o.Text, o.IsCorrect)).ToList()))
                .ToList();

            var checklist = await db.ChecklistItems
                .Where(c => c.OwnerKind == ChecklistOwnerKind.Lesson && c.OwnerId == lesson.Id)
                .OrderBy(c => c.SortOrder)
                .Select(c => new ChecklistItemDto(c.Id, c.Description, c.SortOrder, completedChecklistIds.Contains(c.Id)))
                .ToListAsync();

            var dto = new LessonDetailDto(
                lesson.Id, lesson.Slug, lesson.Title, lesson.Summary,
                lesson.EstimatedMinutes, completedLessonIds.Contains(lesson.Id), bookmark?.Id,
                objectives, prerequisites, blocks, quiz, checklist, referenceLinks);

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
