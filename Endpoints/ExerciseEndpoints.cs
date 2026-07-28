using MentorOS.Contracts.Exercises;
using MentorOS.Data;
using MentorOS.Models;
using MentorOS.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace MentorOS.Endpoints;

public static class ExerciseEndpoints
{
    public static void MapExerciseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/exercises");

        group.MapGet("/", async (int? lessonId, int? topicId, string? tag, DifficultyLevel? difficulty, bool? interviewOnly, AppDbContext db) =>
        {
            var query = db.Exercises
                .Include(e => e.Submissions)
                .Include(e => e.ExerciseTags).ThenInclude(et => et.Tag)
                .AsQueryable();

            if (lessonId is not null) query = query.Where(e => e.LessonId == lessonId);
            if (difficulty is not null) query = query.Where(e => e.DifficultyLevel == difficulty);
            if (interviewOnly == true) query = query.Where(e => e.IsInterviewChallenge);
            if (topicId is not null) query = query.Where(e => e.Lesson != null && e.Lesson.Module!.TopicId == topicId);
            if (!string.IsNullOrWhiteSpace(tag)) query = query.Where(e => e.ExerciseTags.Any(et => et.Tag!.Name == tag));

            var exercises = await query.OrderBy(e => e.SortOrder).ToListAsync();
            return Results.Ok(exercises.Select(ToSummaryDto));
        });

        group.MapGet("/{slug}", async (string slug, AppDbContext db) =>
        {
            var exercise = await db.Exercises
                .Include(e => e.Solutions)
                .Include(e => e.Submissions)
                .Include(e => e.Hints)
                .Include(e => e.ExerciseTags).ThenInclude(et => et.Tag)
                .FirstOrDefaultAsync(e => e.Slug == slug);

            return exercise is null ? Results.NotFound() : Results.Ok(ToDetailDto(exercise));
        });

        group.MapPost("/{id:int}/submissions", async (int id, CreateSubmissionRequest request, AppDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(request.SubmittedCode))
            {
                return Results.BadRequest(new { message = "SubmittedCode is required." });
            }

            var exercise = await db.Exercises
                .Include(e => e.Solutions)
                .Include(e => e.Submissions)
                .Include(e => e.Hints)
                .Include(e => e.ExerciseTags).ThenInclude(et => et.Tag)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (exercise is null) return Results.NotFound();

            var submission = new ExerciseSubmission
            {
                ExerciseId = id,
                SubmittedCode = request.SubmittedCode,
                Notes = request.Notes,
                SelfAssessment = request.SelfAssessment,
                AttemptNumber = exercise.Submissions.Count + 1,
                SubmittedUtc = DateTime.UtcNow,
            };
            // No manual exercise.Submissions.Add(submission) here: EF Core's
            // change tracker already performs relationship fixup for a new
            // entity whose FK matches an already-tracked, already-loaded
            // parent collection — adding it again would duplicate it in the
            // in-memory list (though not in the database).
            db.ExerciseSubmissions.Add(submission);
            await db.SaveChangesAsync();

            return Results.Created($"/api/exercises/{exercise.Slug}", ToDetailDto(exercise));
        });
    }

    private static ExerciseSummaryDto ToSummaryDto(Exercise exercise)
    {
        var latest = exercise.Submissions.OrderByDescending(s => s.SubmittedUtc).FirstOrDefault();
        var tags = exercise.ExerciseTags.Select(et => et.Tag!.Name).ToList();
        return new ExerciseSummaryDto(
            exercise.Id, exercise.Slug, exercise.Title, exercise.DifficultyLevel, exercise.ExerciseType,
            exercise.IsInterviewChallenge, exercise.Language, latest?.SelfAssessment, tags);
    }

    private static ExerciseDetailDto ToDetailDto(Exercise exercise)
    {
        var solutions = exercise.Solutions
            .OrderBy(s => s.SortOrder)
            .Select(s => new ExerciseSolutionDto(
                s.Id, s.ApproachTitle, s.Explanation, s.SolutionCode, s.Language, s.TimeComplexity, s.SpaceComplexity, s.SortOrder))
            .ToList();

        var submissions = exercise.Submissions
            .OrderByDescending(s => s.SubmittedUtc)
            .Select(s => new ExerciseSubmissionDto(s.Id, s.SubmittedCode, s.Notes, s.SelfAssessment, s.AttemptNumber, s.SubmittedUtc))
            .ToList();

        var hints = exercise.Hints.OrderBy(h => h.SortOrder).Select(h => h.Text).ToList();
        var tags = exercise.ExerciseTags.Select(et => et.Tag!.Name).ToList();

        return new ExerciseDetailDto(
            exercise.Id, exercise.Slug, exercise.Title, exercise.Prompt, exercise.DifficultyLevel, exercise.ExerciseType,
            exercise.StarterCode, exercise.Language, exercise.IsInterviewChallenge, exercise.FollowUpQuestions,
            tags, hints, solutions, submissions);
    }
}
