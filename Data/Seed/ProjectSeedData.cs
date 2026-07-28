using MentorOS.Models;
using MentorOS.Models.Enums;

namespace MentorOS.Data.Seed;

// Topic-level "production project roadmaps" — distinct from the existing
// module-level CapstoneProject "mini project". Seeded for 2 flagship topics
// (.NET, System Design) as the bounded slice for this pass; the remaining
// 8 topics keep their module-level mini-project only, for now.
public static class ProjectSeedData
{
    public static (List<LearningPathProject> Projects, List<ChecklistSeed> Checklists) BuildProjects(
        IReadOnlyDictionary<string, int> topicIdBySlug)
    {
        var dotNetProject = BuildDotNetProject(topicIdBySlug["dotnet"]);
        var systemDesignProject = BuildSystemDesignProject(topicIdBySlug["system-design"]);

        var checklists = new List<ChecklistSeed>
        {
            new(ChecklistOwnerKind.Project, "dotnet",
            [
                "Containerize the API with a multi-stage Dockerfile.",
                "Add JWT-based authentication so tasks are scoped per user.",
                "Deploy the container to a cloud compute service (App Service, ECS, Cloud Run, or similar).",
                "Add a CI/CD pipeline that builds, tests, and deploys on every push to main.",
                "Add structured logging and at least one basic health-check endpoint.",
                "Write a README covering architecture, how to run locally, and how it's deployed.",
            ]),
            new(ChecklistOwnerKind.Project, "system-design",
            [
                "Implement the core shorten + redirect endpoints with a real datastore.",
                "Add a cache in front of the redirect path, following the cache-aside pattern.",
                "Generate short codes with a collision-free strategy (counter + base62, or equivalent).",
                "Add basic rate limiting on the shorten endpoint to prevent abuse.",
                "Load-test the redirect path and document the results.",
                "Write up the design trade-offs you made and why, as if presenting to a team.",
            ]),
        };

        return ([dotNetProject, systemDesignProject], checklists);
    }

    private static LearningPathProject BuildDotNetProject(int topicId)
    {
        var now = DateTime.UtcNow;
        return new LearningPathProject
        {
            TopicId = topicId,
            Title = "Deploy a Production-Ready Task Tracker API",
            Description = """
                Take the Task Tracker mini-capstone from ASP.NET Core Basics all the way to a real deployed service: containerized, authenticated, monitored, and shipped through an automated pipeline — the same journey a real feature takes from "it works on my machine" to "it's running in production."
                """,
            PortfolioGuidance = """
                This project demonstrates the full engineering lifecycle, not just a working API — that's exactly what's worth highlighting. When presenting it:

                - Lead with the deployed URL and a 30-second demo, not the code first.
                - Call out the CI/CD pipeline explicitly — many portfolio projects run locally only; a real pipeline that builds/tests/deploys on every push is a meaningful differentiator.
                - Be ready to explain one trade-off you made under time constraints (e.g., "I used a simple API key instead of full OAuth because the scope didn't need multi-provider login") — naming trade-offs shows judgment, not just execution.
                """,
            ArchitectureDiagramFormat = DiagramFormat.StructuredSteps,
            ArchitectureDiagramBody = """
                [{"label":"Client"},{"label":"CI/CD Pipeline","note":"build, test, deploy on push"},{"label":"Container Registry"},{"label":"Cloud Compute","note":"runs the container"},{"label":"SQLite/Managed DB"}]
                """,
            CreatedUtc = now,
            UpdatedUtc = now,
            Milestones =
            [
                new ProjectMilestone { Title = "Containerize", Description = "Write a multi-stage Dockerfile and confirm the API runs identically in a container as it does locally.", SortOrder = 1 },
                new ProjectMilestone { Title = "Add Authentication", Description = "Add JWT-based auth so each user only sees and modifies their own tasks.", SortOrder = 2 },
                new ProjectMilestone { Title = "Automate the Pipeline", Description = "Add a CI/CD workflow that builds, runs tests, and deploys automatically on every push to main.", SortOrder = 3 },
                new ProjectMilestone { Title = "Deploy", Description = "Ship the container to a real cloud compute service, reachable at a public URL.", SortOrder = 4 },
                new ProjectMilestone { Title = "Add Observability", Description = "Add structured logging and a health-check endpoint so you can actually tell if the deployed service is healthy.", SortOrder = 5 },
            ],
        };
    }

    private static LearningPathProject BuildSystemDesignProject(int topicId)
    {
        var now = DateTime.UtcNow;
        return new LearningPathProject
        {
            TopicId = topicId,
            Title = "Build a Production URL Shortener",
            Description = """
                Turn the "Design a URL Shortener" interview question into a real, running service — implementing the exact load-balancer/cache-aside/collision-free-ID patterns from the System Design Fundamentals module, instead of only describing them on a whiteboard.
                """,
            PortfolioGuidance = """
                Whiteboard system design answers are hard to verify; a working implementation isn't. When presenting this project:

                - Show the actual redirect latency with and without the cache warmed up — a concrete before/after number is far more convincing than describing caching in the abstract.
                - Be ready to explain why you chose your specific short-code generation strategy over the alternatives (hash-based vs. counter-based) — this is almost always the first follow-up question.
                - Mention the rate limiting explicitly — it shows you're thinking about abuse and production-readiness, not just the happy path.
                """,
            ArchitectureDiagramFormat = DiagramFormat.StructuredSteps,
            ArchitectureDiagramBody = """
                [{"label":"Client"},{"label":"Load Balancer"},{"label":"App Servers","note":"stateless"},{"label":"Cache","note":"cache-aside on redirect"},{"label":"Datastore","note":"short code -> long URL"}]
                """,
            CreatedUtc = now,
            UpdatedUtc = now,
            Milestones =
            [
                new ProjectMilestone { Title = "Core Endpoints", Description = "Implement POST /shorten and GET /{code} against a real datastore.", SortOrder = 1 },
                new ProjectMilestone { Title = "Collision-Free IDs", Description = "Generate short codes with a counter + base62 encoding (or equivalent) that guarantees no collisions.", SortOrder = 2 },
                new ProjectMilestone { Title = "Add Caching", Description = "Put a cache in front of the redirect path using the cache-aside pattern, and measure the latency improvement.", SortOrder = 3 },
                new ProjectMilestone { Title = "Add Rate Limiting", Description = "Protect the shorten endpoint from abuse with a basic per-IP or per-user rate limit.", SortOrder = 4 },
                new ProjectMilestone { Title = "Load Test", Description = "Run a basic load test against the redirect path and document the results.", SortOrder = 5 },
            ],
        };
    }
}
