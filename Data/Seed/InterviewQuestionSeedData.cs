using MentorOS.Models;
using MentorOS.Models.Enums;

namespace MentorOS.Data.Seed;

// One representative question per QuestionType — behavioral, system design,
// and a mock-interview-day checklist — as the seeded slice of what will
// eventually be a full interview-question bank.
public static class InterviewQuestionSeedData
{
    public static List<InterviewQuestion> BuildQuestions()
    {
        var now = DateTime.UtcNow;

        return
        [
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
                SortOrder = 2,
                CreatedUtc = now,
                UpdatedUtc = now,
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
                SortOrder = 3,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
        ];
    }
}
