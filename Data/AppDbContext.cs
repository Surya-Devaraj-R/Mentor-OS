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
    public DbSet<ChecklistItem> ChecklistItems => Set<ChecklistItem>();
    public DbSet<StreakDay> StreakDays => Set<StreakDay>();
    public DbSet<DailyPlanItem> DailyPlanItems => Set<DailyPlanItem>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<Bookmark> Bookmarks => Set<Bookmark>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<ExerciseSolution> ExerciseSolutions => Set<ExerciseSolution>();
    public DbSet<ExerciseSubmission> ExerciseSubmissions => Set<ExerciseSubmission>();
    public DbSet<ExerciseTag> ExerciseTags => Set<ExerciseTag>();
    public DbSet<ExerciseHint> ExerciseHints => Set<ExerciseHint>();
    public DbSet<InterviewQuestion> InterviewQuestions => Set<InterviewQuestion>();
    public DbSet<InterviewQuestionTag> InterviewQuestionTags => Set<InterviewQuestionTag>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<InterviewQuestionCompany> InterviewQuestionCompanies => Set<InterviewQuestionCompany>();
    public DbSet<LessonPrerequisite> LessonPrerequisites => Set<LessonPrerequisite>();
    public DbSet<LessonObjective> LessonObjectives => Set<LessonObjective>();
    public DbSet<LessonReferenceLink> LessonReferenceLinks => Set<LessonReferenceLink>();
    public DbSet<QuizQuestion> QuizQuestions => Set<QuizQuestion>();
    public DbSet<QuizOption> QuizOptions => Set<QuizOption>();
    public DbSet<LearningPathProject> LearningPathProjects => Set<LearningPathProject>();
    public DbSet<ProjectMilestone> ProjectMilestones => Set<ProjectMilestone>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
