using MentorOS.Models;
using MentorOS.Models.Enums;

namespace MentorOS.Data.Seed;

// One fully-realized module+lesson per topic (the "10-20% of curriculum,
// broad not deep" slice), each touching all 7 LessonContentBlock types.
// More lessons get added later using this exact same pattern.
public static class CurriculumContentSeedData
{
    public static List<Module> BuildModules(IReadOnlyDictionary<string, int> topicIdBySlug)
    {
        return
        [
            BuildCSharpModule(topicIdBySlug["csharp"]),
            BuildDotNetModule(topicIdBySlug["dotnet"]),
            BuildDsaModule(topicIdBySlug["dsa"]),
            BuildSystemDesignModule(topicIdBySlug["system-design"]),
            BuildSqlModule(topicIdBySlug["sql"]),
            BuildCloudModule(topicIdBySlug["cloud"]),
            BuildInterviewPrepModule(topicIdBySlug["interview-prep"]),
        ];
    }

    private static Module BuildCSharpModule(int topicId)
    {
        var lesson = BuildLesson(
            slug: "variables-types-control-flow",
            title: "Variables, Types & Control Flow",
            summary: "Value vs. reference types, type inference, and modern pattern-matching control flow.",
            estimatedMinutes: 30,
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    C# is a **statically-typed**, object-oriented language that compiles to Intermediate Language (IL), which the CLR then JIT-compiles to native code at runtime.

                    Every variable has a type known at compile time. Types fall into two categories:

                    - **Value types** (`int`, `double`, `bool`, `struct`) — store their data directly, live on the stack (or inline in a containing object), and are copied by value on assignment.
                    - **Reference types** (`class`, `string`, arrays, `object`) — store a reference to data on the heap. Assignment copies the reference, not the data.

                    `var` lets the compiler infer a variable's type from its initializer. It is still statically typed — `var count = 5;` makes `count` an `int` forever, it is not dynamic typing.

                    Control flow in modern C# goes beyond `if`/`else` and `for`/`while`. **Pattern matching** (`switch` expressions, `is` patterns) lets you branch on both a value and its shape in one expression, which is often clearer than a chain of `if`/`else if`.
                    """, 1),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Common primitive types**

                    - `int` — 32-bit signed integer
                    - `long` — 64-bit signed integer
                    - `double` — 64-bit floating point
                    - `decimal` — 128-bit, base-10, use for money
                    - `bool` — true/false
                    - `string` — immutable sequence of UTF-16 characters
                    - `char` — a single UTF-16 code unit

                    **Type conversion**

                    - Implicit: only when no data can be lost (`int` -> `long`)
                    - Explicit cast: `(int)someDouble` — can lose precision or throw
                    - `Convert.ToInt32(x)` — handles `null` and different types safely
                    - `int.TryParse(text, out var value)` — never throws, returns `false` on failure
                    """, 2),
                Block(BlockType.CodeSnippet, "Pattern Matching with Switch Expressions", BodyFormat.PlainText, """
                    public record Circle(double Radius);
                    public record Rectangle(double Width, double Height);

                    string Describe(object shape) => shape switch
                    {
                        Circle { Radius: > 10 } => "large circle",
                        Circle => "circle",
                        Rectangle { Width: var w, Height: var h } when w == h => "square",
                        Rectangle => "rectangle",
                        null => "nothing",
                        _ => "unknown shape",
                    };
                    """, 3, language: "csharp"),
                Block(BlockType.Diagram, "From Source Code to Native Code", BodyFormat.StructuredSteps, """
                    [{"label":"C# Source (.cs)"},{"label":"Roslyn Compiler","note":"syntax + semantic analysis"},{"label":"IL (.dll)","note":"platform-independent bytecode"},{"label":"CLR JIT","note":"at runtime"},{"label":"Native Machine Code"}]
                    """, 4),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Prefer `is` pattern matching over explicit casts when you need to branch on type:

                    - Avoid: `if (obj is Circle) { var c = (Circle)obj; ... }`
                    - Prefer: `if (obj is Circle c) { ... }`

                    Use `var` when the type is obvious from the right-hand side (`var list = new List<int>();`), and an explicit type when it improves readability (`int total = ComputeTotal();` where the method name doesn't make the return type obvious).
                    """, 5),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    Interviewers often probe value vs. reference semantics with a small "what does this print?" question. Be ready to explain, out loud, why passing a `struct` to a method doesn't affect the caller's copy, while passing a `class` instance does — and why `string`, despite being a reference type, *behaves* like a value type because it's immutable.
                    """, 6),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Comparing floating-point numbers with `==` (`0.1 + 0.2 == 0.3` is `false` due to binary floating-point rounding). Use a tolerance comparison (`Math.Abs(a - b) < epsilon`) or `decimal` for exact base-10 arithmetic like money.

                    Another common one: boxing a value type into `object` inside a hot loop (e.g., adding `int`s to a non-generic `ArrayList`), which allocates on the heap for every value and silently kills performance.
                    """, 7),
            ]);

        return BuildModule(topicId, "csharp-fundamentals", "C# Fundamentals",
            "Language fundamentals every C# developer needs before going deeper into OOP, LINQ, and async.",
            60, lesson);
    }

    private static Module BuildDotNetModule(int topicId)
    {
        var lesson = BuildLesson(
            slug: "building-first-minimal-api",
            title: "Building Your First Minimal API",
            summary: "The middleware pipeline, dependency injection lifetimes, and writing thin endpoint handlers.",
            estimatedMinutes: 40,
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    ASP.NET Core Minimal APIs let you define HTTP endpoints directly against a `WebApplication` instance — `app.MapGet(...)`, `app.MapPost(...)` — without the ceremony of a full MVC controller class. They're built on the same underlying request pipeline as MVC, just with a lighter-weight programming model.

                    Every request flows through a **middleware pipeline**: an ordered chain of components, each of which can inspect/modify the request, call the next middleware, and then inspect/modify the response on the way back out. Order matters — for example, authentication middleware must run before authorization, and both typically run before routing resolves to your endpoint.

                    **Dependency Injection (DI)** is built into the framework, not bolted on. Services are registered against `builder.Services` with a **lifetime**:

                    - **Transient** — a new instance every time it's requested.
                    - **Scoped** — one instance per HTTP request (this is what `DbContext` should almost always use).
                    - **Singleton** — one instance for the lifetime of the application.
                    """, 1),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Common `builder.Services` calls**

