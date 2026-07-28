using MentorOS.Contracts.Checklists;
using MentorOS.Contracts.Projects;
using MentorOS.Data;
using MentorOS.Models.Enums;
using MentorOS.Services;
using Microsoft.EntityFrameworkCore;

namespace MentorOS.Endpoints;

public static class ProjectEndpoints
{
    public static void MapProjectEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/topics/{slug}/project", async (string slug, AppDbContext db, ProgressService progress) =>
        {
            var topic = await db.Topics.FirstOrDefaultAsync(t => t.Slug == slug);
            if (topic is null) return Results.NotFound();

            var project = await db.LearningPathProjects
                .Include(p => p.Milestones)
                .FirstOrDefaultAsync(p => p.TopicId == topic.Id);
            if (project is null) return Results.NotFound();

            var completedChecklistIds = await progress.GetCompletedIdsAsync(EntityKind.ChecklistItem);

            var milestones = project.Milestones
                .OrderBy(m => m.SortOrder)
                .Select(m => new ProjectMilestoneDto(m.Title, m.Description, m.SortOrder))
                .ToList();

            var checklist = await db.ChecklistItems
                .Where(c => c.OwnerKind == ChecklistOwnerKind.Project && c.OwnerId == project.Id)
                .OrderBy(c => c.SortOrder)
                .Select(c => new ChecklistItemDto(c.Id, c.Description, c.SortOrder, completedChecklistIds.Contains(c.Id)))
                .ToListAsync();

            var dto = new ProjectDetailDto(
                project.Title, project.Description, project.PortfolioGuidance,
                project.ArchitectureDiagramBody, project.ArchitectureDiagramFormat,
                milestones, checklist);

            return Results.Ok(dto);
        });
    }
}
