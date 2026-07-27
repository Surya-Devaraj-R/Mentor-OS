using MentorOS.Models;
using Microsoft.EntityFrameworkCore;

namespace MentorOS.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<CompletionRecord> CompletionRecords => Set<CompletionRecord>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<LessonContentBlock> LessonContentBlocks => Set<LessonContentBlock>();
    public DbSet<CapstoneProject> CapstoneProjects => Set<CapstoneProject>();
    public DbSet<CapstoneChecklistItem> CapstoneChecklistItems => Set<CapstoneChecklistItem>();
    public DbSet<StreakDay> StreakDays => Set<StreakDay>();
    public DbSet<DailyPlanItem> DailyPlanItems => Set<DailyPlanItem>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<Bookmark> Bookmarks => Set<Bookmark>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<ExerciseSolution> ExerciseSolutions => Set<ExerciseSolution>();
    public DbSet<ExerciseSubmission> ExerciseSubmissions => Set<ExerciseSubmission>();
    public DbSet<ExerciseTag> ExerciseTags => Set<ExerciseTag>();
    public DbSet<InterviewQuestion> InterviewQuestions => Set<InterviewQuestion>();
    public DbSet<InterviewQuestionTag> InterviewQuestionTags => Set<InterviewQuestionTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
