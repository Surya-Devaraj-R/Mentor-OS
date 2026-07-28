using MentorOS.Models;
using MentorOS.Models.Enums;

namespace MentorOS.Data.Seed;

// Every topic gets 2 fully-realized lessons (up from 1) — the "broad, real
// depth" slice: objectives, real-world analogy, quiz, completion checklist,
// and reference links on every lesson, not just the content-block prose.
// More lessons get added later using this exact same pattern (and, per the
// design notes, eventually via external content files instead of C# object
// literals once volume genuinely demands it).
public static class CurriculumContentSeedData
{
    private record QuizOptionSeed(string Text, bool IsCorrect);
    private record QuizQuestionSeed(string Question, string Explanation, List<QuizOptionSeed> Options);
    private record ReferenceLinkSeed(string Title, string Url, LinkType Type);

    public static (List<Module> Modules, List<ChecklistSeed> Checklists) BuildModules(IReadOnlyDictionary<string, int> topicIdBySlug)
    {
        var results = new List<(Module Module, List<ChecklistSeed> Checklists)>
        {
            BuildCSharpModule(topicIdBySlug["csharp"]),
            BuildDotNetModule(topicIdBySlug["dotnet"]),
            BuildDsaModule(topicIdBySlug["dsa"]),
            BuildSystemDesignModule(topicIdBySlug["system-design"]),
            BuildSqlModule(topicIdBySlug["sql"]),
            BuildCloudModule(topicIdBySlug["cloud"]),
            BuildGitModule(topicIdBySlug["git"]),
            BuildDevOpsModule(topicIdBySlug["devops"]),
            BuildArchitectureModule(topicIdBySlug["architecture"]),
            BuildSoftSkillsModule(topicIdBySlug["soft-skills"]),
        };

        return (
            results.Select(r => r.Module).ToList(),
            results.SelectMany(r => r.Checklists).ToList());
    }

    // ============================== C# ==============================

    private static (Module, List<ChecklistSeed>) BuildCSharpModule(int topicId)
    {
        var lesson1 = BuildLesson(
            slug: "variables-types-control-flow",
            title: "Variables, Types & Control Flow",
            summary: "Value vs. reference types, type inference, and modern pattern-matching control flow.",
            estimatedMinutes: 30,
            objectives:
            [
                "Explain the difference between value types and reference types, and when each is copied",
                "Use `var` without confusing it with dynamic typing",
                "Replace `if`/`else` chains with pattern-matching switch expressions where it improves clarity",
                "Recognize common floating-point and boxing pitfalls before they cause bugs",
            ],
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
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    A **value type** is like handing someone a photocopy of a document — they can scribble all over their copy, and your original stays untouched.

                    A **reference type** is like sharing a link to a Google Doc — anyone with the link is editing the *same* document, so their changes show up for everyone else holding that link too.
                    """, 2),
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
                    """, 3),
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
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "From Source Code to Native Code", BodyFormat.StructuredSteps, """
                    [{"label":"C# Source (.cs)"},{"label":"Roslyn Compiler","note":"syntax + semantic analysis"},{"label":"IL (.dll)","note":"platform-independent bytecode"},{"label":"CLR JIT","note":"at runtime"},{"label":"Native Machine Code"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Prefer `is` pattern matching over explicit casts when you need to branch on type:

                    - Avoid: `if (obj is Circle) { var c = (Circle)obj; ... }`
                    - Prefer: `if (obj is Circle c) { ... }`

                    Use `var` when the type is obvious from the right-hand side (`var list = new List<int>();`), and an explicit type when it improves readability (`int total = ComputeTotal();` where the method name doesn't make the return type obvious).
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    Interviewers often probe value vs. reference semantics with a small "what does this print?" question. Be ready to explain, out loud, why passing a `struct` to a method doesn't affect the caller's copy, while passing a `class` instance does — and why `string`, despite being a reference type, *behaves* like a value type because it's immutable.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Comparing floating-point numbers with `==` (`0.1 + 0.2 == 0.3` is `false` due to binary floating-point rounding). Use a tolerance comparison (`Math.Abs(a - b) < epsilon`) or `decimal` for exact base-10 arithmetic like money.

                    Another common one: boxing a value type into `object` inside a hot loop (e.g., adding `int`s to a non-generic `ArrayList`), which allocates on the heap for every value and silently kills performance.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "By default (no `ref`/`out`), what happens when you pass a struct to a method and modify a field inside it?",
                    "Structs are value types, so the method receives a copy. Modifying the copy's fields never affects the caller's original struct unless you explicitly pass it by reference.",
                    [
                        new QuizOptionSeed("The original struct outside the method is also modified", false),
                        new QuizOptionSeed("Only the copy inside the method is modified; the caller's struct is unaffected", true),
                        new QuizOptionSeed("A compiler error occurs, since structs are immutable", false),
                        new QuizOptionSeed("It's undefined behavior", false),
                    ]),
                new QuizQuestionSeed(
                    "Which statement correctly describes `var` in C#?",
                    "`var` is resolved to a concrete type by the compiler at compile time, and that type never changes — it's syntactic sugar for static typing, not a substitute for `dynamic`.",
                    [
                        new QuizOptionSeed("It creates a dynamically-typed variable whose type can change at runtime", false),
                        new QuizOptionSeed("Its type is inferred once at compile time and never changes afterward", true),
                        new QuizOptionSeed("It can only be used for primitive types like int and bool", false),
                        new QuizOptionSeed("It is interchangeable with the `dynamic` keyword", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Value Types (C# reference)", "https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-types", LinkType.OfficialDocs),
                new ReferenceLinkSeed("Pattern matching overview", "https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/pattern-matching", LinkType.OfficialDocs),
            ]);

        var lesson1Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson1.Slug,
        [
            "Explain the difference between a value type and a reference type out loud, using your own example",
            "Rewrite one `if`/`else` chain from your own code as a switch expression",
            "Find one place in your code where boxing might be silently happening",
        ]);

        var lesson2 = BuildLesson(
            slug: "oop-interfaces-solid-basics",
            title: "OOP Fundamentals: Classes, Interfaces & SOLID Basics",
            summary: "Interfaces vs. abstract classes, and the SOLID principles that keep object-oriented code maintainable.",
            estimatedMinutes: 40,
            objectives:
            [
                "Decide when to use an interface vs. an abstract class",
                "State each SOLID principle in one sentence, with an example",
                "Spot a Single Responsibility Principle violation in existing code",
                "Depend on an interface instead of a concrete class in a constructor",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    An **interface** is a pure contract — a set of members with no implementation and no state — that a class *implements*. A class can implement any number of interfaces.

                    An **abstract class** can hold both state and partial implementation, and a class can inherit from only one (whether abstract or concrete) — C# has single class inheritance.

                    **SOLID** is five principles for keeping object-oriented code changeable without a rewrite:

                    - **S** — Single Responsibility: a class should have one reason to change.
                    - **O** — Open/Closed: open for extension, closed for modification (add new behavior via new code, not by editing working code).
                    - **L** — Liskov Substitution: a subtype must be usable anywhere its base type is expected, without breaking correctness.
                    - **I** — Interface Segregation: many small, focused interfaces beat one large interface nobody fully implements.
                    - **D** — Dependency Inversion: depend on abstractions (interfaces), not concrete implementations.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    An **interface** is like a standard wall socket — any appliance built to that plug shape works, regardless of who manufactured it or what's inside.

                    An **abstract class** is more like a car chassis platform — several models share the same underlying frame and some parts, but each one finishes the body, engine, and interior differently.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **SOLID at a glance**

                    - Single Responsibility — one reason to change
                    - Open/Closed — extend without modifying
                    - Liskov Substitution — subtypes must honor the base type's contract
                    - Interface Segregation — small, focused interfaces
                    - Dependency Inversion — depend on abstractions, not concretions

                    **Interface vs. abstract class**

                    - Multiple interfaces per class; only one base class
                    - Interface = no state, pure contract; abstract class = can hold fields and partial implementation
                    """, 3),
                Block(BlockType.CodeSnippet, "Depending on an Interface, Not a Concrete Class", BodyFormat.PlainText, """
                    public interface IShapeAreaCalculator
                    {
                        double CalculateArea();
                    }

                    public record Circle(double Radius) : IShapeAreaCalculator
                    {
                        public double CalculateArea() => Math.PI * Radius * Radius;
                    }

                    public record Rectangle(double Width, double Height) : IShapeAreaCalculator
                    {
                        public double CalculateArea() => Width * Height;
                    }

                    // Depends on the interface, not on Circle/Rectangle directly —
                    // new shapes can be added later without changing this method.
                    public double TotalArea(IEnumerable<IShapeAreaCalculator> shapes) =>
                        shapes.Sum(s => s.CalculateArea());
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "One Interface, Many Implementations", BodyFormat.AsciiArt, """
                    IShapeAreaCalculator
                            |
                       -----+-----
                       |         |
                     Circle   Rectangle

                    TotalArea(shapes) only knows about IShapeAreaCalculator —
                    it never needs to change when a new shape is added.
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Take dependencies as interfaces in constructors, not concrete classes — this satisfies Dependency Inversion and makes the class testable (a test can pass in a fake/stub implementation instead of the real one).

                    Don't create an interface for a class that only ever has one implementation "just in case" — that's premature abstraction. YAGNI applies to interfaces too; add the interface when a second implementation, or a test double, actually shows up.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    When asked about SOLID, don't just recite the acronym — describe a real violation you found and fixed (or would fix) in actual code. "I had a class that both validated orders and sent confirmation emails; I split it because a change to email formatting shouldn't require re-testing validation logic" demonstrates understanding; the acronym alone doesn't.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Creating an interface for every single class by default, "for testability" or "for flexibility," even when there's exactly one implementation and no plan for a second — this adds a layer of indirection with no real benefit and makes the codebase harder to navigate.

                    Also common: a "God class" that keeps absorbing new responsibilities because it's already the class that touches everything nearby — a slow-motion Single Responsibility Principle violation.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "A class both processes payments and sends confirmation emails. Which SOLID principle does this violate?",
                    "It has two unrelated reasons to change (payment logic changing, or email formatting changing) — the textbook definition of a Single Responsibility Principle violation.",
                    [
                        new QuizOptionSeed("Single Responsibility Principle", true),
                        new QuizOptionSeed("Open/Closed Principle", false),
                        new QuizOptionSeed("Liskov Substitution Principle", false),
                        new QuizOptionSeed("Interface Segregation Principle", false),
                    ]),
                new QuizQuestionSeed(
                    "What's the main structural difference between an interface and an abstract class in C#?",
                    "C# only allows single class inheritance, so a class can extend one abstract class — but it can implement as many interfaces as it needs, since interfaces don't participate in that single-inheritance chain.",
                    [
                        new QuizOptionSeed("Interfaces can hold fields with state; abstract classes cannot", false),
                        new QuizOptionSeed("A class can implement multiple interfaces but inherit from only one class", true),
                        new QuizOptionSeed("Abstract classes cannot contain any method implementations", false),
                        new QuizOptionSeed("Interfaces are only usable in unit tests", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Interfaces (C# Programming Guide)", "https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/interfaces/", LinkType.OfficialDocs),
                new ReferenceLinkSeed("SOLID principles, explained", "https://www.freecodecamp.org/news/solid-principles-explained-in-plain-english/", LinkType.FurtherReading),
            ],
            prerequisites: [lesson1]);

        var lesson2Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson2.Slug,
        [
            "Find a class in your own code that violates Single Responsibility, and describe how you'd split it",
            "Change one concrete-class constructor dependency into an interface dependency",
            "Explain Liskov Substitution using an example from your own code (or why you haven't hit it yet)",
        ]);

        var module = BuildModule(topicId, "csharp-fundamentals", "C# Fundamentals",
            "Language fundamentals every C# developer needs before going deeper into LINQ and async.",
            100, [lesson1, lesson2]);

        return (module, [lesson1Checklist, lesson2Checklist]);
    }

    // ============================== .NET ==============================

