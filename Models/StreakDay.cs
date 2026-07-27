namespace MentorOS.Models;

// Denormalized daily activity counter, upserted by ProgressService whenever a
// completion or planner-done event happens on that date. Lets the dashboard
// and streak calculation do O(1) lookups per day instead of aggregating an
// ever-growing completion log on every read.
public class StreakDay
{
    public int Id { get; set; }
    public DateOnly ActivityDate { get; set; }
    public int ActivityCount { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }
}
