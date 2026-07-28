using MentorOS.Contracts.Exercises;
using MentorOS.Contracts.InterviewPrep;
using MentorOS.Contracts.Progress;
using MentorOS.Data;
using MentorOS.Models;
using MentorOS.Models.Enums;
using MentorOS.Services;
using Microsoft.EntityFrameworkCore;

namespace MentorOS.Endpoints;

public static class InterviewPrepEndpoints
{
    public static void MapInterviewPrepEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/interview-prep");

        group.MapGet("/questions", async (QuestionType? type, string? company, AppDbContext db, ProgressService progress) =>
        {
            var query = db.InterviewQuestions
                .Include(q => q.QuestionCompanies).ThenInclude(qc => qc.Company)
                .AsQueryable();

            if (type is not null) query = query.Where(q => q.QuestionType == type);
            if (!string.IsNullOrWhiteSpace(company))
            {
                query = query.Where(q => q.QuestionCompanies.Any(qc => qc.Company!.Slug == company));
            }

            var questions = await query.OrderBy(q => q.SortOrder).ToListAsync();
            var completedIds = await progress.GetCompletedIdsAsync(EntityKind.InterviewQuestion);

            return Results.Ok(questions.Select(q => ToDto(q, completedIds)));
        });

        group.MapGet("/questions/{id:int}", async (int id, AppDbContext db, ProgressService progress) =>
        {
            var question = await db.InterviewQuestions
                .Include(q => q.QuestionCompanies).ThenInclude(qc => qc.Company)
                .FirstOrDefaultAsync(q => q.Id == id);
            if (question is null) return Results.NotFound();

            var completedIds = await progress.GetCompletedIdsAsync(EntityKind.InterviewQuestion);
            return Results.Ok(ToDto(question, completedIds));
        });

        group.MapPatch("/questions/{id:int}/complete", async (int id, CompleteRequest request, AppDbContext db, ProgressService progress) =>
        {
            var exists = await db.InterviewQuestions.AnyAsync(q => q.Id == id);
            if (!exists) return Results.NotFound();

            await progress.SetCompletionAsync(EntityKind.InterviewQuestion, id, request.Completed);
            return Results.NoContent();
        });

        group.MapGet("/companies", async (AppDbContext db) =>
        {
            var companies = await db.Companies
                .OrderBy(c => c.Name)
                .Select(c => new CompanyDto(c.Id, c.Name, c.Slug, c.OverviewBody))
                .ToListAsync();

            return Results.Ok(companies);
        });

        group.MapGet("/coding-challenges", async (AppDbContext db) =>
        {
            var exercises = await db.Exercises
                .Include(e => e.Submissions)
                .Include(e => e.ExerciseTags).ThenInclude(et => et.Tag)
                .Where(e => e.IsInterviewChallenge)
                .OrderBy(e => e.SortOrder)
                .ToListAsync();

            var dtos = exercises.Select(e =>
            {
                var latest = e.Submissions.OrderByDescending(s => s.SubmittedUtc).FirstOrDefault();
                var tags = e.ExerciseTags.Select(et => et.Tag!.Name).ToList();
                return new ExerciseSummaryDto(
                    e.Id, e.Slug, e.Title, e.DifficultyLevel, e.ExerciseType,
                    e.IsInterviewChallenge, e.Language, latest?.SelfAssessment, tags);
            });

            return Results.Ok(dtos);
        });
    }

    private static InterviewQuestionDto ToDto(InterviewQuestion question, HashSet<int> completedIds)
    {
        var companies = question.QuestionCompanies.Select(qc => qc.Company!.Name).ToList();
        return new InterviewQuestionDto(
            question.Id, question.QuestionType, question.Title, question.PromptText,
            question.SuggestedApproach, question.SampleAnswer, question.SortOrder,
            completedIds.Contains(question.Id), companies);
    }
}