    private static (Module, List<ChecklistSeed>) BuildDotNetModule(int topicId)
    {
        var lesson1 = BuildLesson(
            slug: "building-first-minimal-api",
            title: "Building Your First Minimal API",
            summary: "The middleware pipeline, dependency injection lifetimes, and writing thin endpoint handlers.",
            estimatedMinutes: 40,
            objectives:
            [
                "Explain what the ASP.NET Core middleware pipeline does and why order matters",
                "Choose the correct DI lifetime (Transient/Scoped/Singleton) for a given service",
                "Write a thin minimal-API endpoint that delegates to a service instead of doing everything inline",
                "Recognize a captive-dependency bug before it ships",
            ],
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
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Middleware is like an airport security line — each checkpoint (ticket check, bag scan, ID check) runs in a fixed order and can turn you away before you ever reach the gate. Change the order (ID check after boarding) and the whole system stops making sense.

                    DI lifetimes are like coffee orders: **Transient** is a fresh cup made per request; **Scoped** is a shared pot for everyone at one table (one request); **Singleton** is the one coffee machine the whole café uses all day.
                    """, 2),
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
                    """, 3),
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
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "The Request Pipeline", BodyFormat.StructuredSteps, """
                    [{"label":"Incoming Request"},{"label":"Exception Handling MW"},{"label":"HTTPS Redirection"},{"label":"Routing"},{"label":"Auth (if configured)"},{"label":"Endpoint Handler"},{"label":"Response"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Register `DbContext` as **Scoped** (the default for `AddDbContext`) and never inject it into a Singleton service directly — that creates a **captive dependency**: the short-lived, per-request `DbContext` gets held alive for the whole application lifetime, and EF Core is not thread-safe, so concurrent requests will corrupt its internal state.

                    Keep endpoint handlers thin — validate input, call a service/repository, map to a DTO, return a result. Business logic belongs in a service class you can unit test without spinning up the whole HTTP pipeline.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    Be ready to explain the three DI lifetimes with a concrete failure scenario, not just the definitions — e.g., "if you inject a Scoped `DbContext` into a Singleton, the Singleton captures the first request's `DbContext` forever, and every later request reuses that same disposed instance." That story demonstrates you understand *why* the lifetimes exist, not just their names.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Forgetting to `await` an async call inside an endpoint handler — the request completes before the operation finishes, and exceptions from the un-awaited task get swallowed instead of surfacing as a 500 response.

                    Also common: returning the raw EF Core entity from an endpoint instead of a DTO, which can leak internal fields, cause circular-reference JSON serialization errors on navigation properties, and tightly couples your API contract to your database schema.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "You inject a Scoped DbContext into a Singleton service's constructor. What actually happens?",
                    "The Singleton is built once, so it captures whatever Scoped instance existed at that moment forever — a 'captive dependency.' Every later request reuses that same, eventually-disposed DbContext instead of getting its own.",
                    [
                        new QuizOptionSeed("The container throws at startup, refusing to build the Singleton", false),
                        new QuizOptionSeed("The Singleton captures one DbContext instance and reuses it for every future request", true),
                        new QuizOptionSeed("A new DbContext is silently created for every method call on the Singleton", false),
                        new QuizOptionSeed("Nothing unusual — Scoped and Singleton are interchangeable in ASP.NET Core", false),
                    ]),
                new QuizQuestionSeed(
                    "Which DI lifetime should DbContext normally use, and why?",
                    "Scoped ties the DbContext's lifetime to one HTTP request — long enough to track one unit of work, short enough to avoid the thread-safety and memory problems of a Singleton DbContext.",
                    [
                        new QuizOptionSeed("Transient, so every query gets a brand-new DbContext", false),
                        new QuizOptionSeed("Scoped, so one DbContext instance is reused for the whole request", true),
                        new QuizOptionSeed("Singleton, so the whole app shares one DbContext for efficiency", false),
                        new QuizOptionSeed("It doesn't matter, EF Core handles thread-safety internally", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Minimal APIs overview", "https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis", LinkType.OfficialDocs),
                new ReferenceLinkSeed("Dependency injection in ASP.NET Core", "https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection", LinkType.OfficialDocs),
            ]);

        var lesson1Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson1.Slug,
        [
            "Explain the three DI lifetimes out loud with a concrete failure scenario for getting one wrong",
            "Find one endpoint in your own code that isn't awaiting an async call correctly",
            "Rewrite one 'fat' endpoint handler to delegate its logic to a service class",
        ]);

        var lesson2 = BuildLesson(
            slug: "ef-core-basics",
            title: "Entity Framework Core Basics: DbContext, Migrations & Querying",
            summary: "Mapping classes to tables, generating migrations, and writing queries that don't silently load your whole database.",
            estimatedMinutes: 45,
            objectives:
            [
                "Explain what DbContext and a migration actually do",
                "Write a LINQ query using Where/Include without accidentally loading unrelated data",
                "Explain the difference between IQueryable and IEnumerable in a query chain",
                "Recognize the N+1 query problem and one way to avoid it",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    `DbContext` is the unit-of-work + repository combo at the center of EF Core: it tracks entity changes in memory and translates LINQ queries into SQL when you actually enumerate them.

                    A **migration** is a generated C# file describing one schema change (add a table, add a column) as a diff against the previous migration. Running `dotnet ef database update` applies any migrations the database hasn't seen yet.

                    LINQ queries against a `DbSet<T>` are `IQueryable<T>` — they build up an *expression tree*, not results, until something forces execution (`ToListAsync()`, `FirstOrDefaultAsync()`, `foreach`). This is why chaining `.Where()` calls before calling `.ToListAsync()` runs as one SQL query with all the filters combined, not one query per `.Where()`.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    `IQueryable` is like building a restaurant order on a paper ticket — you can keep adding items (`.Where()`, `.OrderBy()`) and the kitchen does nothing until the ticket is actually handed in (`.ToListAsync()`). `IEnumerable` is more like already-plated food sitting in front of you — filtering it further means picking through what's already been cooked, in memory, one item at a time.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Common DbContext/LINQ patterns**

                    - `db.Set<T>().Where(x => ...)` — translated to SQL `WHERE`, runs in the database
                    - `.Include(x => x.Related)` — eager-loads a navigation property in the same query
                    - `.AsNoTracking()` — skip change tracking for read-only queries (faster, less memory)
                    - `.FirstOrDefaultAsync()` / `.SingleOrDefaultAsync()` — get zero-or-one row without loading everything
                    - `dotnet ef migrations add Name` / `dotnet ef database update` — generate and apply schema changes
                    """, 3),
                Block(BlockType.CodeSnippet, "Avoiding the N+1 Query Problem", BodyFormat.PlainText, """
                    // N+1 problem: one query for orders, then one MORE query
                    // per order to fetch its customer — N+1 total round trips.
                    var orders = await db.Orders.ToListAsync();
                    foreach (var order in orders)
                    {
                        var customerName = order.Customer.Name; // lazy-loads per row!
                    }

                    // Fixed: eager-load Customer in the SAME query via Include.
                    var ordersWithCustomers = await db.Orders
                        .Include(o => o.Customer)
                        .ToListAsync();
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "From LINQ to Rows", BodyFormat.StructuredSteps, """
                    [{"label":"LINQ Query","note":"IQueryable, not yet executed"},{"label":"EF Core Translates","note":"expression tree -> SQL"},{"label":"Database Executes"},{"label":"Rows Materialized","note":"into C# objects, on ToListAsync()"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Use `.AsNoTracking()` for read-only queries (like most API `GET` endpoints) — it skips EF Core's change-tracking bookkeeping, which is pure overhead when you're never going to call `SaveChangesAsync()` on the results.

                    Push filtering into the database with `.Where()` before materializing results, rather than calling `.ToListAsync()` early and filtering the in-memory list afterward — the latter pulls far more data over the wire than necessary.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked to spot a performance bug in an EF Core code sample, scan for two things first: a `.ToList()` (or equivalent) called *before* filtering is finished, and a navigation property accessed inside a loop without a matching `.Include()` — these two account for the large majority of real-world EF Core performance issues.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Calling `.ToList()` too early in a query chain (e.g., `db.Orders.ToList().Where(...)`) — this pulls every row into memory first, then filters in C#, defeating the entire purpose of a queryable database. The `.Where()` must come *before* the materializing call.

                    Also common: silently triggering the N+1 problem by accessing a lazy-loaded navigation property inside a loop, instead of eager-loading it once with `.Include()`.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "What's wrong with `db.Orders.ToList().Where(o => o.IsActive)`?",
                    "`.ToList()` runs before `.Where()`, so every row in the Orders table is pulled into memory first, and the filter happens afterward in C# — the database never gets to do the filtering itself.",
                    [
                        new QuizOptionSeed("Nothing — the result is identical to filtering before ToList()", false),
                        new QuizOptionSeed("It loads every row from the database before filtering, instead of filtering in SQL", true),
                        new QuizOptionSeed("It throws a compile-time error", false),
                        new QuizOptionSeed("It only returns inactive orders", false),
                    ]),
                new QuizQuestionSeed(
                    "What causes the 'N+1 query problem'?",
                    "Loading a list of N parent rows, then triggering a separate query per row (often via an un-eager-loaded navigation property) to fetch related data — N+1 total round trips instead of one join.",
                    [
                        new QuizOptionSeed("Using .Include() to eager-load related data", false),
                        new QuizOptionSeed("Accessing a related entity inside a loop without eager-loading it first", true),
                        new QuizOptionSeed("Using .AsNoTracking() on a read-only query", false),
                        new QuizOptionSeed("Running a migration twice", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("EF Core: Loading Related Data", "https://learn.microsoft.com/en-us/ef/core/querying/related-data/", LinkType.OfficialDocs),
                new ReferenceLinkSeed("EF Core Migrations overview", "https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/", LinkType.OfficialDocs),
            ],
            prerequisites: [lesson1]);

        var lesson2Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson2.Slug,
        [
            "Find a navigation-property access inside a loop in your own code and fix it with .Include()",
            "Add .AsNoTracking() to one read-only query and explain why it's safe there",
            "Generate and read through one migration file end to end",
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
        };

        var capstoneChecklist = new ChecklistSeed(ChecklistOwnerKind.Capstone, "aspnet-core-basics",
        [
            "Scaffold a minimal API project and add EF Core + SQLite.",
            "Model a TaskItem entity and generate the initial migration.",
            "Implement all 5 CRUD endpoints using DTOs, not raw entities.",
            "Add validation that rejects an empty task title with a 400.",
            "Manually verify every endpoint with curl, including the not-found and validation-failure cases.",
        ]);

        var module = BuildModule(topicId, "aspnet-core-basics", "ASP.NET Core Basics",
            "The middleware pipeline, dependency injection, EF Core, and building your first Minimal API.",
            85, [lesson1, lesson2], capstone);

        return (module, [lesson1Checklist, lesson2Checklist, capstoneChecklist]);
    }

    // ============================== DSA ==============================

    private static (Module, List<ChecklistSeed>) BuildDsaModule(int topicId)
    {
        var lesson1 = BuildLesson(
            slug: "two-pointer-hash-map-patterns",
            title: "Two-Pointer & Hash Map Patterns",
            summary: "The two patterns that solve most array/string interview problems, and when to reach for which.",
            estimatedMinutes: 45,
            objectives:
            [
                "Recognize when a problem calls for a hash map vs. two pointers",
                "State the time/space trade-off between the two approaches for the same problem",
                "Implement Two Sum using a hash map in one pass",
                "Narrate the brute-force-to-optimal path out loud, the way an interviewer expects",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    A huge fraction of array/string interview problems reduce to one of two patterns:

                    **Hash map (or hash set) for O(1) lookups** — trade `O(n)` extra space for turning an `O(n)` linear search into an `O(1)` lookup, collapsing an `O(n²)` brute force into `O(n)`. Reach for this whenever the question is some version of "have I seen this value/complement before?"

                    **Two pointers on a sorted array** — when the array is sorted (or can cheaply be sorted), two pointers starting at opposite ends can converge toward a target in a single `O(n)` pass, because moving the left pointer only increases the sum and moving the right pointer only decreases it — you never need to re-check a pair twice.

                    The two patterns are often interchangeable for the *same* problem (e.g., Two Sum): hash map works on unsorted input in `O(n)` time / `O(n)` space; two pointers needs sorted input but uses `O(1)` extra space.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    A hash map is like a coat-check counter — hand over an item, get an instant lookup ticket; asking "do you have my complement item?" is one counter-check, not a walk down every rack.

                    Two pointers is like two people searching a sorted bookshelf from opposite ends, walking toward each other — because the shelf is sorted, each step in either direction is guaranteed to move you closer to the answer, never further away.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Complexity of common operations**

                    - Hash map/set: average `O(1)` insert, lookup, delete; `O(n)` worst case (hash collisions)
                    - Sorting an array: `O(n log n)` time, enables two-pointer techniques afterward
                    - Two pointers over a sorted array: `O(n)` time, `O(1)` extra space

                    **When to reach for which**

                    - Need original index order preserved, or the array isn't sorted and sorting would lose information you need (like original indices) → hash map
                    - Need `O(1)` space, and the array is already sorted or sorting it doesn't lose information you need → two pointers
                    """, 3),
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
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "Two Pointers Converging on a Sorted Array", BodyFormat.AsciiArt, """
                    Sorted array: [ 2   7   11   15 ]
                                    ^               ^
                                  left            right

                    left + right sum > target  ->  right--
                    left + right sum < target  ->  left++
                    left + right sum == target ->  found the pair
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    In an interview, state the brute-force `O(n²)` solution out loud first — it proves you can solve the problem at all — then explicitly name the trade-off you're making to optimize it ("I can trade `O(n)` space for a hash map to get this down to `O(n)` time"). Interviewers weight *how* you get to the optimal solution as much as the final code.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    When you reach for a hash map, say the complexity trade-off out loud unprompted: "I'll use a hash map here — `O(n)` extra space — to bring this down from `O(n²)` to `O(n)` time." Naming the trade-off before being asked signals you understand it rather than having memorized the pattern.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Writing the nested-loop `O(n²)` brute force and stopping there without recognizing a hash map or two-pointer approach applies — most interviewers expect you to at least identify the optimization opportunity, even under time pressure.

                    Also common: using a hash map when the problem actually needs two pointers on sorted data (e.g., finding a pair in a sorted array without preserving original indices) — it works, but burns unnecessary `O(n)` space and signals you're pattern-matching without understanding *why* two pointers apply here.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "You need to find two numbers in an UNSORTED array that sum to a target, and must return their original indices. What's the best fit?",
                    "Sorting would destroy the original index order you need to return, so a hash map (value -> original index) is the right tool — O(n) time, O(n) space, no sorting required.",
                    [
                        new QuizOptionSeed("Two pointers after sorting the array", false),
                        new QuizOptionSeed("A hash map from value to original index", true),
                        new QuizOptionSeed("Binary search for each element", false),
                        new QuizOptionSeed("A nested loop is the only option", false),
                    ]),
                new QuizQuestionSeed(
                    "Why can two pointers converge in O(n) instead of checking every pair (O(n²))?",
                    "Because the array is sorted, moving the left pointer right only increases the sum and moving the right pointer left only decreases it — each comparison eliminates a whole range of pairs at once, instead of re-checking them individually.",
                    [
                        new QuizOptionSeed("Because it secretly uses a hash map internally", false),
                        new QuizOptionSeed("Because each pointer move provably rules out a whole set of pairs at once, thanks to sorted order", true),
                        new QuizOptionSeed("It doesn't — two pointers is also O(n²) in the worst case", false),
                        new QuizOptionSeed("Because the array is always small enough not to matter", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("NeetCode 150 roadmap", "https://neetcode.io/roadmap", LinkType.FurtherReading),
                new ReferenceLinkSeed("Big O cheat sheet", "https://www.bigocheatsheet.com/", LinkType.FurtherReading),
            ]);

        var lesson1Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson1.Slug,
        [
            "Solve Two Sum from scratch without looking at the solution",
            "State out loud why hash map beats brute force here, including the space trade-off",
            "Identify one past problem you solved with a nested loop that a hash map would have simplified",
        ]);

        var lesson2 = BuildLesson(
            slug: "binary-search-patterns",
            title: "Binary Search Beyond Sorted Arrays",
            summary: "The binary-search template that also solves 'search on the answer' problems, not just finding a value in a sorted array.",
            estimatedMinutes: 40,
            objectives:
            [
                "Write a correct binary search without off-by-one bugs",
                "Recognize when a problem is 'binary search on the answer' rather than a literal search",
                "Explain why binary search requires a monotonic (sorted, or sorted-like) condition",
                "Trace the invariant that keeps a binary search loop correct",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    Binary search isn't just "find a value in a sorted array" — it's a general technique for any problem with a **monotonic condition**: as you move along a range, the answer to "is this good enough?" flips from `false` to `true` (or vice versa) exactly once.

                    Classic form: search a sorted array for a target, narrowing `[left, right]` by comparing the middle element and discarding half the remaining range each time.

                    **Binary search on the answer**: instead of searching an array, search a *range of possible answers* (e.g., "what's the minimum speed to eat all bananas in H hours?") — at each candidate answer, check a monotonic condition ("is this speed fast enough?") and binary search over the candidate values themselves.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Binary search is like guessing a number between 1 and 100 where you're only told "higher" or "lower" — you always guess the midpoint of what's left, and every guess eliminates half the remaining possibilities, so you converge in about 7 guesses instead of 100.

                    "Binary search on the answer" is the same game, except instead of guessing a specific number, you're guessing a *threshold* — like guessing the minimum speed needed to finish a race in time, where "too slow" and "fast enough" flip exactly once as speed increases.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Binary search invariant**

                    - Loop while `left <= right`
                    - `mid = left + (right - left) / 2` — avoids integer overflow vs. `(left + right) / 2`
                    - If `mid` is the answer, return it
                    - Otherwise narrow to the half that could still contain the answer

                    **Recognizing "search on the answer"**

                    - The question asks for a minimum/maximum value satisfying some condition
                    - The condition is monotonic: true for all values above (or below) some threshold, false otherwise
                    """, 3),
                Block(BlockType.CodeSnippet, "Binary Search Template", BodyFormat.PlainText, """
                    public int BinarySearch(int[] sortedNums, int target)
                    {
                        var left = 0;
                        var right = sortedNums.Length - 1;

                        while (left <= right)
                        {
                            var mid = left + (right - left) / 2; // avoids overflow

                            if (sortedNums[mid] == target) return mid;
                            if (sortedNums[mid] < target) left = mid + 1;
                            else right = mid - 1;
                        }

                        return -1; // not found
                    }
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "Narrowing the Search Range", BodyFormat.AsciiArt, """
                    [ 1  3  5  7  9  11  13 ]   target = 9
                      L              R
                            M (7 < 9, discard left half)

                    [          7  9  11  13 ]
                                L    R
                                  M (11 > 9, discard right half)

                    [          9 ]
                    found at index of 9
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Always compute `mid` as `left + (right - left) / 2`, not `(left + right) / 2` — the second form can integer-overflow on very large arrays in languages with fixed-width integers, a classic bug that's easy to avoid by habit.

                    State the monotonic condition explicitly before writing any code for a "search on the answer" problem — if you can't articulate what flips from false to true, binary search doesn't apply yet.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    When a problem asks for a minimum or maximum value satisfying some condition and the brute force is "try every possible value," ask yourself out loud: "is this condition monotonic?" If yes, say so explicitly — recognizing "binary search on the answer" out loud is one of the highest-signal moments in a DSA interview.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Off-by-one errors: using `<` instead of `<=` in the loop condition, or forgetting `mid + 1`/`mid - 1` when narrowing the range (re-including `mid` itself causes infinite loops on a 2-element range).

                    Also common: trying to binary search over data that isn't actually monotonic — binary search silently returns a wrong answer (not an error) if the underlying condition doesn't flip cleanly exactly once.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "Why compute `mid` as `left + (right - left) / 2` instead of `(left + right) / 2`?",
                    "`left + right` can overflow a fixed-width integer type when both are large, even though the true midpoint is well within range. `left + (right - left) / 2` never adds two large numbers together, avoiding the overflow.",
                    [
                        new QuizOptionSeed("It's purely a style preference with no functional difference", false),
                        new QuizOptionSeed("It avoids integer overflow when left and right are both large", true),
                        new QuizOptionSeed("It makes the search run in O(1) instead of O(log n)", false),
                        new QuizOptionSeed("It's required for the array to be considered sorted", false),
                    ]),
                new QuizQuestionSeed(
                    "What property must a condition have for 'binary search on the answer' to apply?",
                    "The condition must be monotonic — as candidate values increase (or decrease), the condition's truth value flips exactly once. Without that guarantee, narrowing the search range can eliminate the actual answer.",
                    [
                        new QuizOptionSeed("The input array must contain only positive integers", false),
                        new QuizOptionSeed("The condition must be monotonic across the range of candidate answers", true),
                        new QuizOptionSeed("The array must already be sorted in descending order", false),
                        new QuizOptionSeed("There must be exactly one valid answer in the array", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Binary Search (NeetCode)", "https://neetcode.io/roadmap", LinkType.FurtherReading),
                new ReferenceLinkSeed("Binary search overflow bug (Java, but the lesson is language-agnostic)", "https://ai.googleblog.com/2006/06/extra-extra-read-all-about-it-nearly.html", LinkType.FurtherReading),
            ],
            prerequisites: [lesson1]);

        var lesson2Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson2.Slug,
        [
            "Implement binary search from scratch without off-by-one errors, twice, from memory",
            "Find one 'search on the answer' style problem and identify its monotonic condition before coding",
            "Explain why `left + (right - left) / 2` is safer than `(left + right) / 2`",
        ]);

        var module = BuildModule(topicId, "arrays-and-hashing", "Arrays & Hashing",
            "The foundational patterns — hash maps, two pointers, and binary search — that unlock most array and string problems.",
            85, [lesson1, lesson2]);

        return (module, [lesson1Checklist, lesson2Checklist]);
    }

    // ============================== System Design ==============================

    private static (Module, List<ChecklistSeed>) BuildSystemDesignModule(int topicId)
    {
        var lesson1 = BuildLesson(
            slug: "scaling-load-balancing-caching",
            title: "Scaling a Single Server: Load Balancing & Caching",
            summary: "Vertical vs. horizontal scaling, load balancer strategies, and the cache-aside pattern.",
            estimatedMinutes: 45,
            objectives:
            [
                "Explain the difference between vertical and horizontal scaling and when each applies",
                "Describe what a load balancer does and why app servers must be stateless behind one",
                "Implement the cache-aside pattern and explain its trade-offs",
                "State the CAP theorem trade-off in your own words",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    Every system design problem starts from the same place: a single server handling all traffic, which eventually can't keep up. There are two ways to add capacity:

                    - **Vertical scaling** — a bigger machine (more CPU/RAM). Simple, but has a hard ceiling and a single point of failure.
                    - **Horizontal scaling** — more machines behind a **load balancer**. No hard ceiling, and machines can fail without taking the whole system down — but it only works if the application servers are **stateless**, so any server can handle any request.

                    A **load balancer** distributes incoming requests across a pool of servers using a strategy like round-robin, least-connections, or consistent hashing, and continuously health-checks servers so it stops routing to ones that are down.

                    **Caching** reduces load on your database/backend by serving frequently-requested data from fast, in-memory storage. The **cache-aside** pattern is the most common: on a read, check the cache first; on a miss, read from the database and populate the cache; on a write, update the database and invalidate (or update) the cache entry.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Vertical scaling is like upgrading one chef to a faster chef — eventually even the fastest chef alone hits a ceiling. Horizontal scaling is like hiring more chefs and having a host (the load balancer) seat customers at whichever chef is free — as long as any chef can cook any dish (statelessness), the restaurant can keep growing by adding chefs.

                    A cache is like keeping today's most-ordered dishes pre-plated near the kitchen door instead of cooking each one from scratch on every order.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **CAP theorem** — under a network partition, a distributed system must choose:

                    - **Consistency** — every read gets the latest write, or an error
                    - **Availability** — every request gets a (possibly stale) response

                    (Partition tolerance isn't optional in a real distributed system, so this is really a consistency-vs-availability trade-off during a partition.)

                    **Common caching strategies**

                    - Cache-aside — app manages cache population/invalidation (most common, most flexible)
                    - Write-through — every write goes to the cache and the database together (simpler, slower writes)
                    - TTL (time-to-live) eviction — accept some staleness in exchange for simplicity
                    """, 3),
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
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "Client to Database, With a Cache", BodyFormat.StructuredSteps, """
                    [{"label":"Client"},{"label":"Load Balancer","note":"health-checks + distributes"},{"label":"App Servers (N)","note":"stateless, horizontally scaled"},{"label":"Cache","note":"cache-aside"},{"label":"Database"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Keep application servers **stateless** — don't store session data in server memory. Put shared state in the cache or database instead, so the load balancer can route any request to any server, and a server can be added, removed, or restarted without losing user sessions.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    Before drawing a single box, clarify requirements out loud: expected read/write ratio, latency targets, consistency requirements, and rough scale (requests/second, data size). A design built on the wrong assumptions — however elegant — reads as a red flag, not a strength.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Jumping straight to "microservices" or "shard the database" in the first two minutes without establishing that the scale actually requires it. Most systems can go a very long way on a well-indexed single database, a cache, and a couple of stateless app servers behind a load balancer — reaching for distributed-systems complexity too early is a common way candidates lose points, not gain them.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "Why must application servers be stateless behind a load balancer?",
                    "If a server holds session state in memory, a request landing on a different server (which the load balancer will do, by design) loses that state. Statelessness is what makes 'any server can handle any request' actually true.",
                    [
                        new QuizOptionSeed("It's not required — load balancers pin each user to one server", false),
                        new QuizOptionSeed("So any server can handle any request, since the balancer doesn't guarantee routing the same user to the same server", true),
                        new QuizOptionSeed("Stateless servers are always faster than stateful ones", false),
                        new QuizOptionSeed("It's only needed for read-only APIs", false),
                    ]),
                new QuizQuestionSeed(
                    "Under a network partition, what does the CAP theorem say you must choose between?",
                    "You must choose between Consistency (every read reflects the latest write, or errors out) and Availability (every request gets a response, possibly stale) — you cannot guarantee both while the partition persists.",
                    [
                        new QuizOptionSeed("Consistency and Availability", true),
                        new QuizOptionSeed("Speed and Security", false),
                        new QuizOptionSeed("Cost and Scalability", false),
                        new QuizOptionSeed("SQL and NoSQL", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("ByteByteGo: System Design basics", "https://bytebytego.com", LinkType.FurtherReading),
                new ReferenceLinkSeed("CAP theorem, explained simply", "https://www.ibm.com/topics/cap-theorem", LinkType.FurtherReading),
            ]);

        var lesson1Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson1.Slug,
        [
            "Draw the client -> load balancer -> app servers -> cache -> database diagram from memory",
            "Explain the cache-aside pattern's read and write paths out loud",
            "State the CAP theorem trade-off using your own words, not the textbook phrasing",
        ]);

        var lesson2 = BuildLesson(
            slug: "databases-at-scale",
            title: "Databases at Scale: Replication, Sharding & SQL vs. NoSQL",
            summary: "How a single database becomes a fleet — read replicas, sharding strategies, and picking SQL vs. NoSQL.",
            estimatedMinutes: 45,
            objectives:
            [
                "Explain the difference between replication and sharding",
                "Describe a read-replica setup and its main risk (replication lag)",
                "Choose a sharding key and explain what makes a good one",
                "Justify a SQL vs. NoSQL choice based on actual data-access patterns, not trends",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    A single database eventually becomes the bottleneck even after your app servers are horizontally scaled. Two distinct techniques address this:

                    **Replication** — copy the entire database onto additional servers (read replicas). Writes go to one primary; reads can be spread across replicas. This scales *read* throughput, not write throughput, and introduces **replication lag** — replicas are eventually consistent, slightly behind the primary.

                    **Sharding** — split the data itself across multiple databases by some **shard key** (e.g., user ID range, geographic region), so each shard holds only a slice of the total data. This scales both reads and writes, at the cost of much harder cross-shard queries and joins.

                    **SQL vs. NoSQL** is a data-shape and access-pattern decision, not a popularity contest: SQL databases give strong consistency and relational integrity (foreign keys, joins, transactions) at the cost of harder horizontal scaling; NoSQL databases (key-value, document, wide-column) trade some of that structure for easier horizontal scaling and flexible schemas.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Replication is like printing extra copies of today's newspaper for more newsstands — everyone reads the same content, but a copy printed a minute ago might be slightly out of date compared to the presses still running (replication lag).

                    Sharding is like splitting one giant phone book into separate books by area code — each book is smaller and faster to search, but finding someone whose area code you don't know means checking multiple books (a cross-shard query).
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Replication vs. sharding**

                    - Replication: full copies, scales reads, introduces replication lag
                    - Sharding: partitioned data, scales reads *and* writes, complicates cross-shard queries/joins

                    **Choosing a good shard key**

                    - Spreads load evenly (avoid a "hot" shard that gets disproportionate traffic)
                    - Matches your most common query pattern (queries that hit one shard are cheap; cross-shard queries are expensive)

                    **SQL vs. NoSQL, roughly**

                    - Need transactions, joins, strong consistency → SQL
                    - Need flexible/evolving schema, massive horizontal write scale, simple access patterns → NoSQL
                    """, 3),
                Block(BlockType.CodeSnippet, "Routing a Query to the Right Shard", BodyFormat.PlainText, """
                    // A simple hash-based shard router: pick a shard deterministically
                    // from the user id, so the same user always lands on the same shard.
                    public int GetShardIndex(int userId, int shardCount) =>
                        Math.Abs(userId.GetHashCode()) % shardCount;

                    var shardIndex = GetShardIndex(userId, shardCount: 4);
                    var connection = shardConnections[shardIndex];
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "Primary-Replica Replication", BodyFormat.StructuredSteps, """
                    [{"label":"App Writes"},{"label":"Primary DB","note":"source of truth"},{"label":"Replicates async","note":"replication lag"},{"label":"Read Replica 1"},{"label":"Read Replica 2"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Pick a shard key based on your actual query patterns, not convenience — a shard key that scatters a single user's data across many shards turns every "get this user's data" query into an expensive cross-shard fan-out.

                    Route latency-sensitive reads that must see the very latest write (e.g., "did my write just succeed?") to the primary, not a replica — replicas are for read scaling where a small amount of staleness is acceptable.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    Don't default to "let's use NoSQL for scale" as a reflex — justify it with the actual access pattern. If the design needs multi-row transactions or relational joins across entities, say so explicitly and pick SQL (with read replicas and/or sharding) instead; interviewers are listening for reasoned trade-offs, not buzzwords.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Confusing replication with sharding — they solve different problems (read scaling with full copies vs. write/storage scaling with partitioned data) and are often needed *together*, not as alternatives to each other.

                    Also common: picking a shard key that looks convenient (like an auto-incrementing row ID) but creates a "hot shard" — all new writes landing on whichever shard currently holds the highest IDs, instead of spreading load evenly.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "A system needs to scale READ throughput for an already-small dataset that all fits on one machine. What's the more direct fit: replication or sharding?",
                    "Replication directly scales read throughput by adding read replicas of the full dataset — sharding is solving a different problem (partitioning data that no longer fits, or write-scaling), which isn't needed here.",
                    [
                        new QuizOptionSeed("Sharding, since it always scales better", false),
                        new QuizOptionSeed("Replication, since the goal is read scaling, not partitioning data that already fits", true),
                        new QuizOptionSeed("Neither applies to read-heavy systems", false),
                        new QuizOptionSeed("They are the same technique with different names", false),
                    ]),
                new QuizQuestionSeed(
                    "What makes a shard key 'bad' in practice?",
                    "A shard key that concentrates load unevenly (a hot shard) defeats the purpose of sharding — some shards do most of the work while others sit idle, and you haven't actually distributed the load.",
                    [
                        new QuizOptionSeed("It's derived from a user ID", false),
                        new QuizOptionSeed("It causes uneven load distribution, creating a disproportionately busy 'hot' shard", true),
                        new QuizOptionSeed("It's a string instead of an integer", false),
                        new QuizOptionSeed("It changes between deployments", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("ByteByteGo: Database scaling patterns", "https://bytebytego.com", LinkType.FurtherReading),
                new ReferenceLinkSeed("SQL vs NoSQL — a practical comparison", "https://aws.amazon.com/nosql/", LinkType.FurtherReading),
            ],
            prerequisites: [lesson1]);

        var lesson2Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson2.Slug,
        [
            "Explain replication lag and one scenario where it causes a real bug",
            "Design a shard key for a hypothetical multi-tenant app and justify it",
            "Argue both sides of a SQL vs. NoSQL choice for one system you've worked on",
        ]);

        var module = BuildModule(topicId, "system-design-fundamentals", "System Design Fundamentals",
            "How to scale a single server into a resilient, horizontally-scaled system with a real database strategy.",
            90, [lesson1, lesson2]);

        return (module, [lesson1Checklist, lesson2Checklist]);
    }

    // ============================== SQL ==============================

    private static (Module, List<ChecklistSeed>) BuildSqlModule(int topicId)
    {
        var lesson1 = BuildLesson(
            slug: "select-join-query-fundamentals",
            title: "SELECT, JOIN, and Query Fundamentals",
            summary: "Logical query evaluation order, join types, and how NULL actually behaves.",
            estimatedMinutes: 30,
            objectives:
            [
                "State SQL's logical evaluation order and use it to debug a confusing query",
                "Choose the correct JOIN type for a given requirement",
                "Explain why NULL = NULL is not true",
                "Write a query using GROUP BY and HAVING correctly",
            ],
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
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    `NULL` is like an empty field on a form, not a zero written on it. Asking "is this blank field equal to that other blank field?" doesn't really have a yes/no answer — both are simply unanswered, which is exactly why SQL returns `NULL` (unknown), not `true`, when you compare `NULL = NULL`.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Join types at a glance**

                    - `INNER JOIN` — intersection only
                    - `LEFT JOIN` — everything on the left, matched or not
                    - `RIGHT JOIN` — everything on the right, matched or not (rare; usually rewritten as a `LEFT JOIN` with tables swapped)
                    - `FULL OUTER JOIN` — union of left and right, matched where possible
                    - `CROSS JOIN` — every row of A paired with every row of B (Cartesian product)

                    **Aggregate functions**: `COUNT`, `SUM`, `AVG`, `MIN`, `MAX` — always used with `GROUP BY` unless aggregating the entire table.
                    """, 3),
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
                    """, 4, language: "sql"),
                Block(BlockType.Diagram, "Join Types", BodyFormat.AsciiArt, """
                    INNER JOIN              LEFT JOIN                FULL OUTER JOIN

                      A    B                  A    B                    A    B
                     ( ( X ) )              (███( X ))                (███( X )███)

                    only the overlap     all of A, unmatched     everything, matched
                                         B becomes NULL          where possible
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Always index columns used in `JOIN` conditions and `WHERE` clauses on large tables — without an index, the database falls back to a full table scan for every query, which is fine at 100 rows and catastrophic at 100 million.

                    Write `JOIN ... ON` conditions explicitly rather than relying on implicit joins via `WHERE` (`FROM a, b WHERE a.id = b.a_id`) — the explicit form is easier to read and harder to accidentally turn into a Cartesian product.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    Classic take-home/whiteboard SQL questions — "find the second-highest salary," "find duplicate emails," "find customers with no orders" — are really testing whether you understand `NULL` handling, `GROUP BY`/`HAVING`, and self-joins, not whether you've memorized syntax. Talk through your logical evaluation order (`FROM` → `WHERE` → `GROUP BY` → `HAVING` → `SELECT`) as you write the query.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Using `SELECT *` in production code — it breaks silently when columns are added, reordered, or removed upstream, and pulls more data over the wire than the caller needs.

                    Also common: trying to filter an aggregate in `WHERE` instead of `HAVING` (`WHERE COUNT(*) > 3` is invalid — `WHERE` runs before aggregation exists), and forgetting that comparing anything to `NULL` with `=` silently returns no rows instead of an error.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "Why does `WHERE COUNT(*) > 3` fail, while `HAVING COUNT(*) > 3` works?",
                    "WHERE filters individual rows before GROUP BY has collapsed them into groups, so no aggregate value exists yet to filter on. HAVING runs after grouping, when COUNT(*) actually has a value per group.",
                    [
                        new QuizOptionSeed("WHERE runs before GROUP BY, so the aggregate doesn't exist yet", true),
                        new QuizOptionSeed("COUNT(*) can only be used in SELECT, never in a filter", false),
                        new QuizOptionSeed("It's a syntax error unrelated to evaluation order", false),
                        new QuizOptionSeed("WHERE and HAVING are functionally identical in every database", false),
                    ]),
                new QuizQuestionSeed(
                    "What does a LEFT JOIN return for rows in the left table with no match in the right table?",
                    "LEFT JOIN keeps every row from the left table regardless of a match — when there's no match, the right table's columns are simply NULL for that row, rather than dropping the row entirely.",
                    [
                        new QuizOptionSeed("The row is dropped from the result entirely", false),
                        new QuizOptionSeed("The row is kept, with NULL in place of the right table's columns", true),
                        new QuizOptionSeed("The query throws an error", false),
                        new QuizOptionSeed("The right table's columns default to zero or empty string", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("SQL JOIN types visualized", "https://www.w3schools.com/sql/sql_join.asp", LinkType.FurtherReading),
                new ReferenceLinkSeed("Understanding NULL in SQL", "https://www.postgresql.org/docs/current/functions-comparison.html", LinkType.OfficialDocs),
            ]);

        var lesson1Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson1.Slug,
        [
            "Write a query using GROUP BY + HAVING from scratch, without looking anything up",
            "Explain why NULL = NULL isn't true, using your own words",
            "Rewrite one implicit join (comma-separated FROM) as an explicit JOIN ... ON",
        ]);

        var lesson2 = BuildLesson(
            slug: "aggregations-subqueries-window-functions",
            title: "Aggregations, Subqueries & Window Functions",
            summary: "Correlated vs. uncorrelated subqueries, and window functions for running totals and rankings without collapsing rows.",
            estimatedMinutes: 40,
            objectives:
            [
                "Distinguish a correlated subquery from an uncorrelated one",
                "Explain when a window function is the right tool instead of GROUP BY",
                "Use RANK() or ROW_NUMBER() to solve a top-N-per-group problem",
                "Recognize when a subquery could be rewritten as a JOIN for clarity or performance",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    A **subquery** is a query nested inside another. An **uncorrelated** subquery runs once, independent of the outer query (e.g., `WHERE salary > (SELECT AVG(salary) FROM employees)`). A **correlated** subquery references a column from the outer query and conceptually re-runs once per outer row (e.g., checking "does this employee earn more than the average *in their own department*?").

                    **Window functions** compute a value across a set of rows *related to the current row*, without collapsing those rows the way `GROUP BY` does — you keep every row, but gain access to aggregates, rankings, or running totals computed over a window of related rows via `OVER (...)`.

                    This is the key difference from `GROUP BY`: `GROUP BY` reduces N rows to fewer summary rows; a window function keeps all N rows and adds a computed column to each.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    `GROUP BY` is like asking "what's the total sales per store?" — you get back one row per store, the individual transactions are gone.

                    A window function is like asking "for each individual transaction, what's the running total for that store so far?" — you keep every transaction row, but each one now also shows a computed value calculated *relative to its group*.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Common window functions**

                    - `ROW_NUMBER() OVER (PARTITION BY dept ORDER BY salary DESC)` — a unique rank per partition, no ties
                    - `RANK()` — like ROW_NUMBER, but ties share a rank and the next rank skips
                    - `DENSE_RANK()` — like RANK, but the next rank doesn't skip after a tie
                    - `SUM(x) OVER (ORDER BY date)` — a running total
                    - `PARTITION BY` — resets the window per group, similar in spirit to GROUP BY but without collapsing rows
                    """, 3),
                Block(BlockType.CodeSnippet, "Top Earner Per Department (Window Function)", BodyFormat.PlainText, """
                    SELECT department, name, salary
                    FROM (
                        SELECT
                            department,
                            name,
                            salary,
                            RANK() OVER (PARTITION BY department ORDER BY salary DESC) AS salary_rank
                        FROM employees
                    ) ranked
                    WHERE salary_rank = 1;
                    """, 4, language: "sql"),
                Block(BlockType.Diagram, "GROUP BY vs. Window Function", BodyFormat.AsciiArt, """
                    GROUP BY department:              Window function OVER (PARTITION BY department):

                    department | avg_salary          department | name  | salary | dept_avg
                    -----------|------------         -----------|-------|--------|----------
                    Eng        | 95000               Eng        | Alice | 100000 | 95000
                    Sales      | 70000                Eng        | Bob   | 90000  | 95000
                                                       Sales      | Carol | 70000  | 70000

                    (2 rows, collapsed)                (all rows kept, annotated)
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Reach for a window function whenever you need a per-row computed value that depends on a group (rankings, running totals, "difference from this group's average") — trying to do this with a correlated subquery per row is usually far slower and harder to read.

                    Before writing a correlated subquery, check if it can be rewritten as a JOIN or a window function — correlated subqueries conceptually execute once per outer row and are often the slowest option available.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    "Find the top N rows per group" (top earner per department, most recent order per customer) is one of the most common SQL interview patterns — recognize it immediately as a `ROW_NUMBER()`/`RANK()` + `PARTITION BY` problem, wrapped in an outer query filtering on the rank.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Using `RANK()` when you actually need `ROW_NUMBER()` (or vice versa) — `RANK()` leaves gaps after ties (1, 2, 2, 4), which silently produces the wrong count if you assumed consecutive integers.

                    Also common: writing a correlated subquery for something a window function or a simple JOIN would express far more clearly and efficiently — a sign of not yet recognizing the window-function pattern.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "What's the key difference between GROUP BY and a window function?",
                    "GROUP BY collapses multiple rows into one summary row per group. A window function keeps every original row and adds a computed value to each, calculated relative to a window of related rows.",
                    [
                        new QuizOptionSeed("Window functions are just a faster syntax for GROUP BY", false),
                        new QuizOptionSeed("GROUP BY collapses rows into groups; window functions keep every row and annotate it", true),
                        new QuizOptionSeed("GROUP BY can only be used with COUNT()", false),
                        new QuizOptionSeed("There is no meaningful difference", false),
                    ]),
                new QuizQuestionSeed(
                    "You need the single highest-paid employee in each department, keeping ties out. Which function fits best?",
                    "ROW_NUMBER() assigns a strictly unique, sequential rank per partition even when salaries tie, guaranteeing exactly one row per department when filtered to rank = 1 — RANK() would let tied employees share rank 1.",
                    [
                        new QuizOptionSeed("RANK()", false),
                        new QuizOptionSeed("ROW_NUMBER()", true),
                        new QuizOptionSeed("COUNT()", false),
                        new QuizOptionSeed("GROUP BY alone, with no window function", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("PostgreSQL: Window Functions", "https://www.postgresql.org/docs/current/tutorial-window.html", LinkType.OfficialDocs),
                new ReferenceLinkSeed("SQL subqueries explained", "https://www.postgresql.org/docs/current/functions-subquery.html", LinkType.OfficialDocs),
            ],
            prerequisites: [lesson1]);

        var lesson2Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson2.Slug,
        [
            "Solve a 'top N per group' problem using ROW_NUMBER() + PARTITION BY from scratch",
            "Find one correlated subquery (yours or an example online) and rewrite it as a JOIN or window function",
            "Explain the difference between RANK() and DENSE_RANK() with a tie example",
        ]);

        var module = BuildModule(topicId, "sql-fundamentals", "SQL Fundamentals",
            "Query evaluation order, joins, NULL handling, and the window functions that solve most SQL interview questions.",
            70, [lesson1, lesson2]);

        return (module, [lesson1Checklist, lesson2Checklist]);
    }

    // ============================== Cloud ==============================

    private static (Module, List<ChecklistSeed>) BuildCloudModule(int topicId)
    {
        var lesson1 = BuildLesson(
            slug: "compute-storage-networking-basics",
            title: "Compute, Storage, and Networking Basics",
            summary: "IaaS/PaaS/SaaS, compute options from VMs to serverless, and storage tiers.",
            estimatedMinutes: 30,
            objectives:
            [
                "Place a given cloud service correctly on the IaaS/PaaS/SaaS spectrum",
                "Choose between VMs, containers, and serverless functions for a given workload",
                "Explain the cost/latency trade-off across storage tiers",
                "Apply least-privilege reasoning to a permissions request",
            ],
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
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    IaaS/PaaS/SaaS is like renting an empty apartment (IaaS — you furnish and maintain everything), a furnished apartment (PaaS — plumbing and appliances are handled, you just live there), or a hotel room (SaaS — everything including daily cleaning is done for you, you just use it).

                    Storage tiers are like a filing system: papers you use daily stay on your desk (hot storage), last year's files go in a nearby cabinet (cool), and old archives go to an offsite warehouse that takes a day to retrieve from (archive).
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Rough service-category equivalents across providers**

                    - Virtual machines: EC2 (AWS) / Azure VMs / Compute Engine (GCP)
                    - Managed containers: ECS/EKS (AWS) / AKS (Azure) / GKE (GCP)
                    - Serverless functions: Lambda (AWS) / Azure Functions / Cloud Functions (GCP)
                    - Object storage: S3 (AWS) / Blob Storage (Azure) / Cloud Storage (GCP)
                    - Managed relational DB: RDS (AWS) / Azure SQL / Cloud SQL (GCP)
                    """, 3),
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
                    """, 4, language: "dockerfile"),
                Block(BlockType.Diagram, "A Request Through a Typical Cloud Deployment", BodyFormat.StructuredSteps, """
                    [{"label":"Client"},{"label":"CDN","note":"cached static assets"},{"label":"Load Balancer"},{"label":"Compute","note":"containers or serverless"},{"label":"Managed Database"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Apply the principle of **least privilege**: grant each service/role only the specific permissions it needs (e.g., read-only access to one storage bucket), not broad admin access "to be safe." A compromised credential with narrow permissions limits the blast radius of a security incident.

                    Prefer **autoscaling** over manually provisioning for peak load — pay for capacity you're actually using, and let the platform scale compute up during traffic spikes and back down afterward.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    When asked to compare compute options, frame the answer around trade-offs, not just facts: "serverless minimizes operational overhead and cost-at-idle, but has cold-start latency and execution time limits, so it's a poor fit for long-running or latency-critical workloads — containers are the better fit there."
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Over-provisioning compute "just in case" instead of configuring autoscaling — this quietly wastes money every hour the extra capacity sits idle. Equally common: granting overly broad IAM permissions during initial setup and never tightening them once the service is working.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "A workload runs for milliseconds, sporadically, with unpredictable traffic. What's the best compute fit?",
                    "Serverless functions bill per invocation and scale to zero when idle, matching sporadic, short-lived workloads without paying for idle capacity — a VM or long-running container would be paid for around the clock regardless of actual usage.",
                    [
                        new QuizOptionSeed("A single large virtual machine, always running", false),
                        new QuizOptionSeed("Serverless functions, since they scale to zero and bill per invocation", true),
                        new QuizOptionSeed("A dedicated physical server", false),
                        new QuizOptionSeed("It doesn't matter — all compute options cost the same", false),
                    ]),
                new QuizQuestionSeed(
                    "What does 'least privilege' mean when granting a service permissions?",
                    "Least privilege means granting only the exact permissions a service needs to do its job — nothing broader 'just in case' — so a compromised credential can only do limited damage.",
                    [
                        new QuizOptionSeed("Give every service admin access to simplify permission management", false),
                        new QuizOptionSeed("Grant only the specific permissions a service actually needs, nothing more", true),
                        new QuizOptionSeed("Only apply security rules to production environments", false),
                        new QuizOptionSeed("Use the same credentials across all services for consistency", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("AWS: IaaS vs PaaS vs SaaS", "https://aws.amazon.com/types-of-cloud-computing/", LinkType.FurtherReading),
                new ReferenceLinkSeed("Principle of least privilege", "https://learn.microsoft.com/en-us/azure/security/fundamentals/identity-management-best-practices", LinkType.OfficialDocs),
            ]);

        var lesson1Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson1.Slug,
        [
            "Classify 3 real cloud services you've heard of as IaaS, PaaS, or SaaS",
            "Explain when you'd choose serverless over containers, with a concrete example workload",
            "Review one set of permissions (yours or a hypothetical) for least-privilege violations",
        ]);

        var lesson2 = BuildLesson(
            slug: "cicd-infrastructure-as-code-basics",
            title: "CI/CD & Infrastructure as Code Basics",
            summary: "Automating build/test/deploy, and describing infrastructure as versioned code instead of clicking through a console.",
            estimatedMinutes: 35,
            objectives:
            [
                "Explain the difference between continuous integration and continuous deployment",
                "Describe why Infrastructure as Code (IaC) is more reliable than manual console changes",
                "Identify the stages a typical CI/CD pipeline runs, in order",
                "Explain idempotency in the context of infrastructure automation",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    **Continuous Integration (CI)** means every code change is automatically built and tested as soon as it's pushed — catching integration problems within minutes instead of at the end of a release cycle.

                    **Continuous Deployment/Delivery (CD)** extends this: a change that passes CI is automatically packaged and released (Deployment), or staged and ready for a one-click release (Delivery).

                    **Infrastructure as Code (IaC)** means describing servers, networks, and configuration in version-controlled files (e.g., Terraform, Bicep) instead of manually clicking through a cloud console. This makes infrastructure changes reviewable (a pull request), repeatable (the same file produces the same environment every time), and auditable (git history is the change log).

                    A key IaC property is **idempotency** — applying the same configuration twice produces the same result as applying it once, rather than creating duplicate resources or erroring out.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Manually configuring infrastructure through a console is like assembling furniture from memory every time, slightly differently each time — eventually two "identical" rooms end up subtly different. Infrastructure as Code is like following a numbered instruction manual — the same steps, in the same order, produce the same result every time, and you can hand the manual to anyone else and get an identical outcome.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Typical CI/CD pipeline stages**

                    1. Checkout code
                    2. Restore dependencies
                    3. Build
                    4. Run automated tests
                    5. Package (e.g., build a container image)
                    6. Deploy to an environment (staging, then production)

                    **CI vs. CD**

                    - CI: automatically build + test on every push
                    - Continuous Delivery: automatically stage a release, human approves the final release step
                    - Continuous Deployment: automatically release with no manual approval step at all
                    """, 3),
                Block(BlockType.CodeSnippet, "A Minimal GitHub Actions CI Workflow", BodyFormat.PlainText, """
                    name: CI

                    on: [push, pull_request]

                    jobs:
                      build-and-test:
                        runs-on: ubuntu-latest
                        steps:
                          - uses: actions/checkout@v4
                          - uses: actions/setup-dotnet@v4
                            with:
                              dotnet-version: '10.0.x'
                          - run: dotnet restore
                          - run: dotnet build --no-restore
                          - run: dotnet test --no-build
                    """, 4, language: "yaml"),
                Block(BlockType.Diagram, "From Commit to Production", BodyFormat.StructuredSteps, """
                    [{"label":"Push Commit"},{"label":"CI: Build + Test"},{"label":"Package Artifact"},{"label":"Deploy: Staging"},{"label":"Deploy: Production"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Keep the CI pipeline fast (a few minutes, not tens of minutes) — a slow pipeline gets skipped, worked around, or simply ignored by a team under deadline pressure, quietly defeating its entire purpose.

                    Store infrastructure definitions in the same version control as application code, and require a pull request review for infrastructure changes exactly like code changes — infrastructure bugs are still bugs.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked to design a deployment pipeline, mention the staging-before-production step and rollback strategy explicitly — a design that only covers "how do we ship" without "how do we un-ship a bad release" is incomplete in most interviewers' eyes.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Making a manual, undocumented change directly in the cloud console "just this once" to fix an urgent issue — this silently diverges the real infrastructure from what's described in code, and the next automated deployment can overwrite or conflict with that manual fix.

                    Also common: a CI pipeline that only builds but never actually runs the test suite, giving false confidence that "CI is green" when nothing was actually verified.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "What does 'idempotent' mean in the context of Infrastructure as Code?",
                    "Idempotent means applying the same configuration multiple times produces the same end state as applying it once — it doesn't create duplicates or error out on re-application, which is essential for automation to be safely re-run.",
                    [
                        new QuizOptionSeed("The configuration can only be applied exactly once, ever", false),
                        new QuizOptionSeed("Applying the same configuration repeatedly produces the same result each time", true),
                        new QuizOptionSeed("The infrastructure automatically scales based on demand", false),
                        new QuizOptionSeed("Changes take effect immediately without any deployment step", false),
                    ]),
                new QuizQuestionSeed(
                    "What's the practical difference between Continuous Delivery and Continuous Deployment?",
                    "Both automatically get a change ready for release after it passes CI. Continuous Delivery stops at a human-approved release step; Continuous Deployment skips that manual gate and releases automatically.",
                    [
                        new QuizOptionSeed("They are exactly the same thing with different names", false),
                        new QuizOptionSeed("Continuous Delivery stages a release for manual approval; Continuous Deployment releases automatically with no manual gate", true),
                        new QuizOptionSeed("Continuous Deployment only applies to database changes", false),
                        new QuizOptionSeed("Continuous Delivery skips automated testing", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("GitHub Actions documentation", "https://docs.github.com/en/actions", LinkType.OfficialDocs),
                new ReferenceLinkSeed("What is Infrastructure as Code?", "https://www.terraform.io/use-cases/infrastructure-as-code", LinkType.FurtherReading),
            ],
            prerequisites: [lesson1]);

        var lesson2Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson2.Slug,
        [
            "Write a minimal CI workflow file for a project of your own, even if you don't run it yet",
            "Explain idempotency in infrastructure automation using your own example",
            "List the stages of a deployment pipeline you've used (or would design) from commit to production",
        ]);

        var module = BuildModule(topicId, "cloud-fundamentals", "Cloud Fundamentals",
            "The service-model spectrum, compute options, and automating deployment with CI/CD and Infrastructure as Code.",
            65, [lesson1, lesson2]);

        return (module, [lesson1Checklist, lesson2Checklist]);
    }

    // ============================== Git ==============================

    private static (Module, List<ChecklistSeed>) BuildGitModule(int topicId)
    {
        var lesson1 = BuildLesson(
            slug: "git-fundamentals-commits-branches-merging",
            title: "Git Fundamentals: Commits, Branches & Merging",
            summary: "What a commit actually is, why branches are cheap, and how a merge really works under the hood.",
            estimatedMinutes: 30,
            objectives:
            [
                "Explain what a commit actually stores, not just what it does",
                "Explain why creating a Git branch is nearly instantaneous",
                "Distinguish a fast-forward merge from a merge with a merge commit",
                "Undo a mistake using the right tool (revert vs. reset) for the situation",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    A Git **commit** is a snapshot of your entire project at that point in time (not just a diff), plus a pointer to its parent commit(s), an author, and a message. Chain enough commits together and you get the project's full history as a linked list (or, with merges, a directed acyclic graph).

                    A **branch** is nothing more than a movable label pointing at one commit — creating a branch just writes a new pointer, which is why it's instantaneous regardless of repository size, unlike some older version control systems where branching meant copying files.

                    A **merge** combines the history of two branches. If one branch is simply "ahead" of the other with no divergent commits, Git can do a **fast-forward merge** — just move the pointer forward, no new commit needed. If both branches have diverged (each has commits the other doesn't), Git creates a **merge commit** with two parents, combining both histories.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    A commit is like a saved checkpoint in a video game — a complete snapshot of the entire game state at that moment, not just "what changed since the last save." A branch is like a bookmark in a book — cheap to place, and moving it doesn't rewrite the book, it just points somewhere new.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Everyday commands**

                    - `git status` — what's changed, what's staged
                    - `git add <file>` / `git add -p` — stage changes (whole file or interactively, hunk by hunk)
                    - `git commit -m "message"` — snapshot staged changes
                    - `git branch <name>` / `git checkout -b <name>` — create (and switch to) a branch
                    - `git log --oneline --graph` — visualize commit history and branch structure

                    **Undoing things**

                    - `git revert <commit>` — creates a NEW commit that undoes a previous one (safe on shared/pushed history)
                    - `git reset --soft <commit>` — moves the branch pointer back, keeps changes staged
                    - `git reset --hard <commit>` — moves the branch pointer back AND discards changes (destructive, local-only)
                    """, 3),
                Block(BlockType.CodeSnippet, "A Typical Feature-Branch Workflow", BodyFormat.PlainText, """
                    git checkout -b feature/add-search
                    # ... make changes ...
                    git add -p
                    git commit -m "Add search endpoint with basic filtering"
                    git push -u origin feature/add-search
                    # ... open a pull request, get it reviewed ...
                    git checkout main
                    git pull
                    git merge feature/add-search
                    """, 4, language: "bash"),
                Block(BlockType.Diagram, "Fast-Forward vs. Merge Commit", BodyFormat.AsciiArt, """
                    Fast-forward (main has no new commits since branching):

                    main:    A---B
                    feature:      \\--C---D

                    after merge, main:  A---B---C---D   (pointer just moves forward)

                    Merge commit (main advanced too, histories diverged):

                    main:    A---B-------E
                    feature:      \\--C---D/

                    after merge, main:  A---B---E---M   (M has two parents: E and D)
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Use `git revert` instead of `git reset --hard` for anything already pushed and shared — revert creates a new, honest commit that undoes prior changes, while a hard reset rewrites history that others may have already built on top of.

                    Commit small, focused changes with a clear message describing *why*, not just *what* — `git diff` already shows what changed; the message should explain the reasoning a diff can't.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked "how do you undo a bad commit that's already been pushed," the correct answer is `git revert`, explained with the reasoning (it doesn't rewrite shared history) — jumping straight to `git reset --hard` on a pushed branch is a red flag in most engineering interviews.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Running `git reset --hard` or force-pushing on a branch other people have already pulled — this rewrites history out from under their local copies, causing confusing conflicts or lost work for teammates.

                    Also common: one giant commit at the end of a day's work with a message like "fixes" — this makes `git log` useless for understanding *why* a change was made, and makes reverting one specific change impossible without reverting everything else in the same commit.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "Why is creating a new Git branch nearly instantaneous, regardless of repository size?",
                    "A branch is just a lightweight pointer (a small file containing a commit hash) — creating one doesn't copy any files or history, it just writes a new pointer at the current commit.",
                    [
                        new QuizOptionSeed("Git only copies changed files, which are usually small", false),
                        new QuizOptionSeed("A branch is just a pointer to a commit, not a copy of the repository", true),
                        new QuizOptionSeed("Branches are created lazily and don't actually exist until first used", false),
                        new QuizOptionSeed("Modern hardware makes any Git operation instantaneous", false),
                    ]),
                new QuizQuestionSeed(
                    "You need to undo a commit that's already been pushed and pulled by teammates. What should you use?",
                    "git revert creates a new commit that undoes the change, preserving history — safe for anything already shared. git reset --hard rewrites history, which breaks things for anyone who already pulled the commits being removed.",
                    [
                        new QuizOptionSeed("git reset --hard, then force-push", false),
                        new QuizOptionSeed("git revert, which creates a new commit undoing the change", true),
                        new QuizOptionSeed("Delete the branch and start over", false),
                        new QuizOptionSeed("Manually edit the remote repository's files", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Git Basics — official documentation", "https://git-scm.com/book/en/v2/Git-Basics-Getting-a-Git-Repository", LinkType.OfficialDocs),
                new ReferenceLinkSeed("Learn Git Branching (interactive)", "https://learngitbranching.js.org/", LinkType.FurtherReading),
            ]);

        var lesson1Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson1.Slug,
        [
            "Explain what a commit stores, in your own words, without looking it up",
            "Create a branch, make a commit, and merge it back, narrating each step",
            "Explain when you'd use git revert instead of git reset --hard",
        ]);

        var lesson2 = BuildLesson(
            slug: "collaborative-git-prs-rebasing-conflicts",
            title: "Collaborative Git: Pull Requests, Rebasing & Resolving Conflicts",
            summary: "Rebase vs. merge, and actually resolving a merge conflict instead of panicking at the markers.",
            estimatedMinutes: 35,
            objectives:
            [
                "Explain the difference between rebase and merge, and when to prefer each",
                "Read Git's conflict markers and resolve a conflict correctly",
                "Describe what a pull request is for beyond 'a button that merges code'",
                "Avoid rebasing commits that have already been pushed and shared",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    **Rebasing** replays your branch's commits on top of another branch's latest commit, producing a linear history with no merge commit — as if you'd started your work *after* the latest change on the target branch, instead of at the same time as it.

                    **Merging** instead combines both histories as-is, preserving the fact that the two branches diverged, usually with a merge commit that has two parents.

                    A **merge conflict** happens when Git can't automatically combine changes — usually because both branches edited the same lines. Git marks the conflicting regions directly in the file with `<<<<<<<`, `=======`, and `>>>>>>>`; resolving it means editing the file to keep the correct final content and removing the markers, then staging and committing (or continuing the rebase).

                    A **pull request** (PR) is a request to merge one branch into another, built around a diff — it's the unit of code review, discussion, and CI validation before code reaches a shared branch, not just a "merge" button.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Merging is like stapling two separate drafts of a document together with a note explaining both were written in parallel. Rebasing is like rewriting your draft as if you'd started typing *after* reading the other person's latest version — the result reads as one continuous document, but it's a rewritten story of how it happened, not the literal original sequence of events.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Rebase vs. merge**

                    - `git merge feature` — preserves both histories, may create a merge commit
                    - `git rebase main` (while on `feature`) — replays feature's commits onto main's tip, linear history, rewrites commit hashes

                    **Golden rule**: never rebase commits that have already been pushed and that someone else might have based work on — rebasing rewrites commit history, and a force-push after rebasing a shared branch breaks everyone else's local copy.

                    **Resolving a conflict**

                    1. Open the conflicting file, find `<<<<<<<` / `=======` / `>>>>>>>` markers
                    2. Edit to the correct final content, delete all three marker lines
                    3. `git add <file>` to mark it resolved
                    4. `git commit` (merge) or `git rebase --continue` (rebase)
                    """, 3),
                Block(BlockType.CodeSnippet, "Resolving a Merge Conflict", BodyFormat.PlainText, """
                    <<<<<<< HEAD
                    public const int MaxRetries = 3;
                    =======
                    public const int MaxRetries = 5;
                    >>>>>>> feature/increase-retries

                    // After deciding 5 is correct and deleting the markers:
                    public const int MaxRetries = 5;
                    """, 4),
                Block(BlockType.Diagram, "Rebase: Replaying Commits on a New Base", BodyFormat.AsciiArt, """
                    Before rebase:
                    main:    A---B---E
                    feature:      \\--C---D

                    After `git rebase main` on feature:
                    main:    A---B---E
                    feature:              \\--C'---D'   (new commits, same changes, new hashes)
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Rebase your own feature branch onto the latest `main` *before* opening a pull request (while it's still only yours), to get a clean, linear diff for reviewers — but merge (don't rebase) once a branch is shared with others or already under review.

                    Write a pull request description that explains *why*, links related context (an issue, a design doc), and calls out anything a reviewer should pay special attention to — a PR with just a title and no description asks the reviewer to reverse-engineer your intent from the diff alone.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked "rebase or merge, which do you prefer," the strong answer isn't picking a side — it's stating the actual rule: rebase freely on your own not-yet-shared branch for a clean history, merge (never rebase) anything that's already been pushed and might have other work built on top of it.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Panicking at conflict markers and running `git merge --abort` repeatedly instead of actually reading the conflicting lines and deciding what the correct final content should be — conflicts are a normal, resolvable part of collaboration, not a sign something is broken.

                    Also common: rebasing (and force-pushing) a branch that a teammate has already pulled and built additional commits on top of — this silently orphans their work from their perspective and causes confusing, hard-to-diagnose sync issues.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "What's the main risk of rebasing a branch that's already been pushed and pulled by a teammate?",
                    "Rebasing rewrites commit history (new commit hashes). If a teammate already pulled the old commits and built more work on top, a force-push of the rebased branch orphans their work relative to the new history, causing confusing conflicts.",
                    [
                        new QuizOptionSeed("Nothing — rebase is always safe regardless of who has pulled the branch", false),
                        new QuizOptionSeed("It rewrites commit history, breaking anyone who already pulled and built on the old commits", true),
                        new QuizOptionSeed("It permanently deletes the branch", false),
                        new QuizOptionSeed("It only affects the remote repository, never local copies", false),
                    ]),
                new QuizQuestionSeed(
                    "What are the three marker lines Git inserts into a file during a merge conflict, in order?",
                    "Git wraps 'your side' and 'their side' of the conflicting change with <<<<<<<, then a ======= divider, then >>>>>>> — resolving means editing to the correct final content and deleting all three marker lines.",
                    [
                        new QuizOptionSeed("<<<<<<<, =======, >>>>>>>", true),
                        new QuizOptionSeed("///, ---, +++", false),
                        new QuizOptionSeed("BEGIN, MIDDLE, END", false),
                        new QuizOptionSeed("<conflict>, <choose>, </conflict>", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Git Rebasing — official documentation", "https://git-scm.com/book/en/v2/Git-Branching-Rebasing", LinkType.OfficialDocs),
                new ReferenceLinkSeed("About pull requests (GitHub docs)", "https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/proposing-changes-to-your-work-with-pull-requests/about-pull-requests", LinkType.OfficialDocs),
            ],
            prerequisites: [lesson1]);

        var lesson2Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson2.Slug,
        [
            "Deliberately create and resolve a merge conflict in a scratch repo",
            "Explain the golden rule of rebasing (never rebase shared history) in your own words",
            "Write a pull request description for one of your own recent changes, as if a reviewer had zero context",
        ]);

        var module = BuildModule(topicId, "git-fundamentals", "Git Fundamentals",
            "Commits, branches, merging, rebasing, and resolving conflicts without panicking.",
            65, [lesson1, lesson2]);

        return (module, [lesson1Checklist, lesson2Checklist]);
    }

    // ============================== DevOps ==============================

    private static (Module, List<ChecklistSeed>) BuildDevOpsModule(int topicId)
    {
        var lesson1 = BuildLesson(
            slug: "cicd-pipelines-automated-testing",
            title: "CI/CD Pipelines & Automated Testing in Practice",
            summary: "Designing a real pipeline: what runs where, how fast feedback should be, and what 'green' actually means.",
            estimatedMinutes: 35,
            objectives:
            [
                "Design a pipeline with clearly separated build, test, and deploy stages",
                "Explain the test pyramid and why unit tests should dominate the count",
                "Identify what should block a merge vs. what should only warn",
                "Explain why flaky tests are worse than no tests at all",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    A production CI/CD pipeline is more than "build then deploy" — it's a series of gates, each one cheaper and faster than the next, designed to catch problems as early (and as cheaply) as possible.

                    The **test pyramid** describes the ideal shape of a test suite: many fast, cheap **unit tests** at the bottom, fewer **integration tests** in the middle (real dependencies like a database, but still scoped), and a small number of slow, expensive **end-to-end tests** at the top that exercise the whole system like a real user would.

                    A pipeline stage should either **block** the merge (a failure means the change is genuinely broken and must not ship) or **warn** (informational, doesn't stop anything) — mixing these up either lets real bugs through or makes engineers start ignoring red pipelines altogether.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    The test pyramid is like a hospital's triage system: lots of quick vital-sign checks happen first (unit tests) because they're fast and catch most obvious problems; fewer, more involved diagnostic tests happen next (integration tests); and full surgery-level intervention (end-to-end tests) is reserved for the rare cases that actually need it. Doing full surgery on every patient "just to be safe" would be accurate, but far too slow and expensive to be practical.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Test pyramid, roughly**

                    - Unit tests: majority of the suite, no external dependencies, milliseconds each
                    - Integration tests: fewer, real dependencies (database, real HTTP calls), seconds each
                    - End-to-end tests: smallest number, whole-system, exercises the UI/API like a user, can take minutes

                    **Pipeline gate design**

                    - Build failure → always blocks
                    - Unit/integration test failure → always blocks
                    - Linting/style warnings → usually warns, doesn't block
                    - Flaky test → fix or quarantine immediately; don't let engineers learn to ignore red
                    """, 3),
                Block(BlockType.CodeSnippet, "A Pipeline with Separated, Ordered Stages", BodyFormat.PlainText, """
                    stages:
                      - build
                      - unit-test
                      - integration-test
                      - deploy-staging
                      - e2e-test
                      - deploy-production

                    # Each stage only runs if the previous one succeeded — a build
                    # failure means unit tests never even attempt to run, saving time.
                    """, 4, language: "yaml"),
                Block(BlockType.Diagram, "The Test Pyramid", BodyFormat.AsciiArt, """
                            /\\
                           /E2E\\        few, slow, whole-system
                          /------\\
                         /  Integ  \\    some, real dependencies
                        /------------\\
                       /   Unit Tests  \\  many, fast, isolated
                      /------------------\\
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Quarantine (or delete) a flaky test immediately rather than letting it sit there failing intermittently — a flaky test that engineers learn to re-run "until it passes" trains the whole team to ignore pipeline failures in general, which is far more dangerous than having one fewer test.

                    Keep the majority of your test count as fast unit tests — if your suite is mostly slow end-to-end tests, feedback arrives too late to be useful during development, and the pipeline itself becomes a bottleneck.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked to design a CI/CD pipeline, explicitly mention the test pyramid shape and where each stage sits in it — an answer that only says "run the tests" without addressing test types, ordering, and blocking vs. warning behavior misses what interviewers are usually screening for.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Building a test suite that's mostly slow end-to-end tests because they "feel more real" — this inverts the pyramid, makes the pipeline slow, and makes failures hard to localize (an E2E failure could be caused by almost anything).

                    Also common: treating a flaky test as acceptable background noise instead of a priority bug — every flaky test erodes trust in the entire pipeline, not just that one test.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "Why should unit tests make up the majority of a healthy test suite?",
                    "Unit tests are fast and isolated, giving quick, precise feedback close to where a bug actually is. A suite dominated by slow end-to-end tests gives slower feedback and makes failures harder to localize.",
                    [
                        new QuizOptionSeed("Unit tests are the only kind that can catch real bugs", false),
                        new QuizOptionSeed("They're fast and isolated, giving quick, precisely-localized feedback", true),
                        new QuizOptionSeed("Integration and end-to-end tests are being phased out industry-wide", false),
                        new QuizOptionSeed("Unit tests are required by most CI platforms", false),
                    ]),
                new QuizQuestionSeed(
                    "What's the biggest danger of an intermittently-failing (flaky) test?",
                    "Beyond the specific test being unreliable, a flaky test teaches engineers to distrust or ignore pipeline failures generally — 're-run until green' becomes a habit that lets genuinely broken changes slip through too.",
                    [
                        new QuizOptionSeed("It slightly slows down the pipeline", false),
                        new QuizOptionSeed("It trains engineers to ignore or re-run past pipeline failures generally, masking real bugs", true),
                        new QuizOptionSeed("It only affects that one test file", false),
                        new QuizOptionSeed("Flaky tests are not actually a real problem in practice", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("The Practical Test Pyramid (Martin Fowler's site)", "https://martinfowler.com/articles/practical-test-pyramid.html", LinkType.FurtherReading),
                new ReferenceLinkSeed("GitHub Actions: Workflow syntax", "https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions", LinkType.OfficialDocs),
            ]);

        var lesson1Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson1.Slug,
        [
            "Sketch a pipeline with clearly separated build/unit/integration/e2e/deploy stages",
            "Identify the current shape of a test suite you've worked on — is it pyramid-shaped or inverted?",
            "Explain why a flaky test is more dangerous than a missing test",
        ]);

        var lesson2 = BuildLesson(
            slug: "monitoring-logging-observability-basics",
            title: "Monitoring, Logging & Observability Basics",
            summary: "The three pillars of observability, and why 'it works on my machine' isn't good enough once something is deployed.",
            estimatedMinutes: 35,
            objectives:
            [
                "Distinguish logs, metrics, and traces and explain what each is for",
                "Explain the difference between monitoring and observability",
                "Design a basic alert that avoids alert fatigue",
                "Describe how distributed tracing helps debug a request across multiple services",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    **Observability** is the ability to understand a system's internal state from its external outputs — not just "is it up or down" (monitoring), but "why is it behaving this way right now," including questions you didn't think to ask in advance.

                    Three pillars make this possible:

                    - **Logs** — discrete, timestamped events ("order 123 failed validation: missing email").
                    - **Metrics** — numeric measurements over time (requests/second, error rate, p99 latency) — cheap to store and great for dashboards/alerts, but they tell you *that* something changed, not *why*.
                    - **Traces** — the path of one request as it flows through multiple services, showing where time was spent and where it failed — essential once a system is more than one process.

                    **Monitoring** typically means watching known metrics against known thresholds ("alert if error rate > 5%"). **Observability** means having enough raw signal (logs/metrics/traces, all correlated) to investigate problems you didn't predict in advance.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Monitoring is like a car's dashboard warning light — it tells you "check engine," a known condition you anticipated and built a sensor for. Observability is like being able to actually pop the hood and trace exactly which wire, sensor, and component caused a *specific, previously-unseen* problem — you don't need to have predicted this exact failure in advance to diagnose it.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Three pillars at a glance**

                    - Logs — discrete events, best for "what exactly happened"
                    - Metrics — numeric time series, best for "how is the system trending, at a glance"
                    - Traces — one request's full journey across services, best for "where did the time/failure actually happen"

                    **Good alert design**

                    - Alert on symptoms users would notice (high error rate, high latency), not every internal fluctuation
                    - Every alert should be actionable — if nobody would do anything differently on receiving it, it shouldn't page anyone
                    """, 3),
                Block(BlockType.CodeSnippet, "Structured Logging Instead of Plain Strings", BodyFormat.PlainText, """
                    // Avoid: a plain string, hard to search/filter/aggregate later.
                    logger.LogInformation($"Order {orderId} failed for user {userId}");

                    // Prefer: structured logging with named properties —
                    // queryable and filterable in a log aggregation tool.
                    logger.LogInformation(
                        "Order {OrderId} failed validation for user {UserId}: {Reason}",
                        orderId, userId, "missing email");
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "A Trace Across Three Services", BodyFormat.StructuredSteps, """
                    [{"label":"API Gateway","note":"12ms"},{"label":"Auth Service","note":"8ms"},{"label":"Order Service","note":"340ms — slow!"},{"label":"Database","note":"310ms of the 340ms"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Use structured logging (named fields, not string interpolation) from day one — it costs almost nothing extra to write and makes logs actually searchable and aggregatable later, instead of grep-ing through plain text.

                    Design every alert to be actionable — if an alert fires and the on-call engineer's honest reaction is "huh, interesting" rather than "I need to do something," it shouldn't have paged anyone. Alert fatigue from noisy, non-actionable alerts is how real incidents get missed.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked to debug a "why is this one request slow" scenario, mention distributed tracing specifically — it's the tool built exactly for "where in this multi-service request did the time actually go," which logs and metrics alone can't answer as directly.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Logging plain interpolated strings instead of structured fields — this works fine until you actually need to search or aggregate logs at scale, at which point free-text log messages are far harder to query than named fields.

                    Also common: alerting on every metric fluctuation "just to be safe," which trains the on-call engineer to mute or ignore alerts — the same alert-fatigue failure mode as flaky tests in a CI pipeline.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "A request is slow, and it passes through 4 microservices. Which observability tool is best suited to find exactly where the time went?",
                    "Distributed tracing follows one request's path across every service it touches, timing each hop — exactly the tool built for 'where in this multi-service request did the time actually go.'",
                    [
                        new QuizOptionSeed("A single aggregate metric like average response time", false),
                        new QuizOptionSeed("Distributed tracing across the request's full path", true),
                        new QuizOptionSeed("A plain text log search on one server", false),
                        new QuizOptionSeed("There's no way to isolate this without redeploying with more logging", false),
                    ]),
                new QuizQuestionSeed(
                    "What makes an alert 'good' rather than noise?",
                    "A good alert is actionable — the person who receives it should have something specific to do. Alerts that fire on normal fluctuations with no real action train people to ignore alerts generally, which is dangerous.",
                    [
                        new QuizOptionSeed("It fires as often as possible, to be thorough", false),
                        new QuizOptionSeed("It's actionable — the recipient has something specific to do about it", true),
                        new QuizOptionSeed("It only ever fires during business hours", false),
                        new QuizOptionSeed("It's based on a metric, never a log", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("OpenTelemetry: Observability primer", "https://opentelemetry.io/docs/concepts/observability-primer/", LinkType.OfficialDocs),
                new ReferenceLinkSeed("Structured logging in .NET", "https://learn.microsoft.com/en-us/dotnet/core/extensions/logging", LinkType.OfficialDocs),
            ],
            prerequisites: [lesson1]);

        var lesson2Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson2.Slug,
        [
            "Convert one plain-string log statement in your own code to structured logging",
            "Design one actionable alert and explain what the on-call engineer should do when it fires",
            "Explain the difference between monitoring and observability in your own words",
        ]);

        var module = BuildModule(topicId, "devops-fundamentals", "DevOps Fundamentals",
            "Designing real CI/CD pipelines with a healthy test pyramid, then actually seeing what's happening in production.",
            70, [lesson1, lesson2]);

        return (module, [lesson1Checklist, lesson2Checklist]);
    }

    // ============================== Architecture ==============================

    private static (Module, List<ChecklistSeed>) BuildArchitectureModule(int topicId)
    {
        var lesson1 = BuildLesson(
            slug: "clean-architecture-separation-of-concerns",
            title: "Clean Architecture & Separation of Concerns",
            summary: "Why business logic shouldn't know your database exists, and how layering makes that possible.",
            estimatedMinutes: 40,
            objectives:
            [
                "Explain the dependency rule at the heart of Clean/Onion Architecture",
                "Identify which layer a piece of code belongs in",
                "Explain why business logic depending on infrastructure details is a design smell",
                "Recognize a layering violation in existing code",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    **Clean Architecture** (and its close relatives, Onion/Hexagonal Architecture) organizes code into concentric layers, with one hard rule: **dependencies only point inward**. Outer layers (UI, database access, external APIs) can depend on inner layers (business logic, domain rules), but never the reverse.

                    At the center sits your **domain/business logic** — the rules that make your application what it is, expressed with no knowledge of HTTP, SQL, or any specific framework. Around that sits **application logic** (use cases/orchestration), and on the outside sit **infrastructure** concerns (the actual database, web framework, external services).

                    The payoff: business logic can be tested with no database, no web server, and no network — and infrastructure details (swap SQL Server for Postgres, REST for gRPC) can change without touching business rules at all.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Clean Architecture is like a company's org chart where the CEO's core strategy doesn't reference "the specific printer brand in the mailroom" — the strategy layer shouldn't know or care about implementation details several layers below it. If the mailroom switches printer vendors, the CEO's strategy document doesn't need a single edit.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **The dependency rule**

                    - Domain/business logic depends on nothing outside itself
                    - Application layer depends on domain, not on infrastructure
                    - Infrastructure (database, web, external APIs) depends inward on application/domain — never the reverse

                    **Smell test**: if your core business logic class has a `using` statement for your ORM, your web framework, or an HTTP client, dependencies are pointing the wrong way.
                    """, 3),
                Block(BlockType.CodeSnippet, "Business Logic That Doesn't Know About EF Core", BodyFormat.PlainText, """
                    // Domain layer: no EF Core, no database, no framework references at all.
                    public class Order
                    {
                        public decimal Total { get; private set; }
                        public bool IsPaid { get; private set; }

                        public void MarkAsPaid()
                        {
                            if (Total <= 0) throw new InvalidOperationException("Cannot pay a zero-total order.");
                            IsPaid = true;
                        }
                    }

                    // Infrastructure layer: THIS is where EF Core/AppDbContext live —
                    // it depends on the domain, the domain never depends on it.
                    public class OrderRepository(AppDbContext db)
                    {
                        public Task<Order?> FindAsync(int id) => db.Orders.FindAsync(id).AsTask();
                    }
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "Dependencies Point Inward", BodyFormat.AsciiArt, """
                            +-------------------------+
                            |     Infrastructure       |   (database, web framework,
                            |   +-----------------+    |    external APIs)
                            |   |  Application    |    |
                            |   |  +-----------+  |    |
                            |   |  |  Domain   |  |    |
                            |   |  +-----------+  |    |
                            |   +-----------------+    |
                            +-------------------------+

                    Arrows of DEPENDENCY point inward only: Infrastructure -> Application -> Domain.
                    Domain never references anything outside itself.
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Keep domain entities free of framework attributes and ORM-specific base classes where practical — a domain class littered with `[Column]`/`[Table]` attributes has already started depending outward on infrastructure concerns.

                    When in doubt about which layer a piece of code belongs in, ask: "would this logic still make sense if we swapped the database or the web framework?" If yes, it's domain/application logic; if the answer depends on the specific technology, it's infrastructure.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked to describe your project's architecture, be ready to point at one specific class and explain which layer it's in and why — a vague "we use clean architecture" without a concrete example of the dependency rule in action doesn't demonstrate real understanding.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Letting business logic directly call the database (or an HTTP client, or read configuration) instead of going through an abstraction the infrastructure layer implements — this is the most common layering violation, and it quietly makes business rules untestable without spinning up real infrastructure.

                    Also common: over-applying Clean Architecture's full ceremony (many layers, many interfaces) to a small, simple application where a couple of well-organized files would have been clearer — architecture should match the actual complexity of the problem, not a template applied by default.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "In Clean/Onion Architecture, which direction should dependencies point?",
                    "Outer layers (infrastructure — database, web framework) depend inward on application and domain logic. The domain never references anything outside itself, which is what lets it be tested and reasoned about in isolation.",
                    [
                        new QuizOptionSeed("Domain logic depends on infrastructure, since infrastructure does the real work", false),
                        new QuizOptionSeed("Infrastructure depends inward on application/domain logic, never the reverse", true),
                        new QuizOptionSeed("All layers depend equally on each other", false),
                        new QuizOptionSeed("Direction doesn't matter as long as the code compiles", false),
                    ]),
                new QuizQuestionSeed(
                    "What's a clear sign that dependencies are pointing the wrong way in a 'clean architecture' codebase?",
                    "If your core business-rule class has a using statement for your ORM, web framework, or HTTP client, it has started depending outward on infrastructure — exactly backwards from the dependency rule.",
                    [
                        new QuizOptionSeed("The domain class has a using statement for your ORM or web framework", true),
                        new QuizOptionSeed("The infrastructure layer references the application layer", false),
                        new QuizOptionSeed("The project has more than three folders", false),
                        new QuizOptionSeed("Unit tests exist for the domain layer", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("The Clean Architecture (Uncle Bob's original post)", "https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html", LinkType.FurtherReading),
                new ReferenceLinkSeed("Common web application architectures (.NET docs)", "https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures", LinkType.OfficialDocs),
            ]);

        var lesson1Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson1.Slug,
        [
            "Find one place in your own code where business logic directly depends on infrastructure, and describe the fix",
            "Explain the dependency rule using your own words and a concrete example",
            "Identify which layer three specific classes in a project of yours belong to",
        ]);

        var lesson2 = BuildLesson(
            slug: "common-design-patterns",
            title: "Design Patterns Every Engineer Should Recognize",
            summary: "Strategy, Repository, and Dependency Injection as patterns — not just as buzzwords — and when each earns its complexity.",
            estimatedMinutes: 40,
            objectives:
            [
                "Explain the Strategy pattern and identify a real use case for it",
                "Explain the Repository pattern's actual purpose beyond 'a class with database methods'",
                "Explain why Dependency Injection is itself an implementation of the Dependency Inversion Principle",
                "Recognize when a pattern is solving a real problem vs. adding needless ceremony",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    Design patterns are named, reusable solutions to recurring problems — useful vocabulary for communicating a design quickly, not a checklist to apply everywhere.

                    **Strategy pattern**: define a family of interchangeable algorithms behind one interface, and let the caller pick which one to use at runtime (e.g., different pricing strategies, different sorting comparators).

                    **Repository pattern**: an abstraction over data access that lets business logic ask for domain objects ("give me this order") without knowing whether they come from SQL, an API, or an in-memory cache — the actual point is testability and decoupling from a specific data-access technology, not just "a class that wraps `DbContext` calls."

                    **Dependency Injection** is itself a concrete implementation of the Dependency Inversion Principle (from SOLID) — instead of a class constructing its own dependencies, they're supplied from outside, usually through the constructor, which is exactly what makes swapping a real implementation for a test double possible.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    The Strategy pattern is like a GPS app letting you choose "fastest route," "shortest route," or "avoid tolls" — same underlying goal (get from A to B), interchangeable strategies for getting there, chosen at the moment you need it.

                    The Repository pattern is like ordering from a restaurant menu instead of walking into the kitchen yourself — you ask for "a burger," and don't need to know whether it's cooked on a grill, in an oven, or by a robot arm. The kitchen (data access) can change entirely without changing how you order.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **When each pattern earns its keep**

                    - Strategy — you have genuinely interchangeable algorithms selected at runtime (not just one `if` branch that will never grow)
                    - Repository — you need to swap or fake the data source for testing, or the same domain data comes from more than one source
                    - Dependency Injection — you need to substitute a real dependency with a test double, or support more than one implementation of an interface

                    If none of these conditions hold, the pattern is likely just ceremony.
                    """, 3),
                Block(BlockType.CodeSnippet, "Strategy Pattern for Pricing", BodyFormat.PlainText, """
                    public interface IPricingStrategy
                    {
                        decimal CalculatePrice(decimal basePrice);
                    }

                    public class StandardPricing : IPricingStrategy
                    {
                        public decimal CalculatePrice(decimal basePrice) => basePrice;
                    }

                    public class HolidaySalePricing : IPricingStrategy
                    {
                        public decimal CalculatePrice(decimal basePrice) => basePrice * 0.8m;
                    }

                    // The caller picks a strategy at runtime; CheckoutService
                    // never needs an if/else chain of pricing rules.
                    public class CheckoutService(IPricingStrategy pricingStrategy)
                    {
                        public decimal GetFinalPrice(decimal basePrice) =>
                            pricingStrategy.CalculatePrice(basePrice);
                    }
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "Repository as an Abstraction Boundary", BodyFormat.AsciiArt, """
                    Business Logic  --->  IOrderRepository (interface)  <---  SqlOrderRepository
                                                                          <---  InMemoryOrderRepository (for tests)

                    Business logic only ever talks to the interface — it never
                    knows or cares which implementation is actually running.
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Introduce a pattern when its specific problem actually shows up (a second algorithm variant appears, a test needs a fake data source) — not preemptively, "because it's a best practice." A single-implementation Repository around a single-implementation Strategy is pure ceremony with no payoff.

                    Name the pattern you're using in code reviews and design discussions ("this is a Strategy for pricing rules") — shared vocabulary makes design discussions faster and less ambiguous than describing the same structure from scratch every time.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    When asked "what design patterns have you used," don't just name-drop — describe the actual problem it solved and what would have gone wrong without it. "I used Strategy because we had four interchangeable discount rules and an `if/else` chain was becoming unreadable and hard to extend" is a real answer; "I use Repository everywhere" is not.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Applying the Repository pattern (or any pattern) reflexively to every entity in a codebase regardless of whether it's ever tested with a fake data source or ever has more than one data source — this adds an abstraction layer that provides no real benefit and makes the code harder to navigate.

                    Also common: confusing "using Dependency Injection" (the DI container wiring things up) with "following the Dependency Inversion Principle" (depending on abstractions) — you can technically use a DI container while still injecting concrete classes, missing the actual point.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "What's the actual point of the Repository pattern, beyond 'a class with database methods'?",
                    "The real value is decoupling business logic from a specific data-access technology — letting you swap the real database for a test double, or support multiple underlying data sources, without touching the code that consumes the repository.",
                    [
                        new QuizOptionSeed("It makes SQL queries run faster", false),
                        new QuizOptionSeed("It decouples business logic from a specific data source, enabling testing and swapping implementations", true),
                        new QuizOptionSeed("It's required by Entity Framework Core", false),
                        new QuizOptionSeed("It automatically generates database migrations", false),
                    ]),
                new QuizQuestionSeed(
                    "When does the Strategy pattern actually earn its complexity?",
                    "Strategy is worth it when there are genuinely multiple interchangeable algorithms chosen at runtime. A single `if` branch that will realistically never grow doesn't need the extra interface and classes.",
                    [
                        new QuizOptionSeed("Whenever there's any conditional logic at all", false),
                        new QuizOptionSeed("When there are genuinely interchangeable algorithms selected at runtime", true),
                        new QuizOptionSeed("Only when using a dependency injection container", false),
                        new QuizOptionSeed("It should always be used instead of if/else for consistency", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Refactoring Guru: Strategy Pattern", "https://refactoring.guru/design-patterns/strategy", LinkType.FurtherReading),
                new ReferenceLinkSeed("Repository pattern (.NET architecture docs)", "https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design", LinkType.OfficialDocs),
            ],
            prerequisites: [lesson1]);

        var lesson2Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson2.Slug,
        [
            "Find one if/else chain in your code that could be a Strategy pattern, and decide if it's actually worth converting",
            "Explain the difference between using a DI container and actually following Dependency Inversion",
            "Describe a Repository (real or hypothetical) and what data source it would let you swap in for tests",
        ]);

        var module = BuildModule(topicId, "software-architecture-fundamentals", "Software Architecture Fundamentals",
            "Clean Architecture's dependency rule, and the design patterns that actually earn their complexity.",
            80, [lesson1, lesson2]);

        return (module, [lesson1Checklist, lesson2Checklist]);
    }

    // ============================== Soft Skills ==============================

    private static (Module, List<ChecklistSeed>) BuildSoftSkillsModule(int topicId)
    {
        var lesson1 = BuildLesson(
            slug: "star-method-behavioral-interviews",
            title: "The STAR Method & Behavioral Interviews",
            summary: "Structuring behavioral answers, the pseudocode-first habit for coding rounds, and the full interview loop.",
            estimatedMinutes: 30,
            objectives:
            [
                "Structure a behavioral answer using STAR without rambling",
                "Prepare a small set of stories that flexibly cover multiple question categories",
                "Recognize the stages of a typical interview loop and what each is actually assessing",
                "Ask a clarifying question when a behavioral prompt is ambiguous, instead of guessing",
            ],
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
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    A STAR answer is like a well-edited news report, not a diary entry: a diary entry meanders through every detail in the order you experienced them; a news report leads with what matters, gives just enough context to make sense of it, and closes with the outcome — which is exactly the ratio an interviewer is listening for.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Common behavioral question categories** (prepare 1-2 STAR stories per category, reused across questions)

                    - Conflict with a teammate or disagreement with a decision
                    - A time you failed, or missed a deadline
                    - Leading without formal authority / influencing a team
                    - Handling ambiguous or changing requirements
                    - A time you received tough feedback
                    - Prioritizing under time pressure with competing deadlines
                    """, 3),
                Block(BlockType.CodeSnippet, "Pseudocode-First Template for Coding Rounds", BodyFormat.PlainText, """
                    function solve(input):
                        // 1. Restate the problem and constraints out loud
                        // 2. State the brute-force approach and its complexity
                        // 3. Identify the bottleneck, propose the optimization
                        // 4. Write the optimized approach as pseudocode BEFORE real code
                        // 5. Only then translate pseudocode -> actual syntax
                        // 6. Trace through one example by hand
                        // 7. State final time/space complexity
                    """, 4),
                Block(BlockType.Diagram, "The Typical Interview Loop", BodyFormat.StructuredSteps, """
                    [{"label":"Recruiter Screen"},{"label":"Technical Phone Screen"},{"label":"Onsite: Coding"},{"label":"Onsite: System Design"},{"label":"Onsite: Behavioral"},{"label":"Hiring Committee"},{"label":"Offer"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Prepare 5-6 flexible STAR stories that each cover multiple categories (a single well-chosen story about a project that went sideways can answer "tell me about a failure," "tell me about conflict," and "tell me about a tight deadline" depending on which angle you emphasize) rather than memorizing a separate story for every possible question.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If a behavioral question is ambiguous ("tell me about a time you showed leadership"), ask a brief clarifying question before answering ("do you mean leading a team, or influencing without authority?") — it shows judgment, and ensures you tell the story that actually answers what they're asking.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Rambling without STAR structure — burying the actual action and result under five minutes of situational context. Equally damaging: bad-mouthing a former manager, team, or employer when describing a conflict or failure — it reads as a lack of professionalism regardless of who was actually at fault.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "Which two parts of a STAR answer do most candidates under-invest in, even though they matter most?",
                    "Action and Result are what actually differentiate a candidate — what YOU specifically did, and the measurable outcome. Most candidates instead over-invest in Situation, spending too long on setup before ever getting there.",
                    [
                        new QuizOptionSeed("Situation and Task", false),
                        new QuizOptionSeed("Action and Result", true),
                        new QuizOptionSeed("Task and Situation", false),
                        new QuizOptionSeed("All four parts deserve exactly equal time", false),
                    ]),
                new QuizQuestionSeed(
                    "A behavioral question is ambiguous — it could mean two different things. What should you do?",
                    "Ask a brief clarifying question before diving into an answer. It shows judgment and ensures the story you tell actually addresses what the interviewer meant, instead of guessing and possibly answering the wrong question entirely.",
                    [
                        new QuizOptionSeed("Guess which interpretation is more likely and answer immediately", false),
                        new QuizOptionSeed("Ask a brief clarifying question before answering", true),
                        new QuizOptionSeed("Answer both interpretations fully, one after another", false),
                        new QuizOptionSeed("Tell the interviewer the question is poorly worded", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Pramp: practice behavioral + technical interviews live", "https://pramp.com", LinkType.FurtherReading),
                new ReferenceLinkSeed("STAR method overview", "https://www.themuse.com/advice/star-interview-method", LinkType.FurtherReading),
            ]);

        var lesson1Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson1.Slug,
        [
            "Write out 2 STAR stories in full, timing yourself to stay under 90 seconds each",
            "Identify which of your prepared stories could flexibly answer 3+ different question categories",
            "Practice asking a clarifying question out loud before answering an ambiguous prompt",
        ]);

        var lesson2 = BuildLesson(
            slug: "effective-technical-communication-feedback",
            title: "Effective Technical Communication & Giving/Receiving Feedback",
            summary: "Writing PR descriptions and status updates people actually read, and handling feedback without getting defensive.",
            estimatedMinutes: 30,
            objectives:
            [
                "Write a status update that leads with the outcome, not the chronology",
                "Give feedback that's specific and actionable instead of vague",
                "Receive critical feedback without becoming defensive, even when it stings",
                "Explain a technical decision to a non-technical stakeholder without jargon",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    Technical communication is a skill, not an afterthought — code that works but is never understood by anyone else creates exactly the same organizational risk as code that doesn't work.

                    **Status updates and PR descriptions** should lead with the outcome or the "why," not a chronological play-by-play — a reader should understand the point in the first sentence, not the fifth paragraph.

                    **Giving feedback** well means being specific and behavior-focused ("this function is 200 lines and does three unrelated things" beats "this code is messy"), and pairing a criticism with a concrete suggestion.

                    **Receiving feedback** well means resisting the urge to immediately explain/defend, actually listening to the full point first, and treating even harsh feedback as information about the work, not a verdict on your worth as an engineer.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    A good status update is like a newspaper headline followed by the story — the reader knows the outcome immediately and can stop reading if that's all they needed. A chronological update is like a mystery novel that saves the reveal for the last page — technically complete, but it wastes a busy reader's time getting there.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **A well-structured PR description**

                    - What changed, and why (the motivation, not just the diff)
                    - How to test it / what was tested
                    - Anything a reviewer should pay special attention to (a risky edge case, a deliberate trade-off)

                    **Feedback checklist**

                    - Specific, not vague ("this variable name doesn't convey its purpose" beats "naming is bad")
                    - Behavior/artifact-focused, not personal ("this function" not "you always")
                    - Paired with a concrete suggestion, not just a complaint
                    """, 3),
                Block(BlockType.CodeSnippet, "Leading with the Outcome", BodyFormat.PlainText, """
                    // Buries the outcome at the end — a busy reader has to
                    // read the whole thing to find out what actually matters.
                    "Today I looked into the slow checkout endpoint, checked the
                    logs, found a missing index, added it, and now it's fast."

                    // Leads with the outcome; details are still there for
                    // anyone who wants them, but they're not required reading.
                    "Fixed: checkout endpoint was 10x slower than expected due
                    to a missing database index — added it, latency dropped
                    from 800ms to 60ms. Root cause: [details below]."
                    """, 4),
                Block(BlockType.Diagram, "Receiving Feedback Without Getting Defensive", BodyFormat.StructuredSteps, """
                    [{"label":"Listen fully","note":"don't interrupt to explain"},{"label":"Ask a clarifying question","note":"if genuinely unclear"},{"label":"Acknowledge the valid part","note":"even if not all of it lands"},{"label":"Respond or act","note":"after you've actually processed it"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Lead every status update, PR description, and incident summary with the outcome or the ask — "what do you need the reader to know or do" — then support it with detail below for whoever wants to go deeper.

                    When receiving critical feedback, let the other person finish their full point before responding, even when you're confident you already know what they're going to say — interrupting to explain/defend is the single most common way feedback conversations go badly.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    "Tell me about a time you received difficult feedback" is asking whether you can take critique without getting defensive — a strong answer names the specific feedback, describes what you did differently afterward, and doesn't spend the story explaining why the feedback was actually wrong.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Responding to feedback with an immediate, detailed defense before genuinely processing it — even when the defense is technically correct, this reads as defensiveness and shuts down the conversation instead of engaging with it.

                    Also common: writing PR descriptions and status updates in strict chronological order ("first I did X, then Y, then Z") instead of leading with the outcome — busy reviewers and stakeholders have to dig for the one sentence that actually matters to them.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "What's the biggest structural problem with a chronological ('first I did X, then Y...') status update?",
                    "It buries the outcome — the part a busy reader actually needs — at the end, forcing them to read the whole thing just to find out what matters. Leading with the outcome respects the reader's time.",
                    [
                        new QuizOptionSeed("It's too short to be useful", false),
                        new QuizOptionSeed("It buries the outcome the reader actually needs at the end", true),
                        new QuizOptionSeed("It contains too much technical detail", false),
                        new QuizOptionSeed("Chronological order is actually the best structure for updates", false),
                    ]),
                new QuizQuestionSeed(
                    "What's the most common way a feedback conversation goes badly?",
                    "Interrupting to explain or defend before actually hearing the full point — even a technically correct defense reads as defensiveness in the moment and shuts down real engagement with the feedback.",
                    [
                        new QuizOptionSeed("Taking too long to respond", false),
                        new QuizOptionSeed("Interrupting to explain or defend before hearing the full point", true),
                        new QuizOptionSeed("Asking a clarifying question", false),
                        new QuizOptionSeed("Acknowledging the valid part of the feedback", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("How to write a good PR description", "https://github.blog/2015-01-21-how-to-write-the-perfect-pull-request/", LinkType.FurtherReading),
                new ReferenceLinkSeed("Radical Candor: giving feedback that helps", "https://www.radicalcandor.com/our-approach/", LinkType.FurtherReading),
            ],
            prerequisites: [lesson1]);

        var lesson2Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson2.Slug,
        [
            "Rewrite one of your own recent status updates to lead with the outcome",
            "Practice giving specific, behavior-focused feedback on a piece of code (yours or someone else's)",
            "Recall a piece of critical feedback you received and describe what you did differently afterward",
        ]);

        var module = BuildModule(topicId, "interview-readiness-fundamentals", "Interview & Communication Readiness",
            "Structuring behavioral answers, understanding the interview loop, and communicating like a senior engineer.",
            60, [lesson1, lesson2]);

        return (module, [lesson1Checklist, lesson2Checklist]);
    }

    // ============================== Shared builders ==============================

    private static Module BuildModule(
        int topicId, string slug, string title, string description, int estimatedMinutes,
        List<Lesson> lessons, CapstoneProject? capstone = null)
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
            Lessons = lessons.Select((lesson, i) => { lesson.SortOrder = i + 1; return lesson; }).ToList(),
            Capstone = capstone,
        };
    }

    private static Lesson BuildLesson(
        string slug, string title, string summary, int estimatedMinutes,
        List<string> objectives, List<LessonContentBlock> blocks,
        List<QuizQuestionSeed>? quiz = null,
        List<ReferenceLinkSeed>? referenceLinks = null,
        List<Lesson>? prerequisites = null)
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
            Objectives = objectives
                .Select((text, i) => new LessonObjective { Text = text, SortOrder = i + 1 })
                .ToList(),
            QuizQuestions = (quiz ?? [])
                .Select((q, i) => new QuizQuestion
                {
                    QuestionText = q.Question,
                    Explanation = q.Explanation,
                    SortOrder = i + 1,
                    Options = q.Options
                        .Select((o, j) => new QuizOption { Text = o.Text, IsCorrect = o.IsCorrect, SortOrder = j + 1 })
                        .ToList(),
                })
                .ToList(),
            ReferenceLinks = (referenceLinks ?? [])
                .Select((r, i) => new LessonReferenceLink { Title = r.Title, Url = r.Url, LinkType = r.Type, SortOrder = i + 1 })
                .ToList(),
            Prerequisites = (prerequisites ?? [])
                .Select(p => new LessonPrerequisite { PrerequisiteLesson = p })
                .ToList(),
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
