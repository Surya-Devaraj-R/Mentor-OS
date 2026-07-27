namespace MentorOS.Contracts.Streaks;

public record StreakSummaryDto(int CurrentStreakDays, int LongestStreakDays);

public record StreakCalendarEntryDto(DateOnly Date, int ActivityCount);
