using MentorOS.Models;
using MentorOS.Models.Enums;

namespace MentorOS.Data.Seed;

// A real interview-question bank across all 4 QuestionTypes, with a few
// entries tagged to specific companies — the seeded slice of what will
// eventually be a much larger bank.
public static class InterviewQuestionSeedData
{
    public static (List<InterviewQuestion> Questions, List<Company> Companies) BuildQuestions()
    {
        var now = DateTime.UtcNow;

        var microsoft = new Company { Name = "Microsoft", Slug = "microsoft", OverviewBody = "Loop typically includes a recruiter screen, 1-2 technical phone screens, then an onsite/virtual onsite with coding, design, and a 'as appropriate' behavioral round. Culture emphasizes a growth mindset and collaboration." };
        var amazon = new Company { Name = "Amazon", Slug = "amazon", OverviewBody = "Every round is explicitly evaluated against the Leadership Principles — prepare STAR stories mapped to specific principles (Ownership, Bias for Action, Customer Obsession), not just generic behavioral stories." };
        var google = new Company { Name = "Google", Slug = "google", OverviewBody = "Emphasizes General Cognitive Ability alongside role-related knowledge — expect open-ended problem solving, not just memorized patterns. Onsite loops often include 4-5 rounds plus a dedicated 'Googleyness' behavioral round." };
        var meta = new Company { Name = "Meta", Slug = "meta", OverviewBody = "Known for a fast-paced loop and heavy emphasis on execution speed and impact. System design rounds often probe depth on one component rather than breadth across many." };

        var questions = new List<InterviewQuestion>
        {
            new InterviewQuestion
            {
                QuestionType = QuestionType.Behavioral,
                Title = "Tell Me About a Time You Disagreed With a Decision",
                PromptText = "Describe a situation where you disagreed with a decision made by a manager, teammate, or the broader team. How did you handle it, and what was the outcome?",
                SuggestedApproach = """
                    Use STAR: briefly set up the **Situation** and **Task**, then spend most of your time on the **Action** — specifically how you raised the disagreement (privately vs. in a meeting, with what evidence or reasoning) — and the **Result**, including what happened even if the decision didn't go your way.

                    Interviewers are listening for: did you raise the disagreement constructively and with evidence, rather than either staying silent or being combative? Did you commit to the final decision once it was made, even if it wasn't yours?
                    """,
                SampleAnswer = """
                    "On a project, I disagreed with a plan to skip integration tests to hit a deadline. I raised it directly with my lead, showing two recent incidents where missing integration tests had caused production bugs, and proposed a smaller scoped test suite as a middle ground instead of the full suite or none at all. My lead agreed to the scoped version. We shipped on time, and the scoped tests caught a real regression before release."
                    """,
                SortOrder = 1,
                CreatedUtc = now,
                UpdatedUtc = now,
                QuestionCompanies = [new InterviewQuestionCompany { Company = amazon }],
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Behavioral,
                Title = "Tell Me About a Time You Failed",
                PromptText = "Describe a time something you owned didn't go as planned. What happened, and what did you change afterward?",
                SuggestedApproach = """
                    Pick a real failure with a genuine lesson, not a disguised humble-brag ("I worked too hard"). Spend the least time on Situation/Task and the most on what you did differently in **Result** — interviewers are specifically listening for evidence of growth, not just an accurate account of what went wrong.
                    """,
                SampleAnswer = """
                    "I shipped a schema migration without a rollback plan, assuming it was low-risk. It caused a brief outage when an edge case in existing data broke the migration halfway through. I wrote the rollback script live under pressure, restored the previous state, and afterward introduced a team checklist requiring a tested rollback plan for any migration touching production data — we haven't had a repeat incident since."
                    """,
                SortOrder = 2,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Behavioral,
                Title = "Describe a Time You Had to Give Difficult Feedback",
                PromptText = "Tell me about a time you had to give a peer or teammate feedback that was hard to deliver. How did you approach it?",
                SuggestedApproach = """
                    Focus on the specific, behavior-focused framing you used (not vague criticism), how you delivered it (privately, with concrete examples), and how the other person responded. Avoid framing the story as "I was right and they were wrong" — interviewers want to see empathy alongside directness.
                    """,
                SampleAnswer = """
                    "A teammate's PRs were consistently missing test coverage for edge cases. Instead of raising it publicly in review comments, I asked for a quick 1:1, showed two specific examples where an untested edge case had caused a bug later, and we agreed on a shared checklist item for PR descriptions. Their PRs improved noticeably within a couple of weeks, and the checklist became a small team-wide habit."
                    """,
                SortOrder = 3,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Behavioral,
                Title = "Tell Me About a Time You Had to Prioritize Competing Deadlines",
                PromptText = "Describe a situation where you had more committed work than time available. How did you decide what to prioritize?",
                SuggestedApproach = """
                    Show the actual reasoning you used to prioritize (impact, urgency, who's blocked by what) and how you communicated the trade-off to stakeholders — interviewers are listening for judgment and proactive communication, not just "I worked extra hours."
                    """,
                SampleAnswer = """
                    "I had two features due the same week for different stakeholders. I estimated both, identified that one blocked three other engineers' work and the other didn't block anyone yet, and proactively told both stakeholders my plan and the revised timeline before either deadline arrived, rather than letting them find out at the deadline. Both agreed the sequencing made sense."
                    """,
                SortOrder = 4,
                CreatedUtc = now,
                UpdatedUtc = now,
                QuestionCompanies = [new InterviewQuestionCompany { Company = google }],
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Explain the Difference Between an Abstract Class and an Interface",
                PromptText = "In C# (or your language of choice), what's the actual difference between an abstract class and an interface, and when would you choose one over the other?",
                SuggestedApproach = """
                    Lead with the structural difference (single class inheritance vs. multiple interface implementation), then give a concrete example of when each fits — don't just recite the definitions.
                    """,
                SampleAnswer = """
                    "A class can inherit from only one class (abstract or not), but implement any number of interfaces. I reach for an abstract class when subtypes share real implementation and state (like a base `Shape` class with a shared `Name` property), and an interface when I just need a contract multiple unrelated types can fulfill (like `IComparable` across totally different types)."
                    """,
                SortOrder = 5,
                CreatedUtc = now,
                UpdatedUtc = now,
                QuestionCompanies = [new InterviewQuestionCompany { Company = microsoft }],
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Walk Through What Happens When You `await` a Task",
                PromptText = "Explain, step by step, what actually happens when your code hits an `await` on a Task in C#.",
                SuggestedApproach = """
                    Cover: the method returns control to its caller at the await point (it doesn't block a thread), a continuation is scheduled to run when the awaited task completes, and execution resumes on a captured context (or a thread pool thread, depending on `ConfigureAwait`). Mention why this matters for scalability (freeing threads instead of blocking them).
                    """,
                SampleAnswer = """
                    "When you await a Task that isn't already complete, the async method returns control to its caller immediately — the calling thread isn't blocked waiting. The compiler generates a state machine that captures where execution paused; when the awaited Task completes, its continuation resumes the method from that point, typically on a thread pool thread (or the original context, depending on `ConfigureAwait(true)` vs `false`). This is why async/await scales well for I/O-bound work — the thread is freed to do other work instead of sitting idle waiting."
                    """,
                SortOrder = 6,
                CreatedUtc = now,
                UpdatedUtc = now,
                QuestionCompanies = [new InterviewQuestionCompany { Company = microsoft }],
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "What's the Difference Between IEnumerable and IQueryable?",
                PromptText = "In the context of EF Core (or LINQ generally), explain the difference between IEnumerable<T> and IQueryable<T>, and why it matters for performance.",
                SuggestedApproach = """
                    Explain that IQueryable builds an expression tree that gets translated to a query at the data source (e.g., SQL), while IEnumerable operates on data already in memory. Give a concrete example of a bug caused by materializing (ToList()) too early, converting an IQueryable to IEnumerable before filtering is finished.
                    """,
                SampleAnswer = """
                    "IQueryable<T> represents a query that hasn't executed yet — chained .Where()/.OrderBy() calls build up an expression tree, and EF Core translates the whole thing into one SQL query when you finally materialize it with ToList() or similar. IEnumerable<T> is already-materialized data; filtering an IEnumerable happens in memory, in C#, row by row. The classic bug is calling .ToList() too early — db.Orders.ToList().Where(...) pulls every row into memory first, then filters in C#, instead of letting the database do the filtering."
                    """,
                SortOrder = 7,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "What's the Difference Between == and .Equals() in C#?",
                PromptText = "Explain how `==` and `.Equals()` differ in C#, especially across value types, reference types, and strings.",
                SuggestedApproach = """
                    Cover: for value types, both typically compare values. For reference types by default, both compare references (identity) unless overridden. `string` is a special case — it overrides both to compare content, not reference identity, even though it's a reference type.
                    """,
                SampleAnswer = """
                    "For value types, == and .Equals() both compare the actual values. For reference types, the default behavior of both is to compare references (are these the same object in memory), unless a class overrides Equals (and ideally ==) to compare content instead. string is a common source of confusion: it's a reference type, but both == and .Equals() are overridden to compare character content, which is why two different string instances with the same text still compare equal."
                    """,
                SortOrder = 8,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.SystemDesign,
                Title = "Design a URL Shortener",
                PromptText = "Design a service like bit.ly: given a long URL, generate a short URL that redirects to it. Design for scale.",
                SuggestedApproach = """
                    Follow a repeatable framework rather than jumping to a diagram:

                    1. **Clarify requirements** — read/write ratio (URL shorteners are read-heavy), expected scale (URLs/day, total URLs), custom aliases needed?, analytics needed?
                    2. **Estimate scale** — back-of-envelope QPS and storage, to justify later design choices.
                    3. **High-level design** — API endpoints (`POST /shorten`, `GET /{code}`), a datastore mapping short code to long URL, and a redirect service.
                    4. **Deep dive** — how are short codes generated (counter + base62 encoding vs. hash-based) and how do you avoid collisions; caching the hot redirect path; database choice and indexing.
                    5. **Wrap up** — bottlenecks and how you'd scale further (read replicas, CDN-level caching of redirects).
                    """,
                SampleAnswer = """
                    High-level: a stateless API layer behind a load balancer, a key-value store (short code -> long URL) as the source of truth, and a cache in front of it for the read-heavy redirect path — this reuses exactly the load-balancer/cache-aside pattern from the System Design Fundamentals lesson. Short codes generated from an auto-incrementing ID encoded in base62 avoid collision-checking entirely, at the cost of making IDs guessable (mitigated by not exposing the raw counter).
                    """,
                SortOrder = 9,
                CreatedUtc = now,
                UpdatedUtc = now,
                QuestionCompanies = [new InterviewQuestionCompany { Company = amazon }, new InterviewQuestionCompany { Company = google }],
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.SystemDesign,
                Title = "Design a Rate Limiter",
                PromptText = "Design a rate limiter that restricts each user to N requests per minute across a fleet of app servers.",
                SuggestedApproach = """
                    Clarify the exact limit semantics first (fixed window vs. rolling window — they behave very differently at the boundary). Propose a shared-state solution (not per-server in-memory counters, which wouldn't work correctly across multiple instances), discuss trade-offs of fixed-window (simple, but allows bursts at window boundaries) vs. sliding-window (more accurate, more storage), and mention where the check happens (API gateway vs. each service).
                    """,
                SampleAnswer = """
                    A shared cache (e.g., Redis) stores a counter keyed by user and time bucket, incremented atomically per request; once the count exceeds N for the current bucket, further requests are rejected with a 429. This works correctly across any number of stateless app servers, since the counter lives centrally rather than in any one server's memory — directly reusing the "app servers must be stateless, shared state lives in a cache" principle from System Design Fundamentals.
                    """,
                SortOrder = 10,
                CreatedUtc = now,
                UpdatedUtc = now,
                QuestionCompanies = [new InterviewQuestionCompany { Company = amazon }, new InterviewQuestionCompany { Company = meta }],
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.MockInterviewChecklist,
                Title = "Onsite Interview Day Checklist",
                PromptText = "A pre-flight checklist to run through the day before (and morning of) an onsite/virtual onsite loop.",
                SuggestedApproach = """
                    - Re-read the job description and note 2-3 things to weave into your questions for the interviewers.
                    - Have 5-6 STAR stories ready to go, not memorized verbatim — know the beats, not a script.
                    - Confirm the loop structure and interviewer names/roles if shared in advance.
                    - Prepare 2-3 thoughtful questions per interviewer (avoid generic ones answerable by the company website).
                    - For virtual loops: test your camera/mic/screen-share setup and have a notepad + water nearby.
                    - Eat something beforehand — a 4-5 hour loop on an empty stomach measurably hurts focus.
                    - Between interviews, take 2 minutes to reset rather than immediately replaying the previous round in your head.
                    """,
                SortOrder = 11,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.MockInterviewChecklist,
                Title = "Take-Home / Async Project Submission Checklist",
                PromptText = "A checklist for submitting a take-home coding assignment or async project as part of an interview loop.",
                SuggestedApproach = """
                    - Include a README explaining how to run the project, and any assumptions/trade-offs you made under time constraints.
                    - Add at least a few automated tests — an untested take-home submission is a missed opportunity to demonstrate testing habits.
                    - Handle at least the obvious edge cases (empty input, invalid input) — don't only implement the happy path.
                    - Keep commit history real and incremental rather than one giant commit — reviewers often look at how you worked, not just the final diff.
                    - If you ran out of time, say so explicitly in the README rather than leaving a reviewer to guess whether something was intentionally skipped or simply missed.
                    """,
                SortOrder = 12,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
        };

        return (questions, [microsoft, amazon, google, meta]);
    }
}
