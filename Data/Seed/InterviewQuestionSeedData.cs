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
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Why Does the Request Object Come Through as Null?",
                PromptText = "A client POSTs data to your endpoint, but the complex request object parameter is null when your action runs — what's happening and how do you fix it?",
                SuggestedApproach = """
                    Walk through the binding pipeline in order of likelihood:
                    - Check the Content-Type header first — if it isn't `application/json` (or another registered input formatter's media type), the JSON formatter never runs and the parameter binds to null with no exception thrown.
                    - Confirm the HTTP verb actually carries a body — a GET request's body is often stripped by clients, proxies, or the framework itself.
                    - Verify the body isn't empty or malformed JSON (a parse failure silently produces null rather than throwing, unless `[ApiController]` triggers automatic 400 validation).
                    - For MVC controllers without `[ApiController]`, confirm `[FromBody]` is explicitly present — automatic inference of body binding for complex types only happens with `[ApiController]`.
                    - Rule out multiple `[FromBody]` parameters on the same action — only one is allowed, and having two silently breaks binding.
                    """,
                SampleAnswer = """
                    "The first thing I check is the Content-Type header, because that's the most common cause — if the client sends `text/plain` or omits Content-Type entirely, ASP.NET Core's JSON input formatter is never selected, so the body is simply never deserialized and the parameter comes through null instead of throwing an error. I'd also confirm the request is actually a POST or PUT with a body, since some clients or proxies strip bodies from GET requests. Next I'd check the raw JSON is well-formed — a parse exception on malformed JSON also surfaces as a null model rather than a crash in some pipeline configurations. If this is an older-style MVC controller without `[ApiController]`, I'd double check `[FromBody]` is explicitly on the parameter, since without the attribute or without `[ApiController]`'s inference, the framework defaults to trying to bind from form/query data instead of the body. Finally, I'd make sure there's only one `[FromBody]` parameter on the action — ASP.NET Core only allows a single body-bound parameter per action, and a second one will fail silently."
                    """,
                DiagramBody = """
                    [{"label":"Request arrives","note":"POST/PUT with a body"},{"label":"Check Content-Type header","note":"must be application/json (or a registered formatter type)"},{"label":"Formatter selected?","note":"no match -> parameter binds to null, no exception"},{"label":"JSON deserialized","note":"malformed JSON also silently yields null"},{"label":"Parameter populated","note":"only if every prior step succeeded"}]
                    """,
                DiagramFormat = DiagramFormat.StructuredSteps,
                SortOrder = 13,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Why Does a List<T> Parameter Come Through as Null?",
                PromptText = "You expect a `List<T>` action parameter to be populated from the request, but it's null — what typically causes this and how would you diagnose it?",
                SuggestedApproach = """
                    Focus on binding source mismatches for collections:
                    - If the list is expected from the query string, confirm the client repeats the key for each item (`?ids=1&ids=2`), not a single comma-separated value or bracketed syntax the default binder doesn't understand.
                    - If the list is expected from the body, confirm the client actually sends a JSON array (`[1,2,3]`) rather than wrapping it in an object, and that Content-Type is `application/json`.
                    - For complex-type lists (`List<SomeDto>`) coming from an action without `[ApiController]`, remember collections of complex types are not inferred as body-bound by default in some configurations — be explicit with `[FromBody]`.
                    - Check that the parameter name matches the key/binding prefix the client is using — a mismatched name gives you null, not an error.
                    """,
                SampleAnswer = """
                    "Nine times out of ten this is a binding-source mismatch. If the list is supposed to come from the query string, the model binder expects the key repeated per value, like `?ids=1&ids=2&ids=3` — if the client instead sends a single comma-separated string or a bracketed array syntax like `ids[]=1`, the default binder won't populate it and you get null. If it's supposed to come from the body, I'd check the client is sending an actual JSON array like `[1,2,3]` and not something like `{ "ids": [1,2,3] }` when the action expects the array to be the entire body. I'd also verify the Content-Type is `application/json` — same root cause as any other body-binding failure. And I'd double-check the parameter name itself; if the DTO or action parameter is named `Ids` but the client's query key or JSON property is `idList`, the binder has nothing to match and defaults to null rather than throwing."
                    """,
                SortOrder = 14,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Why Is a List<T> Parameter Empty Even Though the Client Sent Data?",
                PromptText = "The client insists they sent list data, and the parameter isn't null — it's an empty list. What's the difference from the null case, and what would you check?",
                SuggestedApproach = """
                    An empty (not null) list means the binder recognized the shape but found zero matching items — narrow it down:
                    - For form-urlencoded or query data, confirm the client is using the indexer format the binder expects (`Ids[0]=1&Ids[1]=2`) rather than a format it silently can't parse into elements.
                    - Check for key-name mismatches on the collection itself, e.g. client sends `Ids[]` but the parameter is named `Ids` with no bracket suffix support configured.
                    - For JSON bodies, confirm the array key's JSON property name matches the DTO property (case-insensitive matching covers case, but not different names like `item_ids` vs `ItemIds`).
                    - Rule out the client sending an empty array intentionally, or sending the data as a JSON string instead of a real array (double-encoding).
                    """,
                SampleAnswer = """
                    "Empty is different from null — it tells me the binder recognized this as a collection and successfully bound zero elements, so the mismatch is in how individual items are keyed, not in the overall shape. For form or query binding, I'd check the exact indexer format: ASP.NET Core expects something like `Ids[0]=1&Ids[1]=2`, and if the client is instead sending `Ids[]=1&Ids[]=2` or a single comma-separated string, the binder won't extract any elements. For JSON, I'd verify the array's property name matches the DTO exactly — case-insensitivity covers `Ids` vs `ids`, but not `item_ids` vs `ItemIds`. I'd also check whether the client accidentally serialized the array twice, so the server receives a JSON string containing an array literal instead of an actual array — that also binds to an empty collection rather than throwing. Finally I'd just log the raw request body to confirm what's actually arriving on the wire, since 'the client says they sent it' isn't the same as verifying it with a request trace."
                    """,
                SortOrder = 15,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Why Is an Id Field Always Coming Through as 0?",
                PromptText = "An `Id` property on your request DTO consistently binds to `0` no matter what the client sends — what are the likely causes?",
                SuggestedApproach = """
                    Remember that value types silently default rather than error when the binder finds no match:
                    - Check for a name mismatch between the route template placeholder (e.g. `{id}`) and the DTO/action parameter name (e.g. `ItemId`) — the binder just leaves it at `default(int)`.
                    - Check whether the client is only putting the id in the URL route and assuming it also populates the body DTO's `Id` property — route values and body properties are bound independently.
                    - Check for a missing or non-public setter on the `Id` property, or the DTO being constructed in a way that bypasses the binder (e.g. immutable record without a matching constructor parameter name).
                    - Confirm the client isn't sending the id as a JSON string when the property is typed as `int` in a context where lenient number handling isn't enabled — this can trigger a ModelState error rather than 0, so also check ModelState.
                    """,
                SampleAnswer = """
                    "The key thing to remember is that value types like `int` don't come through as null when binding fails — they just silently default to 0, so this bug is quieter than a reference-type null. My first suspicion is a name mismatch: if the route template is `{id}` but the action parameter or DTO property is named `ItemId`, the route value binder has nothing to match against `Id` and it stays at 0. My second suspicion is that the client is putting the id only in the URL and assuming it flows into the body DTO automatically — route binding and body binding are independent, so if the JSON body itself doesn't include an `Id` property, the deserialized DTO's `Id` defaults to 0 even though the URL clearly has an id in it. I'd also check the `Id` property has a public setter and isn't being excluded by `[JsonIgnore]` or a custom converter. If none of that explains it, I'd check ModelState — if the client is sending `Id` as a quoted string and the server can't convert it, that's often reported as a validation error rather than silently defaulting, so I'd want to confirm which behavior I'm actually seeing before assuming it's just a name mismatch."
                    """,
                SortOrder = 16,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Why Isn't a Route Parameter Binding Correctly?",
                PromptText = "A value embedded in the URL path isn't reaching your action parameter as expected — what would you check to figure out why the route parameter isn't binding?",
                SuggestedApproach = """
                    Work through route template and matching issues specifically:
                    - Confirm the route template placeholder name (e.g. `{orderId}`) matches the action parameter name exactly, or that `[FromRoute(Name = "...")]` is used to map a differently named parameter.
                    - Check route constraints (e.g. `{id:int}`) aren't rejecting a value that doesn't match the constraint, causing the route not to match at all (404) rather than binding null.
                    - Look for route ordering/precedence conflicts — a more general route or another controller's overlapping template intercepting the request before it reaches the intended action.
                    - Remember route values are always strings — if the target parameter is a complex type, it can't bind from a single route segment without a custom model binder or route-to-property mapping.
                    """,
                SampleAnswer = """
                    "First I check that the route template's placeholder name matches the action parameter's name character-for-character — routing matching is case-insensitive but it still needs to be the same name, so `{orderId}` in the template only binds to a parameter literally named `orderId` unless I've added `[FromRoute(Name = "orderId")]` to map it to a differently named parameter. Next I look at route constraints — something like `{id:int}` will make the route simply not match at all if a non-numeric value is sent, which shows up as a 404 rather than a binding failure, and that's a common source of confusion. I'd also check for route conflicts — if another controller or a more general attribute route template overlaps, ASP.NET Core's route precedence rules might be sending the request somewhere other than where the developer expects. And if the parameter is a complex type rather than a simple value, I'd point out that a single route segment can't bind to an object without a custom binder — route values are always strings, so route binding beyond simple types needs explicit handling."
                    """,
                SortOrder = 17,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Why Does a Query String Parameter Come Through as Null?",
                PromptText = "A value you expect to see in the query string isn't reaching your action parameter — it's null instead. What are the likely causes?",
                SuggestedApproach = """
                    Isolate whether this is a naming, encoding, or binding-source problem:
                    - Confirm the query string key name matches the parameter name (case-insensitive matching handles casing, but not different names like `user_name` vs `userName`).
                    - Check for URL encoding issues — special characters that aren't properly encoded can break the key/value pair the server sees.
                    - Confirm the value is genuinely present in the raw query string (log or inspect the raw URL) rather than trusting client-side assumptions.
                    - For complex types, remember `[FromQuery]` on an object binds each property from a matching query key by default — a nested/complex type without matching property names per key won't populate, and the outer object may still be non-null with all-null members.
                    - Rule out the value being sent in the request body instead of the query string, especially on POST requests where developers sometimes assume both are checked.
                    """,
                SampleAnswer = """
                    "I'd start with the simplest explanation: does the query key name actually match the parameter name? ASP.NET Core's query binding is case-insensitive, so `Name` and `name` are fine, but a mismatch like `user_name` in the URL against a parameter named `userName` won't bind — there's no automatic snake_case-to-camelCase translation for query strings. Next I'd check for encoding problems, especially with special characters like `&`, `+`, or `#` that need to be percent-encoded — if they aren't, the query string can get truncated or misparsed before it even reaches the model binder. I'd also just log the raw incoming URL to confirm the value is actually there, since it's common for a client bug to mean the parameter was never sent at all despite what the developer assumes. If this is a complex object being bound with `[FromQuery]`, I'd check each individual property name lines up with a query key, since the object itself might bind non-null while its properties are all null due to name mismatches on the nested properties. And finally, for POST requests, I'd rule out the value actually being sent in the body instead of the query string, since binding sources are independent and a value in the body doesn't get picked up by `[FromQuery]`."
                    """,
                SortOrder = 18,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Why Does ModelState.IsValid Return False When the Request Looks Correct?",
                PromptText = "Your action reports `ModelState.IsValid == false` even though the incoming request looks correct at a glance — how do you track down the real cause?",
                SuggestedApproach = """
                    Don't guess — inspect the actual ModelState errors first, then look for common root causes:
                    - Always enumerate `ModelState.Values.SelectMany(v => v.Errors)` first rather than assuming what's wrong — the real failing field is often not the one the developer suspects.
                    - Check for culture/format mismatches — decimals, dates, or numbers formatted with commas or different separators than the server's configured culture expects.
                    - Check for enum binding failures — the client sending a string that doesn't exactly match an enum member name (or its underlying numeric value if that's expected).
                    - Check whether a property silently bound to its default value due to a name/casing mismatch, then failed a `[Required]` or range validation attribute downstream — the "invalid" field is a symptom of a binding failure, not the real problem.
                    - Check nested/child object validation — a required property nested several levels deep can fail validation while the top-level object still "looks" fine to the developer.
                    """,
                SampleAnswer = """
                    "The first thing I do is stop guessing and actually enumerate `ModelState.Values.SelectMany(v => v.Errors)` to see exactly which field failed and why — 'the request looks correct' is a human assessment, not what the binder actually saw. A lot of the time the real error is a culture or format mismatch — for example a decimal sent as `1,234.56` failing to parse under a server culture that expects a plain period, or a date format that isn't ISO-8601. Enum properties are another common culprit — if the client sends a string that doesn't exactly match an enum member name, binding fails and gets recorded as a validation error rather than a clean model-binding failure. I've also seen cases where a property silently bound to its default value because of a name mismatch, and then failed a `[Required]` attribute — so the ModelState error points at a field that looks 'wrong' but the actual root cause is upstream in the binding, not the validation. And I always check nested objects specifically, since a required property three levels deep in a child DTO can flip `IsValid` to false while the top-level request payload looks completely fine on a quick read."
                    """,
                SortOrder = 19,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Why Isn't [FromBody] Working at All?",
                PromptText = "You've added `[FromBody]` to an action parameter but it never seems to bind anything, regardless of what the client sends — what would cause this?",
                SuggestedApproach = """
                    Look at structural/configuration issues rather than a single request's data:
                    - Check for more than one `[FromBody]` parameter on the same action — only one is permitted, and a second one breaks binding for both.
                    - Confirm `services.AddControllers()` (or equivalent) is registered and JSON input formatters haven't been removed or replaced in `Program.cs`/`Startup.cs`.
                    - Check the DTO type itself — a missing public parameterless constructor (for non-record types), all-private setters, or fields that aren't real properties can prevent System.Text.Json from populating it.
                    - Confirm the action is actually being reached (correct route/verb) — if the wrong action or a fallback route handles the request, `[FromBody]` on the intended action never gets a chance to run.
                    - Check for custom model binders or `[ModelBinder]` attributes elsewhere in the pipeline overriding the default body binding behavior.
                    """,
                SampleAnswer = """
                    "When `[FromBody]` doesn't work at all — not just for one payload, but consistently — I look at configuration and structure rather than the request data itself. First, I check whether there's a second `[FromBody]` parameter on the same action; ASP.NET Core only allows one, and having two silently breaks binding for both. Next I check the DTO type — if it's a plain class, does it have a public parameterless constructor and public setters? System.Text.Json needs a way to construct and populate the object; if it's all read-only properties without a matching constructor, deserialization can quietly produce a default instance. I'd also verify the app's `Program.cs` still has `AddControllers()` wired up normally and that no one removed the JSON input formatters or swapped in a custom formatter that doesn't handle the expected media type. I'd confirm the request is actually hitting the action I think it's hitting — sometimes another route or a catch-all is intercepting it first. And I'd check for any custom `[ModelBinder]` attribes or global binder provider registrations that might be overriding the default body-binding behavior for this parameter type."
                    """,
                SortOrder = 20,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Why Does [FromBody] Return Null Even Though a Body Was Sent?",
                PromptText = "The client swears they're sending a JSON body, and you can see it in the request, but the `[FromBody]` parameter still comes through null — what's the most likely cause?",
                SuggestedApproach = """
                    This is specifically a Content-Type / formatter-selection problem, distinct from "nothing binds at all":
                    - Check the request's Content-Type header first — if it's `text/plain`, missing entirely, or anything other than `application/json` (or another registered formatter's type), the JSON input formatter is never selected and the parameter binds to null even though bytes clearly arrived.
                    - This is extremely common with JavaScript `fetch()` calls that pass `body: JSON.stringify(data)` but forget to explicitly set `headers: { 'Content-Type': 'application/json' }` — fetch defaults to `text/plain;charset=UTF-8` in that case.
                    - Check whether the client sent `multipart/form-data` (common from HTML forms or file-upload libraries) while the server expects raw JSON — those are handled by a completely different formatter path.
                    - Confirm the server hasn't restricted `SupportedMediaTypes` on the JSON formatter in a way that excludes what the client is actually sending.
                    """,
                SampleAnswer = """
                    "This one is almost always a Content-Type mismatch, and it's sneaky because the body genuinely is on the wire — the server just never hands it to the JSON formatter. The most common real-world cause I've hit is a JavaScript `fetch()` call that does `body: JSON.stringify(payload)` but forgets to set `headers: { 'Content-Type': 'application/json' }` explicitly — fetch silently defaults to `text/plain;charset=UTF-8` in that situation, and since there's no input formatter registered for `text/plain` by default, ASP.NET Core can't deserialize it, so `[FromBody]` binds to null instead of throwing. I'd check the actual Content-Type header on the wire, not what the client code intends to send. I'd also check for the form-data variant of this bug — if the client is submitting through an HTML form or a library that defaults to `multipart/form-data`, that goes through a completely different binding path than JSON and `[FromBody]` won't pick it up at all. The fix is almost always on the client side: explicitly set the Content-Type header to `application/json` before sending."
                    """,
                SortOrder = 21,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Why Is a Nested Object in a Request DTO Null Even Though Outer Properties Bind Fine?",
                PromptText = "The top-level properties of your request DTO bind correctly, but a nested/child object property inside it is always null — what would you check?",
                SuggestedApproach = """
                    Since the outer object clearly deserializes, narrow the problem to the nested type or its property mapping:
                    - Check the nested object's property name in the incoming JSON against the DTO's nested property name — case-insensitive matching only covers casing, not naming-convention differences like `shipping_address` vs `ShippingAddress`.
                    - Confirm the client isn't sending the nested object as a JSON string (double-encoded) instead of an actual nested JSON object — that deserializes to null/default rather than throwing.
                    - Check the nested type itself has a public parameterless constructor and public setters (or matching constructor parameters if it's a record/immutable type).
                    - Look for `[JsonIgnore]`, custom `JsonConverter`, or interface/abstract-typed nested properties that need a converter to know how to construct the concrete type.
                    - Check `System.Text.Json`'s default max depth isn't being hit on a deeply nested/circular graph, which can cause silent truncation depending on configuration.
                    """,
                SampleAnswer = """
                    "Since the outer properties bind fine, I know the overall JSON is well-formed and the request is reaching the formatter — so I focus specifically on the nested type. The most common cause I've seen is a naming mismatch on the nested object's key: System.Text.Json's default case-insensitive matching covers things like `Address` vs `address`, but it does not translate naming conventions, so if the client sends `shipping_address` and the DTO property is `ShippingAddress`, that nested object comes through as null while everything else binds because the top-level keys happened to match. Another one I've hit is the client accidentally double-serializing the nested object — sending it as an escaped JSON string instead of an actual object literal — which deserializes to null instead of erroring. I'd also check the nested class itself has a public parameterless constructor and public setters, since a nested type is just as subject to those requirements as the outer one. And if the nested property is typed as an interface or abstract class, I'd check whether a custom `JsonConverter` is registered to tell the deserializer which concrete type to construct — without one, System.Text.Json can't populate it and it stays null."
                    """,
                SortOrder = 22,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "API Returns an Empty List Even Though the Data Exists",
                PromptText = "An endpoint that lists records returns an empty array, but you can see the matching rows sitting in the database. What's going on, and how do you track it down?",
                SuggestedApproach = """
                    Work through the layers between the DB and the response: - Confirm the API is actually hitting the database you think it is (check the connection string per environment — appsettings.Development.json vs appsettings.json, or a LocalDB/file-based SQLite path that differs from where you inserted test data). - Check for a global query filter (e.g., a soft-delete `HasQueryFilter`) silently excluding the rows. - Check the WHERE clause for a type/case mismatch (comparing an int column to a string parameter, or a case-sensitive string comparison that behaves differently in SQLite vs in-memory). - Verify the query isn't running against a tracked, already-materialized in-memory collection instead of hitting the database again. - Log the generated SQL (EF Core logging or `ToQueryString()`) and run it directly against the database to isolate app logic from data/environment issues.
                    """,
                SampleAnswer = """
                    "First thing I check is whether the API is even pointed at the database I'm looking at — it's shockingly common to be inserting test rows into one SQLite file while the app's connection string in appsettings.Development.json points at a different one. Once I've ruled that out, I look for a global query filter — if the entity has `modelBuilder.Entity<T>().HasQueryFilter(x => !x.IsDeleted)` and the seeded rows have `IsDeleted` set weirdly, EF Core will silently strip them out of every query, including this one. Next I'll turn on EF Core's SQL logging or call `.ToQueryString()` on the query and run the exact generated SQL directly against the database — that tells me immediately whether it's a query-translation problem (like comparing an int column to a string parameter that gets client-evaluated oddly) or a genuine data/environment mismatch. In one case it turned out the WHERE clause used `.ToUpper()` on a nullable column and the query filter combined with a case-sensitivity difference between SQLite and our in-memory tests to filter everything out."
                    """,
                SortOrder = 23,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "SaveChanges() Runs but Nothing Actually Gets Saved",
                PromptText = "You call `SaveChanges()` (or `SaveChangesAsync()`) in your endpoint, no exception is thrown, but when you check the database the change never happened. What are the likely causes?",
                SuggestedApproach = """
                    Walk through the tracking and lifetime issues that cause this: - Fire-and-forget mistake: calling `SaveChangesAsync()` without `await`, so the request completes (and often the DbContext gets disposed) before the save actually finishes. - The entity was loaded with `AsNoTracking()`, so EF Core has no tracked snapshot to compare against and doesn't know anything changed. - The entity being modified is attached to a different `DbContext` instance than the one `SaveChanges()` is called on (common with manually-constructed contexts or incorrect DI scope). - The change tracker genuinely sees no changes because the values being "changed" already match the database values. - An exception is being swallowed by an overly broad try/catch around the SaveChanges call, or a surrounding transaction is rolled back afterward. Recommend checking `context.ChangeTracker.Entries()` state and the DI lifetime of the DbContext to confirm which of these applies.
                    """,
                SampleAnswer = """
                    "The most common cause I've hit is a missing `await` — someone calls `_context.SaveChangesAsync()` without awaiting it, the method returns, the HTTP response goes out, and the request-scoped DbContext gets disposed before the save ever completes, so it just silently never happens. Second most common is loading the entity with `AsNoTracking()` for a read and then trying to mutate and save it later — EF Core has no original snapshot to diff against, so `ChangeTracker.DetectChanges()` sees nothing to write. I've also seen it happen when code creates a brand-new `DbContext` instance to modify an entity that was loaded from a different, already-scoped context — the second context has never seen that entity, so there's nothing in its change tracker. To debug it, I'd inspect `context.ChangeTracker.Entries()` right before `SaveChanges()` to see what state EF thinks the entity is in — `Unchanged` when you expect `Modified` tells you immediately it's a tracking problem, not a database problem."
                    """,
                SortOrder = 24,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "PUT/Update Endpoint Doesn't Update the Record",
                PromptText = "A PUT endpoint returns 200 OK, but when you check the database the record still has its old values. What could cause this?",
                SuggestedApproach = """
                    Cover the common causes: - The entity was fetched with `AsNoTracking()` for validation, then the incoming DTO values were applied to that detached copy, which was never re-attached or marked `Modified` before `SaveChanges()`. - Calling `context.Update(entity)` on an entity whose Id is the CLR default (e.g., 0) makes EF Core treat it as a new row (insert) rather than an update, or throws/no-ops depending on setup. - A concurrency token (`RowVersion`/`[Timestamp]`) mismatch causes `DbUpdateConcurrencyException` that's being caught and swallowed instead of surfaced. - The code updates a projected DTO or an anonymous object instead of the actual tracked entity. - Two different DbContext instances are involved — one used to load, a different one (or none) used to save. Suggest checking the entity's `EntityState` right before `SaveChanges()` and confirming the Id used to load the entity matches the Id in the update payload.
                    """,
                SampleAnswer = """
                    "Nine times out of ten this is a tracking problem: the entity gets loaded — often with `AsNoTracking()` because it was fetched through a shared 'get by id' helper meant for reads — the controller copies the PUT payload's values onto that object, and then calls `SaveChanges()` expecting EF to notice. But since the entity was never attached with tracking, EF Core has no idea it changed, so `SaveChanges()` runs, finds zero modified entries, and returns 200 because nothing failed — it just had nothing to do. The fix is either to load the entity with a tracking query for updates, or explicitly call `context.Entry(entity).State = EntityState.Modified` after copying values onto a fresh instance. The other cause I check for is a stale RowVersion/concurrency token — if the client sends back an old RowVersion value, EF Core throws `DbUpdateConcurrencyException`, and if that's wrapped in a try/catch that just logs and returns 200, it looks exactly like a silent no-op update."
                    """,
                SortOrder = 25,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "DELETE Endpoint Returns 404 for a Record That Clearly Exists",
                PromptText = "Calling DELETE on a resource you can see in the database returns 404 Not Found. How do you figure out why?",
                SuggestedApproach = """
                    Cover the likely causes: - Route/model-binding mismatch: the route parameter type (e.g., `int id`) doesn't match the value being sent (a GUID or non-numeric string), so binding fails before your lookup code even runs. - Lookup is by the wrong key — querying by primary key when the client is passing a different unique identifier (e.g., a `Code` or external Id). - A global query filter (soft-delete flag) is excluding the record from the lookup even though the row physically exists. - The API is pointed at a different database/environment than the one being inspected. - Case sensitivity on a string key differs between what's stored and what's queried. - The 404 is coming from routing itself (no route matched) rather than from the "not found" branch of your controller logic — worth distinguishing with logging or by hitting the route directly. Suggest checking the raw request URL/route match first, then the exact WHERE clause used for the lookup.
                    """,
                SampleAnswer = """
                    "I'd first confirm the 404 is coming from my controller's not-found branch and not from routing itself — if the route template is `[HttpDelete("{id:int}")]` and the client sends a non-integer id, ASP.NET Core's routing constraint will reject the match before my code runs, and that also surfaces as 404. Assuming routing is fine, the next suspect is a soft-delete query filter — if the entity has `HasQueryFilter(x => !x.IsDeleted)` configured globally, and the record was already soft-deleted once before, then trying to look it up by Id for the delete operation will never find it, even though `SELECT * FROM Table` shows the row. I'd check that with `context.Set<T>().IgnoreQueryFilters().FirstOrDefault(...)` — if that finds it but the normal query doesn't, that's the smoking gun. Finally I'd double check I'm not looking the record up by the wrong key, like matching on primary key `Id` when the client is actually passing a business key such as `OrderNumber`."
                    """,
                SortOrder = 26,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "EF Core Returns Duplicate Rows for a Single Logical Record",
                PromptText = "A query for a single entity comes back with the same record repeated multiple times in the result set. What causes this and how do you fix it?",
                SuggestedApproach = """
                    Explain the classic cause: eagerly including two or more one-to-many navigation collections in a single query (e.g., `.Include(o => o.Items).Include(o => o.Comments)`) causes EF Core to generate one large SQL JOIN across both collections, producing a cartesian product — the parent row gets repeated once per combination of child rows. Cover the fixes: - Use `.AsSplitQuery()` so EF Core issues a separate SQL query per collection navigation instead of one joined query. - Restructure the query with `.Select()` projections to shape exactly the data needed instead of including full collections. - Note that calling `.Distinct()` on the results only masks the symptom and doesn't fix the underlying inefficient query. Mention that this is specifically an issue with multiple *collection* includes — a single collection include or multiple reference (one-to-one/many-to-one) includes don't cause it.
                    """,
                SampleAnswer = """
                    "This is almost always the cartesian-explosion problem with `Include()`. If I do `.Include(o => o.Items).Include(o => o.Comments)` where an order has, say, 3 items and 4 comments, EF Core's default single-query mode joins both collections into one SQL statement, and the database returns 12 rows for that one order — one for every item/comment combination — which EF then has to de-duplicate on the client for the parent entity, but you still see repeated child data or repeated rows if you're looking at raw results. The real fix is `.AsSplitQuery()`, which tells EF Core to issue separate SQL queries — one for orders, one for items, one for comments — and stitch them together in memory, avoiding the join blow-up entirely. I'd reach for `.AsSplitQuery()` as the default for any query with more than one collection `Include`, and reserve single-query mode for cases where I specifically want one round trip and know the collections are small."
                    """,
                DiagramBody = """
                    [{"label":"Order has 3 items, 4 comments"},{"label":".Include(Items).Include(Comments)","note":"default single-query mode"},{"label":"One SQL JOIN across both collections","note":"3 x 4 = 12 rows returned for one order"},{"label":"EF de-dupes the parent in memory","note":"but the row explosion already happened in the DB"},{"label":"Fix: .AsSplitQuery()","note":"separate queries per collection, no cartesian join"}]
                    """,
                DiagramFormat = DiagramFormat.StructuredSteps,
                SortOrder = 27,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "`.Include()` Isn't Loading the Related Data",
                PromptText = "You added `.Include(x => x.RelatedEntity)` to a query, but the returned object's navigation property is still null or empty. Why would that happen?",
                SuggestedApproach = """
                    Cover the common reasons `Include` gets silently dropped or ignored: - `IQueryable` methods don't mutate in place — if the `Include()` call's result isn't assigned back to the variable that's later executed (e.g., `query.Include(...)` on its own line without reassignment), the include never takes effect. - A `.Select()` projection to a DTO after the `Include()` strips it out — once you project, EF Core needs the navigation explicitly referenced inside the `Select`, not just included beforehand. - Wrong or misspelled navigation path in a multi-level `.ThenInclude()` chain, or including a property that isn't actually configured as a navigation in the model. - Serialization settings (e.g., JSON serializer ignoring cycles or the property) making it look like the data isn't there when it actually loaded fine. - Applying `.AsNoTracking()` isn't the cause by itself, but combined with lazy-loading assumptions can confuse the diagnosis — clarify Include is unrelated to lazy loading configuration.
                    """,
                SampleAnswer = """
                    "The first thing I check is whether the query variable actually captured the result of `.Include()` — since `IQueryable` methods are non-mutating, `query.Include(x => x.RelatedEntity);` as a statement by itself does nothing; it has to be `query = query.Include(x => x.RelatedEntity);` or chained directly into the final `ToListAsync()`. That's a surprisingly common copy-paste bug. If that's not it, I look at whether there's a `.Select()` projection afterward — if you do `.Include(x => x.RelatedEntity).Select(x => new Dto { Name = x.Name })`, the Include is essentially thrown away because the projection never references `RelatedEntity`, so EF Core doesn't even generate the join for it. Last, if it's a multi-level include like `.Include(x => x.Parent).ThenInclude(p => p.GrandParent)`, I double check the path is spelled and typed correctly, since a broken chain there just quietly loads the first level and stops."
                    """,
                SortOrder = 28,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Lazy Loading Isn't Working",
                PromptText = "You're relying on EF Core lazy loading to pull in a related entity when you access a navigation property, but it's coming back null instead of triggering a load. What's likely wrong?",
                SuggestedApproach = """
                    Cover the setup requirements and lifetime issue that break lazy loading: - The `Microsoft.EntityFrameworkCore.Proxies` package isn't installed, or `UseLazyLoadingProxies()` was never called in `OnConfiguring`/`AddDbContext` options. - The navigation property isn't declared `virtual`, so EF Core can't generate a proxy override for it. - The entity class or navigation property is `sealed`/non-overridable, preventing proxy generation. - The `DbContext` has already been disposed by the time the navigation property is accessed — very common in Web APIs where the entity is returned from the action and something (like JSON serialization) touches the navigation property after the request-scoped context has been disposed, causing either a null result or an `ObjectDisposedException`. - The entity was created with `new` instead of coming from the context, so it was never wrapped in a proxy in the first place.
                    """,
                SampleAnswer = """
                    "Lazy loading needs three things to actually work, so I check them in order. First, is the `Microsoft.EntityFrameworkCore.Proxies` package installed and is `optionsBuilder.UseLazyLoadingProxies()` actually wired up in the DbContext configuration — without that, EF just won't generate the dynamic proxy at all. Second, are the navigation properties marked `virtual` — EF Core's proxy works by subclassing your entity and overriding the navigation property getters, so a non-virtual property can never trigger a load. If both of those check out, the most common failure I see in web APIs specifically is that the DbContext is already disposed by the time the navigation is touched — the controller returns the entity, and something downstream, like JSON serialization walking the object graph, tries to access a lazy-loaded property after the request-scoped `DbContext` has already been torn down at the end of the request, which either silently returns null or throws `ObjectDisposedException`. In practice I generally avoid depending on lazy loading in Web APIs for exactly this reason and use explicit `.Include()` instead."
                    """,
                SortOrder = 29,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "`FirstOrDefault()` Returns Null When You're Sure the Data Exists",
                PromptText = "You call `FirstOrDefault()` with a filter you're confident matches an existing row, but it returns null. How do you debug this?",
                SuggestedApproach = """
                    Cover the likely causes: - The query is running against a different `DbContext`/database/environment than the one you're inspecting (dev vs test DB, or a not-yet-committed transaction in another context/connection that this query can't see). - A global query filter (e.g., soft-delete) is excluding the row. - A case-sensitivity or culture-specific string comparison mismatch — SQLite string comparisons can behave differently than in-memory LINQ-to-objects comparisons used in unit tests. - Comparing across mismatched types (nullable vs non-nullable, string vs numeric) causing the translated SQL WHERE clause to behave unexpectedly. - The predicate references navigation properties that require an `Include`/join that isn't present, causing the filter to silently evaluate against null related data. Recommend logging or calling `.ToQueryString()` to see the actual generated SQL and running it directly against the database to isolate the issue.
                    """,
                SampleAnswer = """
                    "My first move is to grab the actual SQL EF Core generated — either through logging or `query.ToQueryString()` — and run it directly against the database, because that immediately tells me whether this is a translation problem or a genuine data mismatch. A lot of the time it turns out to be a global query filter, like a soft-delete filter, quietly excluding the row from every query unless you call `.IgnoreQueryFilters()`. Another common one is a case-sensitivity mismatch — if the filter does `x.Email == email` and the underlying values differ only in casing, SQLite's default string comparison and an in-memory test double can behave completely differently, so a test can pass locally against an in-memory provider but fail against the real SQLite file. I've also seen this happen when the predicate touches a navigation property that wasn't included, so EF Core's translated WHERE clause is effectively comparing against null on the database side even though the related row exists — that one usually needs a `.Where()` rewritten to join explicitly or an `.Include()` added."
                    """,
                SortOrder = 30,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "A Specific SQL Query Is Taking ~20 Seconds to Run",
                PromptText = "One particular query in your API is consistently taking around 20 seconds, while everything else is fast. How do you investigate and fix it?",
                SuggestedApproach = """
                    Cover a structured diagnostic path: - Get the actual generated SQL (EF Core logging/`ToQueryString()`) and run `EXPLAIN QUERY PLAN` (SQLite) or the equivalent execution plan tool against it directly. - Check for a missing index on columns used in WHERE/JOIN/ORDER BY clauses, causing a full table scan. - Look for N+1 query patterns hiding behind what looks like one LINQ statement, or a cartesian-product join from multiple collection `.Include()`s inflating the row count before filtering. - Check whether a function or computed expression is applied to an indexed column in the WHERE clause (e.g., `.ToUpper()`), which prevents the index from being used. - Confirm the query isn't pulling back far more data/columns than needed (no projection, unnecessary Includes) — reducing the result set size directly reduces both DB and transfer time. - Consider whether the table has grown large enough that pagination is now required where it wasn't before.
                    """,
                SampleAnswer = """
                    "I always start by getting the exact SQL EF Core is sending — either from logging or `.ToQueryString()` — and then run `EXPLAIN QUERY PLAN` against it directly in the database, because that tells me immediately whether it's doing a full table scan. The most common root cause at 20-second scale is a missing index on a column being filtered or joined on — if the WHERE clause is filtering on `CustomerId` and there's no index on that column, SQLite has to scan every row. The second thing I look for is whether this 'single query' is actually hiding an N+1 pattern, for example a `foreach` over a list where each iteration triggers its own lazy-loaded query, or a cartesian-product join from including multiple collection navigations that's massively multiplying rows before any filtering happens. I'd also check whether a function is being applied to the filtered column, like `.Where(x => x.Email.ToUpper() == ...)`, since that stops SQLite from using an index that would otherwise apply. Once I identify the actual cause, the fix is almost always adding the right index, restructuring the query to avoid the row explosion, or projecting only the columns actually needed instead of pulling full entities."
                    """,
                DiagramBody = """
                    [{"label":"Capture generated SQL","note":"EF Core logging or .ToQueryString()"},{"label":"Run EXPLAIN QUERY PLAN","note":"reveals full table scan vs. index use"},{"label":"Missing index?","note":"most common cause at this scale"},{"label":"Hidden N+1 or cartesian Include?","note":"check for a foreach triggering per-row queries"},{"label":"Function on an indexed column?","note":"e.g. .ToUpper() defeats the index"},{"label":"Apply targeted fix","note":"add index, restructure query, or project only needed columns"}]
                    """,
                DiagramFormat = DiagramFormat.StructuredSteps,
                SortOrder = 31,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "How You'd Approach Optimizing a Slow API Endpoint End to End",
                PromptText = "Given an API endpoint that's noticeably slow under real usage, walk through your overall approach to diagnosing and speeding it up.",
                SuggestedApproach = """
                    Frame this as a process, not a single fix: - Measure first — use logging, APM tooling, or `Stopwatch`/EF Core logging to find out where the time is actually going (DB query, serialization, external call, etc.) rather than guessing. - Capture and analyze the generated SQL and its execution plan for missing indexes or full table scans. - Look for N+1 queries and multi-collection `Include()` cartesian products; fix with `.AsSplitQuery()`, restructured queries, or eager loading in one shaped query. - Add `.AsNoTracking()` for read-only queries to skip change-tracking overhead. - Project to DTOs with `.Select()` instead of returning full tracked entities, reducing both data transferred and mapping cost. - Add pagination for endpoints returning unbounded result sets. - Consider caching for expensive, infrequently-changing reads. - Make sure I/O (DB calls, HTTP calls to other services) is genuinely async and not blocking threads. - Re-measure after each change to confirm the fix actually moved the needle rather than assuming.
                    """,
                SampleAnswer = """
                    "I treat this as measure, hypothesize, fix, re-measure — in that order, because guessing at performance problems wastes time. First I'd turn on EF Core's SQL logging (or use an APM tool if we have one) to see exactly which part of the request is slow — is it the database round trip, serialization, or something downstream like an external HTTP call. If it's the database, I capture the generated SQL and check the execution plan for missing indexes or full table scans. I'd also look specifically for N+1 patterns and multi-collection `Include()`s that are blowing up the row count — those are the two most common EF Core performance killers I've run into. For anything read-only, I make sure we're using `.AsNoTracking()`, since tracking every entity for a response that's about to be serialized and discarded is pure overhead. I'd also check whether we're returning full entities when a projected DTO with only the needed columns would do, and whether the endpoint paginates — an unbounded `ToListAsync()` on a table that's grown to hundreds of thousands of rows will get slower every month even if nothing in the code changed. After each change, I re-run the same measurement to confirm it actually helped before moving to the next hypothesis, rather than stacking speculative fixes."
                    """,
                SortOrder = 32,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Troubleshoot a 401 Unauthorized Response",
                PromptText = "Users hitting a protected endpoint are getting a 401 Unauthorized response even though they believe they're logged in — walk through how you'd diagnose it.",
                SuggestedApproach = """
                    - Confirm the request actually includes an Authorization header in the form `Bearer <token>` and that the token hasn't expired (check the `exp` claim).
                    - Verify `app.UseAuthentication()` is registered before `app.UseAuthorization()` and before endpoint mapping in the middleware pipeline, and that a default authentication scheme is configured in `AddAuthentication(...)`.
                    - Check that `TokenValidationParameters` (ValidIssuer, ValidAudience, IssuerSigningKey) match exactly how the token was issued.
                    - Rule out clock skew causing a token that's actually still valid to be rejected as expired.
                    - Inspect the `WWW-Authenticate` response header, which JWT bearer middleware populates with the specific validation failure reason.
                    """,
                SampleAnswer = """
                    "First I'd separate 'not authenticated at all' from 'authenticated but rejected' — 401 specifically means the server never accepted the credentials. I'd check the raw request in dev tools or Fiddler to confirm the Authorization header is actually present and formatted as `Bearer <token>`, since a common bug is the client dropping the header on a redirect or forgetting the 'Bearer ' prefix. Next I'd check the token's `exp` claim against the current time — expired tokens are the single most common cause. If the header looks fine, I'd check the middleware pipeline order: `UseAuthentication` has to run before `UseAuthorization` and before the endpoints are mapped, otherwise the principal never gets attached to the request. Then I'd compare `ValidIssuer`, `ValidAudience`, and the signing key configured in `AddJwtBearer` against what the token was actually issued with — a mismatch there fails validation silently from the client's point of view. Finally, the `WWW-Authenticate` header on the 401 response often spells out the exact reason, like `invalid_token` with a description, so I always check that before guessing."
                    """,
                DiagramBody = """
                    [{"label":"Authorization header present?","note":"Bearer <token>, correct prefix"},{"label":"Token exp claim valid?","note":"expired tokens are the #1 cause"},{"label":"UseAuthentication before UseAuthorization?","note":"and before endpoint mapping in the pipeline"},{"label":"ValidIssuer/ValidAudience/signing key match?","note":"compared against how the token was actually issued"},{"label":"Check WWW-Authenticate header","note":"often names the exact validation failure"}]
                    """,
                DiagramFormat = DiagramFormat.StructuredSteps,
                SortOrder = 33,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Troubleshoot a 403 Forbidden Response",
                PromptText = "A user has a valid, unexpired token but still gets a 403 Forbidden response on a specific endpoint — how do you figure out why?",
                SuggestedApproach = """
                    - First confirm this really is 403, not 401 — that means authentication succeeded and the identity is known, but the authorization check failed.
                    - Look at the exact requirement on the endpoint: `[Authorize(Roles = "...")]`, `[Authorize(Policy = "...")]`, or a resource-based check via `IAuthorizationService`.
                    - Decode the JWT and check whether the expected role or claim is actually present and correctly named — a frequent bug is the token carrying a `"role"` or `"roles"` claim that isn't mapped to `ClaimTypes.Role` because `RoleClaimType` wasn't set in `TokenValidationParameters`.
                    - If a custom policy is involved, check the `AuthorizationHandler` logic for the requirement — it may be calling `context.Fail()` or simply never calling `context.Succeed()`.
                    - For resource-based authorization, verify the ownership/ACL check against the specific resource, not just the user's general role.
                    """,
                SampleAnswer = """
                    "Since authentication clearly worked here, I'd focus entirely on the authorization side. I'd start by looking at the attribute on the action — is it `[Authorize(Roles = "Admin")]`, a named policy, or a resource-based check? Then I'd decode the JWT (with a tool like jwt.ms) and check the actual claims the token carries. The bug I've hit most often is a role claim named `"role"` or `"roles"` in the token that never gets mapped to .NET's `ClaimTypes.Role`, because the `RoleClaimType` wasn't set on `TokenValidationParameters` — so `User.IsInRole(...)` silently returns false even though the claim is right there in the token. If it's a custom policy, I'd step through the `AuthorizationHandler` to see whether it's evaluating the right requirement and actually calling `context.Succeed()` for this user. And if it's resource-based — like 'can this user edit this specific record' — I'd check the ownership/ACL lookup itself rather than assuming it's a role problem at all."
                    """,
                SortOrder = 34,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "JWT Works Locally but Fails in Production",
                PromptText = "Your JWT authentication works perfectly on your local machine but every token gets rejected once the API is deployed to production — what would you check?",
                SuggestedApproach = """
                    - Compare `TokenValidationParameters` (signing key, ValidIssuer, ValidAudience) between environments — these are frequently different per `appsettings.{Environment}.json` and easy to get out of sync.
                    - Confirm the signing secret or certificate is actually present in production configuration — user secrets and local dev keys don't get deployed, so production may be falling back to a missing or default key.
                    - If using an external identity provider, confirm the JWKS/metadata endpoint is reachable from the production network and that cached signing keys haven't rotated out of sync.
                    - Check for clock skew between the token issuer and the production server's system clock.
                    - Verify production is correctly behind HTTPS or a reverse proxy forwarding `X-Forwarded-Proto`, since `RequireHttpsMetadata` (true by default) can reject metadata retrieval if the scheme looks wrong.
                    """,
                SampleAnswer = """
                    "This almost always comes down to configuration drift between environments rather than a code bug, since the same code is running in both places. My first check is the signing key: local development often uses a key from user secrets or `appsettings.Development.json` that was never provisioned in production — if production falls back to a different or missing key, every signature validation fails. Next I'd compare `ValidIssuer` and `ValidAudience` values, since production usually has a different base URL and it's easy to hardcode the local one. If we're validating against an external IdP's JWKS endpoint, I'd check that the production environment can actually reach it over the network — firewalls or egress rules sometimes block it — and that the signing keys haven't rotated since we last cached them. I'd also check for clock skew, since containers or VMs can have drifted system time, and a token that's fine by wall-clock time can fail validation if `exp` is checked too strictly. Finally, if there's a reverse proxy or load balancer in front of production, I'd confirm forwarded headers are configured so `RequireHttpsMetadata` doesn't reject requests it thinks are non-HTTPS."
                    """,
                SortOrder = 35,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Secure a Web API End to End",
                PromptText = "What's your checklist for securing an ASP.NET Core Web API end to end, from the network layer down to the code?",
                SuggestedApproach = """
                    - Transport: enforce HTTPS with `UseHttpsRedirection` and `UseHsts`, and disable plaintext HTTP where possible.
                    - Authentication: use a standard scheme (JWT bearer, OAuth2/OpenID Connect) rather than custom token logic.
                    - Authorization: apply least-privilege role/policy-based checks on every endpoint, not just a blanket `[Authorize]`.
                    - Input validation: rely on model validation and data annotations, and never trust client input for IDs or ownership checks.
                    - Secrets management: keep keys/connection strings out of source control, using Key Vault or environment variables, not `appsettings.json`.
                    - CORS: restrict `AllowedOrigins` to known frontends rather than `AllowAnyOrigin`.
                    - Rate limiting/throttling to prevent abuse and brute-force attempts.
                    - Security headers (`X-Content-Type-Options`, `Content-Security-Policy`, etc.) and dependency vulnerability scanning.
                    - Error handling that never leaks stack traces or internal details to clients.
                    """,
                SampleAnswer = """
                    "I think about it in layers. At the transport layer, HTTPS is non-negotiable — `UseHttpsRedirection` plus HSTS in production. On top of that, authentication should use a standard protocol like JWT bearer tokens or OpenID Connect rather than anything homegrown, and authorization should be least-privilege — policy or role checks on every sensitive endpoint, not just a blanket `[Authorize]` at the controller level. For the data going in, I rely on model validation and data annotations, and I never trust a client-supplied ID for an ownership check — I always re-verify against the authenticated user. Secrets — connection strings, signing keys, API keys — go into Key Vault or environment variables, never checked into `appsettings.json`. CORS gets locked down to the specific origins we actually serve, not `AllowAnyOrigin`. I'd add rate limiting on auth endpoints in particular to blunt brute-force and credential-stuffing attempts, and set standard security headers like `X-Content-Type-Options: nosniff` and a reasonable CSP. Finally, error handling matters for security too — a global exception handler that returns a generic ProblemDetails response instead of a raw stack trace, since stack traces can leak internal paths, library versions, or query structure to an attacker."
                    """,
                SortOrder = 36,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Refresh an Expired JWT Token",
                PromptText = "A client's JWT access token has expired mid-session — how do you design token refresh so the user isn't forced to log in again?",
                SuggestedApproach = """
                    - Issue a short-lived access token alongside a longer-lived refresh token at login.
                    - Store the refresh token securely — hashed at rest in the database, and delivered to the client as an HttpOnly, Secure cookie rather than in JavaScript-accessible storage.
                    - Add a dedicated refresh endpoint that validates the refresh token (not expired, not revoked) and issues a new access token.
                    - Rotate the refresh token on every use and detect reuse of an already-rotated token as a signal of theft, revoking the whole token family.
                    - Never try to "extend" the access token itself — access tokens should stay short-lived and stateless; only the refresh flow should be stateful.
                    """,
                SampleAnswer = """
                    "I design this as two tokens with very different lifetimes: a short-lived access token, maybe 15 minutes, used on every API call, and a longer-lived refresh token, maybe 7 to 30 days, used only to get new access tokens. At login, both are issued; the refresh token gets stored hashed server-side and handed to the client as an HttpOnly, Secure cookie so JavaScript can't read it, which limits XSS exposure. When the access token expires, the client calls a `/auth/refresh` endpoint with the refresh token; the server checks it against the stored hash, confirms it hasn't been revoked or expired, and issues a brand-new access token plus a rotated refresh token. Rotating on every use is important — if I ever see an old, already-used refresh token presented again, that's a strong signal it was stolen, so I revoke the entire token family and force a real re-login. I deliberately don't try to make the access token itself refreshable or long-lived — keeping it short and stateless is what makes JWTs cheap to validate, and pushing all the revocation complexity into the refresh token, which is checked against the database anyway, is the right tradeoff."
                    """,
                SortOrder = 37,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Implement Global Exception Handling",
                PromptText = "How do you implement global exception handling in an ASP.NET Core Web API so unhandled exceptions produce consistent, safe responses?",
                SuggestedApproach = """
                    - In .NET 8+, implement `IExceptionHandler` and register it with `AddExceptionHandler<T>()`, paired with `AddProblemDetails()` so failures are returned as standard ProblemDetails JSON.
                    - Alternatively (or in earlier versions), use the built-in `app.UseExceptionHandler("/error")` middleware pointed at a minimal error endpoint/controller.
                    - Log the full exception (message, stack trace, and any relevant context) server-side inside the handler, before shaping the client response.
                    - Return a generic, non-leaking message and an appropriate status code to the client — never the raw exception message or stack trace.
                    - Reserve `app.UseDeveloperExceptionPage()` for the Development environment only.
                    """,
                SampleAnswer = """
                    "In a current .NET 8 project I'd implement `IExceptionHandler`, register it with `builder.Services.AddExceptionHandler<GlobalExceptionHandler>()` and `AddProblemDetails()`, and wire it in with `app.UseExceptionHandler()`. Inside `TryHandleAsync`, I log the full exception through `ILogger` — the actual exception object, not just its message, so the stack trace is preserved in our logs — and then write back a standard `ProblemDetails` response with a generic title and the appropriate status code, mapping specific exception types to specific codes where it makes sense, like a `ValidationException` to 400 and everything unexpected to 500. The client never sees the real exception message or stack trace in production. I'd keep `app.UseDeveloperExceptionPage()` gated to `if (app.Environment.IsDevelopment())` so we still get the full diagnostic page locally. Before .NET 8, I'd get the same result with `app.UseExceptionHandler("/error")` routing to a minimal `/error` endpoint that reads `IExceptionHandlerFeature` to log the exception and return the same shaped ProblemDetails response."
                    """,
                DiagramBody = """
                    [{"label":"Exception thrown in a request handler"},{"label":"IExceptionHandler.TryHandleAsync catches it","note":"registered via AddExceptionHandler<T>()"},{"label":"Log the full exception object","note":"ILogger, not just ex.Message - preserves stack trace"},{"label":"Map exception type to status code","note":"e.g. ValidationException -> 400, unknown -> 500"},{"label":"Return generic ProblemDetails","note":"client never sees the real message or stack trace"}]
                    """,
                DiagramFormat = DiagramFormat.StructuredSteps,
                SortOrder = 38,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Troubleshoot a Bare 500 Internal Server Error",
                PromptText = "An API call returns a 500 Internal Server Error with no useful detail in the response body — how do you find the real cause?",
                SuggestedApproach = """
                    - Don't rely on the client-facing 500 response for diagnosis — by design it should never expose internals in production.
                    - Go straight to centralized logs (Serilog/Seq, Application Insights, CloudWatch, etc.) and search by timestamp, request path, or correlation/trace ID to find the actual exception and stack trace.
                    - Confirm a global exception handler or logging middleware is actually capturing and logging the full exception, not just returning the generic response silently.
                    - If logs are missing detail, reproduce the request against a Staging environment with verbose logging or a temporarily enabled diagnostic page, rather than toggling detailed errors on production.
                    - Check for exceptions thrown outside the normal request pipeline too, like in background services, startup filters, or DI container resolution.
                    """,
                SampleAnswer = """
                    "A bare 500 with no detail is expected behavior in production — the response is intentionally scrubbed — so I wouldn't try to get more out of the HTTP response itself. Instead I'd go to our centralized logging, whether that's Application Insights, Seq, or CloudWatch, and search around the time of the failed request, ideally using a correlation or trace ID if we include one in the response headers, to pull up the actual exception and stack trace. If nothing shows up there, that tells me our global exception handler isn't actually logging before it returns the generic response, which is a bug in itself and worth fixing immediately. If logs still don't have enough context, I'd reproduce the same request against a Staging environment where we can safely turn on more verbose logging or even the developer exception page, rather than risk exposing stack traces on production. I'd also check whether the failure is actually happening outside the normal HTTP pipeline — an exception during DI container resolution at startup, or in a background hosted service — since those sometimes don't get funneled through the same exception-handling middleware as request-scoped exceptions."
                    """,
                SortOrder = 39,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Diagnose the Causes of a 400 Bad Request",
                PromptText = "A client call to your API returns a 400 Bad Request — what are the possible root causes and how do you narrow them down?",
                SuggestedApproach = """
                    - Read the actual response body first — with `[ApiController]`, automatic model validation returns a `ValidationProblemDetails` payload listing exactly which fields failed and why.
                    - Check for malformed JSON in the request body (trailing commas, wrong types, mismatched property names) that fails deserialization before validation even runs.
                    - Check route and query parameter binding — a route value or query string that can't convert to the expected type (e.g., non-numeric text bound to an `int`) triggers a 400 automatically.
                    - Check for custom validation logic — data annotations, FluentValidation rules, or a manual `if (...) return BadRequest(...)` in the action.
                    - Confirm the client is actually sending what the API contract expects — a missing required field or wrong content shape is the most common cause.
                    """,
                SampleAnswer = """
                    "The first thing I do is actually read the response body instead of just the status code — with `[ApiController]`, ASP.NET Core's automatic model validation returns a `ValidationProblemDetails` object that lists exactly which fields failed and why, so most of the time the cause is right there. If the body is empty or unhelpful, I check whether the request JSON even deserializes correctly — a typo in a property name, wrong type, or malformed JSON will fail before validation logic runs at all. Next I look at route and query parameter binding, since something like a non-numeric string bound to an `int` route parameter causes an automatic 400 before the action method is even invoked. If the payload deserializes fine and still fails, I check for custom validation — data annotations on the DTO, a FluentValidation validator, or a manual `if` check inside the action that returns `BadRequest(...)` for a business rule violation. In practice, the vast majority of 400s I've debugged came down to the client sending a field with the wrong type or a required field the API contract expects but the client never included."
                    """,
                SortOrder = 40,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Fix a 415 Unsupported Media Type Response",
                PromptText = "A client's request is being rejected with a 415 Unsupported Media Type response — why does this happen and how do you fix it?",
                SuggestedApproach = """
                    - Check the request's actual Content-Type header — 415 means the server understood the request but refuses to process the body because its declared media type isn't one the endpoint accepts.
                    - Check for a `[Consumes]` attribute on the controller/action restricting accepted media types (e.g., only `application/json`) while the client sent something else, like `text/plain` or `multipart/form-data`.
                    - Confirm the client library is setting the header correctly — e.g., an `HttpClient` call using `StringContent` must pass `"application/json"` as the media type, since the default is `text/plain`.
                    - If the API genuinely needs to accept multiple formats, register/configure the corresponding input formatter rather than fighting the framework default.
                    """,
                SampleAnswer = """
                    "415 specifically means the server is looking at the Content-Type header on the request and deciding it doesn't know how to parse a body of that type for this endpoint — it's different from a malformed body, which would be a 400. My first check is the raw Content-Type header the client actually sent. A very common cause is an `HttpClient` call using `new StringContent(json)` without specifying the media type — that defaults to `text/plain`, so even though the body is valid JSON, the API's JSON input formatter never even looks at it because the Content-Type doesn't match. The fix there is `new StringContent(json, Encoding.UTF8, "application/json")`. Another cause is a `[Consumes("application/json")]` attribute on the action being more restrictive than the client expects, so if we genuinely need to accept, say, XML or form-encoded data too, the fix is to add and configure the matching input formatter rather than just removing the restriction. Either way, the fix is always about aligning the client's declared Content-Type with what the API's configured formatters actually accept."
                    """,
                SortOrder = 41,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Log Exceptions Properly in Production",
                PromptText = "How do you log exceptions properly in a production ASP.NET Core API so they're actually useful when something goes wrong?",
                SuggestedApproach = """
                    - Use the `ILogger<T>` abstraction backed by a structured sink (Serilog, Application Insights, etc.) instead of `Console.WriteLine` or ad hoc file writes.
                    - Log the full exception object, not just its `Message`, so the stack trace and inner exceptions are preserved.
                    - Enrich log entries with request context — correlation/trace ID, request path, and (non-sensitive) user identifier — so a single failure can be traced across a distributed system.
                    - Scrub or avoid logging sensitive data — passwords, tokens, full card numbers, PII — even inside exception details.
                    - Centralize logging in one place, like the global exception handler, instead of scattering inconsistent try/catch/log blocks through business logic.
                    - Set up alerting on Error/Critical log levels so failures are noticed proactively, not discovered from user complaints.
                    """,
                SampleAnswer = """
                    "I always log through the `ILogger<T>` abstraction rather than `Console.WriteLine`, backed by a structured sink like Serilog writing to Seq or Application Insights, so logs are queryable and not just text in a console we'll never see again. When logging an exception, I always pass the exception object itself — `_logger.LogError(ex, "message")` — rather than just `ex.Message`, because the structured sink then captures the full stack trace and any inner exceptions, which is what actually lets you find the root cause later. I make sure every log entry carries context — a correlation or trace ID that flows through the whole request, the request path, and the authenticated user ID if there is one — because in a system with multiple services, an exception message alone often isn't enough to find where things went wrong. I'm careful never to log sensitive data — no passwords, no full tokens, no PII beyond what's needed — even when it would be convenient for debugging. And rather than scattering try/catch/log blocks through every service method, I centralize exception logging in one place, typically the global exception handler, so the logging behavior is consistent everywhere. Finally, logging only matters if someone sees it, so I make sure Error and Critical levels trigger alerts to the team instead of just sitting in a dashboard nobody's watching."
                    """,
                SortOrder = 42,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "API Response Time Jumped From 1s to 15s — How Do You Investigate?",
                PromptText = "An API endpoint that normally responds in about 1 second is suddenly taking 15 seconds. Walk through how you'd investigate the cause.",
                SuggestedApproach = """
                    Structure this as a systematic narrowing-down process rather than a list of guesses:
                    - First check whether it's one endpoint or all endpoints, and whether it correlates with a deploy, a traffic spike, or a specific input — this tells you if it's code, infrastructure, or data-driven.
                    - Look at APM/tracing (Application Insights, or `dotnet-trace`/`dotnet-counters` locally) to see where the time is actually going: in the app, in the database, or in a downstream HTTP call.
                    - Check for N+1 query patterns or a query that lost an index (data volume grew, execution plan changed).
                    - Check for thread pool starvation caused by blocking on async code (`.Result`, `.Wait()`, `Task.Run` misuse) — this shows up as high latency under load even though CPU looks idle.
                    - Check GC behavior (Gen2 collections, high allocation rate) and server resource exhaustion (CPU, memory, DB connection pool exhaustion).
                    - Check any external dependency (third-party API, downstream microservice) for its own latency spike.
                    """,
                SampleAnswer = """
                    "I wouldn't guess — I'd narrow it down using data. First I'd check if this is affecting one endpoint or the whole API, and whether it lines up with a recent deploy, a config change, or a spike in traffic or data volume. If we have Application Insights or similar APM, I'd pull the end-to-end transaction trace for a slow request and see where the 14 extra seconds actually went — in my experience it's almost always one of three things: a database query that used to be fast but now does a table scan because the data grew past what an index could handle efficiently, an N+1 query pattern that got triggered by a new code path or a lazy-loaded navigation property, or a downstream dependency (another service or third-party API) that's slow and we're calling it synchronously without a timeout. I'd also check for thread pool starvation — if someone added a `.Result` or `.Wait()` on a Task somewhere, under load that blocks worker threads and the whole app queues up requests waiting for a free thread, which looks exactly like this symptom. Finally I'd check GC metrics and DB connection pool exhaustion, since both can produce this kind of sudden, load-correlated latency spike. Once I isolate which layer it's in, I profile that specific piece with `dotnet-trace` or SQL Server's actual execution plan rather than guessing."
                    """,
                DiagramBody = """
                    [{"label":"One endpoint or all endpoints?","note":"narrows code vs. infrastructure vs. data"},{"label":"Correlate with deploy/config/traffic change"},{"label":"Pull end-to-end trace (APM)","note":"see where the 14 extra seconds actually went"},{"label":"DB query / N+1 / downstream dependency?","note":"the three most common causes"},{"label":"Thread pool starvation or connection pool exhaustion?","note":"check under-load-only symptoms"},{"label":"Profile the isolated layer","note":"dotnet-trace or execution plan, not a guess"}]
                    """,
                DiagramFormat = DiagramFormat.StructuredSteps,
                SortOrder = 43,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "How Do You Improve the Performance of a Slow API in General?",
                PromptText = "In general, what techniques would you apply to improve the performance of a slow ASP.NET Core Web API?",
                SuggestedApproach = """
                    Group the answer into a few concrete levers rather than a vague "it depends":
                    - **Data access**: add missing indexes, avoid N+1 queries (use `Include`/projection), use `AsNoTracking()` for read-only queries, paginate large result sets instead of returning everything.
                    - **Caching**: cache-aside with `IMemoryCache`/`IDistributedCache` for expensive or frequently-read data, response caching/output caching for cacheable HTTP responses.
                    - **Async and threading**: make I/O-bound calls (DB, HTTP) properly `async`/`await` all the way down, never block on async code.
                    - **Payload and transport**: enable response compression, return only the fields the client needs (DTOs, not full entities), use pagination/streaming for large payloads.
                    - **Scaling**: horizontal scale-out behind a load balancer, connection pooling tuned for the database.
                    - Measure before and after with profiling/APM — don't optimize blind.
                    """,
                SampleAnswer = """
                    "I'd start by measuring, not guessing, using something like Application Insights or `dotnet-trace` to find the actual bottleneck. From there the usual levers are: on the data side, make sure queries are using indexes, eliminate N+1 patterns by projecting or including related data up front, and use `AsNoTracking()` for read-only EF Core queries since it skips change-tracking overhead. Then I'd look at caching — if data doesn't change on every request, an `IMemoryCache` cache-aside pattern for a single instance, or `IDistributedCache` backed by Redis if we're running multiple instances, can turn a DB round-trip into a memory lookup. I'd make sure every I/O-bound call — database, HTTP, file access — is genuinely `async` end to end, since one blocking `.Result` call can starve the thread pool under load. On the wire, I'd enable response compression and trim response DTOs down to only what the client needs instead of serializing full EF entities, and paginate any endpoint that can return large collections. If the app is still CPU- or throughput-bound after that, I'd look at horizontal scaling behind a load balancer rather than trying to squeeze more out of a single instance. The key is always profile first, fix the actual hot path, then re-measure."
                    """,
                SortOrder = 44,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "How Do You Implement Caching in an ASP.NET Core Web API?",
                PromptText = "How would you implement caching in an ASP.NET Core Web API, and what options does the framework give you?",
                SuggestedApproach = """
                    Cover the main mechanisms and how they fit together:
                    - `IMemoryCache` for in-process caching (fast, but per-instance — not shared across multiple app instances).
                    - `IDistributedCache` (backed by Redis, SQL Server, etc.) for a shared cache across multiple instances.
                    - The cache-aside pattern: check the cache first, on a miss fetch from the source and populate the cache, with an appropriate expiration (absolute and/or sliding).
                    - Output caching / response caching middleware for caching entire HTTP responses based on headers/vary-by rules.
                    - Cache invalidation strategy — this is the hard part: expire on write, use short TTLs, or explicitly evict keys when the underlying data changes.
                    """,
                SampleAnswer = """
                    "For data that's expensive to compute or fetch but doesn't change on every request, I use the cache-aside pattern: check the cache, and if it's a miss, fetch from the database and populate the cache with an expiration before returning. For a single-instance API I'd register `IMemoryCache` and inject it, calling `TryGetValue` and `Set` with something like a 5-minute absolute expiration plus maybe a sliding expiration if it's read frequently. If we're running multiple instances behind a load balancer, `IMemoryCache` breaks down because each instance has its own cache and you get inconsistent results, so I'd switch to `IDistributedCache` backed by Redis — same cache-aside coding pattern, just backed by a shared store. For whole-response caching — like a public GET endpoint that returns the same data for everyone for a few seconds — I'd use the Output Caching middleware in .NET 7+ (or Response Caching middleware) with vary-by rules on query string or headers. The part I'm most careful about is invalidation: I either keep TTLs short enough that staleness doesn't matter, or explicitly remove the cache key when the underlying entity is updated, so we don't serve stale data indefinitely."
                    """,
                SortOrder = 45,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "When Would You Use Redis Instead of In-Memory Caching?",
                PromptText = "When would you reach for Redis specifically, versus using ASP.NET Core's built-in in-memory caching?",
                SuggestedApproach = """
                    Focus on the actual trade-off: `IMemoryCache` is per-process, Redis (via `IDistributedCache`) is shared.
                    - `IMemoryCache` is simplest and fastest for a single instance, but breaks down the moment you scale out horizontally — each instance has its own cache, so users can see inconsistent data depending on which instance handles the request, and cache invalidation has to happen on every instance.
                    - Redis solves this by being a shared, external cache that all instances read/write, giving consistent cached data regardless of which instance serves the request.
                    - Redis also gives you features in-memory caching doesn't: TTL-based eviction shared across the fleet, pub/sub for cache invalidation broadcasts, and surviving an individual instance restart/recycle.
                    - Trade-off: Redis adds a network hop and infrastructure to manage, so for a single-instance app or very short-lived data, in-memory is simpler and faster.
                    """,
                SampleAnswer = """
                    "The deciding factor for me is whether the API runs as more than one instance. `IMemoryCache` lives in the process's own memory, so it's extremely fast with zero network overhead, but it's local to that instance — if we're running three instances behind a load balancer and one of them caches a value, the other two don't know about it, and a user can get inconsistent results depending on which instance handles their next request. That also makes invalidation messy, because updating one entity means you'd need to somehow clear the cache on every instance. Redis fixes that by being an external, shared cache — all instances hit the same Redis store through `IDistributedCache`, so the cache is consistent no matter which instance serves the request, and it survives an individual instance restarting or scaling down. I'd also reach for Redis specifically if I need cross-cutting features like publishing an invalidation event via pub/sub, or if the cached data needs to outlive the app process — say, session state or a rate-limiter counter. If it's a single-instance app, or the data is genuinely per-request-cheap to cache locally, I'd stick with `IMemoryCache` since it avoids the extra network round-trip and infrastructure Redis requires."
                    """,
                SortOrder = 46,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "How Do You Design an API to Handle 10,000 Concurrent Requests?",
                PromptText = "How would you design an ASP.NET Core Web API so it can reliably handle 10,000 concurrent requests?",
                SuggestedApproach = """
                    Break the answer into layers rather than one silver bullet:
                    - **Application layer**: fully async request pipeline (no blocking calls) so threads aren't held hostage waiting on I/O; keep handlers lightweight and stateless so any instance can serve any request.
                    - **Scaling**: horizontal scale-out with multiple instances behind a load balancer (Azure App Service scale-out, or Kubernetes with an HPA), since a single instance has a ceiling.
                    - **Data layer**: connection pooling tuned appropriately, read replicas or caching (Redis) to take load off the primary database, since the DB is usually the real bottleneck, not the API layer.
                    - **Offloading work**: push non-critical or slow work to a background queue (Azure Service Bus/queue + worker) instead of doing it inline in the request.
                    - **Resilience**: rate limiting, circuit breakers (e.g., Polly) for downstream calls, and health checks so the load balancer can route around unhealthy instances.
                    - **Configuration**: tune Kestrel limits, thread pool minimum threads, and use Server GC mode for throughput.
                    """,
                SampleAnswer = """
                    "10,000 concurrent requests is really a question about not serializing work that doesn't need to be serialized, and not having a single point of contention. First, the whole request path needs to be truly async — every DB call and outbound HTTP call awaited properly — because if even a few requests block a thread pool thread, the pool exhausts and everything queues up regardless of how much hardware we throw at it. Second, I'd make the app stateless so it can scale horizontally — multiple instances behind a load balancer, whether that's App Service scale-out rules or a Kubernetes HPA — since no single instance can realistically sustain that concurrency alone. Third, I'd assume the database is the actual bottleneck, not the API: I'd tune the connection pool size, add read replicas for read-heavy traffic, and put Redis in front of expensive or frequently-read queries so most requests never hit SQL at all. For anything that doesn't need to complete synchronously — sending an email, generating a report — I'd push it onto a queue like Azure Service Bus and process it with a background worker instead of making the caller wait. Finally I'd add resilience: rate limiting to protect against abuse, Polly-based circuit breakers around downstream dependencies so one slow dependency doesn't cascade into thread exhaustion, and health checks so the load balancer stops routing to an unhealthy instance. I'd load test with something like k6 or Azure Load Testing to validate all of this actually holds up before calling it done."
                    """,
                SortOrder = 47,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "API Works Locally but Fails After Deployment — Troubleshooting Process",
                PromptText = "Your Web API works fine on your local machine but fails after being deployed. How do you troubleshoot it?",
                SuggestedApproach = """
                    Frame this as checking for environment differences between local and deployed, in order of likelihood:
                    - Check the deployed environment's logs first (App Service Log Stream, container logs, or `stdout`/ANCM logs) to see the actual exception rather than guessing.
                    - Configuration differences: missing or wrong values in `appsettings.Production.json`/environment variables/App Service Application Settings that exist locally in `appsettings.Development.json` or user secrets.
                    - Environment/runtime differences: a different .NET runtime version installed on the host, or a framework-dependent deployment missing the expected shared runtime.
                    - Path/OS differences: case-sensitive file paths or missing native dependencies if deployed to Linux while developed on Windows.
                    - Connectivity differences: the deployed environment can't reach a database, storage account, or third-party API that was reachable locally (firewall/network rules, different connection string).
                    - Certificate/HTTPS or CORS differences that only manifest once real client origins/domains are involved.
                    """,
                SampleAnswer = """
                    "The first thing I do is stop guessing and go look at the actual logs from the deployed environment — if it's Azure App Service, that's the Log Stream or Kestrel/ANCM startup logs, since a failure that only happens after deployment almost always throws an exception that tells you exactly what's different. In my experience it's usually one of a few things: a config value that exists in my local `appsettings.Development.json` or user secrets but was never set in the deployed environment's app settings or `appsettings.Production.json` — connection strings and API keys are the classic culprits. Next I check for a runtime mismatch — if I built against .NET 8 but the deployed environment has a different runtime installed, or it's a framework-dependent deployment and the target host doesn't have the matching shared framework, the app fails to start with an ANCM or 'framework not found' error. If it's a Linux-hosted deployment and I developed on Windows, I check for case-sensitive file path bugs, since 'Templates/Email.html' and 'templates/email.html' behave differently there. I'd also check connectivity — can the deployed instance actually reach the database or downstream API, or is there a firewall/network security rule blocking it that didn't apply to my local machine? Working through the logs first almost always tells me which of these it is instead of me having to guess blind."
                    """,
                SortOrder = 48,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Connection String Works Locally but Not in Azure — Why?",
                PromptText = "Your database connection string works fine when you run the API locally, but the same API fails to connect to the database once deployed to Azure. Why might that happen, and how do you fix it?",
                SuggestedApproach = """
                    Cover the two most common real causes, in order:
                    - **Configuration not actually deployed**: the connection string lived only in local `appsettings.Development.json` or user secrets and was never added to Azure App Service's Configuration -> Connection strings / Application settings, so the app falls back to a missing or default value in production.
                    - **Azure SQL firewall rules**: Azure SQL Database blocks all connections by default except from allow-listed IPs; the developer's local IP was allowed (or "Allow Azure services" wasn't enabled), but the App Service's outbound IP isn't allow-listed, so the connection is refused/times out.
                    - Secondary causes worth mentioning: different auth model (SQL auth locally vs. a managed identity expected in Azure), or connection string requiring `Encrypt=True`/`TrustServerCertificate` settings that differ between local SQL and Azure SQL.
                    - Fix: set the connection string in App Service Configuration (as a Connection String, not just a raw setting, so it maps to `ConnectionStrings:` correctly or via environment variable naming), and add a firewall rule (or enable "Allow Azure services and resources to access this server") on Azure SQL for the App Service.
                    """,
                SampleAnswer = """
                    "This is almost always one of two things. The first is that the connection string only exists in my local `appsettings.Development.json` or user secrets, and I never actually added it to the App Service — in Azure, App Service Configuration settings override `appsettings.json` at runtime, so if it's not set there, the app is either using a placeholder value or throwing a null reference on a missing config. The fix is to add it under App Service -> Configuration -> Connection strings (using the correct type, e.g., SQLAzure, so it binds to `ConnectionStrings:DefaultConnection` the way EF Core expects), not just paste it into `appsettings.json` and redeploy, since that's a security risk anyway. The second common cause is Azure SQL's firewall — by default Azure SQL Database rejects all connections except from explicitly allowed IPs. My local IP got added to the firewall at some point, or my local SQL Server just doesn't have this restriction at all, but the App Service's outbound IP was never allow-listed, so the connection times out or gets an explicit 'not allowed to access this server' error. I'd fix that by either adding the specific App Service outbound IPs to the Azure SQL firewall rules, or more commonly enabling 'Allow Azure services and resources to access this server' if we're comfortable with that scope. If we're using managed identity for the database connection instead of SQL auth, I'd also double check the identity has actually been granted access on the SQL side, since that's a different failure mode with a similar symptom."
                    """,
                DiagramBody = """
                    [{"label":"Local: works","note":"appsettings.Development.json or user secrets has the connection string"},{"label":"Deployed to App Service"},{"label":"Is the connection string set in App Service Configuration?","note":"appsettings.json values don't deploy secrets by convention"},{"label":"Is the App Service's outbound IP firewall-allowed on Azure SQL?","note":"Azure SQL blocks all IPs by default"},{"label":"If using managed identity: is it granted DB access?","note":"a separate failure mode with the same symptom"}]
                    """,
                DiagramFormat = DiagramFormat.StructuredSteps,
                SortOrder = 49,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "How Do You Store Secrets Securely in Azure?",
                PromptText = "How do you store secrets — like API keys and connection strings — securely for an API running in Azure, instead of hardcoding them or committing them to source control?",
                SuggestedApproach = """
                    Cover the actual layered approach used in production:
                    - Never put secrets in `appsettings.json` or any file committed to source control — use `appsettings.Development.json` (gitignored) or .NET user secrets (`dotnet user-secrets`) locally instead.
                    - In Azure, use **Azure Key Vault** to store secrets centrally, and grant the App Service access via a **managed identity** (system-assigned or user-assigned) rather than storing a Key Vault access key anywhere.
                    - Reference Key Vault secrets directly from App Service Configuration using Key Vault references (`@Microsoft.KeyVault(...)`), or load them via the `Azure.Extensions.AspNetCore.Configuration.Secrets` provider so they flow into `IConfiguration` like any other setting.
                    - Mention rotation: Key Vault supports secret versioning/rotation without redeploying the app, and managed identity means there's no credential to leak in the first place.
                    """,
                SampleAnswer = """
                    "Locally I never let secrets touch source control — I use .NET user secrets (`dotnet user-secrets set`) or a gitignored `appsettings.Development.json`, so nothing sensitive is ever in a commit. For the deployed app in Azure, I use Azure Key Vault as the source of truth for secrets like connection strings and third-party API keys, and I give the App Service a system-assigned managed identity with a Key Vault access policy (or RBAC role) that lets it read specific secrets — there's no credential to manage or leak, because Azure handles the identity behind the scenes. From there I either wire up Key Vault references directly in the App Service's Application Settings, using the `@Microsoft.KeyVault(SecretUri=...)` syntax, so the value is resolved at runtime without any code changes, or I add the `Azure.Extensions.AspNetCore.Configuration.Secrets` provider in `Program.cs` so Key Vault secrets get pulled straight into `IConfiguration` alongside everything else. The benefit beyond just 'not hardcoding it' is that Key Vault gives us secret rotation and versioning — if a database password changes, I update it in Key Vault and the app picks up the new value without a redeploy, and I get an audit trail of who accessed what."
                    """,
                SortOrder = 50,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "How Do You Monitor an API in Production on Azure?",
                PromptText = "Once your API is running in production on Azure, how do you monitor it to catch issues before or as they happen?",
                SuggestedApproach = """
                    Cover the standard Azure observability stack:
                    - **Application Insights** for request telemetry, dependency tracking (DB/HTTP calls), exceptions, and end-to-end distributed tracing across services.
                    - **Azure Monitor alerts** on key metrics — error rate, response time, CPU/memory, HTTP 5xx count — so the team is notified proactively rather than finding out from users.
                    - **Log Analytics / Kusto (KQL) queries** over App Service diagnostic logs and Application Insights logs for deeper investigation.
                    - **Health check endpoints** (`/health` via `Microsoft.Extensions.Diagnostics.HealthChecks`) that Azure or a load balancer can poll to detect an unhealthy instance.
                    - Custom telemetry/structured logging (e.g., via `ILogger` with correlation IDs) for business-specific events, and dashboards for at-a-glance status.
                    """,
                SampleAnswer = """
                    "The core of it is Application Insights — I wire it into the API so every incoming request, outgoing dependency call (SQL, HTTP, Redis), and unhandled exception gets tracked automatically, and I get end-to-end distributed traces if the request flows through multiple services. On top of that raw telemetry, I set up Azure Monitor alert rules on the metrics that actually matter — server response time crossing a threshold, HTTP 5xx rate spiking, CPU or memory pinned — so the team gets paged or emailed before customers start complaining, instead of finding out reactively. For deeper investigation I use Log Analytics with KQL queries against the App Service diagnostic logs and App Insights data, which is how I'd dig into something like the response-time regression we talked about earlier. I also add a `/health` endpoint using the built-in health checks middleware, which reports on things like DB connectivity, so App Service or a load balancer can detect an unhealthy instance and route around it. And for anything business-specific — like 'checkout failed for this reason' — I use structured logging with `ILogger` and a correlation ID per request so I can trace a single user's request across logs and telemetry. Finally I'd build a dashboard in Azure or App Insights workbooks so the team has an at-a-glance view of error rate, latency percentiles, and throughput rather than digging through raw logs during an incident."
                    """,
                SortOrder = 51,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "How Do You Troubleshoot an Azure App Service That Won't Start or Returns 502/503?",
                PromptText = "Your Azure App Service either won't start at all, or is intermittently returning HTTP 502/503 errors. How do you troubleshoot it?",
                SuggestedApproach = """
                    Structure the answer around the actual diagnostic tools and common root causes:
                    - Start with **Log Stream** / Kestrel and ANCM (ASP.NET Core Module) startup logs in the App Service Diagnose and Solve Problems blade — a 502/503 at startup almost always has a specific exception logged there.
                    - Common root causes to check: a .NET runtime version mismatch between what the app was built/published for and what the App Service's runtime stack is set to; a missing or misconfigured startup command for containerized/Linux deployments; an unhandled exception in `Program.cs`/`Startup` (e.g., a DI resolution failure or missing configuration) that crashes the app on boot.
                    - 503 specifically also points to the App Service being out of resources (memory/CPU) causing worker process recycling, or the instance count/plan being undersized for the load, or a deployment slot swap that didn't warm up properly.
                    - Check for differences between deployment slots (staging vs production) and confirm environment variables/app settings match what's expected.
                    - Use **Kudu/SCM console** to inspect the actual deployed files and confirm the expected build artifacts are present.
                    """,
                SampleAnswer = """
                    "First stop is the App Service's Log Stream, or the Diagnose and Solve Problems blade, since a 502/503 almost always has a concrete startup exception behind it rather than being a mystery. If the app is failing to start entirely, the most common cause I've seen is a .NET runtime mismatch — the app was published targeting, say, .NET 8, but the App Service's configured runtime stack is still on .NET 6, so the ASP.NET Core Module fails to launch the process and you get an ANCM 'failed to start' error in the logs. I'd check the Configuration -> General settings blade to confirm the stack version matches what we built against. If it does start but crashes, I check for an unhandled exception during startup — commonly a DI container failing to resolve a service because a required configuration value or connection string wasn't set as an app setting, which is a startup-time crash rather than a per-request one. If it's specifically intermittent 502/503 under load rather than a hard failure, that usually points to the App Service plan being undersized — the worker process is getting recycled due to memory pressure, or we're on a plan tier that can't handle the concurrency, in which case I'd check the Metrics blade for memory/CPU and consider scaling up or out. I'd also check whether this just happened after a deployment slot swap, since a swap to production without the new slot being warmed up can cause a burst of 503s while the app spins up. Finally I'd use the Kudu/SCM console to browse the actual deployed `wwwroot`/bin folder and confirm the build artifacts that are there actually match what I expect, in case the deployment itself was incomplete."
                    """,
                SortOrder = 52,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Why Do We Use Dependency Injection?",
                PromptText = "Your tech lead asks you to justify why the codebase relies so heavily on Dependency Injection instead of just newing up dependencies directly inside classes — what's your answer?",
                SuggestedApproach = """
                    Cover:
                    - Testability — a class that receives its dependencies through its constructor can have those dependencies replaced with mocks/fakes in unit tests, instead of being hard-wired to real implementations.
                    - Loose coupling — classes depend on abstractions (interfaces) rather than concrete types, so implementations can change without touching consumers.
                    - Inversion of control — the DI container owns the responsibility of constructing the object graph and managing lifetimes, instead of each class managing its own dependencies.
                    - The three built-in ASP.NET Core lifetimes: Transient (new instance every resolution), Scoped (one instance per request), Singleton (one instance for the app's lifetime) — and when each is appropriate.
                    """,
                SampleAnswer = """
                    "We use Dependency Injection mainly for testability and loose coupling. If a service takes its dependencies as constructor parameters typed as interfaces, I can write a unit test that passes in a mock `IPaymentGateway` instead of hitting a real payment provider — without DI, that class would be tightly coupled to a concrete implementation and basically untestable in isolation. It also means I can change an implementation, say swap a SendGrid email sender for an SES one, without touching any of the classes that depend on `IEmailSender`.
                    On top of that, ASP.NET Core's built-in container handles lifetime management for me: Transient services get a new instance every time they're resolved, Scoped services get one instance per HTTP request — which is what I use for anything wrapping a DbContext, since DbContext isn't thread-safe and shouldn't be shared across requests — and Singleton services live for the whole application lifetime, which I use for things like configuration objects or in-memory caches. Getting those lifetimes right matters a lot; injecting a Scoped service into a Singleton, for example, is a classic captive-dependency bug."
                    """,
                SortOrder = 53,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Why Do We Use the Repository Pattern?",
                PromptText = "A teammate asks why you'd wrap EF Core's DbContext behind a Repository interface instead of just using DbContext and DbSet directly — what's the honest justification?",
                SuggestedApproach = """
                    Be honest here — EF Core's `DbContext`/`DbSet<T>` already gives you a Unit of Work (the change tracker plus `SaveChanges`) and a query abstraction (`IQueryable`), so Repository isn't filling a missing EF Core capability. Cover the real reasons teams add it anyway:
                    - Easier unit testing — mocking a narrow `IRepository<T>` interface is simpler than mocking `DbContext`/`DbSet` (which historically required extra setup, though EF Core's InMemory/SQLite providers have reduced this need).
                    - Decoupling business logic from EF Core specifics, in case the ORM or data-access technology ever needs to change.
                    - A smaller, intention-revealing surface (e.g. `GetActiveOrders()`) instead of exposing raw `IQueryable` everywhere.
                    Also mention the honest downside: it can be an unnecessary abstraction over an abstraction that already does most of the work.
                    """,
                SampleAnswer = """
                    "Honestly, EF Core's `DbContext` and `DbSet<T>` already act as a Repository and Unit of Work — `DbSet` gives you a queryable collection with `Add`, `Remove`, `Find`, and LINQ querying, and `DbContext.SaveChanges` commits everything as one unit. So Repository isn't adding a capability EF Core is missing.
                    The real reasons I've seen it used are testability and decoupling. Mocking `IRepository<Order>` in a unit test is more straightforward than setting up a mockable `DbSet` with `IQueryable` support, especially in older EF Core versions. It also keeps EF Core-specific concerns — like `Include()` calls for eager loading, or provider-specific query syntax — out of my service layer, so if we ever needed to swap ORMs, only the repository implementations would change.
                    That said, I'd push back if a team wanted Repository 'because it's a best practice' without a concrete reason — on a small or medium app, wrapping EF Core in a thin Repository just to satisfy the pattern usually adds indirection without adding value, since DbContext is already a well-tested abstraction over the database."
                    """,
                SortOrder = 54,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "When Would You Actually Reach for Repository?",
                PromptText = "Given that DbContext and DbSet already provide querying and change tracking, when would you actually introduce a Repository layer on top of them?",
                SuggestedApproach = """
                    Cover concrete triggers, not vague "best practice" reasoning:
                    - You need to support multiple or swappable data sources (e.g. SQL today, a different store or an external API later) behind one interface.
                    - You want a stable seam for unit tests that doesn't require spinning up a real or in-memory database at all.
                    - Business logic needs a narrow, domain-specific query surface (e.g. `GetOverdueInvoices()`) instead of leaking raw `IQueryable<T>` and EF-specific includes into services/controllers.
                    - Large, multi-team codebases where a consistent data-access convention across the team is worth the extra layer.
                    Also state the counter-case: for smaller apps, mocking DbContext directly or testing against EF Core's InMemory/SQLite providers is often simpler, and Repository would just be unnecessary indirection.
                    """,
                SampleAnswer = """
                    "I reach for Repository when there's a concrete need, not just as a default pattern. The clearest case is when I genuinely might swap or add a data source — say business logic that today reads from SQL Server via EF Core but might later need to read from a cache or a third-party API — a Repository interface hides that detail from the caller.
                    Another real case is when I want tests that don't touch a database at all, including an in-memory provider — mocking `IOrderRepository` is cheaper and faster than configuring an EF Core InMemory context in every test.
                    And in larger codebases with several teams touching the same entities, a Repository with well-named methods like `GetPendingOrdersForCustomer(customerId)` keeps query logic centralized and consistent, instead of every controller writing its own LINQ over `DbSet`.
                    On a small CRUD-heavy API with one team, though, I wouldn't bother — injecting `DbContext` directly and querying `DbSet` is simpler, and EF Core's InMemory or SQLite provider gives me plenty of testability without the extra abstraction layer."
                    """,
                SortOrder = 55,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "When Do You Add Unit of Work to Repository?",
                PromptText = "If you're already using the Repository pattern, when would you also introduce a Unit of Work on top of it?",
                SuggestedApproach = """
                    Cover:
                    - Unit of Work coordinates multiple repositories so all their pending changes commit together in one atomic operation, instead of each repository calling `SaveChanges` independently and risking partial updates.
                    - Point out that `DbContext` itself already behaves like a Unit of Work — its change tracker batches inserts/updates/deletes and `SaveChanges` commits them all in one transaction. So if your repositories all share the same injected `DbContext` instance (which they should, since it's Scoped), you often get Unit of Work "for free" without writing an explicit class for it.
                    - An explicit Unit of Work becomes valuable when you need an operation to span multiple repository calls with an obvious commit/rollback boundary in the calling code (e.g. `unitOfWork.Commit()` after updating an Order repository and an Inventory repository together), or when repositories might be backed by more than one context.
                    """,
                SampleAnswer = """
                    "Unit of Work matters when an operation touches more than one repository and all those changes need to succeed or fail together. For example, placing an order might update an `OrderRepository` and decrement stock through an `InventoryRepository` — if the stock update fails, I don't want the order to have been committed.
                    In an EF Core app, though, if both repositories are constructed on top of the same Scoped `DbContext`, I'm actually getting Unit of Work behavior for free — the DbContext's change tracker collects all the pending changes from both repositories, and a single `SaveChanges()` call commits them together in one transaction. So I don't always need a separate `IUnitOfWork` class; sometimes I just expose `SaveChangesAsync()` from the context, or a thin wrapper around it, and call it once after both repository operations.
                    I'd introduce an explicit Unit of Work abstraction mainly when I want that commit boundary to be obvious in the calling code — a service method that says `await _unitOfWork.CommitAsync()` reads clearly as 'this is the transactional boundary' — or in a codebase where repositories might not all share one context and I need to explicitly wrap an ambient transaction around several of them."
                    """,
                SortOrder = 56,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "How Do You Version a Web API?",
                PromptText = "How would you introduce versioning into an existing ASP.NET Core Web API without breaking clients that are already integrated against it?",
                SuggestedApproach = """
                    Cover the three common strategies and their tradeoffs:
                    - URL segment versioning, e.g. `/api/v1/orders` — very visible and discoverable, easy to test in a browser, plays well with caching, but "pollutes" the URL and means a resource's URI technically changes across versions.
                    - Query string versioning, e.g. `/api/orders?api-version=1.0` — keeps the base URL stable, but query strings can be stripped by some proxies/caches and are easy for clients to forget.
                    - Header-based versioning, e.g. a custom `X-Api-Version` header or media-type versioning via `Accept: application/json;v=2` — keeps URLs clean, but is less discoverable and harder to test casually (can't just hit it in a browser).
                    Mention the `Asp.Versioning` package (formerly `Microsoft.AspNetCore.Mvc.Versioning`) for wiring this into ASP.NET Core, and the importance of a clear deprecation policy/timeline once multiple versions are live.
                    """,
                SampleAnswer = """
                    "It depends on the constraints, but I've most often used URL segment versioning — `/api/v1/orders`, `/api/v2/orders` — because it's the most explicit and discoverable option; clients can see exactly which version they're calling, it's trivial to test, and it plays nicely with reverse proxies and CDN caching since the version is part of the cache key.
                    The alternative is query string versioning like `?api-version=1.0`, which keeps the base route stable but is easy for a client to omit and can get dropped by some intermediary caches. Header-based versioning — a custom header or an `Accept` media-type parameter — keeps URLs completely clean and is arguably the most 'RESTful' since the resource identity doesn't change across versions, but it's harder to explore and debug without tooling.
                    In practice I wire this up with the `Asp.Versioning` package, which supports all three conventions and lets you deprecate old versions gracefully by marking them and returning a deprecation header, rather than breaking clients outright. Whatever strategy you pick, the more important part is having a clear communicated deprecation window so consumers aren't surprised when an old version goes away."
                    """,
                SortOrder = 57,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "What Is Idempotency and Why Does It Matter?",
                PromptText = "What does it mean for a REST API endpoint to be idempotent, and why does that matter when you're designing a Web API?",
                SuggestedApproach = """
                    Cover:
                    - Definition — calling the same request multiple times produces the same end state as calling it once, with no additional side effects from the repeats.
                    - Which verbs are idempotent by spec/design: GET, PUT, DELETE (and HEAD/OPTIONS). POST is explicitly not idempotent — two identical POSTs are expected to create two resources.
                    - Why it matters in practice: networks are unreliable — a client can time out waiting for a response even though the server actually completed the request, and a naive retry could double-charge a customer or create a duplicate order.
                    - Idempotency keys for POST — the client generates a unique key (e.g. `Idempotency-Key` header) per logical operation, and the server stores/dedupes on that key so a retried POST with the same key returns the original result instead of repeating the side effect. Common in payment and order-creation APIs.
                    """,
                SampleAnswer = """
                    "An idempotent endpoint means calling it once or calling it five times in a row leaves the system in the same state — no extra side effects pile up. GET, PUT, and DELETE are supposed to be idempotent by design: PUT-ing the same resource representation twice should just leave it in that state, and deleting an already-deleted resource shouldn't error out or do anything new. POST is the odd one out — by spec it's not idempotent, since POSTing 'create an order' twice is expected to create two orders.
                    This matters most around network reliability and retries. If a client sends a POST to charge a customer, the request succeeds on the server, but the response gets lost due to a network blip, the client's HTTP layer might retry — and without protection, that's a duplicate charge. The standard fix is an idempotency key: the client generates a unique key per logical operation and sends it as a header, like `Idempotency-Key: abc123`. The server checks if it's already processed that key; if so, it just returns the original result instead of re-running the side effect. Stripe's API is the textbook example of this pattern. I make sure PUT and DELETE handlers in my own APIs are actually idempotent in practice too — e.g. a DELETE on a resource that's already gone should return a 204/404 rather than throwing, so retries are always safe."
                    """,
                SortOrder = 58,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Handling Concurrent Updates to the Same Record",
                PromptText = "Two users load the same record, edit different fields, and save around the same time — how do you make sure one update doesn't silently clobber the other?",
                SuggestedApproach = """
                    Cover optimistic concurrency as the default approach in EF Core:
                    - Add a concurrency token (a `rowversion`/`timestamp` column, or a `[ConcurrencyCheck]` property) to the entity.
                    - EF Core includes that column in the `WHERE` clause of the generated `UPDATE`, so if the row changed since it was read, zero rows match and EF Core throws `DbUpdateConcurrencyException`.
                    - Catch that exception and decide a resolution strategy: reload and let the user retry, "client wins" (overwrite with the client's values), "database wins" (discard the client's change), or a merge UI showing both versions.
                    Contrast with pessimistic locking (`SELECT ... FOR UPDATE` / holding a transaction lock across the edit), which prevents the conflict outright but blocks other readers/writers and hurts throughput and scalability — rarely appropriate for a typical stateless web API with pooled connections.
                    """,
                SampleAnswer = """
                    "My default is optimistic concurrency, not locking. I'd add a `rowversion` column — in EF Core that's a `byte[]` property mapped with `[Timestamp]` — to the entity. EF Core automatically includes that column's original value in the `WHERE` clause of the `UPDATE` statement it generates. So if User A and User B both load the record, and User A saves first, the rowversion changes; when User B's save comes through with the old rowversion value, zero rows match the `WHERE` clause, and EF Core throws a `DbUpdateConcurrencyException`.
                    I catch that exception in the service layer and decide what to do — usually I reload the current values from the database, compare them against what the user submitted, and either automatically merge non-conflicting fields (since they may have edited different fields entirely) or surface both versions to the user and let them decide which values to keep. What I try to avoid is a silent last-write-wins overwrite, since that's exactly the bug this pattern is meant to prevent.
                    I'd only reach for pessimistic locking — like `SELECT ... FOR UPDATE` held across the whole edit — in narrow cases like a financial ledger update where losing isn't acceptable and the lock window is very short. For a typical web API, holding a lock across a user's think-time while they're editing a form would tie up a connection from the pool and kill throughput, so optimistic concurrency with a clear conflict-resolution flow is almost always the better tradeoff."
                    """,
                DiagramBody = """
                    [{"label":"User A and User B both load the record","note":"same rowversion value"},{"label":"User A saves first","note":"rowversion updates in the database"},{"label":"User B saves with the old rowversion","note":"WHERE clause matches zero rows"},{"label":"EF Core throws DbUpdateConcurrencyException"},{"label":"Reload, compare, merge or ask the user","note":"never a silent last-write-wins overwrite"}]
                    """,
                DiagramFormat = DiagramFormat.StructuredSteps,
                SortOrder = 59,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "A Dependency Your API Calls Is Slow",
                PromptText = "Your API calls a third-party service that's started responding slowly, and it's dragging down your own API's response times — what's your approach?",
                SuggestedApproach = """
                    Cover a layered response, not just one fix:
                    - Set explicit, sane timeouts on the `HttpClient` so a slow call fails fast instead of hanging indefinitely and tying up resources.
                    - Add a circuit breaker (e.g. Polly's `CircuitBreakerPolicy` via `AddResilienceHandler`/`AddPolicyHandler`) so once failures cross a threshold, further calls fail immediately instead of piling up against a struggling dependency.
                    - Add retry with exponential backoff and jitter for transient failures — but only for idempotent calls.
                    - Cache responses where staleness is acceptable, so you're not calling the slow dependency on every request.
                    - Parallelize independent outbound calls with `Task.WhenAll` instead of awaiting them sequentially, and make sure the call path is genuinely async end-to-end so it doesn't block thread pool threads.
                    - Consider a fallback/default response when the dependency is unavailable, if the business logic allows for graceful degradation.
                    """,
                SampleAnswer = """
                    "First I'd make sure the `HttpClient` calling that service has an explicit timeout — if it's relying on the default, a slow dependency can hold connections and threads open far longer than acceptable. Then I'd wrap the call in a Polly resilience pipeline: a circuit breaker so that once we see a run of failures or timeouts, we stop hammering a struggling service and fail fast for a cooldown period, plus a retry policy with exponential backoff and jitter for transient errors — but only on calls that are safe to retry, since retrying a non-idempotent POST could cause duplicate side effects.
                    Beyond resilience policies, I'd look at whether we can reduce how often we call it at all — caching the response for a reasonable TTL if the data doesn't need to be real-time, which takes pressure off the dependency and off our own response times. If we're calling multiple independent external services in one request handler, I'd make sure we're using `Task.WhenAll` to run them concurrently instead of awaiting them one after another.
                    And if the business case allows it, I'd add a fallback — return cached or default data with a flag indicating it's degraded, rather than failing the whole request just because one downstream call is slow. The overall goal is to contain the blast radius so one flaky dependency doesn't take down our entire API's latency profile."
                    """,
                SortOrder = 60,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Investigating Intermittent Production API Failures",
                PromptText = "Users are reporting that API calls fail every so often, but you can't reproduce it locally and there's no obvious pattern — how do you track it down?",
                SuggestedApproach = """
                    Cover:
                    - Structured logging with correlation/trace IDs threaded through the request (ASP.NET Core's `HttpContext.TraceIdentifier` or a custom correlation-ID middleware) so failures can be traced across services/log entries for the exact failing request.
                    - Centralize logs and traces in an APM tool like Application Insights, and look for correlation between failure rate and time of day, load, or specific endpoints/dependencies.
                    - Check for transient dependency errors — timeouts against SQL Server, external HTTP calls, etc. — which point toward needing a retry policy.
                    - Check for thread pool starvation (blocking async calls with `.Result`/`.Wait()`, or heavy synchronous work) and connection pool exhaustion (SQL "Timeout expired, the timeout period elapsed prior to obtaining a connection from the pool" errors), both of which cause exactly this kind of intermittent-under-load symptom.
                    - Check resource exhaustion under load — CPU spikes, GC pauses, memory pressure.
                    """,
                SampleAnswer = """
                    "Since it's intermittent and load-related, I start by making sure I can actually correlate a failed request end-to-end. If we don't already have one, I'd add correlation IDs — ASP.NET Core gives you a `TraceIdentifier` per request, and if this spans multiple services, I'd propagate a correlation ID header through all of them — so I can pull every log line and trace for one specific failing request in Application Insights instead of guessing.
                    Once I can see actual failing requests, I'd check a few usual suspects: are these failures clustering around load spikes? If so, I'd suspect either thread pool starvation — often caused by blocking calls like `.Result` or `.Wait()` on a Task somewhere in the pipeline, which exhausts the thread pool under concurrency even though it works fine with one request at a time — or SQL connection pool exhaustion, which shows up as 'Timeout expired, the timeout period elapsed prior to obtaining a connection from the pool' errors when too many requests are holding connections open simultaneously, often because a DbContext or connection wasn't disposed properly.
                    I'd also check whether the failures are actually transient errors against a downstream dependency — a database or external API timing out occasionally — in which case the real fix is adding a retry policy, not chasing a phantom bug in our own code. Application Insights' failure and dependency views are usually enough to tell me which of these it is within a few minutes of looking at real data."
                    """,
                SortOrder = 61,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "A Deployment Broke All APIs",
                PromptText = "You just shipped a deployment and immediately every API endpoint starts failing — what are your first troubleshooting steps?",
                SuggestedApproach = """
                    Cover:
                    - Stop the bleeding first — roll back to the last known-good deployment/release rather than debugging live in production while every endpoint is down.
                    - Then diagnose: check configuration differences between environments — missing or mismatched connection strings, app settings, or secrets in the new environment/slot — a very common cause of "works locally, fails everywhere in prod."
                    - Check startup logs for exceptions — DI misconfiguration (a service that can't be resolved), a pending EF Core migration that wasn't applied, or a bad `appsettings` value that throws at startup.
                    - Check infrastructure dependencies that could have changed independently of the deploy — an expired certificate, a changed firewall/network rule, a rotated secret.
                    - Prevention going forward: canary or blue-green deployment with deployment slots and a health-check gate before swapping 100% of traffic, plus automated smoke tests immediately after deploy.
                    """,
                SampleAnswer = """
                    "First priority is restoring service, not root-causing live — I'd roll back to the last known-good deployment immediately rather than debugging with every endpoint down in production. Once traffic is stable again, I'd dig into what actually broke.
                    The most common culprit in my experience is a configuration mismatch between environments — a connection string, an app setting, or a secret that exists locally or in staging but wasn't set (or was set incorrectly) in production. I'd check the startup logs first, since ASP.NET Core will usually throw clearly at startup if DI can't resolve a service or a required configuration value is missing — that's often faster to spot than digging through request-level errors. I'd also check whether there was a pending EF Core migration that got missed, since that alone can take down every endpoint that touches the database.
                    If configuration and migrations check out, I'd look at anything infrastructure-related that might have shifted independently — an expiring TLS certificate, a firewall or network security group rule, a rotated API key or connection secret.
                    Longer-term, this is exactly the failure mode that blue-green or canary deployments with deployment slots are meant to prevent — swap into a staging slot, run automated smoke tests and a health check gate, and only shift full production traffic once those pass, so a bad release never actually reaches 100% of users in the first place."
                    """,
                DiagramBody = """
                    [{"label":"Every API endpoint fails post-deploy"},{"label":"Roll back to last known-good release","note":"restore service before root-causing"},{"label":"Check startup logs","note":"DI resolution failures throw clearly at boot"},{"label":"Check config mismatch between environments","note":"connection strings, app settings, secrets"},{"label":"Check for a missed EF Core migration"},{"label":"Check infrastructure drift","note":"expired cert, firewall rule, rotated secret"},{"label":"Prevent next time: canary/blue-green + smoke tests"}]
                    """,
                DiagramFormat = DiagramFormat.StructuredSteps,
                SortOrder = 62,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
            new InterviewQuestion
            {
                QuestionType = QuestionType.Technical,
                Title = "Diagnosing a Memory Leak Slowing Down the API",
                PromptText = "Your API's response times get progressively worse the longer it's been running, and memory usage keeps climbing until you restart it — how do you find and fix the leak?",
                SuggestedApproach = """
                    Cover:
                    - Capture evidence first: take memory snapshots over time with `dotnet-gcdump`/`dotnet-dump`, or use Application Insights' memory profiling / a tool like PerfView, and compare object counts and retained sizes across snapshots to see what's growing.
                    - Look at what's holding GC roots to the growing objects — that's what tells you it's a genuine leak rather than normal GC behavior.
                    - Check the classic root causes: static event handlers where subscribers are added but never unsubscribed (the publisher holds a reference forever, so subscriber objects can never be collected); undisposed `IDisposable` resources like `HttpClient` instances (created per-request instead of via `IHttpClientFactory`), `DbContext`, streams, or timers not wrapped in `using`; unbounded in-memory caches (`MemoryCache`, a static `Dictionary`, or a custom cache) with no eviction/expiration policy so entries accumulate forever.
                    - Fix: unsubscribe event handlers (or use weak event patterns), ensure disposables are scoped/disposed properly, and add size limits or sliding/absolute expiration to caches.
                    """,
                SampleAnswer = """
                    "First I'd confirm it's an actual leak and not just normal GC behavior under load — I'd take a `dotnet-gcdump` snapshot early on and another one after memory has climbed significantly, then diff them to see which object types are growing and, more importantly, what's still holding a GC root to them. If we already have Application Insights wired up, its memory profiling can often point at the same thing without needing to grab dumps manually.
                    In practice, three causes show up over and over. The first is a static or long-lived event publisher where handlers subscribe but never unsubscribe — the publisher holds a reference to every subscriber forever, so those subscriber objects can never be collected even after they're logically 'done.' The second is undisposed `IDisposable` resources — a classic one is creating a new `HttpClient` per request instead of using `IHttpClientFactory`, which leaks socket handles under load, or a `DbContext` or file stream that isn't wrapped in a `using` block. The third is an unbounded in-memory cache — a static `Dictionary` or a `MemoryCache` instance that entries get added to on every request with no eviction policy, so it just grows without bound.
                    Once the dump tells me which of those it is, the fix follows directly: unsubscribe handlers in a `Dispose` method (or switch to a weak-event pattern if the lifetime relationship is inherently mismatched), make sure disposables are properly scoped and disposed, and add a `SizeLimit` plus expiration policy to any in-memory cache so old entries actually get evicted instead of accumulating indefinitely."
                    """,
                SortOrder = 63,
                CreatedUtc = now,
                UpdatedUtc = now,
            },
        };

        return (questions, [microsoft, amazon, google, meta]);
    }
}
