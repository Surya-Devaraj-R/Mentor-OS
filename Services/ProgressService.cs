using MentorOS.Contracts.Progress;
using MentorOS.Contracts.Streaks;
using MentorOS.Data;
using MentorOS.Models;
using MentorOS.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace MentorOS.Services;

public class ProgressService(AppDbContext db)
{
    public async Task<HashSet<int>> GetCompletedIdsAsync(EntityKind kind)
    {
        var ids = await db.CompletionRecords
            .Where(c => c.EntityKind == kind)
            .Select(c => c.EntityId)
            .ToListAsync();

        return ids.ToHashSet();
    }

    public async Task SetCompletionAsync(EntityKind kind, int entityId, bool completed)
    {
        var existing = await db.CompletionRecords
            .FirstOrDefaultAsync(c => c.EntityKind == kind && c.EntityId == entityId);

        if (completed && existing is null)
        {
            var now = DateTime.UtcNow;
            db.CompletionRecords.Add(new CompletionRecord { EntityKind = kind, EntityId = entityId, CompletedUtc = now });
            await db.SaveChangesAsync();
            await IncrementStreakAsync(DateOnly.FromDateTime(now));
        }
        else if (!completed && existing is not null)
        {
            var completedDate = DateOnly.FromDateTime(existing.CompletedUtc);
            db.CompletionRecords.Remove(existing);
            await db.SaveChangesAsync();
            await DecrementStreakAsync(completedDate);
        }
    }

    public async Task<ProgressSummaryDto> GetSummaryAsync()
    {
        // Total/completed cover every entity type CompletionRecord actually
        // tracks. Exercises use self-assessment via ExerciseSubmission
        // instead of CompletionRecord, so they aren't counted here.
        var totalResources = await db.Resources.CountAsync();
        var totalLessons = await db.Lessons.CountAsync();
        var totalInterviewQuestions = await db.InterviewQuestions.CountAsync();
        var total = totalResources + totalLessons + totalInterviewQuestions;

        var completed = await db.CompletionRecords.CountAsync(c =>
            c.EntityKind == EntityKind.Resource ||
            c.EntityKind == EntityKind.Lesson ||
            c.EntityKind == EntityKind.InterviewQuestion);

        var percent = total == 0 ? 0 : Math.Round(completed * 100.0 / total, 1);

        return new ProgressSummaryDto(total, completed, percent);
    }

    // Public so DailyPlanItem completion (including free-text Custom entries
    // with no backing entity) can also register a day's activity.
    public async Task IncrementStreakAsync(DateOnly date)
    {
        var streakDay = await db.StreakDays.FirstOrDefaultAsync(s => s.ActivityDate == date);
        var now = DateTime.UtcNow;

        if (streakDay is null)
        {
            db.StreakDays.Add(new StreakDay { ActivityDate = date, ActivityCount = 1, CreatedUtc = now, UpdatedUtc = now });
        }
        else
        {
            streakDay.ActivityCount++;
            streakDay.UpdatedUtc = now;
        }

        await db.SaveChangesAsync();
    }

    public async Task DecrementStreakAsync(DateOnly date)
    {
        var streakDay = await db.StreakDays.FirstOrDefaultAsync(s => s.ActivityDate == date);
        if (streakDay is null) return;

        streakDay.ActivityCount = Math.Max(0, streakDay.ActivityCount - 1);
        streakDay.UpdatedUtc = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    public async Task<StreakSummaryDto> GetStreakSummaryAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var activeDates = (await db.StreakDays
                .Where(s => s.ActivityCount > 0)
                .Select(s => s.ActivityDate)
                .ToListAsync())
            .ToHashSet();

        // One grace day: if today has no activity yet, the streak still
        // counts through yesterday rather than reading as broken at 7am.
        var cursor = activeDates.Contains(today) ? today : today.AddDays(-1);
        var current = 0;
        while (activeDates.Contains(cursor))
        {
            current++;
            cursor = cursor.AddDays(-1);
        }

        var longest = 0;
        if (activeDates.Count > 0)
        {
            var sortedDates = activeDates.OrderBy(d => d).ToList();
            var run = 1;
            longest = 1;
            for (var i = 1; i < sortedDates.Count; i++)
            {
                run = sortedDates[i] == sortedDates[i - 1].AddDays(1) ? run + 1 : 1;
                longest = Math.Max(longest, run);
            }
        }

        return new StreakSummaryDto(current, longest);
    }

    public async Task<List<StreakCalendarEntryDto>> GetStreakCalendarAsync(DateOnly from, DateOnly to)
    {
        return await db.StreakDays
            .Where(s => s.ActivityDate >= from && s.ActivityDate <= to)
            .OrderBy(s => s.ActivityDate)
            .Select(s => new StreakCalendarEntryDto(s.ActivityDate, s.ActivityCount))
            .ToListAsync();
    }
}