                    - `AddDbContext<T>()` — registers EF Core, Scoped by default
                    - `AddScoped<TService, TImpl>()` / `AddSingleton` / `AddTransient`
                    - `AddControllers()` — if you want MVC controllers alongside minimal APIs
                    - `ConfigureHttpJsonOptions(...)` — customize System.Text.Json behavior globally

                    **Common `app.Map*` verbs**

                    - `app.MapGet("/path", handler)`
                    - `app.MapPost("/path", handler)`
                    - `app.MapPut` / `MapPatch` / `MapDelete`
                    - `app.MapGroup("/api/things")` — groups related endpoints under one prefix
                    """, 2),
                Block(BlockType.CodeSnippet, "A Thin Minimal API Endpoint Pair", BodyFormat.PlainText, """
                    app.MapGet("/api/tasks/{id:int}", async (int id, AppDbContext db) =>
                    {
                        var task = await db.Tasks.FindAsync(id);
                        return task is null ? Results.NotFound() : Results.Ok(task);
                    });

                    app.MapPost("/api/tasks", async (CreateTaskRequest request, AppDbContext db) =>
                    {
                        var task = new TaskItem { Title = request.Title };
                        db.Tasks.Add(task);
                        await db.SaveChangesAsync();
                        return Results.Created($"/api/tasks/{task.Id}", task);
                    });
                    """, 3, language: "csharp"),
                Block(BlockType.Diagram, "The Request Pipeline", BodyFormat.StructuredSteps, """
                    [{"label":"Incoming Request"},{"label":"Exception Handling MW"},{"label":"HTTPS Redirection"},{"label":"Routing"},{"label":"Auth (if configured)"},{"label":"Endpoint Handler"},{"label":"Response"}]
                    """, 4),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Register `DbContext` as **Scoped** (the default for `AddDbContext`) and never inject it into a Singleton service directly — that creates a **captive dependency**: the short-lived, per-request `DbContext` gets held alive for the whole application lifetime, and EF Core is not thread-safe, so concurrent requests will corrupt its internal state.

                    Keep endpoint handlers thin — validate input, call a service/repository, map to a DTO, return a result. Business logic belongs in a service class you can unit test without spinning up the whole HTTP pipeline.
                    """, 5),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    Be ready to explain the three DI lifetimes with a concrete failure scenario, not just the definitions — e.g., "if you inject a Scoped `DbContext` into a Singleton, the Singleton captures the first request's `DbContext` forever, and every later request reuses that same disposed instance." That story demonstrates you understand *why* the lifetimes exist, not just their names.
                    """, 6),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Forgetting to `await` an async call inside an endpoint handler — the request completes before the operation finishes, and exceptions from the un-awaited task get swallowed instead of surfacing as a 500 response.

                    Also common: returning the raw EF Core entity from an endpoint instead of a DTO, which can leak internal fields, cause circular-reference JSON serialization errors on navigation properties, and tightly couples your API contract to your database schema.
                    """, 7),
            ]);

        var capstone = new CapstoneProject
        {
            Title = "Build and Deploy a Minimal API Task Tracker",
            Description = """
                Apply everything from this module by building a small but complete Minimal API: a task tracker with full CRUD, backed by EF Core and SQLite, following the same patterns (DI lifetimes, thin endpoint handlers, DTOs) used throughout Mentor OS itself.
                """,
            Requirements = """
                - A `TaskItem` entity with `Id`, `Title`, `IsDone`, `CreatedUtc`.
                - EF Core + SQLite persistence, with a migration.
                - Endpoints: `GET /api/tasks`, `GET /api/tasks/{id}`, `POST /api/tasks`, `PATCH /api/tasks/{id}/complete`, `DELETE /api/tasks/{id}`.
                - Request/response DTOs — never serialize the EF entity directly.
                - Basic input validation (reject an empty `Title`) returning `400 Bad Request`.
                """,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow,
            ChecklistItems =
            [
                new CapstoneChecklistItem { Description = "Scaffold a minimal API project and add EF Core + SQLite.", SortOrder = 1 },
                new CapstoneChecklistItem { Description = "Model a TaskItem entity and generate the initial migration.", SortOrder = 2 },
                new CapstoneChecklistItem { Description = "Implement all 5 CRUD endpoints using DTOs, not raw entities.", SortOrder = 3 },
                new CapstoneChecklistItem { Description = "Add validation that rejects an empty task title with a 400.", SortOrder = 4 },
                new CapstoneChecklistItem { Description = "Manually verify every endpoint with curl, including the not-found and validation-failure cases.", SortOrder = 5 },
            ],
        };

        return BuildModule(topicId, "aspnet-core-basics", "ASP.NET Core Basics",
            "The middleware pipeline, dependency injection, and building your first Minimal API.",
            90, lesson, capstone);
    }

    private static Module BuildDsaModule(int topicId)
    {
        var lesson = BuildLesson(
            slug: "two-pointer-hash-map-patterns",
            title: "Two-Pointer & Hash Map Patterns",
            summary: "The two patterns that solve most array/string interview problems, and when to reach for which.",
            estimatedMinutes: 45,
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    A huge fraction of array/string interview problems reduce to one of two patterns:

                    **Hash map (or hash set) for O(1) lookups** — trade `O(n)` extra space for turning an `O(n)` linear search into an `O(1)` lookup, collapsing an `O(n²)` brute force into `O(n)`. Reach for this whenever the question is some version of "have I seen this value/complement before?"

                    **Two pointers on a sorted array** — when the array is sorted (or can cheaply be sorted), two pointers starting at opposite ends can converge toward a target in a single `O(n)` pass, because moving the left pointer only increases the sum and moving the right pointer only decreases it — you never need to re-check a pair twice.

                    The two patterns are often interchangeable for the *same* problem (e.g., Two Sum): hash map works on unsorted input in `O(n)` time / `O(n)` space; two pointers needs sorted input but uses `O(1)` extra space.
                    """, 1),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Complexity of common operations**

                    - Hash map/set: average `O(1)` insert, lookup, delete; `O(n)` worst case (hash collisions)
                    - Sorting an array: `O(n log n)` time, enables two-pointer techniques afterward
                    - Two pointers over a sorted array: `O(n)` time, `O(1)` extra space

                    **When to reach for which**

                    - Need original index order preserved, or the array isn't sorted and sorting would lose information you need (like original indices) → hash map
                    - Need `O(1)` space, and the array is already sorted or sorting it doesn't lose information you need → two pointers
                    """, 2),
                Block(BlockType.CodeSnippet, "Two Sum via Hash Map", BodyFormat.PlainText, """
                    // Two Sum: O(n) time, O(n) space, using a hash map of value -> index.
                    public int[] TwoSum(int[] nums, int target)
                    {
                        var seen = new Dictionary<int, int>(); // value -> index

                        for (var i = 0; i < nums.Length; i++)
                        {
                            var complement = target - nums[i];
                            if (seen.TryGetValue(complement, out var complementIndex))
                            {
                                return [complementIndex, i];
                            }

                            seen[nums[i]] = i;
                        }

                        throw new ArgumentException("No two numbers sum to the target.");
                    }
                    """, 3, language: "csharp"),
                Block(BlockType.Diagram, "Two Pointers Converging on a Sorted Array", BodyFormat.AsciiArt, """
                    Sorted array: [ 2   7   11   15 ]
                                    ^               ^
                                  left            right

                    left + right sum > target  ->  right--
                    left + right sum < target  ->  left++
                    left + right sum == target ->  found the pair
                    """, 4),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    In an interview, state the brute-force `O(n²)` solution out loud first — it proves you can solve the problem at all — then explicitly name the trade-off you're making to optimize it ("I can trade `O(n)` space for a hash map to get this down to `O(n)` time"). Interviewers weight *how* you get to the optimal solution as much as the final code.
                    """, 5),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    When you reach for a hash map, say the complexity trade-off out loud unprompted: "I'll use a hash map here — `O(n)` extra space — to bring this down from `O(n²)` to `O(n)` time." Naming the trade-off before being asked signals you understand it rather than having memorized the pattern.
                    """, 6),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Writing the nested-loop `O(n²)` brute force and stopping there without recognizing a hash map or two-pointer approach applies — most interviewers expect you to at least identify the optimization opportunity, even under time pressure.

                    Also common: using a hash map when the problem actually needs two pointers on sorted data (e.g., finding a pair in a sorted array without preserving original indices) — it works, but burns unnecessary `O(n)` space and signals you're pattern-matching without understanding *why* two pointers apply here.
                    """, 7),
            ]);

        return BuildModule(topicId, "arrays-and-hashing", "Arrays & Hashing",
            "The foundational patterns — hash maps and two pointers — that unlock most array and string problems.",
            90, lesson);
    }

    private static Module BuildSystemDesignModule(int topicId)
    {
        var lesson = BuildLesson(
            slug: "scaling-load-balancing-caching",
            title: "Scaling a Single Server: Load Balancing & Caching",
            summary: "Vertical vs. horizontal scaling, load balancer strategies, and the cache-aside pattern.",
            estimatedMinutes: 45,
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    Every system design problem starts from the same place: a single server handling all traffic, which eventually can't keep up. There are two ways to add capacity:

                    - **Vertical scaling** — a bigger machine (more CPU/RAM). Simple, but has a hard ceiling and a single point of failure.
                    - **Horizontal scaling** — more machines behind a **load balancer**. No hard ceiling, and machines can fail without taking the whole system down — but it only works if the application servers are **stateless**, so any server can handle any request.

                    A **load balancer** distributes incoming requests across a pool of servers using a strategy like round-robin, least-connections, or consistent hashing, and continuously health-checks servers so it stops routing to ones that are down.

                    **Caching** reduces load on your database/backend by serving frequently-requested data from fast, in-memory storage. The **cache-aside** pattern is the most common: on a read, check the cache first; on a miss, read from the database and populate the cache; on a write, update the database and invalidate (or update) the cache entry.
                    """, 1),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **CAP theorem** — under a network partition, a distributed system must choose:

                    - **Consistency** — every read gets the latest write, or an error
                    - **Availability** — every request gets a (possibly stale) response

                    (Partition tolerance isn't optional in a real distributed system, so this is really a consistency-vs-availability trade-off during a partition.)

                    **Common caching strategies**

                    - Cache-aside — app manages cache population/invalidation (most common, most flexible)
                    - Write-through — every write goes to the cache and the database together (simpler, slower writes)
                    - TTL (time-to-live) eviction — accept some staleness in exchange for simplicity
                    """, 2),
                Block(BlockType.CodeSnippet, "Cache-Aside Pattern", BodyFormat.PlainText, """
                    public async Task<Product> GetProductAsync(int id)
                    {
                        var cacheKey = $"product:{id}";

                        if (cache.TryGetValue(cacheKey, out Product? cached))
                        {
                            return cached!;
                        }

                        var product = await db.Products.FindAsync(id)
                            ?? throw new KeyNotFoundException($"Product {id} not found.");

                        cache.Set(cacheKey, product, TimeSpan.FromMinutes(10));
                        return product;
                    }
                    """, 3, language: "csharp"),
                Block(BlockType.Diagram, "Client to Database, With a Cache", BodyFormat.StructuredSteps, """
                    [{"label":"Client"},{"label":"Load Balancer","note":"health-checks + distributes"},{"label":"App Servers (N)","note":"stateless, horizontally scaled"},{"label":"Cache","note":"cache-aside"},{"label":"Database"}]
                    """, 4),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Keep application servers **stateless** — don't store session data in server memory. Put shared state in the cache or database instead, so the load balancer can route any request to any server, and a server can be added, removed, or restarted without losing user sessions.
                    """, 5),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    Before drawing a single box, clarify requirements out loud: expected read/write ratio, latency targets, consistency requirements, and rough scale (requests/second, data size). A design built on the wrong assumptions — however elegant — reads as a red flag, not a strength.
                    """, 6),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Jumping straight to "microservices" or "shard the database" in the first two minutes without establishing that the scale actually requires it. Most systems can go a very long way on a well-indexed single database, a cache, and a couple of stateless app servers behind a load balancer — reaching for distributed-systems complexity too early is a common way candidates lose points, not gain them.
                    """, 7),
            ]);

        return BuildModule(topicId, "system-design-fundamentals", "System Design Fundamentals",
            "How to scale a single server into a resilient, horizontally-scaled system.",
            90, lesson);
    }

    private static Module BuildSqlModule(int topicId)
    {
        var lesson = BuildLesson(
            slug: "select-join-query-fundamentals",
            title: "SELECT, JOIN, and Query Fundamentals",
            summary: "Logical query evaluation order, join types, and how NULL actually behaves.",
            estimatedMinutes: 30,
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    A `SELECT` query is evaluated in a different order than it's written, which explains a lot of confusing SQL behavior:

                    1. `FROM` / `JOIN` — build the working row set
                    2. `WHERE` — filter individual rows
                    3. `GROUP BY` — collapse rows into groups
                    4. `HAVING` — filter groups (this is why you can't filter an aggregate in `WHERE`)
                    5. `SELECT` — compute the output columns
                    6. `ORDER BY` — sort the final result

                    **Joins** combine rows from two tables based on a matching condition:

                    - **INNER JOIN** — only rows with a match in both tables
                    - **LEFT JOIN** — all rows from the left table, plus matches from the right (unmatched right columns are `NULL`)
                    - **FULL OUTER JOIN** — all rows from both tables, matched where possible

                    `NULL` is not a value — it's the absence of one. `NULL = NULL` evaluates to `NULL` (not `true`), which is why you must use `IS NULL` / `IS NOT NULL` rather than `=`/`!=`.
                    """, 1),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Join types at a glance**

                    - `INNER JOIN` — intersection only
                    - `LEFT JOIN` — everything on the left, matched or not
                    - `RIGHT JOIN` — everything on the right, matched or not (rare; usually rewritten as a `LEFT JOIN` with tables swapped)
                    - `FULL OUTER JOIN` — union of left and right, matched where possible
                    - `CROSS JOIN` — every row of A paired with every row of B (Cartesian product)

                    **Aggregate functions**: `COUNT`, `SUM`, `AVG`, `MIN`, `MAX` — always used with `GROUP BY` unless aggregating the entire table.
                    """, 2),
                Block(BlockType.CodeSnippet, "Customers with More Than 3 Orders", BodyFormat.PlainText, """
                    SELECT
                        c.customer_id,
                        c.name,
                        COUNT(o.order_id) AS order_count
                    FROM customers AS c
                    INNER JOIN orders AS o ON o.customer_id = c.customer_id
                    GROUP BY c.customer_id, c.name
                    HAVING COUNT(o.order_id) > 3
                    ORDER BY order_count DESC;
                    """, 3, language: "sql"),
                Block(BlockType.Diagram, "Join Types", BodyFormat.AsciiArt, """
                    INNER JOIN              LEFT JOIN                FULL OUTER JOIN

                      A    B                  A    B                    A    B
                     ( ( X ) )              (███( X ))                (███( X )███)

                    only the overlap     all of A, unmatched     everything, matched
                                         B becomes NULL          where possible
                    """, 4),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Always index columns used in `JOIN` conditions and `WHERE` clauses on large tables — without an index, the database falls back to a full table scan for every query, which is fine at 100 rows and catastrophic at 100 million.

                    Write `JOIN ... ON` conditions explicitly rather than relying on implicit joins via `WHERE` (`FROM a, b WHERE a.id = b.a_id`) — the explicit form is easier to read and harder to accidentally turn into a Cartesian product.
                    """, 5),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    Classic take-home/whiteboard SQL questions — "find the second-highest salary," "find duplicate emails," "find customers with no orders" — are really testing whether you understand `NULL` handling, `GROUP BY`/`HAVING`, and self-joins, not whether you've memorized syntax. Talk through your logical evaluation order (`FROM` → `WHERE` → `GROUP BY` → `HAVING` → `SELECT`) as you write the query.
                    """, 6),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Using `SELECT *` in production code — it breaks silently when columns are added, reordered, or removed upstream, and pulls more data over the wire than the caller needs.

                    Also common: trying to filter an aggregate in `WHERE` instead of `HAVING` (`WHERE COUNT(*) > 3` is invalid — `WHERE` runs before aggregation exists), and forgetting that comparing anything to `NULL` with `=` silently returns no rows instead of an error.
                    """, 7),
            ]);

        return BuildModule(topicId, "sql-fundamentals", "SQL Fundamentals",
            "Query evaluation order, join types, and the NULL-handling gotchas that trip up most candidates.",
            60, lesson);
    }

    private static Module BuildCloudModule(int topicId)
    {
        var lesson = BuildLesson(
            slug: "compute-storage-networking-basics",
            title: "Compute, Storage, and Networking Basics",
            summary: "IaaS/PaaS/SaaS, compute options from VMs to serverless, and storage tiers.",
            estimatedMinutes: 30,
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    Cloud services are usually grouped by how much of the stack the provider manages:

                    - **IaaS** (Infrastructure as a Service) — you manage the OS and up; the provider manages physical hardware/networking/virtualization (e.g., a raw VM).
                    - **PaaS** (Platform as a Service) — the provider also manages the OS/runtime; you just deploy code (e.g., a managed app-hosting service).
                    - **SaaS** (Software as a Service) — you just use a finished application (e.g., a hosted email service).

                    **Compute options**, roughly from most to least control:

                    - **Virtual machines** — full OS control, you manage patching/scaling yourself.
                    - **Containers** — package an app with its dependencies; more portable and faster to start than a VM, typically orchestrated by something like Kubernetes.
                    - **Serverless functions** — you provide only the code; the platform handles provisioning and scaling (including to zero), and you pay per invocation rather than per hour.

                    **Storage tiers** trade cost against latency/durability: hot (frequent access, higher cost), cool/cold (infrequent access, lower cost), archive (rare access, lowest cost, higher retrieval latency).
                    """, 1),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Rough service-category equivalents across providers**

                    - Virtual machines: EC2 (AWS) / Azure VMs / Compute Engine (GCP)
                    - Managed containers: ECS/EKS (AWS) / AKS (Azure) / GKE (GCP)
                    - Serverless functions: Lambda (AWS) / Azure Functions / Cloud Functions (GCP)
                    - Object storage: S3 (AWS) / Blob Storage (Azure) / Cloud Storage (GCP)
                    - Managed relational DB: RDS (AWS) / Azure SQL / Cloud SQL (GCP)
                    """, 2),
                Block(BlockType.CodeSnippet, "Multi-Stage Dockerfile for an ASP.NET Core App", BodyFormat.PlainText, """
                    FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
                    WORKDIR /app
                    EXPOSE 8080

                    FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
                    WORKDIR /src
                    COPY . .
                    RUN dotnet publish -c Release -o /app/publish

                    FROM base AS final
                    WORKDIR /app
                    COPY --from=build /app/publish .
                    ENTRYPOINT ["dotnet", "MentorOS.dll"]
                    """, 3, language: "dockerfile"),
                Block(BlockType.Diagram, "A Request Through a Typical Cloud Deployment", BodyFormat.StructuredSteps, """
                    [{"label":"Client"},{"label":"CDN","note":"cached static assets"},{"label":"Load Balancer"},{"label":"Compute","note":"containers or serverless"},{"label":"Managed Database"}]
                    """, 4),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Apply the principle of **least privilege**: grant each service/role only the specific permissions it needs (e.g., read-only access to one storage bucket), not broad admin access "to be safe." A compromised credential with narrow permissions limits the blast radius of a security incident.

                    Prefer **autoscaling** over manually provisioning for peak load — pay for capacity you're actually using, and let the platform scale compute up during traffic spikes and back down afterward.
                    """, 5),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    When asked to compare compute options, frame the answer around trade-offs, not just facts: "serverless minimizes operational overhead and cost-at-idle, but has cold-start latency and execution time limits, so it's a poor fit for long-running or latency-critical workloads — containers are the better fit there."
                    """, 6),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Over-provisioning compute "just in case" instead of configuring autoscaling — this quietly wastes money every hour the extra capacity sits idle. Equally common: granting overly broad IAM permissions during initial setup and never tightening them once the service is working.
                    """, 7),
            ]);

        return BuildModule(topicId, "cloud-fundamentals", "Cloud Fundamentals",
            "The service-model spectrum, compute options, and the trade-offs behind each.",
            60, lesson);
    }

    private static Module BuildInterviewPrepModule(int topicId)
    {
        var lesson = BuildLesson(
            slug: "star-method-behavioral-interviews",
            title: "The STAR Method & Behavioral Interviews",
            summary: "Structuring behavioral answers, the pseudocode-first habit for coding rounds, and the full interview loop.",
            estimatedMinutes: 25,
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    Behavioral interviews assess how you've actually behaved in past situations, on the theory that past behavior predicts future behavior better than a hypothetical answer. The **STAR** structure keeps your answers concrete and complete:

                    - **Situation** — brief context: where, when, what was the setup.
                    - **Task** — what were you specifically responsible for.
                    - **Action** — what *you* did, specifically (not "we" — the interviewer is evaluating your individual contribution).
                    - **Result** — the outcome, ideally with a measurable impact, and what you learned.

                    A good STAR answer is 60-90 seconds — long enough to be concrete, short enough that the interviewer can follow up. Most candidates over-invest in **Situation** and under-invest in **Action** and **Result**, which are the parts that actually differentiate you.
                    """, 1),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Common behavioral question categories** (prepare 1-2 STAR stories per category, reused across questions)

                    - Conflict with a teammate or disagreement with a decision
                    - A time you failed, or missed a deadline
                    - Leading without formal authority / influencing a team
                    - Handling ambiguous or changing requirements
                    - A time you received tough feedback
                    - Prioritizing under time pressure with competing deadlines
                    """, 2),
                Block(BlockType.CodeSnippet, "Pseudocode-First Template for Coding Rounds", BodyFormat.PlainText, """
                    function solve(input):
                        // 1. Restate the problem and constraints out loud
                        // 2. State the brute-force approach and its complexity
                        // 3. Identify the bottleneck, propose the optimization
                        // 4. Write the optimized approach as pseudocode BEFORE real code
                        // 5. Only then translate pseudocode -> actual syntax
                        // 6. Trace through one example by hand
                        // 7. State final time/space complexity
                    """, 3),
                Block(BlockType.Diagram, "The Typical Interview Loop", BodyFormat.StructuredSteps, """
                    [{"label":"Recruiter Screen"},{"label":"Technical Phone Screen"},{"label":"Onsite: Coding"},{"label":"Onsite: System Design"},{"label":"Onsite: Behavioral"},{"label":"Hiring Committee"},{"label":"Offer"}]
                    """, 4),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Prepare 5-6 flexible STAR stories that each cover multiple categories (a single well-chosen story about a project that went sideways can answer "tell me about a failure," "tell me about conflict," and "tell me about a tight deadline" depending on which angle you emphasize) rather than memorizing a separate story for every possible question.
                    """, 5),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If a behavioral question is ambiguous ("tell me about a time you showed leadership"), ask a brief clarifying question before answering ("do you mean leading a team, or influencing without authority?") — it shows judgment, and ensures you tell the story that actually answers what they're asking.
                    """, 6),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Rambling without STAR structure — burying the actual action and result under five minutes of situational context. Equally damaging: bad-mouthing a former manager, team, or employer when describing a conflict or failure — it reads as a lack of professionalism regardless of who was actually at fault.
                    """, 7),
            ]);

        return BuildModule(topicId, "interview-readiness-fundamentals", "Interview Readiness Fundamentals",
            "Structuring behavioral answers and understanding the shape of the full interview loop.",
            45, lesson);
    }

    private static Module BuildModule(
        int topicId, string slug, string title, string description, int estimatedMinutes,
        Lesson lesson, CapstoneProject? capstone = null)
    {
        var now = DateTime.UtcNow;
        return new Module
        {
            TopicId = topicId,
            Slug = slug,
            Title = title,
            Description = description,
            SortOrder = 1,
            EstimatedMinutes = estimatedMinutes,
            CreatedUtc = now,
            UpdatedUtc = now,
            Lessons = [lesson],
            Capstone = capstone,
        };
    }

    private static Lesson BuildLesson(string slug, string title, string summary, int estimatedMinutes, List<LessonContentBlock> blocks)
    {
        var now = DateTime.UtcNow;
        return new Lesson
        {
            Slug = slug,
            Title = title,
            Summary = summary,
            SortOrder = 1,
            EstimatedMinutes = estimatedMinutes,
            CreatedUtc = now,
            UpdatedUtc = now,
            ContentBlocks = blocks,
        };
    }

    private static LessonContentBlock Block(
        BlockType type, string? title, BodyFormat format, string body, int sortOrder, string? language = null)
    {
        var now = DateTime.UtcNow;
        return new LessonContentBlock
        {
            BlockType = type,
            Title = title,
            BodyFormat = format,
            Body = body.Trim(),
            Language = language,
            SortOrder = sortOrder,
            CreatedUtc = now,
            UpdatedUtc = now,
        };
    }
}
