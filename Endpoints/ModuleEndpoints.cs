using MentorOS.Contracts.Modules;
using MentorOS.Data;
using MentorOS.Models.Enums;
using MentorOS.Services;
using Microsoft.EntityFrameworkCore;

namespace MentorOS.Endpoints;

public static class ModuleEndpoints
{
    public static void MapModuleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/modules");

        group.MapGet("/{slug}", async (string slug, AppDbContext db, ProgressService progress) =>
        {
            var module = await db.Modules
                .Include(m => m.Lessons)
                .Include(m => m.Capstone)
                    .ThenInclude(c => c!.ChecklistItems)
                .FirstOrDefaultAsync(m => m.Slug == slug);

            if (module is null) return Results.NotFound();

            var completedLessonIds = await progress.GetCompletedIdsAsync(EntityKind.Lesson);

            var lessons = module.Lessons
                .OrderBy(l => l.SortOrder)
                .Select(l => new LessonSummaryDto(
                    l.Id, l.Slug, l.Title, l.Summary, l.SortOrder, l.EstimatedMinutes,
                    completedLessonIds.Contains(l.Id)))
                .ToList();

            CapstoneSummaryDto? capstone = module.Capstone is null
                ? null
                : new CapstoneSummaryDto(
                    module.Capstone.Title,
                    module.Capstone.Description,
                    module.Capstone.Requirements,
                    module.Capstone.ChecklistItems
                        .OrderBy(i => i.SortOrder)
                        .Select(i => new CapstoneChecklistItemDto(i.Id, i.Description, i.SortOrder))
                        .ToList());

            var dto = new ModuleDetailDto(
                module.Id, module.Slug, module.Title, module.Description,
                module.EstimatedMinutes, lessons, capstone);

            return Results.Ok(dto);
        });
    }
}
