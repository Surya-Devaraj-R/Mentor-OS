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
            BuildCSharpAsyncAndTestingModule(topicIdBySlug["csharp"]),
            BuildDotNetModule(topicIdBySlug["dotnet"]),
            BuildDotNetProductionReadinessModule(topicIdBySlug["dotnet"]),
            BuildDsaModule(topicIdBySlug["dsa"]),
            BuildDsaGraphsModule(topicIdBySlug["dsa"]),
            BuildSystemDesignModule(topicIdBySlug["system-design"]),
            BuildApiGatewayAndCdnModule(topicIdBySlug["system-design"]),
            BuildSqlModule(topicIdBySlug["sql"]),
            BuildSqlAdvancedModule(topicIdBySlug["sql"]),
            BuildCloudModule(topicIdBySlug["cloud"]),
            BuildCloudObservabilityModule(topicIdBySlug["cloud"]),
            BuildGitModule(topicIdBySlug["git"]),
            BuildGitInternalsModule(topicIdBySlug["git"]),
            BuildDevOpsModule(topicIdBySlug["devops"]),
            BuildDevOpsReliabilityModule(topicIdBySlug["devops"]),
            BuildArchitectureModule(topicIdBySlug["architecture"]),
            BuildEventDrivenArchitectureModule(topicIdBySlug["architecture"]),
            BuildSoftSkillsModule(topicIdBySlug["soft-skills"]),
            BuildLeadershipAndCareerGrowthModule(topicIdBySlug["soft-skills"]),
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

        var lesson3 = BuildLesson(
            slug: "collections-and-generics",
            title: "Collections & Generics: Choosing the Right Data Structure",
            summary: "List, Dictionary, and HashSet internals, the IEnumerable/ICollection/IList hierarchy, and writing your own generic constraints.",
            estimatedMinutes: 40,
            objectives:
            [
                "Choose between List<T>, Dictionary<TKey,TValue>, and HashSet<T> based on the operations you actually need",
                "Explain the difference between IEnumerable<T>, ICollection<T>, and IList<T>, and accept the least restrictive one in a method signature",
                "Write a generic method or class with a type constraint (`where T : ...`)",
                "Explain why Dictionary/HashSet lookups are O(1) on average, and what makes a type a good dictionary key",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    `List<T>` is a dynamically-resizing array — indexed access and appends are fast, but searching for a value (`Contains`) or inserting at the front is `O(n)`.

                    `Dictionary<TKey, TValue>` and `HashSet<T>` are both backed by a hash table: they compute a hash of the key (or value) to jump almost directly to the right bucket, giving average `O(1)` insert, lookup, and delete — dramatically faster than scanning a list, at the cost of losing insertion order and needing a well-behaved `GetHashCode`/`Equals` pair on the key type.

                    Collections are described by an interface hierarchy of increasing capability: `IEnumerable<T>` (just `foreach`, one item at a time, forward-only) → `ICollection<T>` (adds `Count`, `Add`, `Remove`, `Contains`) → `IList<T>` (adds index access, `Insert`, `RemoveAt`). `List<T>` implements all three.

                    **Generics** let a type or method be parameterized over a type (`List<T>`, `Dictionary<TKey, TValue>`) instead of duplicating code per concrete type. A **generic constraint** (`where T : IComparable<T>`, `where T : class`, `where T : new()`) restricts what `T` can be, in exchange for being able to call more members on it inside the generic code.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    A `List<T>` is like a numbered coat check rack — great for "give me item #12," but finding "the coat that belongs to Sam" means checking tickets one by one.

                    A `Dictionary<TKey, TValue>` is like a library's card catalog — you look up a subject and jump straight to the right drawer, instead of walking every shelf.

                    A `HashSet<T>` is like a bouncer's guest list — its only job is answering "is this name on the list?" as fast as possible, with no concept of order or duplicates.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Typical complexity (average case)**

                    - `List<T>`: `Add` O(1) amortized, `Insert(0, x)` O(n), `Contains` O(n), index access O(1)
                    - `Dictionary<TKey,TValue>`: `Add`/lookup/`Remove` O(1), no guaranteed order
                    - `HashSet<T>`: `Add`/`Contains`/`Remove` O(1), no duplicates, no guaranteed order

                    **Interface hierarchy, least to most capable**

                    - `IEnumerable<T>` — read-only, forward-only iteration
                    - `ICollection<T>` — + `Count`, `Add`, `Remove`, `Contains`
                    - `IList<T>` — + index access, `Insert`, `RemoveAt`

                    **Common generic constraints**

                    - `where T : class` / `where T : struct` — reference-type / value-type only
                    - `where T : new()` — must have a public parameterless constructor
                    - `where T : IComparable<T>` — must support ordering comparisons
                    """, 3),
                Block(BlockType.CodeSnippet, "A Generic Repository and a Constrained Generic Method", BodyFormat.PlainText, """
                    public interface IEntity
                    {
                        int Id { get; }
                    }

                    public class Repository<T> where T : class, IEntity
                    {
                        private readonly Dictionary<int, T> _itemsById = new();

                        public void Add(T item) => _itemsById[item.Id] = item;

                        public T? Find(int id) =>
                            _itemsById.TryGetValue(id, out var item) ? item : null;
                    }

                    // Constrained to IComparable<T> so CompareTo is callable inside the method.
                    public static T Max<T>(T first, T second) where T : IComparable<T> =>
                        first.CompareTo(second) >= 0 ? first : second;
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "Collection Interface Hierarchy", BodyFormat.AsciiArt, """
                    IEnumerable<T>        foreach only, forward-only, read-only
                          |
                    ICollection<T>        + Count, Add, Remove, Contains
                          |
                    IList<T>              + index access, Insert, RemoveAt
                          |
                       List<T>            concrete implementation

                    A method that only needs to iterate should accept
                    IEnumerable<T> — the least capable interface it needs.
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Accept the least powerful interface your method actually needs: `IEnumerable<T>` if you only iterate, `ICollection<T>` if you also need `Count` or `Add`, and reserve `IList<T>` for when you truly need index access. This mirrors Interface Segregation from the previous lesson — it maximizes what callers can pass in (an array, a `HashSet<T>`, a LINQ query result) without forcing them to materialize a concrete `List<T>` first.

                    Reach for `HashSet<T>` instead of `List<T>` the moment your only question about a collection is "have I seen this before?" — it turns an `O(n)` scan into an `O(1)` average lookup.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    When asked "List vs. Dictionary vs. HashSet, when would you use each?", answer with the operation you need, not the data itself: ordered sequence you'll index into → `List<T>`; key-to-value lookup → `Dictionary<TKey,TValue>`; pure membership/uniqueness check → `HashSet<T>`. Naming the operation first is what separates understanding from memorized definitions.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Calling `.Contains()` on a `List<T>` inside a hot loop instead of using a `HashSet<T>` — each call is an `O(n)` scan, so doing it `n` times turns an algorithm into `O(n²)` without anyone noticing until it's slow in production.

                    Also common: using a mutable class as a dictionary key (or `HashSet<T>` element) and then mutating a field that `GetHashCode`/`Equals` depends on — the item silently becomes unfindable, because it now hashes to a different bucket than the one it was stored in.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "You need to check membership ('have I seen this value before?') as fast as possible, and don't care about order or duplicates. What should you use?",
                    "HashSet<T> is backed by a hash table, giving average O(1) Contains checks, versus List<T>'s O(n) linear scan — exactly the right fit when order and duplicates don't matter and only membership does.",
                    [
                        new QuizOptionSeed("List<T>", false),
                        new QuizOptionSeed("HashSet<T>", true),
                        new QuizOptionSeed("An array, sorted after every insert", false),
                        new QuizOptionSeed("Queue<T>", false),
                    ]),
                new QuizQuestionSeed(
                    "Why accept `IEnumerable<T>` instead of `List<T>` as a parameter when a method only ever iterates the items once?",
                    "Accepting the least restrictive interface your method needs maximizes what callers can pass in — an array, a HashSet<T>, a LINQ query result — without forcing them to materialize a List<T> they may not already have.",
                    [
                        new QuizOptionSeed("It lets the method accept any enumerable source, not just a concrete List<T>", true),
                        new QuizOptionSeed("IEnumerable<T> iterates faster than List<T>", false),
                        new QuizOptionSeed("It allows the method to add new items to the caller's collection", false),
                        new QuizOptionSeed("It guarantees the sequence is sorted", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Collections (C#)", "https://learn.microsoft.com/en-us/dotnet/standard/collections/", LinkType.OfficialDocs),
                new ReferenceLinkSeed("Generic classes and methods (C# Programming Guide)", "https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/generics", LinkType.OfficialDocs),
            ],
            prerequisites: [lesson1, lesson2]);

        var lesson3Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson3.Slug,
        [
            "Find one List<T>.Contains() call in your own code that's checked in a loop, and replace it with a HashSet<T>",
            "Write a generic method with a `where T : IComparable<T>` constraint from scratch",
            "Explain out loud why accepting IEnumerable<T> instead of List<T> in a method signature is usually the better default",
        ]);

        var lesson4 = BuildLesson(
            slug: "linq-fundamentals",
            title: "LINQ Fundamentals: Deferred Execution & Core Operators",
            summary: "Method vs. query syntax, deferred execution, and the LINQ operators you'll reach for constantly.",
            estimatedMinutes: 35,
            objectives:
            [
                "Explain the difference between method syntax and query syntax, and when to reach for each",
                "Explain deferred execution, and why enumerating the same query twice can produce different results",
                "Use Where, Select, OrderBy, GroupBy, and aggregate operators (Sum/Count/Any/All) correctly",
                "Force immediate execution with ToList()/ToArray() when materializing a snapshot is actually what you need",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    **LINQ** (Language-Integrated Query) is a set of extension methods on `IEnumerable<T>` (and `IQueryable<T>`) that let you filter, project, sort, and aggregate any sequence with a consistent, composable API — the same syntax works over in-memory lists, arrays, XML, and EF Core database queries.

                    LINQ has two equivalent syntaxes: **method syntax** (`items.Where(x => x.IsActive).OrderBy(x => x.Name)`) and **query syntax** (`from x in items where x.IsActive orderby x.Name select x`). They compile to the same calls; method syntax is more common in day-to-day C# and covers a few operators query syntax can't express directly.

                    Most LINQ operators use **deferred execution**: calling `.Where()` or `.Select()` doesn't run anything — it builds up a description of the query. The query only actually runs when something enumerates it: a `foreach`, or a call to `.ToList()`, `.ToArray()`, `.First()`, `.Count()`, etc. This means enumerating the *same* unmaterialized query twice re-runs it twice, against whatever the source looks like *at that moment*.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    A deferred LINQ query is like a recipe card, not a cooked meal — writing `Where(x => x.IsFresh)` is jotting down an instruction, not doing any cooking. Nothing happens until someone actually follows the recipe (`foreach`, `.ToList()`), and if the pantry's contents changed since the recipe was written, that's what gets cooked — the recipe card itself never went stale.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Filtering & projecting**

                    - `.Where(x => ...)` — keep matching elements
                    - `.Select(x => ...)` — project each element into something new
                    - `.OrderBy(x => ...)` / `.OrderByDescending(x => ...)` — sort
                    - `.GroupBy(x => x.Key)` — bucket elements by a key

                    **Element & aggregate operators**

                    - `.First()` / `.FirstOrDefault()` — throws / returns default if empty
                    - `.Single()` / `.SingleOrDefault()` — throws if more than one match
                    - `.Any(predicate)` / `.All(predicate)` — short-circuiting boolean checks
                    - `.Count()`, `.Sum()`, `.Average()`, `.Min()`, `.Max()`
                    - `.Distinct()`, `.Take(n)`, `.Skip(n)`

                    **Forcing immediate execution**: `.ToList()`, `.ToArray()`, `.ToDictionary()`
                    """, 3),
                Block(BlockType.CodeSnippet, "Deferred Execution in Action", BodyFormat.PlainText, """
                    var numbers = new List<int> { 1, 2, 3 };

                    // Nothing has executed yet — this just describes a query.
                    var evens = numbers.Where(n => n % 2 == 0);

                    numbers.Add(4);

                    // The query runs NOW, against the CURRENT state of numbers —
                    // so 4 is included even though it was added after Where() was called.
                    foreach (var n in evens)
                    {
                        Console.WriteLine(n); // prints 2, then 4
                    }

                    // Method syntax, chained and composable:
                    var topActiveNames = people
                        .Where(p => p.IsActive)
                        .OrderByDescending(p => p.Score)
                        .Select(p => p.Name)
                        .Take(3)
                        .ToList(); // materialized here — a real List<string>, not a query
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "Query Built vs. Query Executed", BodyFormat.StructuredSteps, """
                    [{"label":"Where(...)/Select(...) called","note":"builds a query description, runs nothing"},{"label":"Query object exists","note":"IEnumerable<T>, still not executed"},{"label":"foreach / ToList() / First()","note":"execution actually happens here"},{"label":"Source re-read at THIS moment","note":"reflects any changes made since the query was built"},{"label":"Results produced"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Call `.ToList()` (or `.ToArray()`) once when you need to enumerate the same results more than once, or want a stable snapshot that won't change if the underlying source is mutated afterward — don't re-enumerate the same deferred query repeatedly.

                    Keep LINQ chains readable: prefer a few named intermediate variables over one giant chained expression when a query does more than 3–4 operations — the next reader (often you, in six months) will thank you.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If given a "what does this print?" snippet involving LINQ, check for deferred execution first: was the query enumerated more than once, or was the source mutated between building the query and enumerating it? That's the detail most such questions are actually testing.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Enumerating the same deferred LINQ query multiple times without realizing each enumeration re-runs it from scratch — against an EF Core `IQueryable<T>`, this silently means a second round trip to the database for what looks like "just looping over the results again."

                    Also common: assuming a LINQ query is a snapshot taken at the moment `.Where()`/`.Select()` was called — it isn't, unless you explicitly materialize it with `.ToList()`.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "`var query = numbers.Where(n => n > 5); numbers.Add(10); foreach (var n in query) { ... }` — what does the foreach see?",
                    "Where() only builds a query description; nothing executes until the foreach enumerates it. By then numbers already contains 10, so the query re-reads the current state of the list and includes it.",
                    [
                        new QuizOptionSeed("Only the numbers greater than 5 that existed before Add(10) was called", false),
                        new QuizOptionSeed("10 as well, because the query re-evaluates the source at enumeration time, not at Where() time", true),
                        new QuizOptionSeed("An InvalidOperationException, because numbers was modified after Where() was called", false),
                        new QuizOptionSeed("An empty sequence, because Where() takes a snapshot immediately", false),
                    ]),
                new QuizQuestionSeed(
                    "Why can enumerating the same LINQ query twice against an EF Core IQueryable<T> be a performance problem?",
                    "Deferred execution means each enumeration re-runs the full query against its data source. Two foreach loops over the same un-materialized IQueryable<T> trigger two separate round trips to the database — calling .ToList() once avoids the duplicate work.",
                    [
                        new QuizOptionSeed("LINQ automatically caches results after the first enumeration", false),
                        new QuizOptionSeed("Each enumeration re-executes the query against the underlying data source, potentially hitting the database again", true),
                        new QuizOptionSeed("IQueryable<T> can only be enumerated exactly once, ever", false),
                        new QuizOptionSeed("Deferred execution only applies to in-memory collections, never to databases", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Introduction to LINQ queries (C#)", "https://learn.microsoft.com/en-us/dotnet/csharp/linq/get-started/introduction-to-linq-queries", LinkType.OfficialDocs),
                new ReferenceLinkSeed("Query syntax and method syntax in LINQ", "https://learn.microsoft.com/en-us/dotnet/csharp/linq/get-started/query-syntax-and-method-syntax-in-linq", LinkType.OfficialDocs),
            ],
            prerequisites: [lesson3]);

        var lesson4Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson4.Slug,
        [
            "Write a LINQ method-syntax chain (Where/OrderBy/Select/Take) against your own data and materialize it with ToList()",
            "Demonstrate deferred execution to yourself: build a query, mutate the source, then enumerate it and observe the result",
            "Find one place in your own code enumerating the same LINQ query twice, and fix it with a single ToList()",
        ]);

        var module = BuildModule(topicId, "csharp-fundamentals", "C# Fundamentals",
            "Language fundamentals every C# developer needs before going deeper into collections, LINQ, and async.",
            205, [lesson1, lesson2, lesson3, lesson4]);

        return (module, [lesson1Checklist, lesson2Checklist, lesson3Checklist, lesson4Checklist]);
    }

    private static (Module, List<ChecklistSeed>) BuildCSharpAsyncAndTestingModule(int topicId)
    {
        var lesson1 = BuildLesson(
            slug: "async-await-task-based-concurrency",
            title: "Async/Await & Task-Based Concurrency Deep Dive",
            summary: "Task vs. Task<T>, what async/await actually compiles down to, ConfigureAwait, classic deadlock pitfalls, and cooperative cancellation.",
            estimatedMinutes: 40,
            objectives:
            [
                "Explain the difference between Task and Task<T>, and what the compiler generates for an `async` method",
                "Identify why blocking on async code with `.Result` or `.Wait()` can deadlock under a synchronization context",
                "Use `ConfigureAwait(false)` correctly, and explain why ASP.NET Core code rarely needs it",
                "Wire a `CancellationToken` through an async call chain so an operation can be cancelled cooperatively",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    A `Task` represents an asynchronous operation that completes with no result; `Task<T>` represents one that completes with a value of type `T`. Both can be awaited, run concurrently, and inspected for completion/failure/cancellation state.

                    `async`/`await` is compiler sugar over a **state machine**: marking a method `async` doesn't run it on a new thread by itself. The compiler rewrites the method into a class implementing `IAsyncStateMachine`, splitting it into pieces at every `await`. For I/O-bound work (HTTP calls, file/database reads), no thread is occupied at all while waiting — the underlying OS/driver signals completion, and a thread-pool thread only picks up the *continuation* after that. `Task.Run(...)` is different: it explicitly queues CPU-bound work onto the thread pool.

                    By default, awaiting a `Task` captures the current `SynchronizationContext` (if one exists, e.g. in WPF/WinForms/classic ASP.NET) so the continuation resumes on the original thread. ASP.NET Core has **no** synchronization context, which is one reason the classic `.Result` deadlock is rare there — but `ConfigureAwait(false)` is still good practice in library code that might run under a context.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    `await` is like placing an order at a restaurant counter and being handed a buzzer instead of standing there waiting — you go sit down (the thread is freed to do other work) and the buzzer (the continuation) goes off when the food's ready.

                    Calling `.Result` or `.Wait()` synchronously is walking back up to the counter and physically blocking it with your body until your order arrives — and if the kitchen needs to hand the food to *you specifically, standing at the counter* to finish the order (the captured synchronization context), but you're the one blocking the counter, nobody can ever complete the handoff. That's the deadlock.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Task basics**

                    - `Task` — async operation, no return value
                    - `Task<T>` — async operation returning `T`
                    - `Task.Run(() => ...)` — offload CPU-bound work to the thread pool
                    - `Task.WhenAll(t1, t2)` — await multiple tasks concurrently, wait for all
                    - `Task.WhenAny(t1, t2)` — resolves as soon as the first one completes

                    **Awaiting correctly**

                    - `await someTask;` — the safe, non-blocking way to get a result
                    - `someTask.Result` / `someTask.Wait()` — synchronous blocking, deadlock risk, avoid
                    - `.ConfigureAwait(false)` — don't resume on the captured context; continue on any thread-pool thread

                    **Cancellation**

                    - `CancellationTokenSource cts = new();`
                    - `cts.Token` — pass this into async APIs that accept a `CancellationToken`
                    - `cts.Cancel();` — requests cancellation cooperatively (doesn't force-kill anything)
                    - `token.ThrowIfCancellationRequested();` — check inside loops in your own async code
                    """, 3),
                Block(BlockType.CodeSnippet, "Concurrent Awaits, ConfigureAwait, and Cancellation", BodyFormat.PlainText, """
                    public async Task<string> GetUserProfileAsync(int userId, CancellationToken cancellationToken)
                    {
                        // Start both independent calls before awaiting either one, so they
                        // run concurrently instead of being serialized for no reason.
                        Task<User> userTask = _userRepository.GetByIdAsync(userId, cancellationToken);
                        Task<Preferences> prefsTask = _preferencesRepository.GetForUserAsync(userId, cancellationToken);

                        await Task.WhenAll(userTask, prefsTask);

                        User user = userTask.Result;        // safe here: both tasks are already complete
                        Preferences prefs = prefsTask.Result;

                        return $"{user.Name} ({prefs.Theme} theme)";
                    }

                    // Library code (as opposed to ASP.NET Core request-handling code) should
                    // avoid capturing a caller's synchronization context when it doesn't need it:
                    public async Task<byte[]> DownloadReportAsync(string url, CancellationToken cancellationToken)
                    {
                        using var response = await _httpClient
                            .GetAsync(url, cancellationToken)
                            .ConfigureAwait(false);

                        response.EnsureSuccessStatusCode();

                        return await response.Content
                            .ReadAsByteArrayAsync(cancellationToken)
                            .ConfigureAwait(false);
                    }
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "How .Result Deadlocks Under a Synchronization Context", BodyFormat.StructuredSteps, """
                    [{"label":"UI/request thread calls DoWorkAsync().Result","note":"blocks that thread synchronously, waiting for completion"},{"label":"Inside DoWorkAsync, an inner await runs","note":"captures the current SynchronizationContext to resume on later"},{"label":"Inner task finishes on a thread-pool thread","note":"tries to post the continuation back onto the captured context"},{"label":"The captured context's thread is still blocked on .Result","note":"the continuation can never run -> permanent deadlock"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Go "async all the way": once a call chain starts using `await`, keep it `async Task`/`async Task<T>` up through every caller instead of blocking on it partway up with `.Result`/`.Wait()`.

                    Accept a `CancellationToken` parameter on any async method that does real work, and pass it into every awaited call inside it — don't just accept it and ignore it. Avoid `async void` entirely except for UI event handlers, since exceptions thrown from an `async void` method can't be caught by the caller and will crash the process.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    A very common interview question is some variant of "why does this code hang?" showing a synchronous call to `.Result` or `.Wait()` on a task from a context that has a `SynchronizationContext` (classic ASP.NET, WPF, WinForms). Walk through it out loud: the blocked thread is exactly the thread the awaited continuation needs to resume on. Also be ready to state plainly that `async` does not imply "runs on a new thread" — for I/O-bound work, no thread is dedicated at all while waiting.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Mixing synchronous blocking with asynchronous code — calling `.Result`/`.Wait()` on a `Task` instead of awaiting it — is the single most common way to introduce a deadlock or thread-pool starvation into otherwise-correct async code.

                    A close second: accepting a `CancellationToken` parameter but never actually passing it into the async calls made inside the method, silently making cancellation a no-op.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "Why can calling `task.Result` from a WPF UI thread or classic ASP.NET request thread deadlock?",
                    "The blocked thread is the exact same thread the awaited continuation needs to resume on (its captured SynchronizationContext). Since that thread is stuck synchronously waiting on .Result, the continuation can never run, and .Result can never return.",
                    [
                        new QuizOptionSeed("The Task becomes permanently faulted and never completes", false),
                        new QuizOptionSeed("The blocked thread is the same thread the continuation needs in order to resume, so neither can proceed", true),
                        new QuizOptionSeed("A Task can only be awaited once, and .Result counts as a second await", false),
                        new QuizOptionSeed("The garbage collector cannot reach thread-pool tasks and stalls", false),
                    ]),
                new QuizQuestionSeed(
                    "What does calling `.ConfigureAwait(false)` on an awaited task actually do?",
                    "It tells the awaiter not to try to resume the continuation on the original captured synchronization/execution context, allowing it to continue on any available thread-pool thread instead. It does not move work to a background thread by itself, nor does it change cancellation or timeout behavior.",
                    [
                        new QuizOptionSeed("It forces the awaited method to run on a background thread", false),
                        new QuizOptionSeed("It skips resuming on the originally captured context, letting the continuation run on any thread-pool thread", true),
                        new QuizOptionSeed("It cancels the task if it doesn't complete within a default timeout", false),
                        new QuizOptionSeed("It makes the async method execute synchronously", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Asynchronous programming with async and await", "https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/", LinkType.OfficialDocs),
                new ReferenceLinkSeed("Cancellation in managed threads", "https://learn.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads", LinkType.OfficialDocs),
            ]);

        var lesson1Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson1.Slug,
        [
            "Rewrite one blocking `.Result`/`.Wait()` call in your own code so it's awaited all the way up the call stack",
            "Add a `CancellationToken` parameter to one of your own async methods and pass it through to every awaited call inside it",
            "Explain out loud why `await Task.Delay(1000)` does not block a thread for one second",
        ]);

        var lesson2 = BuildLesson(
            slug: "unit-testing-fundamentals-xunit",
            title: "Unit Testing Fundamentals with xUnit",
            summary: "Writing testable code, the Arrange-Act-Assert pattern, and isolating dependencies with mocks using xUnit and Moq.",
            estimatedMinutes: 35,
            objectives:
            [
                "Structure a unit test using the Arrange-Act-Assert pattern",
                "Explain the difference between a mock and a stub, and when to reach for each",
                "Write a testable class by depending on interfaces instead of concrete implementations (constructor injection)",
                "Use `[Theory]`/`[InlineData]` to cover multiple input cases without duplicating near-identical test methods",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    A **unit test** exercises a single unit of behavior — typically one method or one class — in isolation from its real dependencies (databases, HTTP calls, the file system, the clock). In xUnit, a test method marked `[Fact]` runs exactly once; a `[Theory]` with one or more `[InlineData(...)]` attributes runs the same method body once per data row.

                    Good unit tests are commonly summarized by **FIRST**: **F**ast, **I**solated (don't depend on other tests or shared state), **R**epeatable (same result every run, any environment), **S**elf-validating (pass/fail, no manual inspection), **T**imely (written close to the code, not months later).

                    xUnit test methods can themselves be `async Task`, letting you `await` asynchronous code under test directly — no need to block on it with `.Result`/`.Wait()` (see the previous lesson for exactly why that would be a mistake even inside a test).
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    A unit test is like bench-testing a single car engine in isolation before it's ever bolted into a chassis — you feed it known inputs (fuel, electrical signal) and check its outputs directly, without needing a full car, a road, or a driver.

                    An integration test, by contrast, is taking the whole assembled car for a test drive — realistic, but it can't tell you *which specific part* failed if something goes wrong.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **xUnit attributes**

                    - `[Fact]` — a test that always runs the same way, no parameters
                    - `[Theory]` + `[InlineData(...)]` — one test method, many input rows
                    - `Assert.Equal(expected, actual)` / `Assert.True(condition)` / `Assert.Throws<T>(() => ...)`

                    **Moq basics**

                    - `var mock = new Mock<IThing>();` — create a fake implementation of an interface
                    - `mock.Setup(m => m.Method(args)).Returns(value);` — stub a return value
                    - `mock.Object` — the fake instance to inject into the class under test
                    - `mock.Verify(m => m.Method(args), Times.Once);` — assert a call actually happened

                    **Test doubles, briefly**

                    - **Stub** — supplies canned answers, no verification of how it was called
                    - **Mock** — a stub that also lets you verify specific interactions occurred
                    - **Fake** — a lightweight working implementation (e.g. an in-memory repository)
                    """, 3),
                Block(BlockType.CodeSnippet, "Testable Design + an xUnit/Moq Test", BodyFormat.PlainText, """
                    public interface IShippingCostCalculator
                    {
                        decimal CalculateCost(decimal orderTotal, string destinationCountry);
                    }

                    public class OrderService
                    {
                        private readonly IShippingCostCalculator _shippingCalculator;

                        public OrderService(IShippingCostCalculator shippingCalculator)
                        {
                            _shippingCalculator = shippingCalculator;
                        }

                        public decimal GetOrderTotalWithShipping(decimal orderTotal, string country)
                            => orderTotal + _shippingCalculator.CalculateCost(orderTotal, country);
                    }

                    public class OrderServiceTests
                    {
                        [Fact]
                        public void GetOrderTotalWithShipping_AddsCalculatedShippingCost()
                        {
                            // Arrange
                            var mockCalculator = new Mock<IShippingCostCalculator>();
                            mockCalculator
                                .Setup(c => c.CalculateCost(100m, "US"))
                                .Returns(9.99m);
                            var sut = new OrderService(mockCalculator.Object);

                            // Act
                            var total = sut.GetOrderTotalWithShipping(100m, "US");

                            // Assert
                            Assert.Equal(109.99m, total);
                            mockCalculator.Verify(c => c.CalculateCost(100m, "US"), Times.Once);
                        }

                        [Theory]
                        [InlineData(0, "US", 5.00)]
                        [InlineData(50, "CA", 12.50)]
                        public void CalculateCost_ReturnsExpectedShipping(decimal orderTotal, string country, decimal expected)
                        {
                            var calculator = new StandardShippingCostCalculator();

                            var cost = calculator.CalculateCost(orderTotal, country);

                            Assert.Equal(expected, cost);
                        }
                    }
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "Arrange, Act, Assert", BodyFormat.StructuredSteps, """
                    [{"label":"Arrange","note":"build the object under test and its fakes/mocks; set up input data"},{"label":"Act","note":"make the single call to the method or behavior actually being tested"},{"label":"Assert","note":"verify the outcome: a return value, a thrown exception, or a recorded mock interaction"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Name tests so a failure tells you what broke without opening the test body: `MethodName_Scenario_ExpectedResult` (e.g. `GetOrderTotalWithShipping_AddsCalculatedShippingCost`). Keep each test independent — no test should rely on another test having run first or on shared mutable state.

                    Test observable behavior through a class's public API, not its private implementation details — that keeps tests from breaking every time you refactor internals without changing behavior.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    "How would you unit test this class?" is a near-universal interview question once a class has any dependency. The expected answer is: extract the dependency behind an interface, inject it through the constructor, and substitute a mock/stub in the test. Be ready to explain, precisely, the difference between a mock (verifies interactions) and a stub (just supplies data) — interviewers often ask this as a quick follow-up.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Testing private implementation details (e.g. via reflection, or by exposing internals just for the test) instead of the class's public, observable behavior — this makes tests brittle and breaks them on harmless refactors.

                    Also common: letting tests share mutable static state or run order dependencies, so tests pass individually but fail when run together or in a different order; and over-mocking simple, side-effect-free value objects that don't need a test double at all.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "In the Arrange-Act-Assert pattern, what should the 'Act' section of a test contain?",
                    "'Act' should be exactly one call to the method or behavior actually under test — setup belongs in 'Arrange' and verification belongs in 'Assert'. Keeping 'Act' to a single call makes it obvious what a failing test is actually exercising.",
                    [
                        new QuizOptionSeed("All of the test's assertions checking the outcome", false),
                        new QuizOptionSeed("Exactly one call to the method or behavior being tested", true),
                        new QuizOptionSeed("The setup of mock objects and test input data", false),
                        new QuizOptionSeed("Disposal/cleanup of any resources the test used", false),
                    ]),
                new QuizQuestionSeed(
                    "What is the key difference between a mock and a stub in unit-testing terminology?",
                    "A mock is a test double you can verify interactions on (e.g. 'was this method called with these arguments, exactly once?'), while a stub simply returns canned data and is never checked for how or whether it was called.",
                    [
                        new QuizOptionSeed("Stubs are only valid in integration tests, mocks only in unit tests", false),
                        new QuizOptionSeed("A mock lets you verify specific interactions occurred; a stub just supplies canned data with no verification", true),
                        new QuizOptionSeed("There is no real difference; the two terms are fully interchangeable", false),
                        new QuizOptionSeed("A mock can only be created with Moq, a stub only with NSubstitute", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Unit testing C# with xUnit and .NET Core", "https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test", LinkType.OfficialDocs),
                new ReferenceLinkSeed("Moq Quickstart", "https://github.com/devlooped/moq/wiki/Quickstart", LinkType.FurtherReading),
            ],
            prerequisites: [lesson1]);

        var lesson2Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson2.Slug,
        [
            "Write one xUnit [Fact] test for a method in your own code following Arrange-Act-Assert",
            "Refactor one class to depend on an interface instead of a concrete type so it can be unit tested with a mock",
            "Convert two near-duplicate test methods into a single [Theory] with [InlineData] cases",
        ]);

        var module = BuildModule(topicId, "csharp-async-and-testing", "Async Programming & Unit Testing",
            "Task-based asynchronous programming and the testing discipline needed to write and verify reliable, concurrent C# code.",
            75, [lesson1, lesson2], sortOrder: 2);

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

        var lesson3 = BuildLesson(
            slug: "custom-middleware-and-di-patterns",
            title: "Custom Middleware & Dependency Injection Patterns",
            summary: "Writing your own middleware components and injecting dependencies correctly across constructors, factories, and HttpContext.",
            estimatedMinutes: 40,
            objectives:
            [
                "Write a custom middleware component using both the inline `app.Use()` delegate and the `IMiddleware` interface",
                "Explain why middleware order in `Program.cs` directly determines request/response behavior",
                "Identify why conventional middleware classes are effectively singletons, and where that means dependencies must be injected",
                "Recognize the service locator anti-pattern and explain why it undermines the point of DI",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    There are two ways to write custom middleware. **Inline**, with `app.Use(async (context, next) => { ... })` — quick, good for small cross-cutting logic directly in `Program.cs`. **As a class**, either the *conventional middleware* shape (a constructor taking `RequestDelegate next`, plus an `InvokeAsync(HttpContext, ...)` method) or by implementing `IMiddleware` and registering it with `app.UseMiddleware<T>()`.

                    Every middleware wraps the *rest of the pipeline* like a layer: code before `await next(context)` runs on the way **in** (request), code after it runs on the way **out** (response) — and a middleware can short-circuit entirely by never calling `next` at all (e.g., returning a 401 before routing even runs).

                    A subtle but important DI wrinkle: conventional middleware classes (the `RequestDelegate next` constructor style) are built **once**, at app startup, and reused for every request — they behave like singletons even though you never called `AddSingleton` for them. That means constructor-injecting a Scoped service (like `AppDbContext`) into one recreates the exact captive-dependency bug from lesson 1. The fix is to accept Scoped dependencies as extra parameters on `InvokeAsync` itself — those get resolved fresh, per request, from that request's DI scope, not from the constructor.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Middleware is like wrapping a gift in several layers of paper — each layer gets added on the way in and peeled back in the exact reverse order on the way out. Skip the last layer's "unwrap" step (never call `next`) and whatever's underneath never gets revealed at all.

                    A conventional middleware class is like a single reusable gift-wrapping station on a factory line, built once at the start of the shift — it can hold long-lived tools, but it can't privately keep a customer's one-time receipt (a Scoped service) between customers; that has to be handed to it fresh with each new box that comes through.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Inline middleware**

                    ```
                    app.Use(async (context, next) =>
                    {
                        // before: runs on the way in
                        await next(context);
                        // after: runs on the way out
                    });
                    ```

                    **Conventional middleware class** (built once, singleton-like)

                    - Constructor takes `RequestDelegate next` (+ Singleton-safe deps only)
                    - `InvokeAsync(HttpContext context, IScopedService svc)` — Scoped deps go **here**
                    - Registered with `app.UseMiddleware<MyMiddleware>()`

                    **`IMiddleware`** — resolved from DI per the lifetime you register it with

                    - Register: `services.AddScoped<MyMiddleware>();`
                    - Map: `app.UseMiddleware<MyMiddleware>();`
                    - `Task InvokeAsync(HttpContext context, RequestDelegate next)`
                    """, 3),
                Block(BlockType.CodeSnippet, "A Custom Request-Timing Middleware", BodyFormat.PlainText, """
                    public class RequestTimingMiddleware
                    {
                        private readonly RequestDelegate _next;
                        private readonly ILogger<RequestTimingMiddleware> _logger;

                        // Constructor deps must be Singleton-safe — this class is built once.
                        public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
                        {
                            _next = next;
                            _logger = logger;
                        }

                        // Scoped/Transient deps go here, resolved fresh per request.
                        public async Task InvokeAsync(HttpContext context, AppDbContext db)
                        {
                            var stopwatch = Stopwatch.StartNew();

                            await _next(context); // hand off to the rest of the pipeline

                            stopwatch.Stop();
                            _logger.LogInformation(
                                "{Method} {Path} completed in {ElapsedMs}ms",
                                context.Request.Method, context.Request.Path, stopwatch.ElapsedMilliseconds);
                        }
                    }

                    // Program.cs
                    app.UseMiddleware<RequestTimingMiddleware>();
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "The Middleware Onion", BodyFormat.AsciiArt, """
                    Request  -->  [ MW1 in  --> [ MW2 in  --> [ Endpoint ]
                                                                    |
                    Response <--  [ MW1 out <-- [ MW2 out <-- -----+

                    Code before `await next()` in MW1/MW2 runs on the way IN.
                    Code after `await next()` in MW1/MW2 runs on the way OUT,
                    in the REVERSE order (MW2 finishes before MW1 finishes).
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    In a conventional middleware class, only inject truly Singleton-safe dependencies (loggers, `IConfiguration`, other Singletons) through the constructor. Anything Scoped or Transient — `DbContext`, a request-scoped service — must be a parameter on `InvokeAsync`, so the framework resolves it from that request's own DI scope.

                    Prefer constructor injection everywhere else in the app (services, endpoint handlers). Resolving dependencies manually via `IServiceProvider.GetService<T>()` inside a method — the "service locator" pattern — hides a class's real dependencies from its constructor signature and makes them impossible to see without reading the method body.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked to write a piece of custom middleware, narrate the choice out loud: "I'll take `RequestDelegate next` in the constructor since this class is built once at startup, and pull `AppDbContext` in as an `InvokeAsync` parameter instead, since that's Scoped and needs to come from this request's own DI scope." That one sentence signals you understand *why* middleware DI works the way it does, not just the syntax.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Injecting a Scoped service (most commonly `DbContext`) into a conventional middleware class's **constructor** — since the middleware is instantiated once at startup, this captures one instance forever, the exact captive-dependency bug from lesson 1, just in a new location.

                    Also common: forgetting to call `await next(context)` at all in custom middleware, which silently dead-ends the pipeline — the request never reaches routing, the endpoint, or a response, and it just hangs or returns an empty response with no explicit error.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "In a conventional middleware class (constructor takes `RequestDelegate next`), where should a Scoped dependency like `AppDbContext` be injected?",
                    "Conventional middleware classes are instantiated once at startup and reused for every request, behaving like a singleton. A Scoped dependency injected into the constructor would be captured forever — it must instead be a parameter on `InvokeAsync`, which DI resolves fresh from each request's own scope.",
                    [
                        new QuizOptionSeed("In the constructor, alongside RequestDelegate next", false),
                        new QuizOptionSeed("As a parameter on the InvokeAsync method", true),
                        new QuizOptionSeed("It can't be injected into middleware at all", false),
                        new QuizOptionSeed("Only via a static service locator call inside InvokeAsync", false),
                    ]),
                new QuizQuestionSeed(
                    "What happens if a custom middleware never calls `await next(context)`?",
                    "Calling next() is what hands control to the rest of the pipeline. Skipping it short-circuits the request entirely — later middleware, routing, and the endpoint handler never run, so no real response is produced unless the middleware explicitly writes one itself.",
                    [
                        new QuizOptionSeed("The rest of the pipeline still runs afterward automatically", false),
                        new QuizOptionSeed("The request pipeline stops there — later middleware and the endpoint never execute", true),
                        new QuizOptionSeed("The application throws a compile-time error", false),
                        new QuizOptionSeed("It only skips authentication, not routing", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Write custom ASP.NET Core middleware", "https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write", LinkType.OfficialDocs),
                new ReferenceLinkSeed("Dependency injection guidelines", "https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-guidelines", LinkType.OfficialDocs),
            ],
            prerequisites: [lesson1]);

        var lesson3Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson3.Slug,
        [
            "Write one custom middleware class using IMiddleware (or the conventional shape) and register it in Program.cs",
            "Find any Scoped service accidentally injected into a middleware constructor and move it to InvokeAsync",
            "Explain the 'middleware onion' out loud, tracing a request in and the response back out through two layers",
        ]);

        var lesson4 = BuildLesson(
            slug: "configuration-logging-options-pattern",
            title: "Configuration, Logging & the Options Pattern",
            summary: "Reading settings safely with the Options pattern, structured logging with ILogger<T>, and environment-specific configuration.",
            estimatedMinutes: 35,
            objectives:
            [
                "Explain the configuration provider order and how appsettings.{Environment}.json and environment variables override appsettings.json",
                "Bind a strongly-typed settings class using the Options pattern instead of scattering IConfiguration[\"Key\"] lookups",
                "Choose between IOptions<T>, IOptionsSnapshot<T>, and IOptionsMonitor<T> for a given scenario",
                "Write structured log messages using message templates instead of string interpolation",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    `IConfiguration` is built from an ordered stack of **providers** — `appsettings.json`, then `appsettings.{Environment}.json`, then environment variables, then command-line arguments (and user secrets in Development). Later providers **override** matching keys from earlier ones, which is exactly how the same code runs with different settings in Development vs. Production without changing a single line.

                    Reading configuration directly with `builder.Configuration["Jwt:Issuer"]` scatters magic strings everywhere and gives you no compile-time safety. The **Options pattern** fixes this: define a plain settings class, bind a config section to it once (`services.Configure<JwtSettings>(config.GetSection("Jwt"))`), then inject a typed options wrapper wherever it's needed.

                    Three flavors of that wrapper exist for different needs: **`IOptions<T>`** (Singleton, read once, never changes after startup), **`IOptionsSnapshot<T>`** (Scoped, recomputed per request — picks up config file changes without a restart), and **`IOptionsMonitor<T>`** (Singleton, but supports an `OnChange` callback for live reload, useful for long-lived Singleton services).

                    **Logging** works the same DI-first way: inject `ILogger<T>` and call `logger.LogInformation("Order {OrderId} shipped in {ElapsedMs}ms", orderId, elapsed)`. The `{OrderId}`/`{ElapsedMs}` are **named placeholders**, not string interpolation — the logging provider keeps them as separate structured fields, so a log aggregator can filter/query by `OrderId` directly instead of regex-parsing a sentence.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Configuration providers are like sticky notes layered on top of a printed recipe card — the base card (`appsettings.json`) has sensible defaults, and each sticky note on top (`appsettings.Production.json`, then environment variables) can cross out and override just the one line it cares about, without anyone having to reprint the whole card.

                    Structured logging vs. string interpolation is the difference between filling out a labeled spreadsheet row (`OrderId: 42, ElapsedMs: 118`) and writing the same information as a plain sentence in a notebook — the spreadsheet version can be sorted and filtered later; the sentence can only be searched by eye.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Configuration provider order** (last one wins on a conflicting key)

                    1. `appsettings.json`
                    2. `appsettings.{Environment}.json`
                    3. User secrets (Development only)
                    4. Environment variables
                    5. Command-line arguments

                    **Options pattern**

                    - `services.Configure<MySettings>(config.GetSection("MySettings"));`
                    - `IOptions<T>` — Singleton, snapshot taken once at startup
                    - `IOptionsSnapshot<T>` — Scoped, recomputed once per request
                    - `IOptionsMonitor<T>` — Singleton, live-reloads + `OnChange(callback)`

                    **Log levels** (ascending severity)

                    `Trace` < `Debug` < `Information` < `Warning` < `Error` < `Critical`
                    """, 3),
                Block(BlockType.CodeSnippet, "Strongly-Typed Options + Structured Logging", BodyFormat.PlainText, """
                    public class SmtpSettings
                    {
                        public string Host { get; set; } = "";
                        public int Port { get; set; }
                    }

                    // Program.cs — bind the "Smtp" section instead of reading raw keys.
                    builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));

                    public class EmailSender(IOptions<SmtpSettings> options, ILogger<EmailSender> logger)
                    {
                        private readonly SmtpSettings _settings = options.Value;

                        public async Task SendAsync(string to, string subject)
                        {
                            var stopwatch = Stopwatch.StartNew();
                            await DeliverAsync(_settings.Host, _settings.Port, to, subject);

                            // Structured template — {To} and {ElapsedMs} stay queryable fields,
                            // NOT baked into the message string like $"Sent to {to}" would be.
                            logger.LogInformation(
                                "Email sent to {To} in {ElapsedMs}ms", to, stopwatch.ElapsedMilliseconds);
                        }
                    }
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "Configuration Provider Layering", BodyFormat.StructuredSteps, """
                    [{"label":"appsettings.json","note":"base defaults"},{"label":"appsettings.{Environment}.json","note":"overrides per env"},{"label":"User Secrets","note":"Development only"},{"label":"Environment Variables","note":"overrides file-based config"},{"label":"Command-line Args","note":"highest precedence"},{"label":"Final merged IConfiguration"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Bind configuration into a strongly-typed settings class via `services.Configure<T>()` rather than sprinkling `config["Section:Key"]` string lookups across the codebase — a typo in a magic string fails silently at runtime, while a typo in a C# property name fails to compile.

                    Always use structured log message templates (`"Order {OrderId} shipped", orderId`) instead of string interpolation (`$"Order {orderId} shipped"`), and never log secrets (connection strings, API keys, passwords) even at `Debug` level — log sinks are often less tightly access-controlled than the config store itself.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    Be ready to justify a choice between `IOptions<T>` and `IOptionsMonitor<T>` with a scenario, not just a definition: "For a background service that needs to pick up a changed rate-limit threshold without restarting the app, I'd use `IOptionsMonitor<T>` and subscribe to `OnChange` — `IOptions<T>` would only ever see the value from the moment the app started."
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Writing `logger.LogInformation($"Order {orderId} shipped")` — the string-interpolated `$""` bakes the value into the message text itself, so a log aggregator sees one opaque sentence per call instead of a queryable `OrderId` field. The fix is `logger.LogInformation("Order {OrderId} shipped", orderId)`.

                    Also common: hardcoding a connection string or API key directly in C# instead of reading it from configuration — this leaks secrets into source control and makes it impossible to use a different value per environment without a code change.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "appsettings.json sets `Logging:LogLevel:Default` to `Information`, and an environment variable sets the same key to `Warning`. Which value wins?",
                    "Environment variables are a later provider in the configuration stack than appsettings.json, and later providers override earlier ones for the same key — so the environment variable's `Warning` value wins.",
                    [
                        new QuizOptionSeed("appsettings.json always wins, since it's the base file", false),
                        new QuizOptionSeed("The environment variable wins, since later providers override earlier ones", true),
                        new QuizOptionSeed("Both values are merged into a list", false),
                        new QuizOptionSeed("The app throws a configuration conflict exception at startup", false),
                    ]),
                new QuizQuestionSeed(
                    "What's the main problem with `logger.LogInformation($\"Order {orderId} shipped\")` compared to using a message template?",
                    "String interpolation bakes the value directly into the log message text, losing the structured, queryable {OrderId} field that a template preserves — log aggregators can no longer filter or search by OrderId directly.",
                    [
                        new QuizOptionSeed("It's slower to compile", false),
                        new QuizOptionSeed("It loses the structured, queryable OrderId field that a message template would preserve", true),
                        new QuizOptionSeed("It throws a runtime exception", false),
                        new QuizOptionSeed("LogInformation doesn't accept interpolated strings at all", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Configuration in ASP.NET Core", "https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/", LinkType.OfficialDocs),
                new ReferenceLinkSeed("Options pattern in ASP.NET Core", "https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options", LinkType.OfficialDocs),
            ],
            prerequisites: [lesson3]);

        var lesson4Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson4.Slug,
        [
            "Bind one appsettings.json section to a strongly-typed options class instead of using magic-string lookups",
            "Replace one string-interpolated log call in your own code with a structured message template",
            "Explain out loud when you'd reach for IOptionsMonitor<T> instead of IOptions<T>, with a concrete scenario",
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
            "The middleware pipeline, dependency injection, EF Core, custom middleware, configuration/logging, and building your first Minimal API.",
            200, [lesson1, lesson2, lesson3, lesson4], capstone);

        return (module, [lesson1Checklist, lesson2Checklist, lesson3Checklist, lesson4Checklist, capstoneChecklist]);
    }

    private static (Module, List<ChecklistSeed>) BuildDotNetProductionReadinessModule(int topicId)
    {
        var lesson1 = BuildLesson(
            slug: "jwt-authentication-and-authorization",
            title: "Authentication & Authorization in ASP.NET Core",
            summary: "Issuing and validating JWT bearer tokens, protecting endpoints with [Authorize], and enforcing fine-grained access with role- and policy-based authorization.",
            estimatedMinutes: 45,
            objectives:
            [
                "Explain the difference between authentication and authorization and where each fits in the request pipeline",
                "Configure JWT bearer authentication and validate tokens with the correct TokenValidationParameters",
                "Protect minimal API endpoints with [Authorize]/RequireAuthorization using roles and custom policies",
                "Explain why refresh tokens exist and how they reduce the blast radius of a stolen access token",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    **Authentication** answers "who are you?" — it establishes an identity (a `ClaimsPrincipal`) from credentials or a token. **Authorization** answers "are you allowed to do this?" — it runs *after* authentication and checks that established identity against rules (roles, policies, claims). ASP.NET Core keeps these as two distinct middleware components on purpose, and `app.UseAuthentication()` must be registered before `app.UseAuthorization()` in `Program.cs` — authorization has nothing to check against until authentication has populated `HttpContext.User`.

                    A **JWT (JSON Web Token)** is a compact, URL-safe string with three base64url-encoded parts separated by dots: `header.payload.signature`. The header names the signing algorithm, the payload holds **claims** (name, role, expiry, custom data), and the signature is a cryptographic hash of the first two parts using a secret (or private key) — it proves the token wasn't tampered with, but it does **not** encrypt the payload. Anyone can base64-decode a JWT and read its claims; never put secrets in the payload.

                    You wire JWT bearer authentication up with `builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => { ... })`, configuring `TokenValidationParameters` to check the signing key, issuer, audience, and expiry on every incoming request. Once validated, the middleware builds a `ClaimsPrincipal` from the token's claims and attaches it to `HttpContext.User` for the rest of the pipeline — including your `[Authorize]` checks — to use.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    A JWT is like a wristband at a concert venue: the box office (your login endpoint) checks your ticket once at the door, then stamps a tamper-evident wristband with your seating tier printed on it. Every checkpoint inside the venue (your API endpoints) just glances at the wristband — they don't call the box office again to re-verify you. But the wristband is visible to anyone standing next to you (unencrypted payload), and it expires at the end of the night (token expiry) so it can't be reused tomorrow.

                    A refresh token is the re-entry stub the box office keeps on file: when your wristband expires, you don't need to buy a new ticket (re-enter your password) — you show the stub, they check it's still valid and not reported lost, and hand you a fresh wristband.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Registering JWT bearer auth**

                    - `builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => { options.TokenValidationParameters = ...; })`
                    - `builder.Services.AddAuthorization(options => options.AddPolicy("Name", policy => policy.RequireRole(...) / RequireClaim(...)));`
                    - Pipeline order: `app.UseAuthentication(); app.UseAuthorization();` — authentication first, always

                    **Protecting endpoints**

                    - `[Authorize]` on a controller/action — any authenticated user
                    - `[Authorize(Roles = "Admin,Manager")]` — any one of the listed roles
                    - `[Authorize(Policy = "SeniorEngineerOnly")]` — a named policy
                    - Minimal APIs: `app.MapGet(...).RequireAuthorization()` or `.RequireAuthorization(policy => policy.RequireRole("Admin"))`
                    - `[AllowAnonymous]` — explicitly opt an endpoint out of a controller-wide `[Authorize]`
                    """, 3),
                Block(BlockType.CodeSnippet, "Configuring JWT Bearer Auth and Issuing Tokens", BodyFormat.PlainText, """
                    // Program.cs
                    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(options =>
                        {
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                                ValidAudience = builder.Configuration["Jwt:Audience"],
                                IssuerSigningKey = new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"]!)),
                                ClockSkew = TimeSpan.Zero, // don't grant a free 5-minute grace window
                            };
                        });

                    builder.Services.AddAuthorization(options =>
                    {
                        options.AddPolicy("SeniorEngineerOnly", policy =>
                            policy.RequireClaim("level", "senior", "staff", "principal"));
                    });

                    var app = builder.Build();
                    app.UseAuthentication();
                    app.UseAuthorization();

                    app.MapGet("/api/admin/reports", () => Results.Ok("secret reports"))
                        .RequireAuthorization(policy => policy.RequireRole("Admin"));

                    // Issuing an access token after validating credentials
                    public class TokenService(IConfiguration config)
                    {
                        public string CreateAccessToken(AppUser user)
                        {
                            var claims = new List<Claim>
                            {
                                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                                new(ClaimTypes.Name, user.Email),
                                new(ClaimTypes.Role, user.Role),
                            };

                            var key = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(config["Jwt:SigningKey"]!));
                            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                            var token = new JwtSecurityToken(
                                issuer: config["Jwt:Issuer"],
                                audience: config["Jwt:Audience"],
                                claims: claims,
                                expires: DateTime.UtcNow.AddMinutes(15), // short-lived on purpose
                                signingCredentials: creds);

                            return new JwtSecurityTokenHandler().WriteToken(token);
                        }

                        public string CreateRefreshToken() =>
                            Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)); // opaque, stored + checked server-side
                    }
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "Token Issuance and Validation Flow", BodyFormat.StructuredSteps, """
                    [{"label":"Client POSTs credentials to /login"},{"label":"Server validates credentials against the user store"},{"label":"Server issues a short-lived access token + an opaque refresh token","note":"e.g. 15 min access, 14 day refresh"},{"label":"Client calls APIs with Authorization: Bearer <access token>"},{"label":"JWT bearer middleware validates signature, issuer, audience, expiry"},{"label":"Claims populate HttpContext.User"},{"label":"Authorization middleware checks [Authorize]/role/policy requirements"},{"label":"Access token expires -> client POSTs refresh token to /refresh for a new access token","note":"no re-login needed"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Keep access tokens **short-lived** (minutes, not days) so a leaked token has a small window of usefulness, and pair them with a longer-lived, server-tracked refresh token that can be revoked (e.g., on logout or a detected breach) independently of waiting for the access token to expire naturally.

                    Always set `ValidateIssuer`, `ValidateAudience`, `ValidateLifetime`, and `ValidateIssuerSigningKey` to `true` in `TokenValidationParameters` — skipping any of them (a common shortcut in tutorials) reopens the exact attacks JWT validation exists to close, such as accepting a token minted for a completely different application. Store refresh tokens in an `HttpOnly`, `Secure` cookie rather than browser `localStorage`, since `localStorage` is readable by any JavaScript running on the page — including an XSS payload.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked "walk me through what happens when a request comes in with a bearer token," narrate the pipeline in order: authentication middleware extracts and validates the token (signature, issuer, audience, expiry) and builds a `ClaimsPrincipal`; that principal is attached to `HttpContext.User`; authorization middleware then evaluates whatever `[Authorize]`/role/policy requirement is on the matched endpoint against those claims. Interviewers are listening for whether you know authentication and authorization are two separate steps, not one.

                    Also be ready to explain *why* refresh tokens exist rather than just what they are: they let you keep access tokens short-lived (limiting damage from theft) without forcing the user to re-enter credentials every few minutes — the refresh token is the thing you can actually revoke server-side.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Forgetting `app.UseAuthentication()` entirely, or placing it after `app.UseAuthorization()` — every request then gets treated as anonymous, and `[Authorize]` attributes either reject everyone or (worse, if misconfigured) are silently skipped.

                    Also common: treating a JWT's payload as confidential (it's only base64-encoded, not encrypted — never put a password, SSN, or internal-only data in a claim), and issuing access tokens with no expiry or absurdly long expiry "for convenience," which turns a single leaked token into a standing, unrevocable backdoor.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "Why must app.UseAuthentication() be called before app.UseAuthorization() in Program.cs?",
                    "Authorization middleware decides access by inspecting the ClaimsPrincipal on HttpContext.User — but that principal is only populated by authentication middleware. If authorization runs first, there's no identity yet to check any [Authorize]/role/policy rule against.",
                    [
                        new QuizOptionSeed("It isn't required — ASP.NET Core reorders them automatically at startup", false),
                        new QuizOptionSeed("Authorization checks depend on the ClaimsPrincipal that authentication attaches to HttpContext.User", true),
                        new QuizOptionSeed("UseAuthorization() throws an exception if called first", false),
                        new QuizOptionSeed("It only matters for MVC controllers, not minimal APIs", false),
                    ]),
                new QuizQuestionSeed(
                    "What problem do refresh tokens solve that access tokens alone don't?",
                    "They let a client obtain a new short-lived access token without re-entering credentials, so access tokens can stay short-lived (limiting the damage if one is stolen) while the user's session still feels continuous and long-lived.",
                    [
                        new QuizOptionSeed("They let clients get a new access token without re-authenticating, so access tokens can safely stay short-lived", true),
                        new QuizOptionSeed("They encrypt the JWT payload so claims can't be read by the client", false),
                        new QuizOptionSeed("They replace the need for HTTPS on the login endpoint", false),
                        new QuizOptionSeed("They allow one access token to work across unlimited different APIs forever", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Overview of ASP.NET Core Authentication", "https://learn.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-8.0", LinkType.OfficialDocs),
                new ReferenceLinkSeed("Policy-based authorization in ASP.NET Core", "https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies", LinkType.OfficialDocs),
            ]);

        var lesson1Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson1.Slug,
        [
            "Add [Authorize(Roles = \"Admin\")] (or RequireAuthorization on a minimal API) to one endpoint and verify a non-admin token gets a 403",
            "Write out the exact TokenValidationParameters you'd set for a production API, including issuer/audience/lifetime validation, from memory",
            "Explain out loud, in one paragraph, why access tokens should be short-lived while refresh tokens are longer-lived and revocable",
        ]);

        var lesson2 = BuildLesson(
            slug: "testing-aspnet-core-applications",
            title: "Testing ASP.NET Core Applications",
            summary: "Unit testing services with mocked dependencies, and spinning up an in-memory server with WebApplicationFactory to integration-test minimal API endpoints end to end.",
            estimatedMinutes: 45,
            objectives:
            [
                "Distinguish unit tests (mocked dependencies, isolated logic) from integration tests (real pipeline, real HTTP) and know when to reach for each",
                "Mock a dependency with Moq and verify a service's behavior in isolation using xUnit",
                "Use WebApplicationFactory<Program> to boot an in-memory test server and call minimal API endpoints with a real HttpClient",
                "Override a DI registration in a test fixture to swap a real dependency (like a DbContext) for a test double",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    A **unit test** exercises one piece of logic in isolation — typically a single method on a service class — with every collaborator replaced by a **mock** or **stub** so a failure can only mean the unit under test is wrong, not some dependency three layers away. **xUnit** is the most common .NET test framework: `[Fact]` marks a single test method, `[Theory]` plus `[InlineData(...)]` runs the same test body against multiple inputs. **Moq** creates fake implementations of interfaces at runtime: `new Mock<IThing>()`, `.Setup(x => x.Method(...)).Returns(...)` to script behavior, and `.Verify(x => x.Method(...), Times.Once)` to assert a call actually happened.

                    An **integration test** instead boots the real application — real DI container, real middleware pipeline, real routing — and sends real HTTP requests against it in memory. `WebApplicationFactory<TEntryPoint>` (from the `Microsoft.AspNetCore.Mvc.Testing` package) does exactly this: it hosts your app on an in-memory `TestServer` and hands you an `HttpClient` wired straight to it, no real network socket involved. Because a minimal-API `Program.cs` written with top-level statements compiles to an `internal` `Program` class by default, you typically add one line — `public partial class Program { }` — at the bottom of `Program.cs` so `WebApplicationFactory<Program>` has a public type it's allowed to reference.

                    You'll usually still want to replace a few real dependencies even in an integration test — most commonly the database — via `factory.WithWebHostBuilder(builder => builder.ConfigureServices(services => { ... }))`, removing the real `DbContext` registration and adding an in-memory or test-only one instead.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    A unit test is like testing one gear on a workbench, spun by hand — you can tell instantly whether that one gear's teeth are cut correctly, with nothing else in the system able to confuse the result. An integration test is like running the fully assembled engine on a test stand: you're no longer isolating one part, you're confirming the gears, belts, and pistons all actually work together the way the blueprint says they should — closer to reality, but slower to run and harder to pinpoint exactly which part failed when something breaks.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **xUnit basics**

                    - `[Fact]` — a single test case; `[Theory]` + `[InlineData(...)]` — data-driven test cases
                    - `Assert.Equal(expected, actual)`, `Assert.True(...)`, `Assert.ThrowsAsync<TException>(func)`
                    - Constructor = setup that runs before every test in the class; `IDisposable.Dispose()` = teardown

                    **Moq basics**

                    - `var mock = new Mock<IInventoryClient>();`
                    - `mock.Setup(x => x.GetQuantityAsync("SKU-1")).ReturnsAsync(0);`
                    - `mock.Verify(x => x.GetQuantityAsync("SKU-1"), Times.Once);`
                    - Pass `mock.Object` wherever the real interface implementation would go

                    **WebApplicationFactory**

                    - `public class MyTests : IClassFixture<WebApplicationFactory<Program>>`
                    - `factory.CreateClient()` — real `HttpClient` wired to the in-memory `TestServer`
                    - `factory.WithWebHostBuilder(b => b.ConfigureServices(services => { ... }))` — swap a real service for a test double
                    """, 3),
                Block(BlockType.CodeSnippet, "Unit Test (Moq) vs. Integration Test (WebApplicationFactory)", BodyFormat.PlainText, """
                    // Unit test: OrderService in isolation, IInventoryClient mocked
                    public class OrderServiceTests
                    {
                        [Fact]
                        public async Task PlaceOrder_ThrowsWhenInventoryInsufficient()
                        {
                            // Arrange
                            var inventoryMock = new Mock<IInventoryClient>();
                            inventoryMock
                                .Setup(x => x.GetAvailableQuantityAsync("SKU-1"))
                                .ReturnsAsync(0);
                            var sut = new OrderService(inventoryMock.Object);

                            // Act
                            var act = () => sut.PlaceOrderAsync("SKU-1", quantity: 5);

                            // Assert
                            await Assert.ThrowsAsync<InsufficientInventoryException>(act);
                            inventoryMock.Verify(x => x.GetAvailableQuantityAsync("SKU-1"), Times.Once);
                        }
                    }

                    // Integration test: real pipeline, in-memory database swapped in
                    public class TasksEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
                    {
                        private readonly HttpClient _client;

                        public TasksEndpointsTests(WebApplicationFactory<Program> factory)
                        {
                            var customizedFactory = factory.WithWebHostBuilder(builder =>
                            {
                                builder.ConfigureServices(services =>
                                {
                                    var descriptor = services.Single(
                                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                                    services.Remove(descriptor);

                                    services.AddDbContext<AppDbContext>(options =>
                                        options.UseInMemoryDatabase("TestsDb"));
                                });
                            });

                            _client = customizedFactory.CreateClient();
                        }

                        [Fact]
                        public async Task PostTask_ReturnsCreatedWithLocationHeader()
                        {
                            var response = await _client.PostAsJsonAsync("/api/tasks",
                                new { Title = "Write integration tests" });

                            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                            Assert.NotNull(response.Headers.Location);
                        }
                    }
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "The Test Pyramid", BodyFormat.AsciiArt, """
                                /\\
                               /E2E\\           few, slow, expensive -- real browser, real deploy
                              /------\\
                             / Integ. \\        some -- WebApplicationFactory, real pipeline, real HTTP
                            /----------\\
                           /    Unit     \\     many, fast, cheap -- isolated logic, Moq for dependencies
                          /--------------\\

                    Speed and isolation decrease going up; realism and confidence increase going up.
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Keep unit tests fast and deterministic: no real database, no real network call, no `Thread.Sleep`. Every collaborator that talks to the outside world should be an interface you can mock. Structure each test as **Arrange / Act / Assert**, and aim for one meaningful assertion focus per test so a failure tells you exactly what broke.

                    For integration tests, use `WebApplicationFactory<Program>.WithWebHostBuilder(...)` to replace only what genuinely needs replacing — usually the database and any external HTTP clients (payment providers, email senders) — while leaving your own middleware, routing, and DI wiring untouched, since testing that wiring is the entire point of an integration test.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked "how would you test this minimal API endpoint," give a two-tier answer: unit-test the service/business logic it calls into (mock its dependencies with Moq, assert behavior and edge cases fast and in isolation), then add a small number of integration tests via `WebApplicationFactory` that hit the actual route to confirm routing, model binding, DI wiring, and status codes are all correct end to end. Naming both tiers — and explaining *why* you wouldn't rely on only one — signals you understand the test pyramid, not just the tools.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Writing a "unit test" that actually hits a real database or a real third-party API — it's slow, flaky under network conditions, and no longer isolates the one thing you meant to test. If a test needs a real database, it's an integration test; name and treat it as one.

                    Also common: using `WebApplicationFactory` but forgetting to replace external dependencies (real payment gateways, real email senders) in `ConfigureServices`, which can cause a test run to actually send emails or hit third-party rate limits — and asserting on internal implementation details (e.g., a private field) instead of observable behavior (status code, response body, headers), which makes tests brittle to harmless refactors.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "What's the key difference between a unit test and an integration test for a minimal API endpoint?",
                    "A unit test calls the service/handler logic directly with mocked dependencies (Moq), verifying one piece of logic in isolation. An integration test uses WebApplicationFactory to boot the real app — real DI, routing, and middleware — and sends an actual HTTP request through the full pipeline.",
                    [
                        new QuizOptionSeed("There is no real difference — both call the endpoint's C# method directly", false),
                        new QuizOptionSeed("A unit test mocks dependencies and tests logic in isolation; an integration test boots the real pipeline and sends real HTTP requests", true),
                        new QuizOptionSeed("Integration tests never use xUnit, only unit tests do", false),
                        new QuizOptionSeed("Unit tests require a running database; integration tests never touch a database", false),
                    ]),
                new QuizQuestionSeed(
                    "Why does WebApplicationFactory<Program> typically require adding `public partial class Program { }` at the bottom of Program.cs?",
                    "Program.cs written with top-level statements compiles to an internal Program class by default. WebApplicationFactory<TEntryPoint> needs a publicly accessible entry-point type to bootstrap the app in a test project, so the partial class declaration exposes one without changing any runtime behavior.",
                    [
                        new QuizOptionSeed("Top-level statements compile to an internal Program class, and WebApplicationFactory needs a public type reference to bootstrap the app in tests", true),
                        new QuizOptionSeed("It's required so xUnit can discover [Fact] methods in Program.cs", false),
                        new QuizOptionSeed("It enables Moq to mock the Program class directly", false),
                        new QuizOptionSeed("It's only needed when using controllers, not minimal APIs", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Integration tests in ASP.NET Core", "https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests", LinkType.OfficialDocs),
                new ReferenceLinkSeed("Moq — mocking library documentation", "https://github.com/devlooped/moq", LinkType.FurtherReading),
            ]);

        var lesson2Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson2.Slug,
        [
            "Write one unit test for a service method, using Moq to fake its one dependency",
            "Spin up WebApplicationFactory<Program> and call one real endpoint end to end against an in-memory test database",
            "Find one test in your own suite that's secretly an integration test disguised as a unit test (hits a real DB/network) and fix or relabel it",
        ]);

        var module = BuildModule(topicId, "aspnet-core-production-readiness", "Production Readiness: Security & Testing",
            "Securing APIs with JWT bearer authentication and role/policy-based authorization, then verifying behavior with fast unit tests and real-pipeline integration tests via WebApplicationFactory.",
            90, [lesson1, lesson2], sortOrder: 2);

        return (module, [lesson1Checklist, lesson2Checklist]);
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

        var lesson3 = BuildLesson(
            slug: "sliding-window-patterns",
            title: "Sliding Window Patterns",
            summary: "Fixed-size and variable-size sliding windows for turning brute-force substring/subarray scans into linear time.",
            estimatedMinutes: 45,
            objectives:
            [
                "Distinguish fixed-size vs. variable-size sliding window problems",
                "Implement a variable-size window that expands and contracts based on a condition",
                "Explain why sliding window runs in O(n) despite an inner while loop nested in an outer for loop",
                "Recognize sliding window opportunities in substring/subarray problems during an interview",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    A **sliding window** is a contiguous range `[left, right]` over an array or string that you slide across the input, expanding and contracting instead of restarting from scratch — turning many `O(n²)` or `O(n³)` brute-force scans into `O(n)`.

                    There are two shapes:

                    - **Fixed-size window** — the window width `k` is fixed by the problem (e.g., "max sum of any subarray of size k"). Slide by adding the new right element and removing the leftmost element every step; the window never changes size.
                    - **Variable-size window** — the window grows by moving `right` until some condition breaks (e.g., a duplicate character appears), then shrinks by moving `left` until the condition holds again. The window's size *is* the answer you're tracking, not a fixed input.

                    Even though a variable window looks like two nested loops (an inner `while` inside an outer `for`), each pointer (`left` and `right`) only ever moves forward and never resets — so across the whole run, `left` and `right` each move at most `n` times total, giving `O(n)` overall, not `O(n²)`.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    A fixed-size window is like a physical picture frame you slide across a long photo strip one inch at a time — the frame's size never changes, you just drop what falls off the left edge and pick up what enters on the right.

                    A variable-size window is like slowly opening your umbrella to cover a growing group of people (expanding `right`) until it starts raining on someone at the edge (the condition breaks), at which point a few people step out from the left (shrinking `left`) until everyone under it is dry again.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Recognizing a sliding window problem**

                    - Keywords: "contiguous subarray/substring", "longest/shortest/max/min ... satisfying a condition"
                    - The condition can be checked incrementally (adding/removing one element updates it in O(1) or close to it)

                    **Fixed-size window template**

                    - Maintain a running aggregate (sum, count, frequency map) for the current window
                    - Each step: add `nums[right]`, remove `nums[right - k]`, move both pointers forward by one

                    **Variable-size window template**

                    - Expand `right` one step, update the window's state
                    - While the window violates the condition, shrink from `left` and update state
                    - After the inner while, the window is valid — record/update the answer
                    """, 3),
                Block(BlockType.CodeSnippet, "Longest Substring Without Repeating Characters", BodyFormat.PlainText, """
                    // Longest substring without repeating characters: O(n) time, O(min(n, charset)) space.
                    public int LengthOfLongestSubstring(string s)
                    {
                        var lastSeenAt = new Dictionary<char, int>();
                        var left = 0;
                        var longest = 0;

                        for (var right = 0; right < s.Length; right++)
                        {
                            var c = s[right];

                            // If c was seen inside the current window, jump left past it.
                            if (lastSeenAt.TryGetValue(c, out var seenIndex) && seenIndex >= left)
                            {
                                left = seenIndex + 1;
                            }

                            lastSeenAt[c] = right;
                            longest = Math.Max(longest, right - left + 1);
                        }

                        return longest;
                    }
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "Expanding and Jumping the Window", BodyFormat.AsciiArt, """
                    s = a b c a b c b b
                        0 1 2 3 4 5 6 7

                    right=2: window [0..2] "abc"      length 3 (best so far)
                    right=3: 'a' repeats inside window -> left jumps to 1
                             window [1..3] "bca"       length 3
                    right=6: 'b' repeats inside window -> left jumps to 5
                             window [5..6] "cb"        length 2

                    Longest substring without repeats: length 3 (e.g. "abc")
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Maintain the window's "state" (a frequency map, running sum, or count of violations) incrementally as you expand/shrink — never recompute it from scratch inside the loop, or you turn an O(n) sliding window back into O(n²)/O(n·k) by accident.

                    For fixed-size windows, initialize by processing the first `k` elements outside the main loop, then slide one step at a time inside it — this avoids an off-by-one on the very first window.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    Say the shape of the window out loud before coding: "the window size is fixed at k" vs. "the window grows until a condition breaks, then shrinks." Naming which of the two templates applies signals you're pattern-matching correctly, and tells the interviewer which invariant to expect you to maintain.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Recomputing the window's sum/frequency map from scratch every time the window moves, instead of incrementally adding the new element and removing the old one — this silently turns an O(n) sliding window into O(n·k) or worse.

                    Also common: letting `left` overshoot `right`, and — on variable-size problems — recording the answer before the inner while loop has fully shrunk the window back to valid, which can record a length that was never actually valid.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "You need the length of the longest subarray containing at most 2 distinct values. Which window shape fits?",
                    "The valid window size isn't known upfront — it depends on how many distinct values are currently inside it — so you grow `right` until there are more than 2 distinct values, then shrink `left` until there are 2 or fewer again. That's a variable-size window.",
                    [
                        new QuizOptionSeed("A variable-size window that expands and shrinks based on the distinct-count condition", true),
                        new QuizOptionSeed("A fixed-size window, since 'at most 2' implies a constant width", false),
                        new QuizOptionSeed("Two pointers on a sorted copy of the array only", false),
                        new QuizOptionSeed("Binary search over the array's values", false),
                    ]),
                new QuizQuestionSeed(
                    "Why is a two-pointer sliding window O(n) rather than O(n²), despite an inner while loop nested inside an outer for loop?",
                    "Both `left` and `right` only ever move forward and never reset back — across the entire run, each pointer advances at most n times total, so the combined work across all iterations of the inner loop is bounded by O(n), not O(n) per outer step.",
                    [
                        new QuizOptionSeed("Each pointer moves forward at most n times total across the whole run, so total movement is bounded by O(n)", true),
                        new QuizOptionSeed("It secretly isn't O(n) — it's O(n²), it just runs fast on small inputs", false),
                        new QuizOptionSeed("Because it uses a hash map internally to skip elements", false),
                        new QuizOptionSeed("It only works correctly on inputs smaller than about 1000 elements", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Window Sliding Technique (GeeksforGeeks)", "https://www.geeksforgeeks.org/window-sliding-technique/", LinkType.FurtherReading),
                new ReferenceLinkSeed("Longest Substring Without Repeating Characters (LeetCode)", "https://leetcode.com/problems/longest-substring-without-repeating-characters/", LinkType.FurtherReading),
            ],
            prerequisites: [lesson1]);

        var lesson3Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson3.Slug,
        [
            "Implement 'Longest Substring Without Repeating Characters' from scratch without checking the solution",
            "Explain out loud when a fixed-size window applies vs. when a variable-size window applies",
            "Find one brute-force nested-loop scan in your own code that a sliding window could replace",
        ]);

        var lesson4 = BuildLesson(
            slug: "trees-recursive-traversal",
            title: "Trees & Recursive Traversal: DFS, BFS, and BST Basics",
            summary: "Recursive depth-first traversal, iterative breadth-first traversal, and the binary search tree invariant that makes search O(log n).",
            estimatedMinutes: 50,
            objectives:
            [
                "Implement the three DFS traversal orders (preorder, inorder, postorder) recursively",
                "Implement BFS (level-order traversal) iteratively using a queue",
                "Explain the BST invariant and why an inorder traversal of a BST yields sorted output",
                "Choose DFS vs. BFS based on what the problem actually asks for",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    A **binary tree** is a node with up to two children (`left`, `right`). There are two fundamentally different ways to visit every node:

                    - **Depth-first search (DFS)** — go as deep as possible down one branch before backtracking. Naturally recursive: a tree is defined in terms of smaller trees (its subtrees), so a recursive function calling itself on `node.Left` and `node.Right` mirrors the data structure's own definition. DFS has three orderings, based on *when* you visit the current node relative to its children: **preorder** (node, left, right), **inorder** (left, node, right), **postorder** (left, right, node).
                    - **Breadth-first search (BFS)** — visit level by level, left to right. Implemented iteratively with a queue: dequeue a node, enqueue its children, repeat — the queue's FIFO order naturally produces level-by-level output.

                    A **binary search tree (BST)** adds one invariant on top of the shape: for every node, everything in its left subtree is smaller, and everything in its right subtree is larger. That invariant is what makes search, insert, and delete run in `O(log n)` on a balanced tree (each comparison eliminates one whole subtree — the same idea as binary search on an array) — and it's also why an **inorder** traversal of a BST always visits nodes in sorted order.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    DFS is like exploring a maze by always taking the first turn you see and following that corridor all the way to a dead end before backtracking to try the next branch — you go deep before you go wide.

                    BFS is like ripples spreading out from a stone dropped in water — everything at distance 1 gets visited before anything at distance 2, expanding outward one ring at a time. That's also why BFS is the natural tool for "shortest path in an unweighted graph/tree" — it finds the nearest thing first, guaranteed.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **DFS traversal orders**

                    - Preorder (node, left, right) — good for copying/serializing a tree
                    - Inorder (left, node, right) — yields sorted order on a BST
                    - Postorder (left, right, node) — good for deleting/freeing a tree bottom-up

                    **BFS**

                    - Iterative, uses a `Queue<T>`
                    - Natural fit for level-order output and shortest-path-in-unweighted-structure problems

                    **BST invariant**

                    - left subtree < node < right subtree, for every node
                    - Search/insert: O(log n) average (balanced), O(n) worst case (degenerates toward a linked list)
                    """, 3),
                Block(BlockType.CodeSnippet, "Recursive Inorder DFS and Iterative BFS Level-Order", BodyFormat.PlainText, """
                    public class TreeNode
                    {
                        public int Value;
                        public TreeNode? Left;
                        public TreeNode? Right;
                    }

                    // Inorder DFS (recursive): left, node, right -> sorted order on a BST.
                    public void InorderTraversal(TreeNode? node, List<int> result)
                    {
                        if (node is null) return; // base case

                        InorderTraversal(node.Left, result);
                        result.Add(node.Value);
                        InorderTraversal(node.Right, result);
                    }

                    // BFS (iterative): level-order traversal using a queue.
                    public List<List<int>> LevelOrder(TreeNode? root)
                    {
                        var levels = new List<List<int>>();
                        if (root is null) return levels;

                        var queue = new Queue<TreeNode>();
                        queue.Enqueue(root);

                        while (queue.Count > 0)
                        {
                            var levelSize = queue.Count;
                            var currentLevel = new List<int>();

                            for (var i = 0; i < levelSize; i++)
                            {
                                var node = queue.Dequeue();
                                currentLevel.Add(node.Value);

                                if (node.Left is not null) queue.Enqueue(node.Left);
                                if (node.Right is not null) queue.Enqueue(node.Right);
                            }

                            levels.Add(currentLevel);
                        }

                        return levels;
                    }
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "DFS vs. BFS on the Same Tree", BodyFormat.AsciiArt, """
                                4
                              /   \\
                             2     6
                            / \\   / \\
                           1   3 5   7

                    Inorder DFS (left, node, right):
                      1, 2, 3, 4, 5, 6, 7          <- sorted, because this is a BST

                    BFS / level order (queue: dequeue, enqueue children):
                      Level 0: 4
                      Level 1: 2, 6
                      Level 2: 1, 3, 5, 7
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Always null-check before recursing (`if (node is null) return;`) as the very first line of a recursive tree function — this is the base case, and skipping it is the most common source of a `NullReferenceException` in tree code.

                    Pick the traversal to match the question: need sorted output from a BST -> inorder; need to clone/serialize a tree -> preorder; need to safely delete a tree bottom-up -> postorder; need the shortest path or level-by-level structure -> BFS, not DFS.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    When given a tree problem, say which traversal you're choosing and why before writing code: "Since this is a BST and I need sorted output, I'll do an inorder traversal" demonstrates you understand *why* the traversal produces that order, not just that you memorized three names.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Forgetting the null base case in a recursive traversal, causing a `NullReferenceException` the first time recursion reaches a leaf's non-existent child.

                    Also common: reaching for DFS on a "shortest path" or "minimum depth" style question, when BFS is what actually guarantees finding the nearest answer first — DFS can find *a* path, but not necessarily the shortest one, without extra bookkeeping BFS gets for free.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "You need to print the values of a binary search tree in ascending sorted order. Which traversal should you use?",
                    "Inorder visits the left subtree, then the current node, then the right subtree. On a BST that means all-smaller values, then the current value, then all-larger values — which is exactly sorted order.",
                    [
                        new QuizOptionSeed("Inorder traversal (left, node, right)", true),
                        new QuizOptionSeed("Preorder traversal (node, left, right)", false),
                        new QuizOptionSeed("Postorder traversal (left, right, node)", false),
                        new QuizOptionSeed("BFS / level-order traversal", false),
                    ]),
                new QuizQuestionSeed(
                    "You need the minimum depth (shortest root-to-leaf path) of a binary tree. Why is BFS usually preferred over plain recursive DFS here?",
                    "BFS explores level by level, so the very first leaf it reaches is guaranteed to be the shallowest one — it can stop immediately. Plain DFS would need to fully explore multiple branches and compare their depths to find the minimum, doing extra work BFS avoids by construction.",
                    [
                        new QuizOptionSeed("BFS visits nodes level by level, so the first leaf it finds is guaranteed to be the shallowest", true),
                        new QuizOptionSeed("DFS is asymptotically slower than BFS on every tree", false),
                        new QuizOptionSeed("DFS cannot be implemented recursively on a binary tree", false),
                        new QuizOptionSeed("BFS always uses less memory than DFS, regardless of tree shape", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Tree Traversals: Inorder, Preorder, Postorder (GeeksforGeeks)", "https://www.geeksforgeeks.org/tree-traversals-inorder-preorder-and-postorder/", LinkType.FurtherReading),
                new ReferenceLinkSeed("LeetCode Explore: Binary Tree", "https://leetcode.com/explore/learn/card/data-structure-tree/", LinkType.FurtherReading),
            ],
            prerequisites: [lesson2]);

        var lesson4Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson4.Slug,
        [
            "Implement all three DFS traversals (preorder, inorder, postorder) recursively from scratch",
            "Implement BFS level-order traversal iteratively using a Queue<T>, without looking at the solution",
            "Explain why inorder traversal of a BST yields sorted output, using the BST invariant",
        ]);

        var module = BuildModule(topicId, "arrays-and-hashing", "Arrays & Hashing",
            "The foundational patterns — hash maps, two pointers, binary search, sliding windows, and tree traversal — that unlock most array, string, and tree problems.",
            220, [lesson1, lesson2, lesson3, lesson4]);

        return (module, [lesson1Checklist, lesson2Checklist, lesson3Checklist, lesson4Checklist]);
    }

    private static (Module, List<ChecklistSeed>) BuildDsaGraphsModule(int topicId)
    {
        var lesson1 = BuildLesson(
            slug: "graph-traversal-topological-sort",
            title: "Graph Traversal & Topological Sort",
            summary: "Adjacency-list representation, BFS/DFS on graphs, cycle detection, and topological sort via Kahn's algorithm.",
            estimatedMinutes: 45,
            objectives:
            [
                "Represent a graph as an adjacency list in C# and explain why it beats an adjacency matrix for sparse graphs",
                "Implement BFS and DFS traversal on a graph, tracking visited nodes to avoid infinite loops on cycles",
                "Detect a cycle in a directed graph using the recursion-stack technique, not just a visited set",
                "Produce a valid topological order using Kahn's algorithm and explain what a leftover, unordered node means",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    A **graph** is a set of nodes (vertices) connected by edges, which may be **directed** (a one-way relationship, like "course A must come before course B") or **undirected** (a two-way relationship, like "these two people are friends"), and **weighted** or **unweighted**.

                    Two common representations:

                    - **Adjacency matrix** — an `n x n` grid where `matrix[i][j]` is true/weighted if an edge exists. `O(1)` edge lookup, but `O(n^2)` space even for a sparse graph.
                    - **Adjacency list** — a map from each node to the list of its neighbors. `O(V + E)` space, which is far smaller than `O(V^2)` for sparse graphs (most interview graphs are sparse), at the cost of `O(degree)` edge lookup instead of `O(1)`.

                    Both BFS and DFS visit every node and every edge once, so both run in `O(V + E)` time on an adjacency list. The difference is *order*: BFS explores level by level using a queue (nearest nodes first — the right tool for shortest path in an unweighted graph); DFS explores as deep as possible before backtracking, using recursion or an explicit stack.

                    A **topological sort** is a linear ordering of a directed graph's nodes such that every edge `u -> v` has `u` appearing before `v` — it only exists if the graph is a **DAG** (directed, acyclic graph). If the graph has a cycle, no valid ordering exists.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    A directed graph of course prerequisites is the clearest mental model for topological sort: "Intro to CS" must come before "Data Structures," which must come before "Algorithms." A topological sort is just a valid semester-by-semester course plan that never schedules a class before its prerequisite — and if the prerequisites contain a cycle (A requires B, B requires A), no valid plan can ever exist, which is exactly why cycle detection and topological sort are two sides of the same coin.

                    BFS is like a phone tree — you call your direct contacts first, then they call their direct contacts, spreading out one ring at a time, which is why it finds the *nearest* person first. DFS is like fully exploring one hallway of a building to its end before backtracking to try the next hallway.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Complexity (adjacency list, V vertices, E edges)**

                    - Build adjacency list from an edge list: `O(V + E)`
                    - BFS / DFS traversal: `O(V + E)` time, `O(V)` space (visited set + queue/recursion stack)
                    - Kahn's algorithm (topological sort): `O(V + E)` time, `O(V)` space

                    **Cycle detection in a DIRECTED graph**

                    - A plain "visited" set is NOT enough — reaching an already-visited node from a *different, finished* branch is fine (not a cycle)
                    - You need a second set: nodes currently on the **recursion stack** (in progress). Reaching one of *those* is a back-edge — a real cycle.

                    **Kahn's algorithm (BFS on in-degrees)**

                    1. Compute in-degree (number of incoming edges) for every node
                    2. Enqueue all nodes with in-degree 0 (no unmet prerequisites)
                    3. Dequeue a node, add it to the order, decrement its neighbors' in-degrees
                    4. Any neighbor whose in-degree just hit 0 gets enqueued
                    5. If the final order has fewer nodes than the graph, the rest form a cycle
                    """, 3),
                Block(BlockType.CodeSnippet, "Adjacency List, BFS, DFS, Cycle Detection, and Kahn's Algorithm", BodyFormat.PlainText, """
                    // Adjacency list representation: node -> list of neighbors it points to.
                    public class Graph
                    {
                        private readonly Dictionary<int, List<int>> _adjacency = new();

                        public void AddEdge(int from, int to)
                        {
                            if (!_adjacency.ContainsKey(from)) _adjacency[from] = new List<int>();
                            if (!_adjacency.ContainsKey(to)) _adjacency[to] = new List<int>();
                            _adjacency[from].Add(to); // directed edge: from -> to
                        }

                        public IReadOnlyList<int> Neighbors(int node) =>
                            _adjacency.TryGetValue(node, out var list) ? list : new List<int>();

                        public IEnumerable<int> Nodes => _adjacency.Keys;
                    }

                    // BFS: explores level by level using a queue. O(V + E).
                    public List<int> Bfs(Graph graph, int start)
                    {
                        var visited = new HashSet<int> { start };
                        var order = new List<int>();
                        var queue = new Queue<int>();
                        queue.Enqueue(start);

                        while (queue.Count > 0)
                        {
                            var node = queue.Dequeue();
                            order.Add(node);

                            foreach (var neighbor in graph.Neighbors(node))
                            {
                                if (visited.Add(neighbor)) // Add returns false if already present
                                {
                                    queue.Enqueue(neighbor);
                                }
                            }
                        }

                        return order;
                    }

                    // DFS (recursive): explores as deep as possible before backtracking.
                    public void Dfs(Graph graph, int node, HashSet<int> visited, List<int> order)
                    {
                        if (!visited.Add(node)) return; // already visited -> stop (avoids infinite loops on cycles)

                        order.Add(node);
                        foreach (var neighbor in graph.Neighbors(node))
                        {
                            Dfs(graph, neighbor, visited, order);
                        }
                    }

                    // Cycle detection in a DIRECTED graph: track nodes on the CURRENT
                    // recursion stack, not just visited-ever. A back-edge to a node
                    // still on the stack means a cycle.
                    public bool HasCycle(Graph graph)
                    {
                        var visited = new HashSet<int>();
                        var onStack = new HashSet<int>();

                        bool Visit(int node)
                        {
                            if (onStack.Contains(node)) return true;  // back-edge -> cycle found
                            if (visited.Contains(node)) return false; // already fully explored, no cycle here

                            visited.Add(node);
                            onStack.Add(node);

                            foreach (var neighbor in graph.Neighbors(node))
                            {
                                if (Visit(neighbor)) return true;
                            }

                            onStack.Remove(node); // done exploring this node's branch
                            return false;
                        }

                        return graph.Nodes.Any(Visit);
                    }

                    // Topological sort via Kahn's algorithm (BFS on in-degrees).
                    public List<int> TopologicalSort(Graph graph)
                    {
                        var inDegree = graph.Nodes.ToDictionary(n => n, _ => 0);
                        foreach (var node in graph.Nodes)
                        {
                            foreach (var neighbor in graph.Neighbors(node))
                            {
                                inDegree[neighbor] = inDegree.GetValueOrDefault(neighbor) + 1;
                            }
                        }

                        var queue = new Queue<int>(inDegree.Where(kv => kv.Value == 0).Select(kv => kv.Key));
                        var order = new List<int>();

                        while (queue.Count > 0)
                        {
                            var node = queue.Dequeue();
                            order.Add(node);

                            foreach (var neighbor in graph.Neighbors(node))
                            {
                                inDegree[neighbor]--;
                                if (inDegree[neighbor] == 0) queue.Enqueue(neighbor);
                            }
                        }

                        if (order.Count != inDegree.Count)
                        {
                            throw new InvalidOperationException("Graph has a cycle -- no valid topological order exists.");
                        }

                        return order;
                    }
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "Tracing Kahn's Algorithm on a Prerequisite DAG", BodyFormat.AsciiArt, """
                    Course prerequisite graph (edge = "must take before"):

                      101 --> 201 --> 301
                       \\            /
                        ----> 202 --

                    In-degrees:  101:0   201:1   202:1   301:2

                    Queue: [101]                          Order: []
                      dequeue 101 -> order:[101]
                      neighbors 201,202: in-degree--  ->  201:0, 202:0
                      enqueue 201, 202                    Queue: [201, 202]

                      dequeue 201 -> order:[101,201]
                      neighbor 301: in-degree-- -> 301:1   Queue: [202]

                      dequeue 202 -> order:[101,201,202]
                      neighbor 301: in-degree-- -> 301:0
                      enqueue 301                          Queue: [301]

                      dequeue 301 -> order:[101,201,202,301]
                                                            Queue: []

                    Topological order: 101, 201, 202, 301
                    (order.Count == 4 == total nodes -> no cycle)
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    For directed-graph cycle detection, always maintain two sets: `visited` (ever explored) and `onStack` (on the *current* path). Checking `visited` alone gives false positives — two independent branches can legitimately point to the same already-finished node without forming a cycle.

                    Prefer an adjacency list over a matrix by default in interviews unless the graph is dense or you need `O(1)` "is there an edge between u and v" lookups — most real-world and interview graphs are sparse, and the space savings are usually the whole point of bringing it up.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    When a problem mentions "prerequisites," "dependencies," "build order," or "course schedule," say the words "topological sort" out loud immediately — it's one of the most recognizable interview signals, and naming it early shows pattern recognition before you've written a line of code. Then explicitly check: "is this a DAG, or do I need to detect a cycle first?"
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Using a single `visited` set to detect cycles in a *directed* graph — this only works for undirected graphs. In a directed graph, revisiting a finished node from another branch is normal and not a cycle; only revisiting a node still on the active recursion stack is.

                    Also common: forgetting that Kahn's algorithm's final `order.Count` must equal the total node count — silently returning a partial order without checking for leftover nodes hides the fact that a cycle exists.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "You run DFS on a DIRECTED graph and mark nodes 'visited' the moment you first reach them, never removing them. Why is a visited-only set NOT enough to detect a cycle?",
                    "Visited-only tracking can't distinguish a cross-edge to an already-finished node (fine, not a cycle) from a back-edge to a node still on the current recursion stack (a real cycle). You need a second set -- nodes currently 'in progress' on the active path -- to tell the two apart.",
                    [
                        new QuizOptionSeed("Visited-only tracking can't distinguish a finished cross-edge from a back-edge to a node still on the current recursion stack", true),
                        new QuizOptionSeed("DFS cannot be run on directed graphs at all", false),
                        new QuizOptionSeed("Directed graphs never contain cycles by definition", false),
                        new QuizOptionSeed("Cycle detection requires BFS instead of DFS", false),
                    ]),
                new QuizQuestionSeed(
                    "After running Kahn's algorithm, the resulting topological order contains fewer nodes than the graph has in total. What does that indicate?",
                    "A node only ever enters the queue once its in-degree reaches 0. Nodes stuck in a cycle can never reach in-degree 0 (each depends on another in the cycle), so they're permanently excluded from the order -- a shortfall means the leftover nodes form a cycle.",
                    [
                        new QuizOptionSeed("The leftover nodes form a cycle among themselves", true),
                        new QuizOptionSeed("The graph is merely disconnected but still acyclic", false),
                        new QuizOptionSeed("The leftover nodes simply have no edges at all", false),
                        new QuizOptionSeed("The queue was dequeued in the wrong order", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Topological Sorting (GeeksforGeeks)", "https://www.geeksforgeeks.org/topological-sorting/", LinkType.FurtherReading),
                new ReferenceLinkSeed("LeetCode Explore: Graph", "https://leetcode.com/explore/learn/card/graph/", LinkType.FurtherReading),
            ]);

        var lesson1Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson1.Slug,
        [
            "Build an adjacency list from a raw edge list and implement BFS and DFS on it from scratch",
            "Implement directed-graph cycle detection using the recursion-stack technique, without looking at the solution",
            "Implement Kahn's algorithm for topological sort and trace it by hand on a 4-node DAG",
        ]);

        var lesson2 = BuildLesson(
            slug: "dynamic-programming-fundamentals",
            title: "Dynamic Programming Fundamentals",
            summary: "Recognizing overlapping subproblems and optimal substructure, then solving them with memoization or tabulation.",
            estimatedMinutes: 45,
            objectives:
            [
                "Identify overlapping subproblems and optimal substructure in a problem before writing any code",
                "Implement a top-down memoized solution using a cache to avoid recomputing the same subproblem",
                "Convert a memoized recursive solution into a bottom-up tabulated one, and further into O(1) space where possible",
                "Solve classic DP problems (climbing stairs, coin change) using both memoization and tabulation",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    **Dynamic programming (DP)** applies when a problem has two properties:

                    - **Overlapping subproblems** — a naive recursive solution solves the *exact same smaller subproblem* many times (e.g., `climbStairs(3)` gets recomputed dozens of times while computing `climbStairs(10)`).
                    - **Optimal substructure** — the optimal answer to the whole problem can be built directly from optimal answers to its subproblems (e.g., `ways(n) = ways(n-1) + ways(n-2)`).

                    There are two equivalent implementation styles:

                    - **Memoization (top-down)** — write the natural recursive solution, then cache each subproblem's result the first time it's computed (usually in a dictionary or array keyed by the subproblem's parameters). Every later call for the same input becomes an `O(1)` cache hit.
                    - **Tabulation (bottom-up)** — flip the recursion into an iterative loop that fills a table from the base cases upward, so every subproblem is computed exactly once, in dependency order, with no recursion or call-stack overhead. Often lets you shrink the table to just the last one or two values (rolling variables), reaching `O(1)` extra space.

                    Both give the same asymptotic time complexity, roughly "number of distinct subproblems x work per subproblem" — the whole point of DP is collapsing exponential brute force into that much smaller number.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Memoization is like a student solving a hard homework set who writes each sub-answer on a sticky note the first time they work it out — the next time that exact same sub-question comes up in a later problem, they just glance at the sticky note instead of re-deriving it from scratch.

                    Tabulation is like filling in a multiplication table starting from `1 x 1` and working up to `12 x 12` in order — by the time you need `7 x 8`, you've already filled in everything smaller, so you never have to stop and go compute a prerequisite value mid-row.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Climbing Stairs** (ways to reach step n, moving 1 or 2 steps at a time)

                    - Naive recursion: `O(2^n)` time — recomputes the same `climbStairs(k)` exponentially many times
                    - Memoized (top-down): `O(n)` time, `O(n)` space (cache + call stack)
                    - Tabulated (bottom-up, rolling variables): `O(n)` time, `O(1)` space

                    **Coin Change** (fewest coins to make an amount; -1 if impossible)

                    - Tabulated: `O(amount x coins.Length)` time, `O(amount)` space
                    - Recurrence: `minCoins[a] = 1 + min(minCoins[a - coin])` over every usable coin

                    **Spotting DP in an interview**

                    - The brute force is recursive and clearly re-solves identical smaller inputs -> overlapping subproblems
                    - The problem asks for a "best/min/max/count of ways" value built from smaller instances of itself -> optimal substructure
                    """, 3),
                Block(BlockType.CodeSnippet, "Climbing Stairs (Memo + Tabulation) and Coin Change (Tabulation)", BodyFormat.PlainText, """
                    // --- Climbing Stairs: top-down memoization ---
                    // "How many distinct ways to climb n stairs, taking 1 or 2 steps at a time?"
                    public int ClimbStairsMemo(int n, Dictionary<int, int>? cache = null)
                    {
                        cache ??= new Dictionary<int, int>();

                        if (n <= 2) return n; // base cases: 1 way for 1 stair, 2 ways for 2 stairs
                        if (cache.TryGetValue(n, out var cached)) return cached; // overlapping subproblem already solved

                        var result = ClimbStairsMemo(n - 1, cache) + ClimbStairsMemo(n - 2, cache);
                        cache[n] = result; // store before returning
                        return result;
                    }

                    // --- Climbing Stairs: bottom-up tabulation, O(1) space ---
                    // Optimal substructure: ways(n) = ways(n-1) + ways(n-2), so we only ever
                    // need the previous two values, not a full table.
                    public int ClimbStairsTabulation(int n)
                    {
                        if (n <= 2) return n;

                        var oneStepBack = 2;
                        var twoStepsBack = 1;

                        for (var i = 3; i <= n; i++)
                        {
                            var current = oneStepBack + twoStepsBack;
                            twoStepsBack = oneStepBack;
                            oneStepBack = current;
                        }

                        return oneStepBack;
                    }

                    // --- Coin Change: bottom-up tabulation ---
                    // Fewest coins to make `amount`; -1 if impossible.
                    public int CoinChange(int[] coins, int amount)
                    {
                        var minCoins = new int[amount + 1];
                        Array.Fill(minCoins, amount + 1); // sentinel meaning "not yet reachable"
                        minCoins[0] = 0; // base case: 0 coins needed to make amount 0

                        for (var subAmount = 1; subAmount <= amount; subAmount++)
                        {
                            foreach (var coin in coins)
                            {
                                if (coin <= subAmount)
                                {
                                    minCoins[subAmount] = Math.Min(minCoins[subAmount], minCoins[subAmount - coin] + 1);
                                }
                            }
                        }

                        return minCoins[amount] > amount ? -1 : minCoins[amount];
                    }
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "Overlapping Subproblems in climbStairs(5)", BodyFormat.AsciiArt, """
                    climbStairs(5)
                    +-- climbStairs(4)
                    |    +-- climbStairs(3)
                    |    |    +-- climbStairs(2)
                    |    |    +-- climbStairs(1)
                    |    +-- climbStairs(2)          <-- recomputed! (overlapping subproblem)
                    +-- climbStairs(3)               <-- recomputed! (overlapping subproblem)
                         +-- climbStairs(2)          <-- recomputed again!
                         +-- climbStairs(1)          <-- recomputed again!

                    Without memoization: the same climbStairs(k) is re-derived from
                    scratch every time it reappears -> O(2^n) total calls.

                    With memoization: each distinct climbStairs(k) for k = 1..5 is
                    computed exactly once and cached -> only 5 real computations,
                    every repeat is an O(1) lookup -> O(n) total.
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Always write the naive recursive solution first and identify the recurrence relation (e.g., `ways(n) = ways(n-1) + ways(n-2)`) before jumping to code — memoization and tabulation are just two mechanical ways to execute the same recurrence efficiently, and neither makes sense until the recurrence itself is right.

                    Once a tabulated solution only reads the last one or two entries of the table (as in Climbing Stairs), replace the array with a couple of rolling variables to drop space from `O(n)` to `O(1)` — a common interview follow-up question.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    Name the two DP properties out loud before coding: "This has overlapping subproblems because [specific reason], and optimal substructure because [specific reason]." Interviewers use this exact vocabulary to gauge whether you actually understand DP or are just pattern-matching to "the problem looks like a past LeetCode question."
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Adding a cache dictionary to a recursive solution but forgetting to check it *before* recomputing, or forgetting to actually store the result *after* computing it — either mistake silently degrades back to the exponential naive solution while looking like memoization.

                    Also common: getting the base cases wrong (e.g., off-by-one on `climbStairs(0)` or `climbStairs(1)`), which then quietly propagates a wrong answer through every larger subproblem built on top of it.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "A naive recursive climbStairs(n) runs in O(2^n) time. After adding memoization (caching each climbStairs(k) result the first time it's computed), what is the new time complexity, and why?",
                    "There are only n distinct subproblems -- climbStairs(1) through climbStairs(n). Memoization guarantees each one is fully computed exactly once; every later call for the same k becomes an O(1) cache lookup, so total work is O(n).",
                    [
                        new QuizOptionSeed("O(n) -- each distinct subproblem is computed exactly once and cached", true),
                        new QuizOptionSeed("Still O(2^n) -- memoization doesn't help recursive solutions", false),
                        new QuizOptionSeed("O(log n) -- memoization turns it into a binary search", false),
                        new QuizOptionSeed("O(n^2) -- every cache lookup costs an extra linear scan", false),
                    ]),
                new QuizQuestionSeed(
                    "What two properties must a problem have for dynamic programming to apply?",
                    "Overlapping subproblems (the same smaller subproblem recurs many times in a naive recursive solution) and optimal substructure (the optimal answer to the whole problem can be built directly from optimal answers to its subproblems).",
                    [
                        new QuizOptionSeed("Overlapping subproblems and optimal substructure", true),
                        new QuizOptionSeed("Sorted input and a monotonic condition", false),
                        new QuizOptionSeed("A single base case and no recursion allowed", false),
                        new QuizOptionSeed("O(1) space and tail recursion", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Dynamic Programming (GeeksforGeeks)", "https://www.geeksforgeeks.org/dynamic-programming/", LinkType.FurtherReading),
                new ReferenceLinkSeed("LeetCode Explore: Dynamic Programming", "https://leetcode.com/explore/learn/card/dynamic-programming/", LinkType.FurtherReading),
            ]);

        var lesson2Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson2.Slug,
        [
            "Solve Climbing Stairs with a memoized recursive solution, then convert it to bottom-up tabulation",
            "Solve Coin Change with tabulation and explain the sentinel value used for 'unreachable'",
            "For one classic DP problem, state its overlapping subproblems and optimal substructure out loud before writing any code",
        ]);

        var module = BuildModule(topicId, "graphs-and-dynamic-programming", "Graphs & Dynamic Programming",
            "Graph representations and traversal algorithms — BFS, DFS, cycle detection, topological sort — plus the memoization and tabulation techniques that turn exponential brute force into polynomial time.",
            90, [lesson1, lesson2], sortOrder: 2);

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

        var lesson3 = BuildLesson(
            slug: "message-queues-async-communication",
            title: "Message Queues & Asynchronous Communication",
            summary: "Decoupling services with queues and pub/sub, and the delivery guarantees you actually get in practice.",
            estimatedMinutes: 45,
            objectives:
            [
                "Explain why asynchronous messaging decouples producers and consumers in both time and load",
                "Distinguish a point-to-point queue from a publish/subscribe topic and know when to use each",
                "Explain at-least-once, at-most-once, and exactly-once delivery, and why 'exactly-once' end-to-end is effectively a myth",
                "Design a consumer that stays correct even when it receives the same message twice",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    A **message queue** decouples a producer from a consumer: the producer writes a message to a broker and moves on immediately, instead of blocking on the consumer actually processing it. This absorbs traffic spikes (the queue grows instead of the consumer falling over) and lets producer and consumer scale, deploy, and fail independently.

                    There are two core messaging shapes:

                    - **Point-to-point (queue)** — each message is delivered to exactly one consumer out of a pool. Good for distributing work items (e.g., "resize this image") across a pool of workers.
                    - **Publish/subscribe (topic)** — each message is fanned out to *every* subscriber. Good for broadcasting an event (e.g., "order placed") to multiple independent services that each need to react.

                    Real systems (Kafka, RabbitMQ, SQS/SNS) blend both: Kafka topics are partitioned, and each partition behaves like a point-to-point queue *within* a consumer group, while multiple consumer groups each get their own full copy of the stream — pub/sub across groups, point-to-point within a group.

                    **Delivery guarantees** describe what happens when something fails mid-delivery:

                    - **At-most-once** — a message might be lost, but is never redelivered. Fire-and-forget.
                    - **At-least-once** — a message is never lost, but might be redelivered (e.g., consumer crashes after processing but before acknowledging). This is the practical default for most brokers.
                    - **Exactly-once** — the message is processed precisely once. True end-to-end exactly-once requires cooperation between the broker *and* the consumer's side effects (e.g., a transactional write that records "processed message #123" atomically with the business update) — the broker alone cannot guarantee it.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    A message queue is like dropping a letter in a mailbox instead of hand-delivering it and waiting on the doorstep — you trust the postal system (the broker) to get it there, and you're free to walk away and do something else the moment it's in the box.

                    A point-to-point queue is like a single ticket-number line at a deli counter — each ticket is served by exactly one clerk. Pub/sub is like a store's PA announcement — everyone in the building hears the same page, and each department reacts to it independently.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Queue vs. topic**

                    - Queue (point-to-point) — one consumer per message, used to distribute work across a pool
                    - Topic (pub/sub) — every subscriber gets a copy, used to broadcast events

                    **Delivery guarantees**

                    - At-most-once — may lose messages, never duplicates
                    - At-least-once — never loses messages, may duplicate (the common default)
                    - Exactly-once — requires idempotent, transactional consumer logic; the broker can't provide this alone

                    **Common brokers**

                    - Kafka — partitioned log, high throughput, consumer groups, replay-able
                    - RabbitMQ — flexible routing (exchanges), strong point-to-point queue semantics
                    - SQS/SNS — managed queue (SQS) + managed pub/sub (SNS), at-least-once by default
                    """, 3),
                Block(BlockType.CodeSnippet, "An Idempotent Consumer (Deduping At-Least-Once Delivery)", BodyFormat.PlainText, """
                    // The broker guarantees at-least-once delivery, so this message
                    // might arrive twice. Track processed message IDs so a duplicate
                    // is a safe no-op instead of double-charging a customer.
                    public async Task HandleOrderPaidAsync(OrderPaidMessage message)
                    {
                        var alreadyProcessed = await db.ProcessedMessages
                            .AnyAsync(m => m.MessageId == message.MessageId);

                        if (alreadyProcessed)
                        {
                            return; // duplicate delivery — safe to ignore
                        }

                        await using var transaction = await db.Database.BeginTransactionAsync();

                        await fulfillmentService.ShipOrderAsync(message.OrderId);
                        db.ProcessedMessages.Add(new ProcessedMessage { MessageId = message.MessageId });
                        await db.SaveChangesAsync();

                        await transaction.CommitAsync(); // side effect + dedupe record commit together
                    }
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "Producer to Consumer Group, Through a Broker", BodyFormat.StructuredSteps, """
                    [{"label":"Producer"},{"label":"Broker","note":"queue or partitioned topic"},{"label":"Consumer Group A","note":"work distributed across N workers"},{"label":"Consumer Group B","note":"separate copy of the stream, e.g. analytics"},{"label":"Dead-Letter Queue","note":"messages that fail repeatedly land here, not lost"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Make consumers **idempotent** by design — track a message/idempotency ID and short-circuit on a duplicate — rather than assuming the broker will never redeliver. At-least-once is the realistic default, so idempotency is what actually makes that safe.

                    Configure a **dead-letter queue** for messages that fail processing repeatedly, instead of retrying forever or silently dropping them — it keeps a poison message from blocking the whole queue while preserving it for investigation.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    When a design calls for messaging, state the delivery guarantee you're assuming out loud ("I'll assume at-least-once, so the consumer needs to be idempotent") before writing consumer logic — interviewers use this as a strong signal you understand that "exactly-once" is not something you get for free just by picking a fancy broker.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Assuming a message broker provides exactly-once delivery out of the box, then writing a consumer that isn't idempotent — a redelivered message silently double-processes (double-charges, double-ships, double-emails).

                    Also common: forgetting that ordering is only guaranteed *within* a single partition/queue, not across the whole topic — a design that depends on global ordering across multiple partitions will see events arrive out of order under load.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "A consumer crashes right after processing a message but before acknowledging it to the broker. Under at-least-once delivery, what happens?",
                    "Since the broker never received the acknowledgment, it assumes the message wasn't processed and redelivers it — which is exactly why at-least-once consumers must be idempotent, since the same message can legitimately be processed more than once.",
                    [
                        new QuizOptionSeed("The message is permanently lost", false),
                        new QuizOptionSeed("The broker redelivers the message, so the consumer may process it again", true),
                        new QuizOptionSeed("The broker automatically detects the duplicate and skips redelivery", false),
                        new QuizOptionSeed("The entire queue is paused until manually restarted", false),
                    ]),
                new QuizQuestionSeed(
                    "An 'order placed' event needs to reach three independent services (billing, shipping, analytics), each processing it fully on their own. Which messaging shape fits?",
                    "Publish/subscribe (a topic) fans the same event out to every subscriber — each service gets its own copy and reacts independently, unlike a point-to-point queue where only one consumer in a pool would receive it.",
                    [
                        new QuizOptionSeed("A single point-to-point queue shared by all three services", false),
                        new QuizOptionSeed("Publish/subscribe, so each service receives its own copy of the event", true),
                        new QuizOptionSeed("A synchronous HTTP call chained across all three services", false),
                        new QuizOptionSeed("There's no way to deliver one event to three consumers", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("System Design Primer: Communication", "https://github.com/donnemartin/system-design-primer#communication", LinkType.FurtherReading),
                new ReferenceLinkSeed("Amazon SQS: At-Least-Once Delivery", "https://docs.aws.amazon.com/AWSSimpleQueueService/latest/SQSDeveloperGuide/FIFO-queues.html", LinkType.OfficialDocs),
            ],
            prerequisites: [lesson1]);

        var lesson3Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson3.Slug,
        [
            "Draw the producer -> broker -> consumer group -> dead-letter queue diagram from memory",
            "Explain why at-least-once delivery requires an idempotent consumer, using your own example",
            "Describe a real scenario where point-to-point queueing is the wrong fit and pub/sub is correct",
        ]);

        var lesson4 = BuildLesson(
            slug: "cap-theorem-consistency-models",
            title: "CAP Theorem & Consistency Models",
            summary: "Why partition tolerance isn't optional, and the practical spectrum from strong to eventual consistency real systems pick from.",
            estimatedMinutes: 45,
            objectives:
            [
                "Explain why a real distributed system is actually choosing between consistency and availability, not freely picking 2 of 3",
                "Differentiate strong consistency, eventual consistency, and read-your-writes consistency with a concrete example of each",
                "Explain how quorum-based reads/writes (N/W/R) let you tune the consistency-availability trade-off instead of picking one extreme",
                "Choose an appropriate consistency model for a given feature, rather than applying one model to an entire system",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    The **CAP theorem** states that a distributed data store can provide at most two of three guarantees: **C**onsistency (every read sees the latest write), **A**vailability (every request gets a response), and **P**artition tolerance (the system keeps working when network nodes can't reach each other).

                    In practice this isn't really a 3-way menu: network partitions *will* happen, so partition tolerance isn't optional for any system that spans more than one machine. That leaves a real choice only during a partition: return a possibly-stale answer (favor **A**vailability) or refuse to answer until consistency can be guaranteed (favor **C**onsistency). The **PACELC** extension makes this more honest: even *without* a partition (Else), you still trade **L**atency against **C**onsistency.

                    Consistency isn't binary — it's a spectrum:

                    - **Strong consistency** — every read reflects the most recent write, everywhere, immediately (e.g., a bank balance after a transfer).
                    - **Eventual consistency** — replicas converge to the same value *eventually*, with no guarantee about how long "eventually" takes (e.g., a social media like count).
                    - **Read-your-writes consistency** — a middle ground: a user is guaranteed to see their *own* writes immediately, even if other users might briefly see a stale value.

                    Many distributed databases (e.g., Dynamo-style stores) let you tune this per-operation with **quorums**: with `N` replicas, a write must be acknowledged by `W` of them and a read must query `R` of them. Setting `W + R > N` guarantees every read overlaps with at least one replica that saw the latest write — a knob between strict consistency and maximum availability, rather than an all-or-nothing choice.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Strong consistency is like a single shared bank ledger everyone reads from directly — the instant a deposit is written, every teller sees the new balance, no exceptions.

                    Eventual consistency is like a group chat where phones briefly lose signal on a subway — everyone's message eventually shows up for everyone else, but for a few seconds different people can see a different set of messages, and that's an accepted trade-off for the chat never just freezing while it waits for a signal.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **CAP, honestly stated**

                    - Partition tolerance: not optional in a real distributed system
                    - The real trade-off during a partition: Consistency vs. Availability
                    - PACELC: even without a partition, Latency trades against Consistency

                    **Consistency models, from strongest to weakest**

                    - Strong — every read sees the latest write, everywhere
                    - Read-your-writes — you always see your own writes; others may lag
                    - Eventual — replicas converge eventually, no bound on "eventually"

                    **Quorum tuning**: with `N` replicas, `W` = write quorum, `R` = read quorum

                    - `W + R > N` — guarantees strong-ish (quorum) consistency, at some latency cost
                    - `W + R <= N` — faster, more available, but reads can return stale data
                    """, 3),
                Block(BlockType.CodeSnippet, "Choosing a Quorum Per Operation", BodyFormat.PlainText, """
                    // N = 3 replicas total for this key.
                    // Reading an account balance: favor correctness over speed.
                    var balance = await store.ReadAsync(
                        key: $"account:{accountId}",
                        quorum: ReadQuorum.All);          // R = 3, W + R > N guaranteed

                    // Reading a "like" count on a post: favor speed over freshness.
                    var likeCount = await store.ReadAsync(
                        key: $"post:{postId}:likes",
                        quorum: ReadQuorum.One);           // R = 1, may be briefly stale

                    // Same store, two different consistency choices —
                    // picked per feature, not once for the whole system.
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "A Partition Forces a Choice", BodyFormat.AsciiArt, """
                    Before partition:        During partition:

                    [Node A]---[Node B]      [Node A]   X   [Node B]
                       both see writes         network cut, can't sync

                    Client writes to A during the partition. Node B can't hear about it. When a
                    client asks Node B for the value, the system must choose:

                      CP choice: Node B refuses to answer (unavailable) until it can confirm
                                 it has the latest value -> consistent, but not available.

                      AP choice: Node B answers anyway, with what it currently has
                                 -> available, but possibly stale (inconsistent).
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Pick a consistency model **per feature**, not once for the entire system — a bank balance and a "likes" counter living in the same product have very different tolerance for staleness, and forcing both through the same strong-consistency path (or the same eventually-consistent path) is usually wrong for one of them.

                    When using a quorum-based store, state your `N`/`W`/`R` choice explicitly and connect it to the guarantee it buys you (`W + R > N` for read-your-writes-style guarantees) rather than treating the quorum settings as an unexplained default.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    Don't recite "CAP means you pick 2 of 3" as if it's the whole answer — that line undersells your understanding. Instead say partition tolerance is a given for any real distributed system, so the actual decision is Consistency vs. Availability *during a partition*, and mention PACELC to show you know the trade-off exists even when the network is healthy. That distinction is exactly what separates a memorized answer from real understanding.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Treating CAP as "pick any 2 of 3," including partition tolerance as something you could simply opt out of — any system with more than one node over a real network cannot assume partitions won't happen, so this framing misrepresents the theorem.

                    Also common: assuming "NoSQL" automatically means eventual consistency and "SQL" automatically means strong consistency — many NoSQL stores offer tunable/quorum consistency, and the right model depends on the specific feature, not the database category.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "Why is 'partition tolerance' generally not treated as an optional trade-off in CAP discussions?",
                    "Any distributed system spanning more than one machine over a real network will eventually experience a partition, so refusing to tolerate one isn't a viable design choice — the real, live trade-off only exists between Consistency and Availability once a partition occurs.",
                    [
                        new QuizOptionSeed("Because partitions never actually happen in production", false),
                        new QuizOptionSeed("Because a multi-node system over a real network will eventually experience one, so not tolerating it isn't practical", true),
                        new QuizOptionSeed("Because partition tolerance is guaranteed automatically by TCP", false),
                        new QuizOptionSeed("Because CAP only applies to single-node databases", false),
                    ]),
                new QuizQuestionSeed(
                    "In a quorum-based store with N replicas, what does setting W + R > N guarantee?",
                    "If the write quorum and read quorum together exceed the total number of replicas, any read quorum is mathematically guaranteed to overlap with at least one replica that received the latest write — ensuring the read sees it.",
                    [
                        new QuizOptionSeed("That writes will never fail", false),
                        new QuizOptionSeed("That every read quorum overlaps with at least one replica holding the latest write", true),
                        new QuizOptionSeed("That the system will tolerate an unlimited number of partitions", false),
                        new QuizOptionSeed("That reads will always be faster than writes", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("System Design Primer: Consistency Patterns", "https://github.com/donnemartin/system-design-primer#consistency-patterns", LinkType.FurtherReading),
                new ReferenceLinkSeed("Amazon DynamoDB: Read Consistency", "https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/HowItWorks.ReadConsistency.html", LinkType.OfficialDocs),
            ],
            prerequisites: [lesson2]);

        var lesson4Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson4.Slug,
        [
            "Explain, without saying 'pick 2 of 3,' why partition tolerance isn't really optional",
            "Describe one feature that needs strong consistency and one that's fine with eventual consistency, from a real or hypothetical app",
            "Work through why W + R > N guarantees a quorum read sees the latest write",
        ]);

        var module = BuildModule(topicId, "system-design-fundamentals", "System Design Fundamentals",
            "How to scale a single server into a resilient, horizontally-scaled system with a real database strategy, decoupled asynchronous communication, and a defensible consistency model.",
            180, [lesson1, lesson2, lesson3, lesson4]);

        return (module, [lesson1Checklist, lesson2Checklist, lesson3Checklist, lesson4Checklist]);
    }

    private static (Module, List<ChecklistSeed>) BuildApiGatewayAndCdnModule(int topicId)
    {
        var lesson1 = BuildLesson(
            slug: "api-gateway-rate-limiting",
            title: "API Gateway & Rate Limiting Design",
            summary: "What an API gateway centralizes for every service behind it, and the token-bucket and sliding-window algorithms that make rate limiting actually work.",
            estimatedMinutes: 45,
            objectives:
            [
                "Explain what cross-cutting responsibilities an API gateway centralizes on behalf of backend services",
                "Implement the token bucket algorithm and explain why it allows controlled bursts",
                "Compare fixed window, sliding window, and token bucket rate limiting and their failure modes",
                "Decide where to enforce a rate limit (client, gateway, or per-service) and why the gateway is usually right",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    An **API gateway** sits between clients and a fleet of backend services and centralizes work that every service would otherwise have to duplicate:

                    - **Routing** — mapping a public path (`/orders/*`) to the right internal service, hiding internal topology from clients.
                    - **Authentication/authorization** — validating a token or API key once, at the edge, instead of in every downstream service.
                    - **TLS termination** — decrypting HTTPS once at the gateway so internal services can speak plain HTTP within a trusted network.
                    - **Rate limiting & throttling** — protecting backends from being overwhelmed, whether by a traffic spike, a buggy client in a retry loop, or deliberate abuse.
                    - **Request/response transformation and aggregation** — reshaping payloads, or fanning one client request out to several internal services and combining the results.

                    **Rate limiting** specifically answers "how many requests is this client allowed in a given period, and what happens when they exceed it?" Three common algorithms:

                    - **Fixed window** — count requests in discrete windows (e.g., per calendar minute). Simple, but allows up to 2x the limit right across a window boundary (a burst at 0:59 plus a burst at 1:00 both fit their own windows).
                    - **Sliding window** (log or counter) — counts requests in a continuously moving window ending "now," avoiding the boundary-burst problem at the cost of more state.
                    - **Token bucket** — a bucket holds up to `capacity` tokens, refilling at a steady rate; each request consumes one token and is rejected when the bucket is empty. This is the industry-standard choice because it allows a controlled burst (spend the full bucket at once) while still enforcing a steady-state average rate over time.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    An API gateway is like the front desk of a large office building: visitors check in once (auth), get directed to the right floor and department (routing) without knowing the building's internal layout, and the front desk staff turn away anyone trying to bring in an unreasonable number of guests at once (rate limiting) — no individual department has to re-check IDs at their own door.

                    A token bucket is like a movie theater that hands out a fixed number of tickets that "regenerate" one every few minutes: you can burn through a small stockpile of saved-up tickets to bring a big group in all at once, but once they're gone you're limited to however fast new ones trickle in.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Rate limiting algorithms, compared**

                    - Fixed window — simplest, cheapest; allows up to ~2x burst at window boundaries
                    - Sliding window log — most accurate; stores a timestamp per request, memory grows with request rate
                    - Sliding window counter — approximates the sliding log cheaply by weighting the previous window's count
                    - Token bucket — allows bursts up to bucket capacity, enforces a steady average rate; the common default
                    - Leaky bucket — like token bucket but smooths output to a strictly constant rate (queues excess instead of allowing bursts)

                    **Where to enforce a limit**

                    - Client-side throttling — cooperative only, never trust it alone
                    - API gateway — the standard place: one enforcement point in front of every service
                    - Per-service — sometimes still needed for a resource-specific limit (e.g., an expensive search endpoint), layered on top of the gateway's general limit
                    """, 3),
                Block(BlockType.CodeSnippet, "Token Bucket Rate Limiter", BodyFormat.PlainText, """
                    public class TokenBucketRateLimiter
                    {
                        private readonly int capacity;
                        private readonly double refillTokensPerSecond;
                        private double tokens;
                        private DateTime lastRefillUtc;
                        private readonly object gate = new();

                        public TokenBucketRateLimiter(int capacity, double refillTokensPerSecond)
                        {
                            this.capacity = capacity;
                            this.refillTokensPerSecond = refillTokensPerSecond;
                            tokens = capacity;
                            lastRefillUtc = DateTime.UtcNow;
                        }

                        // Returns true (and consumes a token) if the request is allowed.
                        public bool TryConsume()
                        {
                            lock (gate)
                            {
                                Refill();
                                if (tokens < 1)
                                {
                                    return false; // caller should respond 429 Too Many Requests
                                }

                                tokens -= 1;
                                return true;
                            }
                        }

                        private void Refill()
                        {
                            var now = DateTime.UtcNow;
                            var elapsedSeconds = (now - lastRefillUtc).TotalSeconds;
                            tokens = Math.Min(capacity, tokens + elapsedSeconds * refillTokensPerSecond);
                            lastRefillUtc = now;
                        }
                    }

                    // 100 requests allowed to burst, refilling at 10/sec (i.e. ~10 req/s sustained)
                    var limiter = new TokenBucketRateLimiter(capacity: 100, refillTokensPerSecond: 10);
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "A Request's Path Through the Gateway", BodyFormat.StructuredSteps, """
                    [{"label":"Client"},{"label":"API Gateway","note":"TLS termination, authn/authz"},{"label":"Rate Limiter","note":"token bucket per API key; reject with 429 if empty"},{"label":"Router"},{"label":"Service A / B / C","note":"internal, trusted network"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Key rate limits by API key or authenticated user ID, not just by client IP — many legitimate users can share one IP (corporate NAT, mobile carrier NAT), and a single IP-based limit punishes all of them for one user's traffic.

                    When a request is rejected, return `429 Too Many Requests` with a `Retry-After` header telling the client exactly when to try again, instead of a bare error the client has to guess how to handle.

                    If the gateway runs as multiple replicas, back the limiter with a shared store (e.g., Redis with an atomic `INCR`/Lua script) instead of in-process memory — otherwise each replica enforces its own independent limit, and the effective limit becomes (configured limit x replica count).
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked to design rate limiting, name the algorithm trade-off explicitly (token bucket allows bursts; sliding window is more precise but costlier) and then immediately raise the distributed-state problem: a gateway that scales to N replicas can't count accurately without a shared, atomic counter, which is usually where the interesting discussion (Redis, race conditions, clock skew) actually lives.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Implementing a fixed window counter and not noticing it permits roughly double the intended rate at the boundary between two windows (a burst at the end of one window plus a burst at the start of the next both pass, even though they land within one another's true "the last 60 seconds").

                    Also common: storing rate-limit counters in each gateway instance's local memory behind a load balancer — with 5 replicas and no shared state, a client can get roughly 5x the intended limit simply by having requests spread across instances.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "Why does the token bucket algorithm allow a burst of requests, while still enforcing a long-run average rate?",
                    "Tokens accumulate up to the bucket's capacity even while idle, so a client that hasn't made requests in a while can spend that saved-up capacity all at once (a burst) — but once the bucket is empty, requests are limited to however fast it refills, which is the steady-state average rate.",
                    [
                        new QuizOptionSeed("It doesn't allow bursts — it enforces a strictly constant rate like the leaky bucket", false),
                        new QuizOptionSeed("Unused capacity accumulates as tokens (up to the bucket size), letting a burst spend that saved-up allowance at once", true),
                        new QuizOptionSeed("It resets to full capacity at the start of every calendar minute", false),
                        new QuizOptionSeed("It only counts requests from authenticated users", false),
                    ]),
                new QuizQuestionSeed(
                    "An API gateway runs as 5 replicas behind a load balancer, each tracking rate limit counters in local process memory. What goes wrong?",
                    "With no shared state, each of the 5 replicas independently allows up to the configured limit — so a client whose requests get spread across replicas can effectively get up to 5x the intended limit. A shared, atomic store (e.g., Redis) is required for the limit to hold across replicas.",
                    [
                        new QuizOptionSeed("Nothing — in-memory counters are accurate as long as the limiter code is correct", false),
                        new QuizOptionSeed("The effective limit becomes roughly (configured limit x number of replicas), since each replica counts independently", true),
                        new QuizOptionSeed("The gateway will crash under any load", false),
                        new QuizOptionSeed("Rate limiting stops working entirely and no requests are ever rejected", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("System Design Primer: Rate limiting", "https://github.com/donnemartin/system-design-primer", LinkType.FurtherReading),
                new ReferenceLinkSeed("Cloudflare: What is rate limiting?", "https://www.cloudflare.com/learning/bots/what-is-rate-limiting/", LinkType.FurtherReading),
            ]);

        var lesson1Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson1.Slug,
        [
            "Implement a token bucket limiter from scratch and explain why it allows bursts",
            "Explain the fixed-window boundary-burst problem to someone else without looking it up",
            "Describe how you'd make rate limiting correct across multiple gateway replicas",
        ]);

        var lesson2 = BuildLesson(
            slug: "cdn-edge-caching",
            title: "CDN & Edge Caching",
            summary: "How a CDN moves content physically closer to users, and the cache-control headers and invalidation strategies that keep edge caches correct.",
            estimatedMinutes: 40,
            objectives:
            [
                "Explain how a CDN reduces latency by serving content from edge locations near users",
                "Distinguish origin-pull from origin-push CDN behavior and know when each fits",
                "Use Cache-Control directives (max-age, s-maxage, no-store, must-revalidate) correctly for static vs. dynamic content",
                "Choose a cache invalidation strategy (TTL, versioned URLs, explicit purge) for a given deployment scenario",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    A **CDN (Content Delivery Network)** is a globally-distributed set of proxy servers ("edge" points of presence, or PoPs) that cache and serve content from a location physically close to the requesting user, instead of every request traveling all the way to your **origin** server. Physical distance is a hard latency floor — round-trip time is bounded by the speed of light — so serving from a nearby edge node instead of a distant origin is often the single biggest latency win available.

                    Two ways content gets onto the edge:

                    - **Origin pull (reverse proxy mode)** — the CDN has no content until the first request for it arrives; on a miss, it fetches from the origin, caches the response, and serves it to that request and every subsequent one until the entry expires. This is the common default and requires no separate upload step.
                    - **Origin push** — you explicitly upload/publish content to the CDN ahead of time. Better suited to a fixed catalog of large assets (e.g., video files) where you don't want the first requester in each region to pay a cold-cache penalty.

                    Whether an edge node can cache a response — and for how long — is governed by HTTP caching headers, primarily `Cache-Control`:

                    - `max-age=N` — how long (seconds) a **browser** may cache the response
                    - `s-maxage=N` — how long a **shared cache** (CDN, reverse proxy) may cache it, overriding `max-age` for shared caches specifically
                    - `no-cache` — may be cached, but must be revalidated with the origin before each use
                    - `no-store` — must not be cached anywhere, ever (e.g., responses containing sensitive per-user data)
                    - `public` / `private` — whether shared caches are allowed to store the response at all (`private` means only the end-user's own browser may cache it)
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    A CDN is like a national retail chain building regional warehouses close to customers instead of shipping every single order from one central warehouse across the country — most orders (cache hits) ship from the nearby warehouse in a day; only the rare item the regional warehouse doesn't stock (a cache miss) has to be requested from the central warehouse first.

                    `Cache-Control: no-store` is like a "do not photocopy, shred after reading" stamp on a document — no intermediate stop, however convenient, is allowed to keep a copy.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Cache-Control directives**

                    - `max-age=<seconds>` — browser cache lifetime
                    - `s-maxage=<seconds>` — shared/CDN cache lifetime (wins over `max-age` for shared caches)
                    - `no-cache` — cacheable, but must revalidate with origin every time (via `ETag`/`If-None-Match`)
                    - `no-store` — never cache, anywhere
                    - `public` — shared caches (CDN) may store it
                    - `private` — only the end-user's browser may store it
                    - `must-revalidate` — once stale, must not be served without revalidating, even if the origin is unreachable
                    - `stale-while-revalidate=<seconds>` — serve a stale copy immediately while revalidating in the background

                    **Invalidation strategies**

                    - Short TTL — accept some staleness; simplest, no purge mechanism needed
                    - Versioned/fingerprinted URLs (`app.a1b2c3.js`) — cache the asset "forever" (`max-age=31536000, immutable`); a new deploy is a new URL, so invalidation is automatic and instant
                    - Explicit purge/cache tags — actively evict specific edge-cached entries on demand; needed for content that changes unpredictably and can't wait out a TTL
                    """, 3),
                Block(BlockType.CodeSnippet, "Setting Cache-Control per Content Type", BodyFormat.PlainText, """
                    app.Use(async (context, next) =>
                    {
                        await next();

                        var path = context.Request.Path;

                        if (path.StartsWithSegments("/static") && path.Value!.Contains('.'))
                        {
                            // Fingerprinted, immutable build assets: cache forever at the
                            // browser AND the CDN edge. A new deploy ships a new filename,
                            // so there is nothing to invalidate.
                            context.Response.Headers.CacheControl =
                                "public, max-age=31536000, immutable";
                        }
                        else if (path.StartsWithSegments("/api/catalog"))
                        {
                            // Semi-dynamic: browsers must revalidate on every use, but the
                            // CDN edge may serve a copy for 60s and refresh it in the
                            // background for up to 30s after that while still serving fast.
                            context.Response.Headers.CacheControl =
                                "public, max-age=0, s-maxage=60, stale-while-revalidate=30";
                        }
                        else if (path.StartsWithSegments("/api/account"))
                        {
                            // Per-user, sensitive: never cache at a shared edge node.
                            context.Response.Headers.CacheControl = "private, no-store";
                        }
                    });
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "Cache Hit vs. Cache Miss at the Edge", BodyFormat.StructuredSteps, """
                    [{"label":"Client (nearest region)"},{"label":"Edge PoP","note":"cache HIT -> serve immediately, no origin round-trip"},{"label":"Edge PoP (on MISS)","note":"forwards request to origin"},{"label":"Origin Server","note":"generates/serves response, sets Cache-Control"},{"label":"Edge PoP caches response","note":"serves this and future requests until TTL expires"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Fingerprint (hash the content into the filename of) static build assets and serve them with a far-future `max-age`/`immutable` — this turns cache invalidation into a non-problem, since a changed file is, by definition, a new URL that was never cached.

                    Never mark a response `public` if it can contain per-user or sensitive data (session tokens, account details, personalized pricing) — a shared edge cache serving user A's response to user B is a real, embarrassing data leak, not a hypothetical one.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    Be ready to name what a CDN does *not* solve: highly personalized, per-user, write-heavy, or rapidly-changing data gets little benefit from edge caching and may need a different approach entirely (e.g., edge compute/edge functions, or simply accepting the origin round-trip). A design that reflexively says "put a CDN in front of everything" without this caveat reads as memorized rather than understood.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Deploying a new version of an app but keeping the same asset filenames (no fingerprinting/versioning) — users can be stuck on a stale, previously-cached JS/CSS bundle for as long as its `max-age`, sometimes long past the point where it's now incompatible with the current API.

                    Also common: setting `Cache-Control: public` on a response that includes a `Set-Cookie` header or other per-user content — some caches will store and replay that exact response, including its cookie, to a different user entirely.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "Why does versioning/fingerprinting static asset filenames (e.g., `app.a1b2c3.js`) make cache invalidation simple?",
                    "A changed file produces a different hash and therefore a different URL. Old cached entries are simply never requested again (they're not \"invalidated\" so much as abandoned), while the new URL is a guaranteed cache miss the first time anyone requests it — so you can safely cache each version forever with no purge step required.",
                    [
                        new QuizOptionSeed("It forces the CDN to check the origin on every single request", false),
                        new QuizOptionSeed("A content change produces a new URL, so old cached entries are simply never re-requested and the new URL is naturally a fresh cache miss", true),
                        new QuizOptionSeed("It disables caching for that asset entirely", false),
                        new QuizOptionSeed("It only works for HTML pages, not JS or CSS", false),
                    ]),
                new QuizQuestionSeed(
                    "A response contains per-user account data and is accidentally marked `Cache-Control: public, max-age=300`. What's the risk?",
                    "`public` permits shared caches, including CDN edge nodes, to store the response and serve it to other users who request the same URL — meaning one user's private account data could be served to a different user entirely until the entry expires.",
                    [
                        new QuizOptionSeed("None — max-age=300 is short enough to be safe", false),
                        new QuizOptionSeed("A shared/edge cache may store and replay that user's private response to a different user requesting the same URL", true),
                        new QuizOptionSeed("The response will simply never be cached because it's dynamic", false),
                        new QuizOptionSeed("It only affects the user's own browser cache, which is safe by definition", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("System Design Primer: Content Delivery Network (CDN)", "https://github.com/donnemartin/system-design-primer", LinkType.FurtherReading),
                new ReferenceLinkSeed("MDN: Cache-Control header", "https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Cache-Control", LinkType.OfficialDocs),
            ],
            prerequisites: [lesson1]);

        var lesson2Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson2.Slug,
        [
            "Explain the difference between max-age and s-maxage to someone else correctly",
            "Design a cache invalidation strategy for a hypothetical app's static assets and its semi-dynamic API responses",
            "List one scenario where putting a CDN in front of a service would NOT help, and explain why",
        ]);

        var module = BuildModule(topicId, "traffic-management-gateways-and-cdns", "Traffic Management: API Gateways & CDNs",
            "How systems manage and shape incoming traffic before it reaches application servers — centralizing cross-cutting concerns at an API gateway, protecting backends with real rate-limiting algorithms, and pushing static content out to the edge with a CDN.",
            85, [lesson1, lesson2], sortOrder: 2);

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

        var lesson3 = BuildLesson(
            slug: "indexing-query-performance",
            title: "Indexing & Query Performance",
            summary: "How B-tree indexes turn table scans into fast lookups, when they stop helping, and reading a query plan to tell the difference.",
            estimatedMinutes: 40,
            objectives:
            [
                "Explain how a B-tree index turns an O(n) scan into an O(log n) lookup",
                "Decide whether a given WHERE/JOIN condition can actually use an index",
                "Read a basic query plan and recognize a full table scan vs. an index seek",
                "Recognize the write-amplification cost of adding an index",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    Without an index, finding rows that match a condition means a **full table scan** — the database reads every row and checks the condition, `O(n)` in the number of rows.

                    An **index** is a separate, ordered data structure (almost always a **B-tree**) that maps column values to the rows containing them. Because a B-tree stays sorted and balanced, looking up a value is `O(log n)` — the database walks down the tree instead of scanning every row. This is sometimes called an **index seek**, as opposed to a **full scan**.

                    Indexes aren't free. Every `INSERT`/`UPDATE`/`DELETE` has to keep every index on that table up to date, so more indexes mean slower writes and more storage — indexing is a read/write trade-off, not a pure win.

                    A **composite index** (an index on more than one column) can only be used efficiently from its **leftmost columns** inward — an index on `(customer_id, status)` speeds up `WHERE customer_id = ?` and `WHERE customer_id = ? AND status = ?`, but does *nothing* for `WHERE status = ?` alone, because the index is physically sorted by `customer_id` first.

                    To see whether a query actually uses an index, ask the database to show its **query plan** — the command differs by engine (`EXPLAIN` in MySQL/PostgreSQL, `EXPLAIN QUERY PLAN` in SQLite, `EXPLAIN ANALYZE` for actual runtime numbers in PostgreSQL), but the concept is the same everywhere: it shows whether a step is a scan or a seek, and roughly how many rows it expects to touch.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    A table without an index is like a phone book with pages ripped out and shuffled — finding "everyone named Patel" means reading every single page front to back.

                    An index is the same phone book, alphabetized: you jump straight to the "P" section instead of reading the whole book. But now imagine you also have to keep the book alphabetized every time someone moves into town — that's the write cost of maintaining an index. A composite index sorted by (last name, first name) lets you jump straight to "Patel, Raj" — but it's useless if all you know is someone's first name is "Raj," because the book isn't sorted by first name at all.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **When an index helps**

                    - High-selectivity columns (many distinct values — e.g., `email`, `order_id`)
                    - Columns frequently used in `WHERE`, `JOIN ... ON`, and `ORDER BY`
                    - The leftmost column(s) of a composite index

                    **When an index won't be used (or won't help)**

                    - Low-selectivity columns (a `bool` or a status with 3 values on a huge table) — often cheaper to scan
                    - Wrapping the column in a function: `WHERE UPPER(email) = 'X'` can't use a plain index on `email`
                    - A leading wildcard: `WHERE name LIKE '%smith'` can't use a standard B-tree index (`'smith%'` can)
                    - Querying a non-leftmost column of a composite index in isolation

                    **Reading a plan**: look for words like "Seq Scan"/"Table Scan" (full scan) vs. "Index Scan"/"Index Seek" (using the index), and the estimated row count.
                    """, 3),
                Block(BlockType.CodeSnippet, "Creating and Checking a Composite Index", BodyFormat.PlainText, """
                    -- Index sorted by customer_id first, then status.
                    CREATE INDEX idx_orders_customer_status
                        ON orders (customer_id, status);

                    -- Uses the index (leftmost column, or leftmost + second column):
                    SELECT * FROM orders WHERE customer_id = 42;
                    SELECT * FROM orders WHERE customer_id = 42 AND status = 'shipped';

                    -- Does NOT use this index efficiently (status isn't the leftmost column):
                    SELECT * FROM orders WHERE status = 'shipped';

                    -- Ask the engine what it actually did (syntax varies by database:
                    -- EXPLAIN in MySQL/PostgreSQL, EXPLAIN QUERY PLAN in SQLite).
                    EXPLAIN
                    SELECT * FROM orders WHERE customer_id = 42;
                    """, 4, language: "sql"),
                Block(BlockType.Diagram, "Full Scan vs. B-Tree Index Seek", BodyFormat.AsciiArt, """
                    No index (full scan):

                    row1 -> row2 -> row3 -> row4 -> ... -> rowN
                    (check every row for a match; O(n))

                    With a B-tree index on customer_id:

                                    [ 50 ]
                                   /      \\
                              [ 20 ]      [ 80 ]
                              /   \\        /    \\
                          [10] [30,42]  [60]  [90,99]

                    Looking up customer_id = 42:
                    50 -> go left -> 20 -> go right -> [30, 42] -> found
                    (a few hops down the tree; O(log n))
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Index the columns your `WHERE` clauses, `JOIN ... ON` conditions, and `ORDER BY` clauses actually use — not every column "just in case." Put the most selective, most frequently-filtered column first in a composite index so the leftmost-prefix rule works in your favor for the widest range of queries.

                    Avoid wrapping an indexed column in a function or arithmetic in your `WHERE` clause (`WHERE UPPER(email) = ?`, `WHERE price * 1.1 > ?`) — most databases can't use a plain index through a transformed expression, silently falling back to a full scan.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked "how would you speed up this slow query," don't just say "add an index" — name *which* column(s), explain why they're selective enough to be worth indexing, and mention the trade-off out loud ("this speeds up reads on `customer_id` but adds overhead to every insert into `orders`"). Naming the trade-off is what separates "memorized the word index" from actually understanding it.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Adding an index to every column "for safety" — each extra index slows down every write on that table and consumes storage, often with no read benefit if the column is rarely filtered on or has low selectivity (like a boolean flag on a large table).

                    Also common: assuming an index exists and is being used without checking the query plan — a query can silently fall back to a full scan (function-wrapped column, leading wildcard, non-leftmost composite column) while the developer assumes the index is doing its job.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "You have a composite index on `(customer_id, status)`. Which query can use it efficiently?",
                    "A composite index is physically sorted by its leftmost column first, then the next. A query filtering only on `status` can't use this index efficiently because `status` isn't the leftmost column — the index is only useful when `customer_id` is part of the filter.",
                    [
                        new QuizOptionSeed("WHERE status = 'shipped'", false),
                        new QuizOptionSeed("WHERE customer_id = 42 AND status = 'shipped'", true),
                        new QuizOptionSeed("Both queries use the index equally well", false),
                        new QuizOptionSeed("Neither query can use the index", false),
                    ]),
                new QuizQuestionSeed(
                    "Why does `WHERE UPPER(email) = 'JANE@EXAMPLE.COM'` typically fail to use a plain index on `email`?",
                    "A standard B-tree index is built on the raw column values. Wrapping the column in a function like UPPER() means the database would have to compute that function for every row to compare it, which defeats the purpose of the index — so most engines fall back to a full scan unless a function-based (expression) index exists.",
                    [
                        new QuizOptionSeed("Indexes only work on numeric columns, never text", false),
                        new QuizOptionSeed("The function transforms the column's value, so the plain index on the raw column can't be used to satisfy it", true),
                        new QuizOptionSeed("UPPER() is not valid SQL syntax", false),
                        new QuizOptionSeed("It works exactly the same as filtering on email directly", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Use The Index, Luke", "https://use-the-index-luke.com/", LinkType.FurtherReading),
                new ReferenceLinkSeed("PostgreSQL: Indexes", "https://www.postgresql.org/docs/current/indexes.html", LinkType.OfficialDocs),
            ],
            prerequisites: [lesson1]);

        var lesson3Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson3.Slug,
        [
            "Run EXPLAIN (or your database's equivalent) on one of your own queries and identify a scan vs. a seek",
            "Create a composite index and explain, in your own words, why column order matters",
            "Find one query in your own code that filters through a function-wrapped column and would silently skip an index",
        ]);

        var lesson4 = BuildLesson(
            slug: "transactions-isolation-levels",
            title: "Transactions & Isolation Levels",
            summary: "ACID guarantees, the standard isolation levels, and how concurrent transactions can deadlock.",
            estimatedMinutes: 40,
            objectives:
            [
                "State the four ACID properties and what each one actually guarantees",
                "Distinguish dirty reads, non-repeatable reads, and phantom reads from each other",
                "Choose an appropriate isolation level for a given concurrency requirement",
                "Explain how a deadlock forms and one strategy to avoid one",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    A **transaction** groups multiple statements into one all-or-nothing unit, started with `BEGIN`/`START TRANSACTION` and ended with `COMMIT` (keep all changes) or `ROLLBACK` (discard all changes). Transactions guarantee **ACID**:

                    - **Atomicity** — every statement in the transaction succeeds, or none of them take effect.
                    - **Consistency** — a transaction moves the database from one valid state to another, never violating constraints.
                    - **Isolation** — concurrent transactions don't see each other's uncommitted changes (how strictly is controlled by the **isolation level**).
                    - **Durability** — once committed, the change survives a crash.

                    Isolation is a spectrum, not all-or-nothing, because full isolation between every concurrent transaction is expensive. The ANSI SQL standard defines four levels, each preventing more "phenomena" than the last:

                    - **Read Uncommitted** — can see another transaction's uncommitted changes (a **dirty read**).
                    - **Read Committed** — never sees uncommitted data, but re-reading the same row twice in one transaction can return different values if another transaction committed in between (a **non-repeatable read**).
                    - **Repeatable Read** — the same row read twice returns the same value for the whole transaction, but a range query re-run later can return new rows another transaction inserted (a **phantom read**).
                    - **Serializable** — transactions behave as if run one at a time, in some order — no dirty reads, non-repeatable reads, or phantoms, at the cost of the most blocking/retries.

                    A **deadlock** happens when two transactions each hold a lock the other one needs: transaction A holds a lock on row 1 and wants row 2, while transaction B holds a lock on row 2 and wants row 1 — neither can proceed, so the database detects the cycle and forcibly rolls one of them back.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Isolation levels are like how strictly a shared document is locked while you're editing it. **Read Uncommitted** is peeking at someone else's still-being-typed, unsaved draft. **Read Committed** is only ever seeing their saved versions, but the document might change between two glances. **Repeatable Read** is like taking a personal snapshot of the pages you've already looked at, so they can't change under you — but new pages someone else adds later can still show up. **Serializable** is like getting the whole document to yourself until you're done, as if no one else were editing it at all.

                    A deadlock is two people trying to merge onto a single-lane bridge from opposite ends, each already halfway across and blocking the other — neither can back up or move forward until someone gives way.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Isolation levels vs. phenomena prevented**

                    | Level            | Dirty Read | Non-repeatable Read | Phantom Read |
                    |------------------|:----------:|:--------------------:|:------------:|
                    | Read Uncommitted | Possible   | Possible              | Possible     |
                    | Read Committed   | Prevented  | Possible              | Possible     |
                    | Repeatable Read  | Prevented  | Prevented             | Possible     |
                    | Serializable     | Prevented  | Prevented             | Prevented    |

                    **Rule of thumb**: use the lowest isolation level that still meets your correctness requirement — higher isolation means more locking/retries and less concurrency.
                    """, 3),
                Block(BlockType.CodeSnippet, "A Transaction with an Explicit Isolation Level", BodyFormat.PlainText, """
                    SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;

                    BEGIN TRANSACTION;

                    UPDATE accounts SET balance = balance - 100 WHERE account_id = 1;
                    UPDATE accounts SET balance = balance + 100 WHERE account_id = 2;

                    COMMIT;
                    -- Both updates take effect together, or neither does
                    -- (atomicity) — if anything fails, ROLLBACK undoes both.
                    """, 4, language: "sql"),
                Block(BlockType.Diagram, "How a Deadlock Forms", BodyFormat.StructuredSteps, """
                    [{"label":"Transaction A: locks Row 1"},{"label":"Transaction B: locks Row 2"},{"label":"Transaction A: requests Row 2","note":"blocked — B holds it"},{"label":"Transaction B: requests Row 1","note":"blocked — A holds it"},{"label":"Database detects the cycle","note":"A waits on B, B waits on A"},{"label":"One transaction is rolled back","note":"to break the deadlock"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Keep transactions as short as possible and touch rows in a **consistent order** across your codebase (e.g., always update the lower account ID first) — most deadlocks come from two code paths locking the same rows in opposite orders, and consistent ordering eliminates the cycle entirely.

                    Default to the lowest isolation level that satisfies your correctness needs (often Read Committed) rather than reaching for Serializable everywhere — extra isolation isn't free, and most application logic doesn't actually need it.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked to design a "transfer money between accounts" function, say the isolation trade-off out loud: what could go wrong at Read Committed (another transaction reading the balance mid-transfer), and why wrapping both updates in one transaction with appropriate isolation — not just "using a transaction" as a magic word — is what actually prevents it.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Holding a transaction open across slow, unrelated work (an external API call, user think-time) — every lock it holds blocks other transactions the entire time, and it dramatically increases deadlock risk. Keep transactions limited to the database work itself.

                    Also common: assuming "I wrapped it in a transaction" alone prevents concurrency bugs, without considering which isolation level is actually in effect — the default isolation level (often Read Committed) still allows non-repeatable reads, which can matter a lot for logic like "check balance, then debit it."
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "Which isolation level prevents dirty reads but still allows a non-repeatable read?",
                    "Read Committed guarantees you never see another transaction's uncommitted (dirty) data, but it doesn't guarantee that re-reading the same row twice in one transaction returns the same value — another transaction can commit a change in between, causing a non-repeatable read.",
                    [
                        new QuizOptionSeed("Read Uncommitted", false),
                        new QuizOptionSeed("Read Committed", true),
                        new QuizOptionSeed("Repeatable Read", false),
                        new QuizOptionSeed("Serializable", false),
                    ]),
                new QuizQuestionSeed(
                    "What actually causes a deadlock between two transactions?",
                    "A deadlock is a circular wait: transaction A holds a lock transaction B needs, while B simultaneously holds a lock A needs. Neither can proceed, so the database detects the cycle and forcibly rolls one transaction back.",
                    [
                        new QuizOptionSeed("One transaction runs a query that is too slow", false),
                        new QuizOptionSeed("Two transactions each hold a lock the other one needs, forming a circular wait", true),
                        new QuizOptionSeed("A transaction is missing a COMMIT statement", false),
                        new QuizOptionSeed("The database ran out of storage space", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("PostgreSQL: Transaction Isolation", "https://www.postgresql.org/docs/current/transaction-iso.html", LinkType.OfficialDocs),
                new ReferenceLinkSeed("PostgreSQL: Explicit Locking", "https://www.postgresql.org/docs/current/explicit-locking.html", LinkType.OfficialDocs),
            ],
            prerequisites: [lesson2]);

        var lesson4Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson4.Slug,
        [
            "Explain all four ACID properties out loud with a one-sentence definition each",
            "Describe a real scenario where Read Committed isn't strict enough and Repeatable Read is needed",
            "Trace through a two-transaction deadlock scenario and describe how consistent lock ordering would avoid it",
        ]);

        var module = BuildModule(topicId, "sql-fundamentals", "SQL Fundamentals",
            "Query evaluation order, joins, NULL handling, window functions, indexing, and transactions.",
            210, [lesson1, lesson2, lesson3, lesson4]);

        return (module, [lesson1Checklist, lesson2Checklist, lesson3Checklist, lesson4Checklist]);
    }

    private static (Module, List<ChecklistSeed>) BuildSqlAdvancedModule(int topicId)
    {
        var lesson1 = BuildLesson(
            slug: "schema-design-and-normalization",
            title: "Schema Design & Normalization",
            summary: "Normal forms up through 3NF, foreign keys and referential integrity, and when to deliberately break the rules by denormalizing.",
            estimatedMinutes: 40,
            objectives:
            [
                "Define 1NF, 2NF, and 3NF and identify which normal form a given table violates",
                "Design foreign keys with correct referential integrity actions (CASCADE, RESTRICT, SET NULL)",
                "Explain the difference between a partial dependency and a transitive dependency",
                "Decide, with justification, when deliberately denormalizing a schema is the right engineering trade-off",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    **Normalization** is the process of organizing a schema to eliminate redundancy and the update anomalies redundancy causes. It's defined as a series of **normal forms**, each stricter than the last.

                    A table is in **First Normal Form (1NF)** when every column holds a single, atomic value (no comma-separated lists or repeating groups) and there's a well-defined primary key. A column like `phone_numbers = "555-1234, 555-5678"` violates 1NF.

                    **Second Normal Form (2NF)** requires 1NF, plus: every non-key column must depend on the *entire* primary key, not just part of it. This only matters for tables with a **composite primary key** — a **partial dependency** is when a column depends on only one part of that key (e.g., in `order_items(order_id, product_id, product_name, quantity)`, `product_name` depends only on `product_id`, not on the full `(order_id, product_id)` key).

                    **Third Normal Form (3NF)** requires 2NF, plus: no non-key column depends on another non-key column (a **transitive dependency**). If `employees(employee_id, department_id, department_name)` stores `department_name` redundantly on every employee row, that's a transitive dependency — `department_name` depends on `department_id`, which depends on `employee_id`, not directly on the key.

                    A **functional dependency** X -> Y means: given a value of X, Y is uniquely determined. Every normal form rule is really a rule about which functional dependencies are allowed to exist in a table.

                    **Foreign keys** enforce **referential integrity** — a foreign key column's value must either be `NULL` or match an existing value in the referenced table's primary key. `ON DELETE`/`ON UPDATE` actions control what happens to dependent rows: `CASCADE` (propagate the delete/update), `RESTRICT`/`NO ACTION` (block the operation if dependents exist), or `SET NULL` (clear the foreign key on dependents).
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Denormalized data is like writing your friend's address on every single birthday card list, holiday card list, and wedding invite list you keep — when they move, you have to hunt down and update every list, and if you miss one, you now have contradictory addresses on file for the same person.

                    Normalization is keeping one shared address book: every list just points to "friend #42," and you update the address in exactly one place. Foreign keys are the rule that says you're not allowed to write down "friend #99" on a list unless friend #99 actually exists in the address book.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Normal forms at a glance**

                    - 1NF: atomic columns, no repeating groups, a primary key exists
                    - 2NF: 1NF + no partial dependency on part of a composite key
                    - 3NF: 2NF + no transitive dependency between non-key columns
                    - (BCNF, 4NF, 5NF exist but 3NF is sufficient for the overwhelming majority of application schemas)

                    **Foreign key referential actions**

                    - `ON DELETE CASCADE` — deleting the parent deletes its children too
                    - `ON DELETE RESTRICT` / `NO ACTION` — block deleting a parent that still has children
                    - `ON DELETE SET NULL` — deleting the parent nulls out the child's foreign key (column must be nullable)
                    - The same three options exist for `ON UPDATE`, though updating primary keys is rare in practice

                    **When to denormalize deliberately**: read-heavy reporting tables, caching a computed/aggregated value to avoid an expensive join on every request, or a documented performance trade-off — never as a substitute for understanding the normalized design first.
                    """, 3),
                Block(BlockType.CodeSnippet, "Normalizing a Schema and Adding Referential Actions", BodyFormat.PlainText, """
                    -- Before: violates 2NF and 3NF. product_name depends only on
                    -- product_id (partial dependency), and customer_city depends on
                    -- customer_id, not on the order_item key (transitive dependency).
                    CREATE TABLE order_items_denormalized (
                        order_id      INTEGER,
                        product_id    INTEGER,
                        product_name  VARCHAR(100),
                        customer_id   INTEGER,
                        customer_city VARCHAR(100),
                        quantity      INTEGER,
                        PRIMARY KEY (order_id, product_id)
                    );

                    -- After: normalized to 3NF with foreign keys enforcing referential
                    -- integrity and explicit referential actions.
                    CREATE TABLE customers (
                        customer_id INTEGER PRIMARY KEY,
                        city        VARCHAR(100) NOT NULL
                    );

                    CREATE TABLE products (
                        product_id   INTEGER PRIMARY KEY,
                        product_name VARCHAR(100) NOT NULL
                    );

                    CREATE TABLE orders (
                        order_id    INTEGER PRIMARY KEY,
                        customer_id INTEGER NOT NULL,
                        FOREIGN KEY (customer_id) REFERENCES customers (customer_id)
                            ON DELETE RESTRICT
                    );

                    CREATE TABLE order_items (
                        order_id   INTEGER NOT NULL,
                        product_id INTEGER NOT NULL,
                        quantity   INTEGER NOT NULL,
                        PRIMARY KEY (order_id, product_id),
                        FOREIGN KEY (order_id) REFERENCES orders (order_id)
                            ON DELETE CASCADE,
                        FOREIGN KEY (product_id) REFERENCES products (product_id)
                            ON DELETE RESTRICT
                    );
                    """, 4, language: "sql"),
                Block(BlockType.Diagram, "From Unnormalized Table to 3NF", BodyFormat.StructuredSteps, """
                    [{"label":"Unnormalized table","note":"repeating groups, e.g. a comma-separated phone_numbers column"},{"label":"1NF","note":"split repeating groups out; every column becomes atomic"},{"label":"2NF","note":"remove partial dependencies; move columns that depend on only part of a composite key into their own table"},{"label":"3NF","note":"remove transitive dependencies; move columns that depend on a non-key column into their own table"},{"label":"Normalized schema","note":"customers, products, orders, order_items — each fact stored exactly once"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Design to 3NF by default — it eliminates the update anomalies (insert, update, and delete anomalies) that come from storing the same fact in more than one place. Only denormalize deliberately, for a measured performance reason, and document *why* directly in the schema (a comment on the column) so the next engineer doesn't "fix" it by re-normalizing and silently breaking the cache.

                    Always name a referential action explicitly (`ON DELETE CASCADE`/`RESTRICT`/`SET NULL`) rather than relying on the database's default — the default varies by engine, and silently picking the wrong one is how you either get orphaned rows or accidentally cascade-delete data you meant to keep.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    "Design a database schema for X" (a library, a ride-sharing app, a social network) is one of the most common SQL/system-design crossover questions. Walk through your entities and their relationships first, name your primary and foreign keys out loud, and explicitly state your normalization decisions — "I'm keeping `product_name` only in the `products` table, not duplicated on every order line, to avoid it going stale" is exactly the kind of reasoning interviewers are listening for.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Storing a derived or duplicated value (a customer's city copied onto every order row, a running total stored instead of computed) without a clear, deliberate reason and a plan for keeping it in sync — this is how two "copies" of the same fact quietly drift apart over time.

                    Also common: adding a foreign key without deciding on its `ON DELETE` behavior, then being surprised in production when deleting a parent row either fails unexpectedly (default `RESTRICT`/`NO ACTION` in many engines) or silently cascades further than intended.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "A table `order_items(order_id, product_id, product_name, quantity)` has a composite primary key of `(order_id, product_id)`, but `product_name` only depends on `product_id`. Which normal form does this violate?",
                    "This is a partial dependency: product_name depends on only part of the composite key (product_id), not the whole key (order_id, product_id). That's exactly what 2NF prohibits.",
                    [
                        new QuizOptionSeed("1NF, because product_name is not atomic", false),
                        new QuizOptionSeed("2NF, because product_name has a partial dependency on only part of the composite key", true),
                        new QuizOptionSeed("3NF, because product_name depends on another non-key column", false),
                        new QuizOptionSeed("It doesn't violate any normal form", false),
                    ]),
                new QuizQuestionSeed(
                    "You delete a customer row and want every one of that customer's orders deleted automatically as part of the same operation. Which foreign key referential action achieves this?",
                    "ON DELETE CASCADE propagates the delete to dependent rows automatically. ON DELETE RESTRICT would block the delete while orders still reference the customer, and ON DELETE SET NULL would leave the orders in place with a null customer_id instead of deleting them.",
                    [
                        new QuizOptionSeed("ON DELETE RESTRICT", false),
                        new QuizOptionSeed("ON DELETE CASCADE", true),
                        new QuizOptionSeed("ON DELETE SET NULL", false),
                        new QuizOptionSeed("Foreign keys can't trigger any action automatically", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("PostgreSQL: Foreign Keys", "https://www.postgresql.org/docs/current/ddl-constraints.html", LinkType.OfficialDocs),
                new ReferenceLinkSeed("Database normalization overview", "https://en.wikipedia.org/wiki/Database_normalization", LinkType.FurtherReading),
            ]);

        var lesson1Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson1.Slug,
        [
            "Take one denormalized table you've worked with and normalize it to 3NF on paper",
            "Add a foreign key with an explicit ON DELETE action and explain why you chose CASCADE, RESTRICT, or SET NULL",
            "Explain the difference between a partial dependency and a transitive dependency in your own words",
        ]);

        var lesson2 = BuildLesson(
            slug: "cte-and-recursive-queries",
            title: "Common Table Expressions & Recursive Queries",
            summary: "Using WITH to name subqueries for readability, and recursive CTEs to walk hierarchical data like org charts and category trees.",
            estimatedMinutes: 40,
            objectives:
            [
                "Rewrite a nested subquery as a named CTE using WITH to improve readability",
                "Explain the anchor member / recursive member structure of a recursive CTE",
                "Write a recursive CTE that walks a self-referencing hierarchy (e.g., an employee/manager org chart)",
                "Recognize the risk of an infinite recursive CTE and how to guard against it",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    A **Common Table Expression (CTE)** is a named, temporary result set defined with a `WITH` clause and used within a single query, as if it were a table. CTEs exist purely for readability and reuse *within one query* — they don't persist beyond it, and they're not a materialized view or an index.

                    This is functionally identical to writing the same logic as a nested subquery — the benefit is purely that a well-named CTE reads top-to-bottom like a sentence, instead of forcing the reader to parse inside-out.

                    A **recursive CTE** (`WITH RECURSIVE` in the ANSI standard, PostgreSQL, SQLite, and MySQL 8+; plain `WITH` in SQL Server, which treats recursion as implicit) can reference *itself*, which makes it the standard tool for walking self-referencing hierarchical data — org charts, category trees, bill-of-materials explosions, folder structures. It has two parts joined by `UNION ALL`:

                    1. **Anchor member** — a normal query with no self-reference; it produces the starting row(s) (e.g., the top-level employee with no manager).
                    2. **Recursive member** — a query that references the CTE's own name, joining the previous result to find the "next level." The engine re-runs this repeatedly, feeding each iteration's output back in as input, until an iteration produces zero rows.

                    Because a recursive CTE keeps re-running until it produces no new rows, a self-referencing table with a cycle (row A points to B, B points back to A) can recurse forever — most engines provide a safety limit (SQL Server's `MAXRECURSION`; PostgreSQL requires you to break cycles yourself) but you shouldn't rely on that as your only protection.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    A plain CTE is like giving a long, complicated phrase a nickname at the top of a conversation ("let's call the customers who spent over $1,000 'VIPs'") so the rest of the conversation can just say "VIPs" instead of repeating the whole definition every time.

                    A recursive CTE is like passing a message down a chain of command and asking each person to add their own name before passing it further: you start with the CEO (the anchor), each step asks "who reports to the people I just found?" (the recursive member), and you keep going until someone has no one reporting to them — at which point the chain naturally stops growing.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **CTE anatomy**

                    - `WITH name AS (subquery) SELECT ... FROM name` — a plain CTE, scoped to the one statement that follows it
                    - Multiple CTEs: `WITH a AS (...), b AS (...) SELECT ... FROM a JOIN b ...`
                    - A CTE is not indexed, not cached across queries, and not a substitute for a real table or materialized view

                    **Recursive CTE anatomy**

                    - `WITH RECURSIVE name AS (anchor UNION ALL recursive-member-referencing-name) SELECT ... FROM name`
                    - Anchor member: runs once, no self-reference
                    - Recursive member: references `name`, runs repeatedly until it returns 0 rows
                    - Must use `UNION ALL`, not `UNION` (a plain `UNION` would try to de-duplicate against a still-growing result, which most engines disallow or handle inconsistently)
                    - Always include a depth counter or path column to detect/limit cycles in real (not guaranteed acyclic) data
                    """, 3),
                Block(BlockType.CodeSnippet, "A Plain CTE, Then a Recursive CTE Over an Org Chart", BodyFormat.PlainText, """
                    -- Plain CTE: named subquery for readability, no recursion.
                    WITH department_totals AS (
                        SELECT department_id, SUM(salary) AS total_salary
                        FROM employees
                        GROUP BY department_id
                    )
                    SELECT d.department_id, d.total_salary
                    FROM department_totals d
                    WHERE d.total_salary > 500000;

                    -- Recursive CTE: walk an org chart from the top down, tracking depth.
                    WITH RECURSIVE org_chart AS (
                        -- Anchor member: employees with no manager (the top of the chart).
                        SELECT
                            employee_id,
                            manager_id,
                            name,
                            0 AS depth
                        FROM employees
                        WHERE manager_id IS NULL

                        UNION ALL

                        -- Recursive member: find each employee whose manager was just found.
                        SELECT
                            e.employee_id,
                            e.manager_id,
                            e.name,
                            oc.depth + 1
                        FROM employees e
                        INNER JOIN org_chart oc ON e.manager_id = oc.employee_id
                    )
                    SELECT employee_id, name, depth
                    FROM org_chart
                    ORDER BY depth, employee_id;
                    """, 4, language: "sql"),
                Block(BlockType.Diagram, "How a Recursive CTE Executes", BodyFormat.StructuredSteps, """
                    [{"label":"Anchor member runs once","note":"employees WHERE manager_id IS NULL -> e.g. just the CEO, depth 0"},{"label":"Recursive member, iteration 1","note":"find employees whose manager_id matches the anchor's employee_id -> CEO's direct reports, depth 1"},{"label":"Recursive member, iteration 2","note":"find employees reporting to depth-1 rows -> depth 2, and so on"},{"label":"Iteration N","note":"recursive member returns 0 new rows -> recursion stops"},{"label":"Final result","note":"UNION ALL of every iteration's rows, i.e. the whole org chart with a depth per employee"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Reach for a CTE whenever naming an intermediate result would make a query easier to read, especially when the same subquery would otherwise be repeated more than once in the statement — a CTE lets you define it once and reference it multiple times.

                    For recursive CTEs over real-world data, always add a `depth` (or accumulated `path`) column and a `WHERE depth < N` guard in the recursive member — even a schema you believe is a strict tree can end up with bad data that forms a cycle, and an unguarded recursive CTE against a cycle will run until it exhausts memory or hits the engine's hard recursion limit.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    "Given an employee table with a self-referencing manager_id, find everyone under a given manager" (the full reporting chain, not just direct reports) is an extremely common SQL interview question, and it's a near-verbatim recursive CTE: anchor on the given manager, recursive member joins employees to the growing result on `manager_id`. Naming it immediately as "a recursive CTE" and sketching the anchor/recursive-member split signals real fluency, not just syntax memorization.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Using `UNION` instead of `UNION ALL` in a recursive CTE — `UNION` tries to de-duplicate against a result set that's still being built, which most engines either reject outright or handle in a way that silently produces wrong results; recursive CTEs require `UNION ALL`.

                    Also common: writing a recursive CTE with no depth guard against data that isn't guaranteed acyclic (a `manager_id` that could, through bad data, eventually point back up the chain) — this can recurse indefinitely and take down the query (or the whole connection) instead of failing fast.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "In a recursive CTE, what is the purpose of the anchor member?",
                    "The anchor member is a normal query with no self-reference; it produces the starting row(s) that the recursive member then repeatedly builds on. Without it, there would be nothing for the recursive member to join against on the first iteration.",
                    [
                        new QuizOptionSeed("It runs repeatedly until no new rows are produced", false),
                        new QuizOptionSeed("It provides the starting row(s), with no reference to the CTE itself", true),
                        new QuizOptionSeed("It de-duplicates the final result", false),
                        new QuizOptionSeed("It sets the engine's recursion limit", false),
                    ]),
                new QuizQuestionSeed(
                    "Why must a recursive CTE use UNION ALL instead of UNION?",
                    "UNION would need to de-duplicate the recursive member's output against a result set that is still being built iteration by iteration, which isn't well-defined for most engines. UNION ALL simply appends each iteration's rows without attempting de-duplication, which is what recursive evaluation requires.",
                    [
                        new QuizOptionSeed("UNION ALL is only a stylistic preference with no functional difference", false),
                        new QuizOptionSeed("UNION would require de-duplicating against a result set that's still being built recursively, which isn't well-defined; UNION ALL avoids that", true),
                        new QuizOptionSeed("UNION is not valid syntax inside a WITH clause at all", false),
                        new QuizOptionSeed("UNION ALL is required only for performance, not correctness", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("PostgreSQL: WITH Queries (CTEs)", "https://www.postgresql.org/docs/current/queries-with.html", LinkType.OfficialDocs),
                new ReferenceLinkSeed("SQLite: The WITH Clause", "https://www.sqlite.org/lang_with.html", LinkType.OfficialDocs),
            ]);

        var lesson2Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson2.Slug,
        [
            "Rewrite a nested subquery you've written before as a named CTE",
            "Write a recursive CTE that walks a self-referencing hierarchy (org chart, category tree, or folder structure) from scratch",
            "Explain, out loud, why a recursive CTE must use UNION ALL and needs a cycle/depth guard on real-world data",
        ]);

        var module = BuildModule(topicId, "schema-design-and-advanced-queries", "Schema Design & Advanced Queries",
            "Normalization and referential integrity for solid schema design, plus common table expressions for readable and recursive queries.",
            80, [lesson1, lesson2], sortOrder: 2);

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

        var lesson3 = BuildLesson(
            slug: "containers-and-orchestration-basics",
            title: "Containers & Orchestration Basics: Docker to Kubernetes",
            summary: "Docker images vs. running containers, the problem Kubernetes actually solves, and the core objects — pods, deployments, services — that make it work.",
            estimatedMinutes: 40,
            objectives:
            [
                "Explain the difference between a container image and a running container",
                "Explain what problem Kubernetes solves that manually running containers doesn't",
                "Describe the relationship between pods, deployments, and services",
                "Read a Kubernetes Deployment/Service manifest and explain what it declares",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    A **container image** is an immutable, layered filesystem snapshot — your app, its runtime, and its dependencies bundled together. A **running container** is a live process created from that image, with its own isolated filesystem view and process namespace, but sharing the host OS kernel — unlike a VM, which virtualizes an entire OS.

                    **Docker** builds images from a `Dockerfile` (a set of layered instructions), stores them in a registry (Docker Hub, or a private ACR/ECR), and runs containers from those images on a single host.

                    Running containers by hand works for a demo, but breaks down at production scale: what restarts a crashed container? What happens when a host dies? How do containers find each other as they're rescheduled onto different hosts? **Kubernetes** is a container orchestrator that answers all three — it continuously reconciles a *desired state* (declared in YAML) against actual state, rescheduling and restarting containers as needed across a cluster of machines.

                    Three core Kubernetes objects:

                    - **Pod** — the smallest deployable unit; one or more tightly-coupled containers that share a network namespace and are always scheduled together.
                    - **Deployment** — declares how many replicas of a Pod template should exist, and manages rolling updates when the template changes.
                    - **Service** — a stable network endpoint (a virtual IP + DNS name) that load-balances traffic across whichever Pods currently match a label selector, even as individual Pods are replaced.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    A container image is like a shipping container's manifest and contents sealed at the factory — identical every time it's loaded. A running container is that container actually sitting on a ship, doing its job, sharing the same ship (the host kernel) as every other container aboard, unlike a VM, which would be its own separate ship.

                    Kubernetes is like a port's automated dispatch system: if a container gets damaged (a Pod crashes), the system loads a fresh replacement without waiting for a human to notice. If a truck route closes (a node goes down), dispatch reroutes cargo through a different truck automatically — you declare "I always want 3 of this container available," and the system keeps that promise no matter what fails underneath it.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Everyday Docker commands**

                    - `docker build -t myapp:1.0 .` — build an image from a Dockerfile
                    - `docker run -p 8080:8080 myapp:1.0` — run a container, mapping a host port to a container port
                    - `docker ps` / `docker logs <container>` — inspect running containers and their output
                    - `docker compose up` — start every service defined in `docker-compose.yml` together

                    **Everyday kubectl commands**

                    - `kubectl get pods` / `kubectl get deployments` / `kubectl get services`
                    - `kubectl apply -f deployment.yaml` — create or update objects to match a manifest
                    - `kubectl scale deployment myapp --replicas=5` — change the desired replica count
                    - `kubectl logs <pod-name>` / `kubectl describe pod <pod-name>` — debug a specific Pod
                    """, 3),
                Block(BlockType.CodeSnippet, "A Kubernetes Deployment and Service", BodyFormat.PlainText, """
                    apiVersion: apps/v1
                    kind: Deployment
                    metadata:
                      name: mentoros-api
                    spec:
                      replicas: 3
                      selector:
                        matchLabels:
                          app: mentoros-api
                      template:
                        metadata:
                          labels:
                            app: mentoros-api
                        spec:
                          containers:
                            - name: mentoros-api
                              image: myregistry.azurecr.io/mentoros-api:1.0
                              ports:
                                - containerPort: 8080
                    ---
                    apiVersion: v1
                    kind: Service
                    metadata:
                      name: mentoros-api
                    spec:
                      selector:
                        app: mentoros-api
                      ports:
                        - port: 80
                          targetPort: 8080
                    """, 4, language: "yaml"),
                Block(BlockType.Diagram, "From Deployment to Traffic", BodyFormat.AsciiArt, """
                    Deployment (desired state: 3 replicas)
                            |
                       ReplicaSet
                       /    |    \\
                     Pod   Pod   Pod    <- each runs 1+ containers, scheduled on any node
                       \\    |    /
                        Service (stable virtual IP + DNS name)
                            |
                    Load-balances traffic across whichever Pods
                    currently match the label selector
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Keep each container doing one job (one process per container) — a container running both a web server and a cron daemon is harder to restart, scale, and reason about independently than two single-purpose containers.

                    Set resource requests and limits (CPU/memory) on every container in a Deployment — without them, one misbehaving Pod can starve every other Pod on the same node, and the scheduler can't make good placement decisions.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked "why not just run Docker containers directly on a few VMs," answer with what Kubernetes actually automates: self-healing (restarting crashed Pods), declarative rolling updates and rollbacks, horizontal scaling, and service discovery across a constantly-changing set of Pod IPs — running containers by hand solves none of these once you have more than a handful of services.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Treating a Pod as if it has a stable identity or IP address — Pods are ephemeral and get replaced (with a new IP) constantly; anything that needs a stable address to talk to a Pod should go through a Service, never a Pod's IP directly.

                    Also common: skipping resource requests/limits "to keep the YAML simple," which lets a single leaking container silently degrade every other workload scheduled onto the same node.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "A Pod crashes and Kubernetes replaces it. What happens to the Pod's original IP address?",
                    "Pods are ephemeral; a replacement Pod gets a new IP address. Anything that needs to reach the workload reliably should address it through a Service, whose stable virtual IP and DNS name don't change as Pods are replaced.",
                    [
                        new QuizOptionSeed("It stays the same, since Kubernetes preserves Pod IPs across restarts", false),
                        new QuizOptionSeed("The replacement Pod gets a new IP; clients should use a Service instead of a Pod IP directly", true),
                        new QuizOptionSeed("The Deployment stops working until the IP is manually reassigned", false),
                        new QuizOptionSeed("Kubernetes pauses all other Pods until the IP is restored", false),
                    ]),
                new QuizQuestionSeed(
                    "What's the main problem Kubernetes solves that manually running Docker containers on a few servers does not?",
                    "Kubernetes continuously reconciles declared desired state (replica count, image version) against actual state — restarting crashed containers, rescheduling around failed nodes, and load-balancing across Pods — all automatically, which manual `docker run` commands provide no mechanism for.",
                    [
                        new QuizOptionSeed("It makes container images smaller", false),
                        new QuizOptionSeed("It automatically maintains a desired state — restarting, rescheduling, and load-balancing containers as failures happen", true),
                        new QuizOptionSeed("It replaces the need for a container runtime entirely", false),
                        new QuizOptionSeed("It eliminates the need for a Dockerfile", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Docker overview", "https://docs.docker.com/get-started/overview/", LinkType.OfficialDocs),
                new ReferenceLinkSeed("Kubernetes: Deployments", "https://kubernetes.io/docs/concepts/workloads/controllers/deployment/", LinkType.OfficialDocs),
            ],
            prerequisites: [lesson1]);

        var lesson3Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson3.Slug,
        [
            "Write a docker-compose.yml that runs your own app plus one dependency (e.g. a database) together",
            "Explain what a Deployment, Pod, and Service each do, out loud, without notes",
            "Describe how Kubernetes restarts a crashed container, in your own words",
        ]);

        var lesson4 = BuildLesson(
            slug: "cloud-security-fundamentals",
            title: "Cloud Security Fundamentals: IAM, Secrets & Network Boundaries",
            summary: "Identity and access management, least-privilege roles, secrets management, and network security groups that keep cloud resources locked down.",
            estimatedMinutes: 40,
            objectives:
            [
                "Explain the difference between authentication and authorization",
                "Design an IAM policy that follows least privilege for a specific task",
                "Explain why secrets must never be hardcoded or committed to source control, and where they should live instead",
                "Explain what a network security group / firewall rule actually restricts",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    **Identity and Access Management (IAM)** answers two separate questions for every request: **authentication** ("who are you?" — verifying identity, e.g. via a signed token or API key) and **authorization** ("what are you allowed to do?" — checking that identity against a policy). A request can be authenticated but still fail authorization, and the two failures should be treated differently.

                    An IAM **policy** attaches a set of allowed actions on specific resources to an identity (a user, a service, or a role). Applying **least privilege** means writing that policy as narrowly as the task allows — e.g., "read objects from bucket X" instead of "full access to all storage" — so a leaked credential or a compromised service can only do limited damage.

                    **Secrets** (API keys, database passwords, connection strings, certificates) must never be hardcoded in source code or committed to version control — once in git history, a secret is effectively permanently exposed, even if deleted in a later commit. Instead, secrets belong in a dedicated secrets manager (e.g., a cloud key vault) or are injected as environment variables/mounted files at deploy time, pulled fresh by the running service rather than baked into an image or a repo.

                    A **network security group (NSG)** (also called a security group, or a firewall rule set depending on the provider) is a set of allow/deny rules controlling what network traffic can reach a resource — by port, protocol, and source. Combined, IAM and NSGs form **defense in depth**: even if one layer is bypassed, another layer still limits the blast radius.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Authentication is like showing ID at a building's front desk ("prove you are who you say you are"); authorization is the separate question of which floors your keycard actually opens once you're inside — you can be a verified employee (authenticated) and still be denied entry to the server room (unauthorized).

                    A network security group is like a building's front-door policy: it decides who's even allowed to approach which door, regardless of what they're carrying — a perimeter control, separate from (and in addition to) checking each person's ID once they're inside.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Authentication vs. authorization**

                    - Authentication — verifying identity (login, API key, signed token)
                    - Authorization — verifying permission (does this identity's policy allow this action on this resource?)

                    **Secrets: where they should (and shouldn't) live**

                    - Never: hardcoded in source, committed to git, baked into a container image layer
                    - Yes: a secrets manager / key vault, injected as env vars or mounted files at runtime, referenced by name in CI/CD (not pasted into pipeline YAML)

                    **Rough network-boundary equivalents across providers**

                    - AWS: Security Groups (instance-level) + Network ACLs (subnet-level)
                    - Azure: Network Security Groups (NSGs)
                    - GCP: Firewall Rules (VPC-level)
                    """, 3),
                Block(BlockType.CodeSnippet, "A Kubernetes NetworkPolicy Restricting Traffic", BodyFormat.PlainText, """
                    apiVersion: networking.k8s.io/v1
                    kind: NetworkPolicy
                    metadata:
                      name: api-allow-from-frontend-only
                    spec:
                      podSelector:
                        matchLabels:
                          app: mentoros-api
                      policyTypes:
                        - Ingress
                      ingress:
                        - from:
                            - podSelector:
                                matchLabels:
                                  app: mentoros-frontend
                          ports:
                            - protocol: TCP
                              port: 8080
                    """, 4, language: "yaml"),
                Block(BlockType.Diagram, "Defense in Depth: Layered Security", BodyFormat.AsciiArt, """
                       Internet
                          |
                     [ Network Security Group / Firewall ]   <- only allow expected ports/sources
                          |
                     [ Authentication ]                      <- verify identity (who is this?)
                          |
                     [ Authorization / IAM Policy ]           <- verify permission (allowed to do this?)
                          |
                     [ Application + Data ]                   <- the actual resource being protected

                    Each layer assumes the one before it might fail —
                    a bypass at one layer still has to get through the next.
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Store every secret in a dedicated secrets manager and reference it by name at deploy time — never paste a real secret value into a Dockerfile, a YAML manifest, or a CI/CD pipeline definition, since all three commonly end up in source control.

                    Grant IAM roles per-service, scoped to exactly the resources that service touches, and review them periodically — permissions that were correct at launch tend to become overly broad over time as nobody removes access that's no longer needed.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked how you'd secure a new cloud service, describe layers, not just one control: network boundaries (who can even reach it), authentication (who are they), authorization (what can they do), and secrets handling (how does it get its credentials) — naming all four, unprompted, signals defense-in-depth thinking rather than a single point of protection.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Committing a real API key or connection string to git "temporarily" to get something working — even one commit permanently exposes it in history, and rotating it afterward is mandatory, not optional, once that happens.

                    Also common: granting a service or CI pipeline broad "admin" or "contributor" access at setup time to unblock work quickly, then never tightening it once the immediate problem is solved.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "A request presents a valid, correctly-signed API key but tries to delete a resource its policy doesn't permit. What's the correct way to describe this failure?",
                    "The request is authenticated (the key proves who's making it) but not authorized (the policy doesn't grant that action) — these are separate checks, and failing one doesn't mean the other also failed.",
                    [
                        new QuizOptionSeed("An authentication failure, since the key must be invalid", false),
                        new QuizOptionSeed("An authorization failure — identity was verified, but the policy doesn't permit this action", true),
                        new QuizOptionSeed("A network security group failure", false),
                        new QuizOptionSeed("This scenario can't happen; a valid key always has full access", false),
                    ]),
                new QuizQuestionSeed(
                    "Why is committing a secret to git considered permanently exposed, even if you delete it in a later commit?",
                    "Git preserves full history by design — the secret still exists in an earlier commit object and is reachable by anyone who can read the repository's history, so deleting it from the latest commit doesn't remove it from the past ones. The only real fix is rotating (invalidating) the secret.",
                    [
                        new QuizOptionSeed("It isn't — deleting the file in a new commit fully removes it", false),
                        new QuizOptionSeed("Git preserves the old commit containing the secret in its history, unless that history is explicitly rewritten and the secret rotated", true),
                        new QuizOptionSeed("Secrets are automatically encrypted by git once committed", false),
                        new QuizOptionSeed("Only the most recent commit is ever accessible to other users", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("AWS: IAM security best practices", "https://docs.aws.amazon.com/IAM/latest/UserGuide/best-practices.html", LinkType.OfficialDocs),
                new ReferenceLinkSeed("Kubernetes: Network Policies", "https://kubernetes.io/docs/concepts/services-networking/network-policies/", LinkType.OfficialDocs),
            ],
            prerequisites: [lesson2]);

        var lesson4Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson4.Slug,
        [
            "Write (or find) an IAM policy for one of your services and check it against least privilege",
            "Confirm no real secret exists in your own git history using `git log -p` or a secrets-scanning tool",
            "Explain the difference between authentication and authorization using your own example",
        ]);

        var module = BuildModule(topicId, "cloud-fundamentals", "Cloud Fundamentals",
            "The service-model spectrum, compute options, containers and orchestration, security fundamentals, and automating deployment with CI/CD and Infrastructure as Code.",
            145, [lesson1, lesson2, lesson3, lesson4]);

        return (module, [lesson1Checklist, lesson2Checklist, lesson3Checklist, lesson4Checklist]);
    }

    private static (Module, List<ChecklistSeed>) BuildCloudObservabilityModule(int topicId)
    {
        var lesson1 = BuildLesson(
            slug: "serverless-functions-and-event-triggers",
            title: "Serverless & Functions-as-a-Service",
            summary: "When serverless fits a workload (and when it doesn't), what actually causes a cold start, and the statelessness constraints that shape how a function must be written.",
            estimatedMinutes: 35,
            objectives:
            [
                "Decide whether serverless functions are a good fit for a given workload, or whether containers/VMs are the better choice",
                "Explain what causes a cold start and how to reduce its impact",
                "Design a function around statelessness: no reliance on local disk/memory surviving between invocations, and safe handling of duplicate/retried events",
                "Wire a function to at least two different event-driven trigger types (HTTP, queue, storage, schedule)",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    **Functions-as-a-Service (FaaS)** is the most abstracted compute option: you provide only a function's code, and the platform provisions the runtime, scales the number of running instances (including down to zero when idle), and bills per invocation and execution duration rather than per hour of a running server.

                    Functions are **event-driven** — each invocation is triggered by something happening: an HTTP request, a message landing in a queue, a file uploaded to object storage, or a schedule (cron-style). The platform is responsible for matching incoming events to available (or newly created) function instances.

                    Two properties fall directly out of this model:

                    - **Statelessness** — nothing an invocation writes to local memory or local disk is guaranteed to exist on the next invocation, because that next invocation may run on a completely different instance. Anything that must persist (a processed-order record, a session) has to live in external storage (a database, cache, or object store).
                    - **Cold starts** — when no idle ("warm") instance is available to handle an incoming event, the platform must provision a brand-new execution environment: start a container/sandbox, load the language runtime, and load your code and its dependencies, before your function's first line even runs. A "warm" invocation, reusing an existing instance, skips all of that and starts almost instantly.

                    Serverless is a strong fit for **spiky, sporadic, short-lived** workloads (an image-resize triggered by an upload, a nightly report, a low-traffic API). It's a poor fit for **long-running, persistently-connected, or highly latency-sensitive** workloads (a WebSocket held open for hours, a process needing consistent sub-millisecond response with no cold-start risk) — those still belong on containers or VMs.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    A serverless function is like a pop-up food stall that only assembles itself when a customer walks up, and disassembles when there's no one in line — you never pay rent for it sitting idle, but the very first customer of the day waits slightly longer while the stall is being set up (the cold start). A dedicated restaurant (a VM or long-running container) is always staffed and ready the instant someone walks in, but you're paying for that staff whether anyone shows up or not.

                    Statelessness is like a hotel room, not your own bedroom: whatever you leave lying around isn't guaranteed to be there (or even in the same room) the next time you check in — anything you actually need again has to go in your suitcase (external storage), not on the nightstand.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Good fit for serverless**

                    - Sporadic, bursty, or low-traffic HTTP APIs
                    - Reacting to an event: file uploaded, message queued, row inserted
                    - Scheduled/cron-style jobs (nightly cleanup, periodic report)
                    - Short-lived data transforms (resize an image, parse a file)

                    **Poor fit for serverless**

                    - Long-running or persistently-connected work (WebSockets, long batch jobs beyond the platform's max execution duration)
                    - Workloads needing guaranteed low, consistent latency with zero cold-start tolerance
                    - Heavy in-memory caching that needs to survive across requests

                    **Common trigger types**

                    - HTTP request (synchronous API)
                    - Queue/topic message (asynchronous, at-least-once delivery)
                    - Object storage event (file created/updated/deleted)
                    - Schedule (cron expression)
                    - Stream (e.g., a change-data-capture or event-stream record)
                    """, 3),
                Block(BlockType.CodeSnippet, "A Stateless, Idempotent Queue-Triggered Function", BodyFormat.PlainText, """
                    public class OrderCreatedFunction
                    {
                        private readonly IOrderStore _orderStore;
                        private readonly IEmailClient _emailClient;
                        private readonly ILogger<OrderCreatedFunction> _logger;

                        public OrderCreatedFunction(IOrderStore orderStore, IEmailClient emailClient, ILogger<OrderCreatedFunction> logger)
                        {
                            _orderStore = orderStore;
                            _emailClient = emailClient;
                            _logger = logger;
                        }

                        [Function("OrderCreatedFunction")]
                        public async Task Run([QueueTrigger("order-created-queue")] OrderCreatedEvent orderEvent)
                        {
                            // Queue triggers are at-least-once: the platform may redeliver the
                            // same message (after a timeout, a retry, or an instance restart).
                            // Checking an idempotency key makes a duplicate delivery a safe no-op
                            // instead of double-charging or double-emailing the customer.
                            if (await _orderStore.HasProcessedAsync(orderEvent.IdempotencyKey))
                            {
                                _logger.LogInformation("Duplicate delivery for {Key}, skipping", orderEvent.IdempotencyKey);
                                return;
                            }

                            await _orderStore.MarkProcessedAsync(orderEvent.IdempotencyKey);
                            await _emailClient.SendOrderConfirmationAsync(orderEvent.CustomerEmail, orderEvent.OrderId);

                            // Nothing durable is kept in local memory or on local disk here --
                            // the next invocation could land on a brand-new instance with none
                            // of this one's state, warm or cold.
                        }
                    }
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "Event to Response, Cold vs. Warm", BodyFormat.StructuredSteps, """
                    [{"label":"Event Source","note":"HTTP request, queue message, storage upload, or schedule fires"},{"label":"Platform Receives Event"},{"label":"Warm instance available?","note":"no: cold start -- provision sandbox, load runtime + code | yes: reuse existing instance"},{"label":"Function Instance Executes","note":"one invocation, treated as stateless"},{"label":"Response / Downstream Call"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Design every function to be **idempotent** against retries — event-driven triggers are typically at-least-once, not exactly-once, so the same event can and eventually will be delivered more than once.

                    Keep deployment packages small and dependencies minimal; a smaller package loads faster during a cold start. Where the platform supports it (e.g., provisioned/reserved concurrency) and cold-start latency genuinely matters for the workload, pay to keep a minimum number of instances warm rather than trying to eliminate cold starts entirely.

                    Separate the trigger-handling code from the actual business logic (a thin function wrapper calling into a plain, unit-testable class) — this keeps the logic testable without needing to invoke the real cloud trigger infrastructure in tests.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    When asked to compare serverless to containers for a given workload, lead with the trade-off, not a definition: "serverless minimizes idle cost and operational overhead, but pays for that with cold-start latency and an execution-time ceiling, so it's a strong fit for this sporadic image-processing workload but a poor fit for the always-on matching service that needs consistent sub-50ms latency." That framing shows you understand the constraint, not just the buzzword.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Assuming a value written to a local variable, in-memory cache, or local disk during one invocation will still be there on the next one — it might be, if the same warm instance happens to be reused, but that's never guaranteed and code that depends on it will fail unpredictably in production.

                    Also common: not handling duplicate event delivery (treating "at-least-once" as if it were "exactly-once"), and shipping an oversized deployment package full of unused dependencies that quietly makes every cold start slower than it needs to be.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "What primarily causes a serverless function's 'cold start'?",
                    "A cold start happens when no idle (warm) execution environment exists for the incoming event, so the platform must provision a new sandbox, start the language runtime, and load the function's code and dependencies before the function's first line can run.",
                    [
                        new QuizOptionSeed("A syntax error in the function's code", false),
                        new QuizOptionSeed("No warm instance is available, so the platform must provision a new runtime and load the code before executing", true),
                        new QuizOptionSeed("The cloud provider intentionally throttles requests during business hours", false),
                        new QuizOptionSeed("Cold starts only happen for functions triggered by HTTP requests", false),
                    ]),
                new QuizQuestionSeed(
                    "Which workload is the poorest fit for a serverless function, all else being equal?",
                    "A long-running, persistently-connected process with in-memory session state clashes with two core constraints of serverless: statelessness (no guaranteed in-memory persistence across invocations) and a maximum execution duration -- it belongs on a container or VM instead.",
                    [
                        new QuizOptionSeed("A REST endpoint invoked a few times per minute", false),
                        new QuizOptionSeed("A nightly batch job triggered on a schedule", false),
                        new QuizOptionSeed("A long-running WebSocket connection held open for hours with in-memory session state", true),
                        new QuizOptionSeed("Resizing an image in response to a storage upload event", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("AWS Lambda: Understanding Lambda function scaling", "https://docs.aws.amazon.com/lambda/latest/dg/lambda-scaling.html", LinkType.OfficialDocs),
                new ReferenceLinkSeed("Azure Functions triggers and bindings concepts", "https://learn.microsoft.com/en-us/azure/azure-functions/functions-triggers-bindings", LinkType.OfficialDocs),
            ]);

        var lesson1Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson1.Slug,
        [
            "List one workload from your own experience that fits serverless well, and one that doesn't, with your reasoning",
            "Find (or add) an idempotency check in a queue- or event-triggered function you've written or reviewed",
            "Explain out loud two ways to reduce cold-start impact for a latency-sensitive function",
        ]);

        var lesson2 = BuildLesson(
            slug: "cloud-observability-metrics-logs-tracing",
            title: "Cloud Observability: Metrics, Logs & Distributed Tracing",
            summary: "The three pillars of observability, propagating a correlation ID, and reading a distributed trace as a request crosses multiple services.",
            estimatedMinutes: 35,
            objectives:
            [
                "Distinguish metrics, logs, and traces, and pick the right one for a given debugging question",
                "Explain how a correlation/trace ID lets events from a single logical request be linked across service boundaries",
                "Read a distributed trace spanning multiple services to find which downstream call is responsible for added latency",
                "Set an alert on a user-facing signal (an SLI) instead of a raw resource metric that produces noisy, low-value alerts",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    **Observability** is commonly described as three pillars, each answering a different kind of question:

                    - **Metrics** — numeric measurements sampled over time (request count, p99 latency, CPU usage, error rate). Cheap to store and aggregate, great for dashboards and "is something wrong right now" alerting, but they can't tell you *which specific request* caused a spike.
                    - **Logs** — discrete, timestamped events with arbitrary detail ("order 4821 failed validation: missing shipping address"). Great for "what exactly happened," but scanning raw logs across many service instances to reconstruct one request's journey is slow and error-prone without a shared identifier.
                    - **Traces** — the record of a single request's journey as it moves through a system, made of **spans**: one span per unit of work (an HTTP handler, a downstream call, a database query), each with a start time, duration, and a parent span. All spans belonging to one request share a **trace ID**, so a tracing backend can stitch them back into a single tree showing exactly where time was spent.

                    A **correlation ID** (in a full tracing system, the trace ID) is generated at the entry point of a request and passed along on every downstream call — typically via an HTTP header — so that every log line, span, and metric emitted anywhere in the system for that request can be tied back together. The W3C **Trace Context** standard defines a common `traceparent` header so that services built with different languages and vendors can still propagate the same trace ID consistently.

                    Because capturing a full trace for every single request at high volume is expensive, most tracing systems apply **sampling** — recording a representative subset of traces (e.g., 1 in 100, or always sampling slow/error requests) rather than every one.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Think of a package shipped through several carriers before reaching your door. **Metrics** are the carrier's daily dashboard: "12,000 packages delivered today, average transit time 2.1 days" — useful for spotting a company-wide slowdown, useless for finding *your* package. **Logs** are the individual scan events at each depot: timestamped, detailed, but scattered across different carriers' systems. A **trace** is what you get by following one specific tracking number across every carrier that touched your package — the same identifier links every scan into one door-to-door story, showing you exactly which depot held onto it the longest.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Three pillars at a glance**

                    - Metrics — numeric, aggregated, cheap, great for dashboards/alerting, no per-request detail
                    - Logs — detailed, per-event, hard to correlate across services without a shared ID
                    - Traces — one request's full journey as a tree of spans, linked by a trace ID

                    **Key terms**

                    - **Span** — one unit of work within a trace (a handler, a downstream call, a query)
                    - **Trace ID / correlation ID** — the identifier shared by everything belonging to one logical request
                    - **p50 / p95 / p99 latency** — the median / 95th-percentile / 99th-percentile response time; p99 exposes the slow tail that an average hides
                    - **SLI / SLO / error budget** — Service Level Indicator (a measured signal, e.g., error rate), Objective (the target for it, e.g., 99.9%), and the allowed room to miss it before it's a problem

                    **Common tooling** — Prometheus/Grafana and CloudWatch (metrics); the ELK stack and CloudWatch Logs (logs); Jaeger, Zipkin, AWS X-Ray, and Application Insights (traces); OpenTelemetry as the vendor-neutral instrumentation standard tying all three together.
                    """, 3),
                Block(BlockType.CodeSnippet, "Correlated Logging with .NET's Built-In Activity API", BodyFormat.PlainText, """
                    public class OrderService
                    {
                        private static readonly ActivitySource ActivitySource = new("OrderService");
                        private readonly ILogger<OrderService> _logger;

                        public OrderService(ILogger<OrderService> logger) => _logger = logger;

                        public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
                        {
                            // Starting an Activity creates a new span; if a traceparent header
                            // was received from an upstream caller, this span is automatically
                            // linked as a child of that same trace -- no manual ID plumbing needed.
                            using var activity = ActivitySource.StartActivity("CreateOrder");
                            activity?.SetTag("order.customerId", request.CustomerId);

                            // Activity.Current.TraceId flows into the logging scope, so every
                            // log line emitted here -- and in any downstream service that
                            // received the propagated traceparent header -- can be filtered
                            // down to exactly this one request.
                            using var _ = _logger.BeginScope(new Dictionary<string, object>
                            {
                                ["TraceId"] = Activity.Current?.TraceId.ToString() ?? "none",
                            });

                            _logger.LogInformation("Creating order for customer {CustomerId}", request.CustomerId);

                            var order = await PersistAndChargeAsync(request);

                            _logger.LogInformation("Order {OrderId} created in {ElapsedMs}ms", order.Id, activity?.Duration.TotalMilliseconds);
                            return order;
                        }
                    }
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "One Trace ID, Stitched Across Three Services", BodyFormat.StructuredSteps, """
                    [{"label":"Client Request","note":"generates a trace id, or forwards one it already received"},{"label":"API Gateway","note":"root span begins here"},{"label":"Order Service","note":"child span, same trace id, called by the gateway"},{"label":"Payment Service","note":"child span, called by Order Service"},{"label":"Database Call","note":"innermost span -- often where latency is hiding"},{"label":"Trace Assembled","note":"the tracing backend stitches every span sharing this trace id into one tree"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Use **structured logging** (named fields like `{CustomerId}`, not string concatenation) so log fields can be filtered and aggregated, and always propagate trace context using the standard W3C `traceparent` header rather than a homegrown one, so every service and vendor tool in the stack can read it consistently.

                    Alert on **SLIs that reflect what users actually experience** (p99 latency, error rate, availability) with a defined SLO and error budget, rather than alerting directly on raw resource metrics like CPU or memory, which can be perfectly normal while users are still failing requests (or can spike harmlessly with no user impact at all).

                    Sample traces deliberately: capture a small baseline percentage of all requests, but always capture requests that are slow or that error, so the traces you keep are the ones actually worth looking at.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked "how would you debug a request that's slow somewhere across five microservices," don't describe grepping logs in each service one at a time — describe pulling up the distributed trace for that request's trace ID and reading straight to the span with the largest duration. If the system doesn't have full tracing yet, the fallback answer is still solid: "at minimum, I'd make sure a correlation ID is generated at the edge and logged by every service it touches, so I can filter all their logs down to one request even without a full trace view."
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Logging without any correlation or trace ID, so a single slow or failed request leaves behind scattered log lines across several services with nothing tying them together — reconstructing what happened becomes guesswork.

                    Also common: alerting purely on infrastructure metrics (CPU, memory, disk) instead of user-facing SLIs, which either misses real user-facing incidents entirely or pages someone for a CPU blip nobody actually experienced as a problem; and leaving trace sampling unset, which either produces blind spots (too little sampled) or an unexpectedly large tracing bill (too much sampled).
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "A request takes 4 seconds end-to-end across three microservices. Which observability pillar is best suited to pinpoint exactly which downstream call caused the slowdown?",
                    "Distributed tracing breaks a single request into a tree of spans, each with its own duration, so you can see precisely which span (and therefore which service/call) accounts for the time -- something aggregated metrics and uncorrelated logs can't show for one specific request.",
                    [
                        new QuizOptionSeed("An aggregated metrics dashboard showing average latency", false),
                        new QuizOptionSeed("Centralized logs searched without any shared correlation or trace id", false),
                        new QuizOptionSeed("A distributed trace showing the full span tree for that one request", true),
                        new QuizOptionSeed("Increasing log verbosity to Debug level across all services", false),
                    ]),
                new QuizQuestionSeed(
                    "What is the main purpose of propagating a trace/correlation ID across service-to-service calls?",
                    "Propagating a shared trace/correlation id lets every log line, span, and metric emitted anywhere in the system for one logical request be linked back together, even as that request crosses process and service boundaries -- without it, there's no reliable way to reconstruct one request's full journey.",
                    [
                        new QuizOptionSeed("To encrypt the request payload in transit", false),
                        new QuizOptionSeed("To let every log line, span, and metric from one logical request be linked together across services", true),
                        new QuizOptionSeed("To reduce the total number of HTTP headers sent", false),
                        new QuizOptionSeed("To automatically retry any request that fails", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("OpenTelemetry: Traces", "https://opentelemetry.io/docs/concepts/signals/traces/", LinkType.OfficialDocs),
                new ReferenceLinkSeed(".NET distributed tracing concepts", "https://learn.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing", LinkType.OfficialDocs),
            ]);

        var lesson2Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson2.Slug,
        [
            "Add a correlation/trace id to one log statement in your own project and confirm it appears on every related log line for that request",
            "Sketch the span tree for a request that crosses 2+ services you've worked with, marking where you'd expect latency to hide",
            "Replace one raw-resource-metric alert (CPU/memory) with an alert based on a user-facing SLI (p99 latency or error rate)",
        ]);

        var module = BuildModule(topicId, "serverless-and-observability", "Serverless & Observability",
            "Building event-driven, stateless functions-as-a-service, and seeing inside a distributed system once it's running in production — metrics, logs, and distributed traces working together.",
            70, [lesson1, lesson2], sortOrder: 2);

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

        var lesson3 = BuildLesson(
            slug: "undoing-changes-reset-revert-reflog",
            title: "Undoing Changes Safely: Reset, Revert, Restore & the Reflog Safety Net",
            summary: "What reset's three modes actually touch, when to reach for revert or restore instead, and why the reflog means most 'disasters' are recoverable.",
            estimatedMinutes: 35,
            objectives:
            [
                "Explain what --soft, --mixed, and --hard each do to HEAD, the staging index, and the working directory",
                "Choose revert over reset for anything already pushed and shared",
                "Recover a commit that a hard reset appeared to destroy, using git reflog",
                "Shelve in-progress changes with git stash to switch context without losing work",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    Git gives you several distinct ways to undo something, and picking the wrong one is how "undoing a mistake" turns into a bigger mistake.

                    **`git reset <commit>`** moves the current branch's pointer (HEAD) to a different commit, with three modes controlling how much else it touches:

                    - `--soft` — moves HEAD only; the staging index and working directory are untouched, so the undone commit's changes end up staged, ready to re-commit.
                    - `--mixed` (the default) — moves HEAD and resets the staging index, but leaves the working directory files exactly as they were.
                    - `--hard` — moves HEAD, resets the staging index, AND overwrites the working directory. The undone changes are gone from disk.

                    **`git revert <commit>`** doesn't move anything — it creates a brand-new commit whose changes are the exact opposite of the target commit, so history only ever grows forward. That's what makes it safe to use on commits other people have already pulled.

                    **`git restore <file>`** (and the older `git checkout -- <file>`) discards uncommitted changes to a single file, restoring it to its last-committed state, without touching commit history at all. `git restore --staged <file>` unstages a file without discarding its edits.

                    **`git stash`** temporarily shelves uncommitted changes (staged and unstaged) onto a stack, giving you a clean working directory to switch branches or pull latest; `git stash pop` brings them back.

                    The **reflog** (`git reflog`) is Git's local safety net: it records every commit HEAD has pointed to on your machine — including ones a hard reset just "deleted" — for about 90 days by default, so a bad reset is almost never truly unrecoverable.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    `git reset --soft` is like un-checking-out library books but leaving them stacked on your desk — the loan record is undone, but the books are still right there. `git reset --hard` is like un-checking-out the books AND shredding them — nothing left to work with.

                    The reflog is like a security camera in that same library: even after the books are shredded, there's still a recording of exactly where they were, which is usually enough to reconstruct what you lost.

                    `git stash` is like sweeping the papers on your desk into a drawer when a surprise visitor arrives, so you can deal with them later without losing anything.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Undo commands at a glance**

                    - `git reset --soft HEAD~1` — undo last commit, keep changes staged
                    - `git reset --mixed HEAD~1` — undo last commit, keep changes unstaged (default mode)
                    - `git reset --hard HEAD~1` — undo last commit AND discard its changes (destructive)
                    - `git revert <commit>` — undo a commit by adding a new, opposite commit (safe, shareable)
                    - `git restore <file>` — discard uncommitted changes to one file
                    - `git restore --staged <file>` — unstage a file without discarding its edits

                    **Stash**

                    - `git stash` — shelve uncommitted changes
                    - `git stash pop` — reapply the most recent stash and remove it from the stack
                    - `git stash list` — see everything currently stashed

                    **Reflog**

                    - `git reflog` — see every place HEAD has pointed, locally, recently
                    - `git reset --hard <hash-from-reflog>` — recover a commit a hard reset removed
                    """, 3),
                Block(BlockType.CodeSnippet, "Reset, Revert, Stash & Reflog in Practice", BodyFormat.PlainText, """
                    # Uncommit the last 2 commits but keep their changes staged
                    git reset --soft HEAD~2

                    # Uncommit and unstage the last commit, but keep the file edits on disk
                    git reset --mixed HEAD~1

                    # Discard the last commit AND its changes entirely (destructive, local-only)
                    git reset --hard HEAD~1

                    # Undo a commit that's already been pushed, without rewriting history
                    git revert a1b2c3d

                    # Restore a single file to how it looked in the last commit
                    git restore src/Config.cs

                    # Shelve messy in-progress work to switch branches cleanly
                    git stash
                    git checkout main
                    # ... handle the urgent thing ...
                    git checkout feature/my-work
                    git stash pop

                    # "I just did something I regret" — the reflog remembers every HEAD move
                    git reflog
                    # 3a7f2e1 HEAD@{0}: reset: moving to HEAD~1
                    # d9c4b80 HEAD@{1}: commit: Add retry logic
                    git reset --hard d9c4b80   # recover the commit the reset just threw away
                    """, 4, language: "bash"),
                Block(BlockType.Diagram, "What Each Reset Mode Touches", BodyFormat.AsciiArt, """
                    Mode      HEAD pointer   Staging index   Working directory
                    --soft    moves          untouched       untouched
                    --mixed   moves          reset           untouched
                    --hard    moves          reset           reset (DESTRUCTIVE)

                    git reset --soft HEAD~1   -> commit undone, changes still staged
                    git reset --mixed HEAD~1  -> commit undone, changes unstaged but present
                    git reset --hard HEAD~1   -> commit undone, changes gone entirely
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Before running `git reset --hard` or any destructive rewrite, know that `git reflog` exists — Git keeps a local log of every place HEAD has pointed for about 90 days by default, so a "disaster" is almost always recoverable with `git reset --hard <reflog-entry>`.

                    Prefer `git restore` for file-level undo over the older, overloaded `git checkout` syntax — `restore` was split out specifically because `checkout` did too many unrelated things (switching branches AND restoring files), and that ambiguity caused real mistakes.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked "you just ran `git reset --hard` and lost work, what do you do," don't say "it's gone" — say `git reflog`, find the commit hash from before the reset, and `git reset --hard` back to it. Knowing the reflog exists is one of the most practical, high-signal pieces of Git knowledge in an interview, because it shows you've actually recovered from a real mistake before, not just read about the commands.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Reaching for `git reset --hard` out of habit to "clean up" without realizing it permanently discards uncommitted working-directory changes with no confirmation prompt — always run `git status` (and `git stash` if unsure) before a hard reset.

                    Also common: confusing `git revert` (safe, adds a new commit, fine for shared history) with `git reset` (rewrites history, unsafe once pushed and pulled by others) — using them interchangeably in conversation is a quick way to signal you don't actually understand the difference.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "You accidentally ran `git reset --hard HEAD~3`, discarding three commits. Is this recoverable?",
                    "Yes, almost always. The reflog records every commit HEAD has pointed to locally, even ones a hard reset appears to remove. Find the old commit hash with `git reflog` and run `git reset --hard <hash>` to get it back.",
                    [
                        new QuizOptionSeed("No — --hard permanently deletes commits with no way to recover them", false),
                        new QuizOptionSeed("Yes — find the commit hash in git reflog and reset --hard back to it", true),
                        new QuizOptionSeed("Yes, but only by contacting your Git hosting provider's support team", false),
                        new QuizOptionSeed("Only if the commits had already been pushed to a remote", false),
                    ]),
                new QuizQuestionSeed(
                    "Which git reset mode moves HEAD back a commit but leaves the undone changes staged, ready to re-commit?",
                    "--soft only moves the HEAD/branch pointer; it deliberately leaves the staging index and working directory alone, so the previously-committed changes land back in the staging area as if you'd just run git add.",
                    [
                        new QuizOptionSeed("--hard", false),
                        new QuizOptionSeed("--soft", true),
                        new QuizOptionSeed("--mixed", false),
                        new QuizOptionSeed("git revert", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("git-reset — official documentation", "https://git-scm.com/docs/git-reset", LinkType.OfficialDocs),
                new ReferenceLinkSeed("Git Tools: Stashing and Cleaning", "https://git-scm.com/book/en/v2/Git-Tools-Stashing-and-Cleaning", LinkType.OfficialDocs),
            ],
            prerequisites: [lesson1]);

        var lesson3Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson3.Slug,
        [
            "Run git reset --soft, --mixed, and --hard on scratch commits and explain what each one left behind",
            "Deliberately hard-reset away a commit, then recover it using git reflog",
            "Use git stash to switch branches mid-change, then reapply it with git stash pop",
        ]);

        var lesson4 = BuildLesson(
            slug: "advanced-git-workflows-rebase-cherrypick-bisect",
            title: "Advanced Git Workflows: Interactive Rebase, Cherry-Pick & Bisect",
            summary: "Cleaning up local history with interactive rebase, porting single commits with cherry-pick, and binary-searching for a bug's origin with bisect.",
            estimatedMinutes: 40,
            objectives:
            [
                "Use interactive rebase to squash, reorder, and reword commits before opening a pull request",
                "Cherry-pick a single commit onto another branch without merging its entire history",
                "Use git bisect to binary-search commit history for the exact change that introduced a bug",
                "Compare trunk-based development and Git Flow, and state a concrete trade-off between them",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    **Interactive rebase** (`git rebase -i HEAD~n`) opens an editable list of your last `n` commits and lets you rewrite local history before sharing it: `pick` keeps a commit as-is, `reword` edits its message, `squash`/`fixup` merge it into the previous commit, `drop` removes it, and `edit` pauses so you can amend it. It's how a day of messy "wip", "fix typo", "actually fix it" commits becomes one or two clean, reviewable commits.

                    **Cherry-pick** (`git cherry-pick <commit>`) applies the changes from one specific commit onto your current branch as a new commit — useful for porting a single bug fix to a release branch without merging in everything else that branch's source has accumulated since.

                    **Bisect** (`git bisect`) finds the exact commit that introduced a regression using binary search: you tell it one commit you know is `bad` (has the bug) and one you know is `good` (doesn't), and Git checks out the midpoint commit for you to test. You mark it `good` or `bad`, and Git halves the remaining range again — finding the culprit in `O(log n)` checks instead of testing every commit one by one.

                    **Workflow strategies**: **trunk-based development** keeps everyone committing small, frequent changes directly (or via very short-lived branches) to a single main branch, often behind feature flags. **Git Flow** uses long-lived `develop`, `release`, and `feature` branches with a more formal release process. Trunk-based favors speed and continuous integration; Git Flow favors more structured, scheduled releases — most modern, fast-shipping teams lean trunk-based.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Interactive rebase is like editing a rough draft before submitting it — merging duplicate paragraphs, reordering sections, and fixing typos — so the reader sees a clean final version instead of your entire messy drafting process.

                    Cherry-pick is like photocopying one specific page out of a colleague's notebook instead of asking to merge your entire notebook with theirs.

                    Bisect is the classic "guess a number, narrow the range" game applied to commit history: instead of reading every page of a 1,000-page book to find where a typo was introduced, you check the middle page, decide if the typo exists yet, and repeat on the correct half — finding it in about 10 checks instead of 1,000.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Interactive rebase commands** (inside the editor `git rebase -i` opens)

                    - `pick` — keep the commit as-is
                    - `reword` — keep the changes, edit the commit message
                    - `squash` — combine into the previous commit, merge messages
                    - `fixup` — combine into the previous commit, discard this message
                    - `drop` — remove the commit entirely
                    - `edit` — pause here so you can amend the commit

                    **Cherry-pick & bisect**

                    - `git cherry-pick <hash>` — apply one commit's changes onto the current branch
                    - `git cherry-pick -x <hash>` — same, and records the original commit hash in the message
                    - `git bisect start` / `git bisect bad` / `git bisect good <hash>` — begin the binary search
                    - `git bisect reset` — end the session and return to where you started

                    **Trunk-based vs. Git Flow**

                    - Trunk-based — short-lived branches, frequent small merges to main, feature flags for incomplete work
                    - Git Flow — long-lived develop/release/feature branches, more formal, scheduled releases
                    """, 3),
                Block(BlockType.CodeSnippet, "Interactive Rebase, Cherry-Pick & Bisect", BodyFormat.PlainText, """
                    # Squash the last 4 WIP commits into clean ones before opening a PR
                    git rebase -i HEAD~4
                    # editor shows:
                    #   pick   a1b2c3d Add search endpoint
                    #   squash e4f5a6b wip
                    #   squash 7c8d9e0 fix typo
                    #   reword 1a2b3c4 add tests
                    # -> save and edit the combined commit message when prompted

                    # Port just the hotfix commit onto the release branch
                    git checkout release/2.4
                    git cherry-pick a1b2c3d

                    # Binary-search for the commit that introduced a regression
                    git bisect start
                    git bisect bad                # current commit is broken
                    git bisect good v1.9.0         # this old tag was known to work
                    # Git checks out the midpoint; you build/test it, then:
                    git bisect good                # (or `git bisect bad`)
                    # ... repeat until Git reports the first bad commit ...
                    git bisect reset               # done — return to your original HEAD
                    """, 4, language: "bash"),
                Block(BlockType.Diagram, "Bisect: Binary-Searching for the Bad Commit", BodyFormat.StructuredSteps, """
                    [{"label":"Mark current commit bad","note":"git bisect bad"},{"label":"Mark an old known-good commit","note":"git bisect good v1.9.0"},{"label":"Git checks out the midpoint commit"},{"label":"You build/test it"},{"label":"Mark it good or bad","note":"git bisect good | git bisect bad"},{"label":"Git halves the remaining range and repeats"},{"label":"First bad commit identified","note":"git bisect reset to finish"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Only interactively rebase commits that are still local and not yet pushed and shared — exactly like plain rebase, rewriting commit history that others may have already pulled and built on top of causes the same painful, confusing divergence.

                    When bisecting, write a small script that exits `0` (good) or `1` (bad) and hand it to `git bisect run <script>` instead of testing manually at every step — it turns a multi-step manual process into one command that finds the culprit unattended.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked how you'd track down which commit introduced a regression in a codebase with thousands of commits, say "bisect" and explain why it's fast: it's a binary search, so it finds the culprit in roughly `log2(n)` tests instead of checking every commit — for 1,000 commits, that's about 10 checks, not 1,000. Naming the complexity class unprompted is a strong signal.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Interactively rebasing (and force-pushing) commits a teammate has already pulled and built more work on top of — this silently rewrites the history their local branch depends on and causes the same shared-history breakage as a plain rebase, just easier to trigger since it's so convenient locally.

                    Also common: forgetting to run `git bisect reset` after finding the culprit, leaving the repository checked out at some arbitrary commit in detached-HEAD state instead of back on your actual branch.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "You have 6 messy 'wip' commits on your local, not-yet-pushed feature branch and want 2 clean commits before opening a PR. What's the right tool?",
                    "git rebase -i HEAD~6 opens an editable list of those 6 commits, letting you use squash/fixup to combine related ones and reword to write clear final messages — exactly the local-history cleanup this scenario calls for.",
                    [
                        new QuizOptionSeed("git rebase -i HEAD~6, using squash/fixup to combine related commits", true),
                        new QuizOptionSeed("git reset --hard HEAD~6", false),
                        new QuizOptionSeed("git revert HEAD~6", false),
                        new QuizOptionSeed("git cherry-pick HEAD~6", false),
                    ]),
                new QuizQuestionSeed(
                    "What does git bisect actually do?",
                    "Bisect performs a binary search between a commit known to be good and one known to be bad, checking out the midpoint for you to test at each step — it finds the exact commit that introduced a regression in roughly log2(n) steps instead of checking every commit one by one.",
                    [
                        new QuizOptionSeed("It binary-searches commit history between a known-good and known-bad commit to find the one that introduced a bug", true),
                        new QuizOptionSeed("It automatically resolves merge conflicts using a bisection algorithm", false),
                        new QuizOptionSeed("It splits one large commit into two smaller, separate commits", false),
                        new QuizOptionSeed("It merges two branches by alternating commits from each", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("git-rebase — official documentation", "https://git-scm.com/docs/git-rebase", LinkType.OfficialDocs),
                new ReferenceLinkSeed("git-bisect — official documentation", "https://git-scm.com/docs/git-bisect", LinkType.OfficialDocs),
            ],
            prerequisites: [lesson2]);

        var lesson4Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson4.Slug,
        [
            "Use git rebase -i to squash 3+ WIP commits into one clean, well-described commit on a scratch branch",
            "Cherry-pick a single commit from one branch onto another and confirm only that change came across",
            "Use git bisect (manually, or with a test script via git bisect run) to find a bug you introduced on purpose",
        ]);

        var module = BuildModule(topicId, "git-fundamentals", "Git Fundamentals",
            "Commits, branches, merging, rebasing, resolving conflicts, undoing mistakes safely, and advanced workflows like interactive rebase, cherry-pick, and bisect.",
            140, [lesson1, lesson2, lesson3, lesson4]);

        return (module, [lesson1Checklist, lesson2Checklist, lesson3Checklist, lesson4Checklist]);
    }

    private static (Module, List<ChecklistSeed>) BuildGitInternalsModule(int topicId)
    {
        var lesson1 = BuildLesson(
            slug: "git-internals-objects-refs-and-the-git-directory",
            title: "Git Internals: Objects, Refs & the .git Directory",
            summary: "What blobs, trees, and commits actually are on disk, how refs and HEAD really point at things, and what 'detached HEAD' means.",
            estimatedMinutes: 35,
            objectives:
            [
                "Explain what a blob, tree, and commit object each store, and how they reference each other by hash",
                "Explain what a ref actually is, and how HEAD normally points at a branch, which points at a commit",
                "Explain what 'detached HEAD' means and why it is not inherently dangerous, just easy to lose track of",
                "Locate and interpret the key contents of the .git directory (objects, refs, HEAD, index)",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    Git is, underneath the porcelain commands (`git commit`, `git checkout`, ...), a **content-addressable object database** — every piece of content Git tracks is stored as an object, named by the SHA-1 hash (SHA-256 on newer repos) of its own content. There are four object types:

                    - **blob** — the raw content of a single file, nothing else. No filename, no permissions, just bytes. Two files with identical content anywhere in the repo (or its entire history) hash to the same blob and are stored only once.
                    - **tree** — a directory listing: a set of entries, each with a file mode, a type (`blob` or `tree`), a name, and the hash of that entry. A tree referencing sub-trees is how Git represents nested folders.
                    - **commit** — a pointer to exactly one tree (the project's full state at that point), the hash(es) of its parent commit(s), author/committer identity and timestamps, and a message. Note there's no "diff" stored anywhere — a commit's diff is *computed* on demand by comparing its tree to its parent's tree.
                    - **tag** (annotated tags only) — a pointer to a commit plus a message and optionally a GPG signature.

                    A **ref** is nothing more than a small file containing a 40-(or 64-)character hash — `.git/refs/heads/main` contains the hash of the commit `main` currently points to. Moving a branch is just overwriting that one file with a new hash.

                    **HEAD** is normally a *symbolic* ref: `.git/HEAD` contains the text `ref: refs/heads/main`, i.e. "look at whatever `main` points to." When you check out a branch, Git updates HEAD to point at that branch's ref, and the branch ref moves as you commit. When you instead check out a raw commit hash or a tag directly, Git can't attach HEAD to a branch — so it writes the raw commit hash straight into `.git/HEAD` instead. This is a **detached HEAD**: HEAD points directly at a commit, with no branch pointing at it. Commits you make in this state are real commits, but if you switch to another branch afterward with nothing pointing at them, they become unreachable and are eventually garbage-collected.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Blobs, trees, and commits are like a filing system built entirely out of content-addressed folders: a blob is a single sheet of paper, filed under a code that's computed from its exact text — so two identical sheets anywhere in the building end up filed under the same code, only once. A tree is an index card listing which sheets (or other index cards) belong in a folder. A commit is a Post-it note stuck to one specific index card, listing who filed it, when, and which earlier Post-it note came before it.

                    A ref is a labeled sticky arrow pointing at one of those Post-it notes — moving the "main" arrow to a new note is trivial, it's just re-sticking one arrow. HEAD is normally "the arrow that follows wherever the main arrow points." Detached HEAD is what happens when you point HEAD's arrow directly at one specific Post-it note instead of at another arrow — perfectly fine to look at, but if you walk away and nothing else is pointing at that note, it can get cleared out later.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Inspecting objects**

                    - `git cat-file -t <hash>` — print an object's type (blob/tree/commit/tag)
                    - `git cat-file -p <hash>` — pretty-print an object's content
                    - `git hash-object <file>` — compute the hash a file's content would get (add `-w` to actually write it)
                    - `git rev-parse HEAD` — resolve HEAD all the way down to its concrete commit hash

                    **Refs and HEAD**

                    - `cat .git/HEAD` — see whether HEAD is symbolic (`ref: refs/heads/...`) or detached (a raw hash)
                    - `git symbolic-ref HEAD` — print which branch HEAD currently points at (errors if detached)
                    - `git update-ref refs/heads/main <hash>` — move a branch pointer directly, bypassing commit/merge

                    **Where things live under `.git/`**

                    - `.git/objects/` — the object database (loose objects, later compacted into pack files)
                    - `.git/refs/heads/`, `.git/refs/tags/` — one file per branch/tag, each holding a commit hash
                    - `.git/index` — the staging area (a binary file, not human-readable)
                    - `.git/logs/` — the reflog, a local history of everywhere HEAD and branches have pointed
                    """, 3),
                Block(BlockType.CodeSnippet, "Poking at the Object Database Directly", BodyFormat.PlainText, """
                    # Create a blob object directly, bypassing the working directory entirely
                    echo "hello world" | git hash-object -w --stdin
                    # ce013625030ba8dba906f756967f9e9ca394464a

                    # Ask Git what type of object that hash actually is
                    git cat-file -t ce013625030ba8dba906f756967f9e9ca394464a
                    # blob

                    # Print its raw content
                    git cat-file -p ce013625030ba8dba906f756967f9e9ca394464a
                    # hello world

                    # A commit's tree, in raw form (mode, type, hash, name per entry)
                    git cat-file -p HEAD^{tree}
                    # 100644 blob a1b2c3d4e5f6...  README.md
                    # 040000 tree e5f6a7b8c9d0...  src

                    # What HEAD actually contains right now
                    cat .git/HEAD
                    # ref: refs/heads/main

                    # Detach HEAD by checking out a commit hash directly instead of a branch
                    git checkout 4f2c9a1

                    cat .git/HEAD
                    # 4f2c9a1d8e7b...   <- a raw commit hash, no "ref:" prefix => detached HEAD

                    # If you commit here and want to keep the work, give it a branch NOW:
                    git switch -c rescue-this-work

                    # Otherwise, get back onto a real branch (fine if nothing new was committed):
                    git checkout main
                    """, 4, language: "bash"),
                Block(BlockType.Diagram, "The Object Graph, and What HEAD Points At", BodyFormat.AsciiArt, """
                    refs/heads/main --> commit C2 --(parent)--> commit C1
                                            |                      |
                                          tree T2                tree T1
                                        /         \\                |
                                 blob(a.txt)   blob(b.txt)     blob(a.txt v1)

                    Normal (attached) HEAD:
                      HEAD --> refs/heads/main --> C2
                      (HEAD is a symbolic ref: "follow whatever main points to")

                    Detached HEAD (after `git checkout C1` directly):
                      HEAD --> C1
                      (HEAD holds C1's raw hash; no branch ref points at C1 --
                       a new commit made here is unreachable the moment you switch away)
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    If you end up in detached HEAD on purpose (inspecting an old release, bisecting) and make a commit worth keeping, immediately run `git switch -c <some-branch-name>` before doing anything else — that attaches a real ref to the commit you just made, so it survives switching away and is safe from garbage collection.

                    Reach for `git cat-file -p` when you want to *understand* something Git did, not just accept it — seeing the literal tree/commit objects a merge or rebase produced turns "Git did something confusing" into "here is exactly what changed and why."
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked "what is a Git commit, really," the strongest answer names the object model directly: a commit object pointing at one tree (a full snapshot, not a diff) and at its parent commit(s) — and that a diff is computed on demand by comparing trees, never stored. Being able to say "blobs store content, trees store structure, commits store history" in one breath is a reliable signal you've gone past memorizing commands.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Panicking after `git checkout <commit-hash>` prints "You are in 'detached HEAD' state" — it's a normal, safe mode for looking at old code; the actual mistake is committing new work there and then switching branches without first running `git switch -c <name>` to keep it.

                    Also common: assuming a commit's diff is stored somewhere — it isn't. Every diff you see (`git show`, `git log -p`) is computed on the fly by comparing that commit's tree object to its parent's tree object.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "What does a Git blob object actually store?",
                    "A blob stores only the raw bytes of a file's content — no filename, no path, no file mode. Those live in the tree object that references the blob by hash, which is also why identical content anywhere in the repo's history is stored as a single blob.",
                    [
                        new QuizOptionSeed("Just the file's raw content, with no filename or file mode attached", true),
                        new QuizOptionSeed("The file's content plus its filename and path", false),
                        new QuizOptionSeed("A directory listing of files and sub-directories", false),
                        new QuizOptionSeed("The author, message, and parent pointer for a change", false),
                    ]),
                new QuizQuestionSeed(
                    "You run `git checkout 4f2c9a1` (a raw commit hash, not a branch name). What does `.git/HEAD` contain afterward, and what's the risk?",
                    "HEAD becomes 'detached' — .git/HEAD holds the raw commit hash directly instead of a symbolic 'ref: refs/heads/...' line. New commits made in this state have no branch pointing at them, so switching away without first creating a branch (git switch -c) can leave them unreachable and eligible for garbage collection.",
                    [
                        new QuizOptionSeed("The raw commit hash directly (detached HEAD); unreferenced new commits there can be lost if you switch away", true),
                        new QuizOptionSeed("ref: refs/heads/4f2c9a1, functioning exactly like checking out a normal branch", false),
                        new QuizOptionSeed("Nothing changes — Git automatically creates a branch for you", false),
                        new QuizOptionSeed("Git refuses the checkout unless the hash matches an existing branch or tag", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Git Internals — Git Objects", "https://git-scm.com/book/en/v2/Git-Internals-Git-Objects", LinkType.OfficialDocs),
                new ReferenceLinkSeed("Git Internals — Git References", "https://git-scm.com/book/en/v2/Git-Internals-Git-References", LinkType.OfficialDocs),
            ]);

        var lesson1Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson1.Slug,
        [
            "Use git hash-object -w and git cat-file -p to create and read back a blob by hand",
            "Run cat .git/HEAD before and after deliberately checking out a commit hash, and explain the difference",
            "Recover from a self-induced detached HEAD by creating a branch with git switch -c before switching away",
        ]);

        var lesson2 = BuildLesson(
            slug: "git-hooks-and-automating-your-workflow",
            title: "Git Hooks & Automating Your Workflow",
            summary: "Running linters and tests automatically at commit/push time with Git hooks, and why teams use Husky or the pre-commit framework instead of raw .git/hooks scripts.",
            estimatedMinutes: 30,
            objectives:
            [
                "Explain what a Git hook is, where hooks live, and why they must be made executable",
                "Write a pre-commit hook that blocks a commit when linting or tests fail",
                "Explain why scripts placed directly in .git/hooks aren't shared with teammates, and how core.hooksPath, Husky, or the pre-commit framework solve that",
                "Distinguish client-side hooks (pre-commit, commit-msg, pre-push) from server-side hooks (pre-receive, update)",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    A **Git hook** is a script Git runs automatically at a specific point in its normal command lifecycle — before a commit is finalized, before a push leaves your machine, after a commit lands, and more. Hooks live as individual executable files in `.git/hooks/`, named after the event they run on (`pre-commit`, `commit-msg`, `pre-push`, ...); a fresh repo ships that directory full of `*.sample` files showing the expected format, disabled until you remove the `.sample` suffix and make the file executable (`chmod +x`).

                    **Client-side hooks** run on a developer's own machine as part of everyday commands: `pre-commit` (runs before the commit message editor even opens; a non-zero exit aborts the commit — ideal for linting/tests), `prepare-commit-msg` and `commit-msg` (can generate or validate the commit message itself, e.g. enforcing a format), `post-commit` (runs after the commit exists; notification-only, can't stop anything), and `pre-push` (runs before anything is sent to the remote — a natural place to run a fuller test suite than pre-commit has time for).

                    **Server-side hooks** (`pre-receive`, `update`, `post-receive`) run on the *remote* when it receives pushed commits — used for centrally-enforced policy (rejecting force-pushes to protected branches, running CI, triggering deployments) that no individual developer can bypass by skipping their local hooks.

                    The critical gotcha: `.git/` is Git's own local metadata directory — it is never tracked, committed, or pushed as part of the repository's content. A hook script sitting in `.git/hooks/` on your machine exists only on your machine. To actually *share* a hook with a team, either point Git at a tracked directory with `git config core.hooksPath <dir>`, or use a wrapper tool — **Husky** (Node ecosystem) or the **pre-commit framework** (language-agnostic, Python-based) — which installs a tiny script into `.git/hooks` for you that then runs whatever tracked, version-controlled hook definitions live in the repo.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    A Git hook is like a security checkpoint at a specific doorway in a building — the `pre-commit` checkpoint stands right before the "finalize this submission" door and can turn you back before you ever get through; the `post-commit` checkpoint stands just past a door that's already closed, so it can only ring a bell, not stop you.

                    Raw `.git/hooks` scripts are like a checkpoint guard's personal notes taped to their own desk — helpful to that one guard, invisible to every other guard at every other desk (teammate's machine), because those notes never leave the building along with anything else. Husky or the pre-commit framework is like publishing the checkpoint's official rulebook company-wide and having every guard's desk automatically load a copy of it.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Common client-side hooks, in the order they can fire**

                    - `pre-commit` — before the commit message prompt; can abort (lint/tests)
                    - `prepare-commit-msg` — can programmatically edit the default message template
                    - `commit-msg` — validates/rewrites the message itself; can abort (e.g. enforce Conventional Commits)
                    - `post-commit` — after the commit exists; notification only, cannot abort
                    - `pre-push` — before anything is sent to the remote; can abort (fuller test suite)

                    **Server-side hooks (run on the remote, unavoidable by clients)**

                    - `pre-receive` — before any refs are updated; can reject the whole push
                    - `update` — once per ref being updated; can reject individual branches
                    - `post-receive` — after the push is accepted; triggers CI/CD, notifications

                    **Sharing hooks with a team**

                    - `git config core.hooksPath .githooks` — point Git at a tracked directory instead of `.git/hooks`
                    - Husky (`npx husky init`) — Node-ecosystem wrapper that installs and syncs hooks automatically
                    - pre-commit framework (`pre-commit install`) — language-agnostic, config-driven hook manager
                    """, 3),
                Block(BlockType.CodeSnippet, "A Pre-Commit Lint Hook, Shared with the Whole Team", BodyFormat.PlainText, """
                    #!/bin/sh
                    # .githooks/pre-commit -- must be executable: chmod +x .githooks/pre-commit

                    # Only lint the files that are actually staged for this commit
                    files=$(git diff --cached --name-only --diff-filter=ACM -- '*.js' '*.jsx')
                    if [ -n "$files" ]; then
                      npx eslint $files
                      if [ $? -ne 0 ]; then
                        echo "ESLint failed -- fix the errors above before committing."
                        exit 1
                      fi
                    fi

                    # ---- Making Git actually use this tracked script ----

                    # Raw .git/hooks/pre-commit is NOT tracked by git and is never pushed --
                    # core.hooksPath fixes that by pointing Git at a committed directory instead:
                    git config core.hooksPath .githooks
                    git add .githooks/pre-commit
                    git commit -m "Share the pre-commit lint hook with the team via core.hooksPath"

                    # Husky (Node projects) automates the same idea, synced through package.json:
                    npx husky init
                    echo "npx lint-staged" > .husky/pre-commit
                    git add .husky/pre-commit package.json
                    git commit -m "Add Husky pre-commit hook running lint-staged"

                    # pre-commit framework (language-agnostic) reads a tracked .pre-commit-config.yaml:
                    pip install pre-commit
                    pre-commit install   # writes the actual .git/hooks/pre-commit shim for you
                    """, 4, language: "bash"),
                Block(BlockType.Diagram, "What Runs, In Order, When You Run `git commit`", BodyFormat.StructuredSteps, """
                    [{"label":"pre-commit","note":"runs first; non-zero exit aborts before a message is even requested"},{"label":"prepare-commit-msg","note":"can pre-fill or template the default commit message"},{"label":"commit-msg","note":"validates the final message; non-zero exit aborts the commit"},{"label":"commit object is created"},{"label":"post-commit","note":"runs after the fact; notification only, cannot abort anything"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Keep `pre-commit` hooks fast (seconds, not minutes) and scoped to only the staged files — a slow pre-commit hook trains developers to reach for `git commit --no-verify` out of habit, which quietly defeats the entire point of having the hook.

                    Put the slower, fuller test suite in `pre-push` instead of `pre-commit` — it only runs once per push rather than once per commit, so it can afford to be more thorough without punishing every single small commit along the way.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked how you'd enforce code quality automatically before code reaches CI, mention hooks specifically by name (pre-commit for fast local lint, pre-push for a fuller local test pass, server-side pre-receive as the real, unbypassable gate) rather than a vague "we run linters" — and mention that raw `.git/hooks` scripts don't sync across a team on their own, which is exactly why tools like Husky or the pre-commit framework exist.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Writing a great `pre-commit` script directly in `.git/hooks/pre-commit`, committing the rest of the project, and being confused when a teammate's machine doesn't run it — `.git/` is never tracked or pushed, so that script only ever existed locally. Use `core.hooksPath`, Husky, or the pre-commit framework so the hook itself lives in tracked, version-controlled files.

                    Also common: treating client-side hooks as a security boundary — any developer can bypass `pre-commit`/`pre-push` with `--no-verify` or by simply not installing them, so anything that must be enforced (not just encouraged) belongs in a server-side hook or CI, not only a local one.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "You write a great linting script directly into .git/hooks/pre-commit and commit the rest of the repo. Why doesn't a teammate who clones the repo get this hook automatically?",
                    ".git/ is Git's own local metadata directory — it is never tracked, committed, or pushed as part of the repository's content, so a script placed directly in .git/hooks exists only on the machine it was created on. Sharing a hook requires core.hooksPath pointing at a tracked directory, or a tool like Husky or the pre-commit framework.",
                    [
                        new QuizOptionSeed("Because .git/ is never tracked or pushed, so its contents never leave the original machine", true),
                        new QuizOptionSeed("Because hooks are disabled by default and must be manually re-enabled per developer", false),
                        new QuizOptionSeed("Because Git hooks only run on the same operating system they were written on", false),
                        new QuizOptionSeed("Because pre-commit hooks require a paid Git hosting plan to sync", false),
                    ]),
                new QuizQuestionSeed(
                    "Which client-side hook runs after a commit already exists, and therefore cannot prevent that commit from happening?",
                    "post-commit fires only after the commit object has already been created — it's useful for notifications or triggering local tooling, but by the time it runs, there's nothing left to abort. pre-commit and commit-msg both run before the commit is finalized and can still fail the commit.",
                    [
                        new QuizOptionSeed("pre-commit", false),
                        new QuizOptionSeed("commit-msg", false),
                        new QuizOptionSeed("post-commit", true),
                        new QuizOptionSeed("pre-push", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Customizing Git — Git Hooks", "https://git-scm.com/book/en/v2/Customizing-Git-Git-Hooks", LinkType.OfficialDocs),
                new ReferenceLinkSeed("pre-commit — a framework for managing Git hooks", "https://pre-commit.com/", LinkType.FurtherReading),
            ],
            prerequisites: [lesson1]);

        var lesson2Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson2.Slug,
        [
            "Write and enable a pre-commit hook that blocks the commit when a lint command fails",
            "Share that hook with a team using core.hooksPath (or Husky / the pre-commit framework) instead of leaving it in .git/hooks",
            "Explain out loud why a client-side hook is not a security boundary, and what belongs server-side or in CI instead",
        ]);

        var module = BuildModule(topicId, "git-internals-and-automation", "Git Internals & Automation",
            "What Git actually stores on disk (blobs, trees, commits, refs) and how detached HEAD really works, plus automating your workflow with pre-commit/pre-push hooks and tools like Husky and the pre-commit framework.",
            65, [lesson1, lesson2], sortOrder: 2);

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

        var lesson3 = BuildLesson(
            slug: "deployment-strategies-blue-green-canary",
            title: "Deployment Strategies: Blue-Green, Canary & Feature Flags",
            summary: "Shipping change to production without a full-stop outage window, and separating 'the code is deployed' from 'the feature is live.'",
            estimatedMinutes: 35,
            objectives:
            [
                "Explain the difference between deploying code and releasing a feature to users",
                "Compare blue-green, canary, and rolling deployments and their rollback trade-offs",
                "Use a feature flag to decouple deployment from release",
                "Choose an automated rollback condition for a progressive rollout",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    **Deploying** code (getting a new build onto production infrastructure) and **releasing** a feature (making its behavior visible to users) are two different events, even though teams often collapse them into one "deploy = release" moment. Separating them is what makes safe rollouts possible.

                    Three common deployment strategies trade off differently:

                    - **Blue-green** — run two identical, full environments ("blue" and "green"); the new version deploys to the idle one, gets smoke-tested with zero live traffic, then a router/load balancer instantly cuts traffic over. Rollback is just cutting back — just as instant.
                    - **Canary** — route a small percentage of live traffic (e.g., 5%) to the new version, watch error rate/latency, then progressively increase the percentage (5% -> 25% -> 50% -> 100%) if metrics stay healthy. Limits the "blast radius" of a bad release to a subset of real users.
                    - **Rolling** — replace old instances with new ones a few at a time, in place, so there's never a moment with double the infrastructure — but both versions run simultaneously mid-rollout, and rollback means rolling forward again, not an instant switch.

                    A **feature flag** decouples the two further: code can be deployed to production "dark" (flag off, inert), then released later — instantly, without a redeploy — by flipping the flag, and just as instantly killed by flipping it back if something looks wrong.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Blue-green is like having two identical stages set up side by side — the show plays on Stage A while Stage B is fully rehearsed and lit; when it's ready, the spotlight instantly swings to Stage B, and swinging back if something goes wrong is just as fast.

                    Canary deployment is named for the literal canary in a coal mine — miners sent a canary ahead into the tunnel as an early warning system, because if something was wrong with the air, the canary showed it before any miner was exposed. A canary release exposes a small slice of real users first, as an early warning, before the whole user base is exposed to a bad change.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Strategy comparison**

                    - Blue-green — instant cutover and instant rollback; needs ~2x infrastructure during the switch; no "mixed version" period
                    - Canary — gradual traffic shift; limits blast radius; needs good real-time metrics and an automated rollback trigger
                    - Rolling — no extra infrastructure; old and new versions run side by side mid-rollout; rollback is slower (roll forward again)
                    - Feature flag — decouples "deployed" from "released"; instant kill switch with no redeploy; adds flag-cleanup debt if left in code too long

                    **Rule of thumb**: deploy dark behind a flag, then release progressively (canary-style) by ramping the flag's rollout percentage.
                    """, 3),
                Block(BlockType.CodeSnippet, "Progressive Canary Rollout with an Automated Rollback Gate", BodyFormat.PlainText, """
                    apiVersion: argoproj.io/v1alpha1
                    kind: Rollout
                    metadata:
                      name: checkout-service
                    spec:
                      strategy:
                        canary:
                          steps:
                            - setWeight: 5
                            - pause: { duration: 10m }
                            - setWeight: 25
                            - pause: { duration: 10m }
                            - setWeight: 50
                            - pause: { duration: 10m }
                            - setWeight: 100
                          # If error-rate/latency analysis fails at any paused step,
                          # traffic weight is automatically rolled back to 0 for the
                          # new version — no human has to notice and react first.
                          analysis:
                            templates:
                              - templateName: error-rate-and-latency-check
                            startingStep: 1
                    """, 4, language: "yaml"),
                Block(BlockType.Diagram, "Canary Rollout Progression", BodyFormat.StructuredSteps, """
                    [{"label":"Deploy canary","note":"5% of live traffic"},{"label":"Watch error rate & p99 latency","note":"auto-rollback if unhealthy"},{"label":"Ramp to 25%"},{"label":"Ramp to 50%"},{"label":"Ramp to 100%","note":"old version fully retired"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Deploy behind a feature flag set to "off" by default, then release progressively by ramping the flag's rollout percentage — this means a bad release can be killed instantly (flip the flag) without waiting on a redeploy or a rollback pipeline run.

                    Pick an automated, metric-based rollback condition (error rate, p99 latency) before a canary or progressive rollout starts, not while it's already in flight — deciding the threshold under incident pressure leads to hesitation exactly when speed matters most.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked to design a safe rollout for a high-traffic service, explicitly name canary with an automated rollback gate as the default answer, and mention feature flags as the way to decouple "shipped" from "live" — interviewers are often listening for whether you treat blast-radius limitation and automated (not human-watched) rollback as first-class requirements, not an afterthought.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Doing an instant full cutover with no staged traffic percentage and no automated health check — this means 100% of users hit a broken change at once, and someone has to notice and react manually before damage is contained.

                    Also common: leaving feature flags in code long after a feature is fully released and stable ("flag debt") — every permanent flag is a permanent branch that has to be reasoned about, tested, and eventually cleaned up.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "In a canary deployment, what's the main purpose of an automated rollback triggered by error-rate/latency checks?",
                    "It limits blast radius by pulling the new version back before it reaches all users, the moment the subset of real traffic reveals a regression — without waiting for a human to notice and react.",
                    [
                        new QuizOptionSeed("It automatically rolls back the new version before it's exposed to all users if metrics regress", true),
                        new QuizOptionSeed("It's purely a cost-optimization technique, unrelated to safety", false),
                        new QuizOptionSeed("It replaces the need for any pre-deploy testing", false),
                        new QuizOptionSeed("It only applies to blue-green deployments, not canary", false),
                    ]),
                new QuizQuestionSeed(
                    "What's the key difference between deploying code and releasing a feature?",
                    "Deploying puts new code on production infrastructure; releasing (often via a feature flag) is the separate act of making that code's behavior visible and active for users — the two can happen at completely different times.",
                    [
                        new QuizOptionSeed("They're always the same event — a deploy always makes a feature visible to users", false),
                        new QuizOptionSeed("Deploying puts code on production servers; releasing is separately making it visible/active to users", true),
                        new QuizOptionSeed("Releasing means restarting the production servers", false),
                        new QuizOptionSeed("There's no meaningful difference in modern deployment pipelines", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("BlueGreenDeployment (Martin Fowler's site)", "https://martinfowler.com/bliki/BlueGreenDeployment.html", LinkType.FurtherReading),
                new ReferenceLinkSeed("Kubernetes: Performing a Rolling Update", "https://kubernetes.io/docs/tutorials/kubernetes-basics/update/update-intro/", LinkType.OfficialDocs),
            ],
            prerequisites: [lesson1]);

        var lesson3Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson3.Slug,
        [
            "Explain the rollback trade-off between blue-green, canary, and rolling deployments in your own words",
            "Identify one feature in your own project that could ship dark behind a flag before being released",
            "Sketch a canary rollout plan with traffic percentages and an automated rollback condition",
        ]);

        var lesson4 = BuildLesson(
            slug: "incident-response-on-call-basics",
            title: "Incident Response & On-Call Basics",
            summary: "Severity levels, runbooks, and blameless postmortems — how to respond to a production incident, and actually learn from it afterward.",
            estimatedMinutes: 35,
            objectives:
            [
                "Classify an incident by severity and explain what response each level demands",
                "Follow a runbook during an active incident instead of improvising under pressure",
                "Write a blameless postmortem that surfaces systemic fixes, not blame",
                "Explain why a 'root cause' is usually plural, not singular",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    Production incidents get classified by **severity** so the response scales to the actual impact instead of every alert triggering the same panic (or the same shrug):

                    - **SEV1** — full outage or major data-integrity risk, all hands, immediate page.
                    - **SEV2** — significant degradation (a feature down, elevated errors for many users), urgent but not all-hands.
                    - **SEV3/SEV4** — minor, limited-impact issues, handled during business hours, no page.

                    During an active incident, a designated **incident commander** coordinates the response (who's investigating what, who's communicating status), while responders follow a **runbook** — a pre-written, step-by-step guide for a known failure mode ("API error rate spiking: check X, then Y, then Z") — rather than improvising from scratch under pressure.

                    After the incident is resolved, a **blameless postmortem** documents the timeline, contributing factors, and follow-up actions. "Blameless" doesn't mean nothing went wrong — it means the write-up focuses on *why the system allowed the failure* (missing alert, unclear runbook, a single point of failure) rather than which person made a mistake, because a single-person "root cause" is almost never the whole story.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Severity levels and a runbook work like a fire department's incident command system: a small kitchen fire gets one engine and a checklist; a multi-alarm building fire gets a designated incident commander coordinating multiple crews, all following pre-drilled procedures instead of inventing a plan on the spot. Nobody improvises hose placement mid-fire — the procedure was written and rehearsed beforehand, precisely so no one has to think it up under pressure.

                    A blameless postmortem is like an aviation crash investigation: investigators don't stop at "the pilot made an error" — they ask why the cockpit design, training, or procedures made that error easy to make, because grounding one pilot doesn't fix a system that will produce the same error again with someone else at the controls.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Severity levels, roughly**

                    - SEV1 — full outage / data integrity risk — immediate page, all hands
                    - SEV2 — significant degradation — urgent, dedicated responder(s)
                    - SEV3/SEV4 — minor / cosmetic — business hours, no page

                    **During an incident**

                    - One incident commander coordinates; responders execute the runbook
                    - Status updates go to one shared channel, on a cadence — not scattered DMs
                    - Mitigate first (stop the bleeding — e.g., rollback), root-cause later

                    **Blameless postmortem contains**

                    - Timeline of detection, escalation, mitigation, resolution
                    - Contributing factors (plural), not a single scapegoat "root cause"
                    - Concrete follow-up actions with owners and due dates
                    """, 3),
                Block(BlockType.CodeSnippet, "A Runbook Step: Fast Mitigation Before Root-Causing", BodyFormat.PlainText, """
                    #!/usr/bin/env bash
                    # Runbook: "checkout-service error rate > 5%"
                    # Step 1 — confirm impact before doing anything else.
                    curl -s https://status.internal/checkout-service/error-rate

                    # Step 2 — mitigate FIRST: roll back to the last known-good version.
                    # Don't wait to find the root cause before stopping user impact.
                    kubectl rollout undo deployment/checkout-service

                    # Step 3 — confirm the rollback actually resolved the symptom.
                    watch -n 10 'curl -s https://status.internal/checkout-service/error-rate'

                    # Root-cause investigation happens AFTER impact is stopped,
                    # feeding into the postmortem — not during the live incident.
                    """, 4, language: "bash"),
                Block(BlockType.Diagram, "Incident Lifecycle", BodyFormat.StructuredSteps, """
                    [{"label":"Detect","note":"alert fires or user report"},{"label":"Triage & Declare","note":"assign severity + incident commander"},{"label":"Mitigate","note":"stop user impact — e.g., rollback"},{"label":"Resolve","note":"confirm metrics back to normal"},{"label":"Blameless Postmortem","note":"contributing factors + follow-up actions"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Mitigate before you root-cause: rolling back or failing over to stop user impact takes priority over understanding exactly why something broke — the full investigation belongs in the postmortem, not in the middle of an active SEV1.

                    Write the postmortem within a day or two while details are fresh, list contributing factors as a list (plural), and assign every follow-up action a concrete owner and due date — a postmortem with no tracked action items is just a story, not a fix.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked to walk through how you'd handle a production incident, narrate it in order — detect, assign severity, mitigate (rollback) before investigating root cause, then resolve and write a blameless postmortem — interviewers are often checking whether you reach for "stop the bleeding first" instead of debugging live in production while users are still affected.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Naming a single person's mistake as "the root cause" in a postmortem — this discourages honest reporting next time (people hide details that might implicate them) and almost always ignores the systemic factors (no alert existed, the runbook was outdated, a single point of failure) that let one person's mistake become an outage in the first place.

                    Also common: skipping a postmortem for a "near miss" that didn't fully become an outage — near misses are the cheapest opportunity to find and fix a systemic gap before it causes a real incident.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "Why should postmortems be blameless rather than naming which individual made the mistake?",
                    "Blaming individuals discourages honest, detailed reporting of what actually happened, which makes it harder to find the systemic factors (missing alerts, outdated runbooks, single points of failure) that let one mistake turn into an outage — and those factors will cause the same failure again with someone else at the controls.",
                    [
                        new QuizOptionSeed("Blameless means nothing went wrong and no one needs to change anything", false),
                        new QuizOptionSeed("Naming an individual discourages honest reporting and hides the systemic factors that actually need fixing", true),
                        new QuizOptionSeed("It's a legal requirement in most engineering organizations", false),
                        new QuizOptionSeed("Postmortems are only useful for SEV1 incidents, so blame doesn't matter for smaller ones", false),
                    ]),
                new QuizQuestionSeed(
                    "During an active SEV1 incident with an assigned incident commander, an engineer spots what looks like a quick fix. What should they do?",
                    "Report it through the incident's coordinated channel (to the incident commander) rather than applying it unilaterally — uncoordinated changes during an active incident can make impact worse or muddy the timeline needed for the postmortem.",
                    [
                        new QuizOptionSeed("Apply the fix immediately without telling anyone, to save time", false),
                        new QuizOptionSeed("Report it to the incident commander / shared channel so the change is coordinated and tracked", true),
                        new QuizOptionSeed("Wait until the postmortem meeting to mention it", false),
                        new QuizOptionSeed("Page every engineer in the company to get consensus first", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Google SRE Book: Managing Incidents", "https://sre.google/sre-book/managing-incidents/", LinkType.FurtherReading),
                new ReferenceLinkSeed("Google SRE Book: Postmortem Culture", "https://sre.google/sre-book/postmortem-culture/", LinkType.FurtherReading),
            ],
            prerequisites: [lesson2]);

        var lesson4Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson4.Slug,
        [
            "Write a one-page runbook for a service you own: what to check first, who to page, how to roll back",
            "Draft a blameless postmortem for a real (or hypothetical) incident, focused on systemic fixes with owners",
            "Explain the difference between a SEV1 and a SEV3 and what response each demands",
        ]);

        var module = BuildModule(topicId, "devops-fundamentals", "DevOps Fundamentals",
            "Designing real CI/CD pipelines with a healthy test pyramid, safely rolling out change to production, seeing what's happening once it's live, and responding when something breaks.",
            140, [lesson1, lesson2, lesson3, lesson4]);

        return (module, [lesson1Checklist, lesson2Checklist, lesson3Checklist, lesson4Checklist]);
    }

    private static (Module, List<ChecklistSeed>) BuildDevOpsReliabilityModule(int topicId)
    {
        var lesson1 = BuildLesson(
            slug: "infrastructure-as-code-terraform-deep-dive",
            title: "Infrastructure as Code Deep Dive with Terraform",
            summary: "Managing infrastructure as versioned, reviewable code: what the state file actually is, the plan/apply workflow as a safety gate, reusable modules, and catching drift before it causes an incident.",
            estimatedMinutes: 40,
            objectives:
            [
                "Explain why infrastructure as code makes infrastructure changes reviewable, repeatable, and versioned like application code",
                "Describe what Terraform's state file is for, and why it must be stored remotely with locking in a team setting",
                "Walk through the init -> plan -> apply workflow and explain why 'plan' is a safety gate, not a formality",
                "Detect infrastructure drift and explain why a reusable module reduces it across environments",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    **Infrastructure as code (IaC)** means infrastructure (servers, networks, databases, load balancers) is defined in declarative configuration files, checked into version control, reviewed like any other code change, and applied by a tool — instead of being clicked together by hand in a cloud console, where changes are undocumented and unrepeatable.

                    **Terraform** is a widely used IaC tool built around a declarative loop: you write HCL config describing the desired end state ("one S3 bucket, one VPC with 3 subnets"), and Terraform figures out what API calls are needed to make reality match it — you never script the individual create/update/delete calls yourself.

                    To do that, Terraform needs to remember what it already created. That's the **state file** — a JSON record mapping each resource in your config to the real-world object it corresponds to (an actual AWS bucket ARN, a real VPC ID). Without state, Terraform would have no way to know "this bucket already exists, just update its tags" versus "create a new one from scratch."

                    Because state is the single source of truth about what's real, it must be shared across a team via a **remote backend** (e.g., an S3 bucket) with **locking** (e.g., a DynamoDB table) — otherwise two engineers running `apply` at the same time can corrupt the state or apply conflicting changes to the same real infrastructure.

                    A **module** is a reusable, parameterized bundle of resources (e.g., "a standard VPC" or "a standard S3 bucket with the right tags and encryption") — used the same way across dev/staging/prod so environments don't quietly diverge from each other over time.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    The state file is like a warehouse's shipping manifest: it doesn't just describe what *should* be on the shelves, it records exactly what's already there and where, so a new shipment can be reconciled against reality instead of guessing. If someone rearranges the warehouse by hand without updating the manifest, the next shipment gets calculated wrong — that mismatch between manifest and reality is exactly what infrastructure **drift** is.

                    A Terraform module is like a standardized IKEA furniture kit: instead of every store re-inventing "a bookshelf" from raw lumber with slightly different measurements each time, everyone assembles the same numbered kit — so every bookshelf in every store is consistent, and improving the design once improves it everywhere it's used.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Core concepts**

                    - Provider — plugin that talks to an API (AWS, Azure, GCP, Kubernetes, ...)
                    - Resource — one managed object (`aws_s3_bucket`, `aws_instance`, ...)
                    - Data source — read-only lookup of something Terraform doesn't manage
                    - Variable / Output — parameterize a config / expose a value to callers
                    - Module — reusable, parameterized bundle of resources

                    **CLI workflow**

                    - `terraform init` — download providers, configure the backend
                    - `terraform validate` / `terraform fmt` — syntax check / auto-format
                    - `terraform plan` — dry run: shows add/change/destroy diff, changes nothing
                    - `terraform apply` — executes exactly the reviewed plan
                    - `terraform state list` / `state show <addr>` — inspect what's tracked
                    - `terraform import` — bring an existing, unmanaged resource under management
                    - `terraform destroy` — tear down everything the config manages
                    """, 3),
                Block(BlockType.CodeSnippet, "Remote State Backend, a Module, and a Managed Resource", BodyFormat.PlainText, """
                    terraform {
                      required_version = ">= 1.5.0"

                      # Remote backend: state lives in S3, locking via DynamoDB so two
                      # engineers can never apply against the same state at once.
                      backend "s3" {
                        bucket         = "acme-tfstate"
                        key            = "prod/network/terraform.tfstate"
                        region         = "us-east-1"
                        dynamodb_table = "acme-tfstate-lock"
                        encrypt        = true
                      }
                    }

                    provider "aws" {
                      region = var.aws_region
                    }

                    # Reusable module — same VPC shape used in dev/staging/prod,
                    # parameterized instead of copy-pasted per environment.
                    module "vpc" {
                      source     = "./modules/vpc"
                      cidr_block = "10.0.0.0/16"
                      az_count   = 3
                    }

                    resource "aws_s3_bucket" "artifacts" {
                      bucket = "acme-build-artifacts-${var.environment}"

                      tags = {
                        Environment = var.environment
                        ManagedBy   = "terraform"
                      }
                    }

                    output "bucket_name" {
                      value = aws_s3_bucket.artifacts.bucket
                    }
                    """, 4, language: "hcl"),
                Block(BlockType.Diagram, "The Init -> Plan -> Apply Workflow", BodyFormat.StructuredSteps, """
                    [{"label":"terraform init","note":"downloads providers, configures remote backend"},{"label":"terraform plan","note":"dry run — shows add/change/destroy diff, changes nothing"},{"label":"Review the plan","note":"human or CI approval gate before anything real happens"},{"label":"terraform apply","note":"executes only the reviewed diff"},{"label":"State updated","note":"real resource IDs recorded for the next plan"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Always store state remotely with locking (S3+DynamoDB, Terraform Cloud, or equivalent) the moment more than one person touches an environment — local state files are a guaranteed source of corruption and conflicting applies, and they can't be safely shared or reviewed.

                    Run `terraform plan` in CI on every pull request that touches infrastructure and require a human to read the diff before `apply` runs — treating the plan output as a real code-review artifact, not a formality to click past, is what catches an accidental "destroy and recreate" before it happens to a production database.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked how you'd manage infrastructure safely across a team, mention the state file and locked remote backend by name — it's the detail that separates "I've used Terraform" from actually understanding why concurrent applies are dangerous without it. Also explicitly frame `plan` as a safety gate: it previews exactly what will change before anything real happens.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Manually editing or deleting a resource in the cloud console after it's under Terraform management — this creates drift (state says one thing, reality says another), and the next `plan`/`apply` can produce confusing or destructive results trying to reconcile the mismatch.

                    Also common: committing local state files to git (they can contain secrets like database passwords in plaintext) or skipping the locking mechanism on a "just this once" apply — the one time two people apply concurrently without a lock is the time state gets corrupted.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "What is the primary purpose of Terraform's state file?",
                    "The state file maps each resource declared in configuration to the real-world object it corresponds to, so Terraform knows what already exists and exactly what needs to change on the next plan/apply — without it, every apply would have no memory of prior runs.",
                    [
                        new QuizOptionSeed("It's just a human-readable log of past terraform commands", false),
                        new QuizOptionSeed("It maps configuration to real infrastructure IDs so Terraform knows what already exists", true),
                        new QuizOptionSeed("It stores a cached copy of the Terraform binary and providers", false),
                        new QuizOptionSeed("It replaces the need to ever run terraform plan", false),
                    ]),
                new QuizQuestionSeed(
                    "Why is terraform plan considered a critical safety gate rather than an optional step?",
                    "Plan is a dry run: it shows exactly which resources will be added, changed, or destroyed without touching real infrastructure, giving a human (or CI) the chance to catch a dangerous change — like an accidental destroy-and-recreate — before apply makes it real.",
                    [
                        new QuizOptionSeed("It shows the exact add/change/destroy diff before any real infrastructure is touched", true),
                        new QuizOptionSeed("It's only useful for generating documentation", false),
                        new QuizOptionSeed("It automatically fixes any configuration errors it finds", false),
                        new QuizOptionSeed("It permanently locks the state file so no one else can ever run apply", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Terraform: State", "https://developer.hashicorp.com/terraform/language/state", LinkType.OfficialDocs),
                new ReferenceLinkSeed("Terraform: Modules", "https://developer.hashicorp.com/terraform/language/modules", LinkType.OfficialDocs),
            ]);

        var lesson1Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson1.Slug,
        [
            "Write a minimal Terraform config for one resource and run terraform plan against it",
            "Explain why the state file needs a remote backend with locking in a team setting",
            "Describe one example of infrastructure drift and how you'd detect it before it causes an incident",
        ]);

        var lesson2 = BuildLesson(
            slug: "chaos-engineering-reliability-testing",
            title: "Chaos Engineering & Reliability Testing",
            summary: "Deliberately injecting controlled failure into a system to prove it's actually as resilient as you believe — before an outage runs the experiment for you.",
            estimatedMinutes: 35,
            objectives:
            [
                "Explain the core chaos engineering loop: hypothesize about steady-state behavior, then test it under injected failure",
                "Design a chaos experiment with a deliberately minimized, controlled blast radius",
                "Distinguish chaos engineering from simply causing an outage on purpose",
                "Identify the steady-state metrics and abort condition needed before running an experiment safely",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    **Chaos engineering** is the discipline of deliberately injecting failure into a system — killing an instance, adding network latency, exhausting memory — to verify it actually behaves the way you assume, instead of finding out for the first time during a real outage.

                    The core loop, per the widely-cited **Principles of Chaos Engineering**: first define a measurable **steady state** (e.g., "p99 checkout latency stays under 300ms, error rate stays under 0.1%"). Then form a **hypothesis** that steady state holds even under a specific real-world failure ("...even if one checkout-service instance is killed"). Then run a controlled experiment that injects exactly that failure, with a deliberately **minimized blast radius** (one instance, one availability zone, a small percentage of traffic — never "everything, everywhere" on the first attempt).

                    This is fundamentally different from an unplanned outage: it's hypothesis-driven, measured against a defined steady state, scoped to limit real user impact, and — critically — reversible, with an automated **abort condition** that halts the experiment the instant the steady state is actually violated.

                    If the hypothesis holds, confidence in that failure mode is now backed by evidence, not assumption. If it doesn't hold, a real weakness was found in a controlled setting, with responders watching and ready to intervene — instead of at 3 a.m. during a genuine incident.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Chaos engineering is like a fire drill: a building doesn't wait for a real fire to discover whether its evacuation plan actually works — it deliberately triggers the alarm under controlled conditions, with staff ready to intervene, specifically so gaps in the plan are found on a Tuesday afternoon instead of during an actual emergency.

                    It's also like a vaccine: a small, controlled, deliberately limited-scope challenge is introduced specifically to test and strengthen a system's response — not a full-blown uncontrolled infection. The scope is the whole point; nobody would call an actual illness "medicine."
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Principles of chaos engineering, roughly**

                    - Define a measurable steady state (a metric, not a vibe)
                    - Hypothesize the steady state holds under a specific real-world event
                    - Vary real-world events: instance/pod loss, added latency, dependency failure, resource exhaustion, AZ/network partition
                    - Minimize blast radius, and only expand it as confidence grows
                    - Automate experiments and their abort condition — don't rely on a human noticing fast enough

                    **Common failure injections**

                    - Kill a single instance/pod
                    - Inject artificial network latency or packet loss
                    - Fail a downstream dependency / third-party API
                    - Exhaust CPU, memory, or disk on one node
                    - Partition network traffic between two services or availability zones
                    """, 3),
                Block(BlockType.CodeSnippet, "A Chaos Toolkit Experiment: Kill One Pod, Check Steady State", BodyFormat.PlainText, """
                    {
                      "version": "1.0.0",
                      "title": "Checkout service stays healthy when one instance is killed",
                      "description": "Verifies p99 latency and error rate stay within SLO after terminating a single checkout-service pod",
                      "steady-state-hypothesis": {
                        "title": "Checkout service is healthy",
                        "probes": [
                          {
                            "type": "probe",
                            "name": "p99-latency-below-300ms",
                            "tolerance": true,
                            "provider": {
                              "type": "python",
                              "module": "chaosprometheus.probes",
                              "func": "query_bool_threshold",
                              "arguments": {
                                "query": "histogram_quantile(0.99, checkout_latency_seconds)",
                                "threshold": 0.3
                              }
                            }
                          }
                        ]
                      },
                      "method": [
                        {
                          "type": "action",
                          "name": "terminate-one-checkout-pod",
                          "provider": {
                            "type": "python",
                            "module": "chaosk8s.pod.actions",
                            "func": "terminate_pods",
                            "arguments": { "label_selector": "app=checkout-service", "rand": true, "qty": 1 }
                          }
                        }
                      ],
                      "rollbacks": [
                        {
                          "type": "action",
                          "name": "restore-replica-count",
                          "provider": {
                            "type": "python",
                            "module": "chaosk8s.actions",
                            "func": "scale_deployment",
                            "arguments": { "name": "checkout-service", "replicas": 6 }
                          }
                        }
                      ]
                    }
                    """, 4, language: "json"),
                Block(BlockType.Diagram, "Chaos Experiment Lifecycle", BodyFormat.StructuredSteps, """
                    [{"label":"Define steady state","note":"e.g., p99 latency + error rate SLO"},{"label":"Form a hypothesis","note":"'stays healthy if one instance dies'"},{"label":"Design minimal blast radius","note":"one pod, one AZ, small traffic slice"},{"label":"Run experiment with an abort switch","note":"auto-stop if steady state is violated"},{"label":"Measure vs. steady state","note":"hypothesis holds, or a real bug was found"},{"label":"Fix or expand scope","note":"widen blast radius only once confident"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Start every new experiment in staging, with the smallest blast radius that still tests the hypothesis (one instance, not the whole fleet) — only run it in production, and only widen its scope, after it's passed safely at a smaller scale with responders watching.

                    Always build in an automated abort condition tied to the same steady-state metric being tested (e.g., auto-stop the experiment if error rate crosses a threshold) — a chaos experiment without a kill switch is just an outage you started on purpose.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked how you'd validate a distributed system's resilience claims, name chaos engineering specifically and frame it around a steady-state hypothesis and blast-radius control — naming a concrete failure injection (kill a pod, add latency, fail a dependency) plus how you'd measure and abort shows you understand it as a rigorous, measured experiment rather than "randomly break things and see what happens."
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Running an experiment directly against production with no abort condition and without informing on-call — this turns a controlled experiment into an actual unplanned incident, with no safety net and a confused on-call engineer who wasn't expecting the alert.

                    Also common: treating chaos engineering as "randomly turning things off" with no steady-state hypothesis or measurement plan — without a defined baseline to compare against, there's no way to tell whether the system passed, failed, or the result was meaningless noise.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "What distinguishes chaos engineering from simply causing an outage on purpose?",
                    "Chaos engineering is a controlled experiment: it starts from a specific hypothesis about steady-state behavior, uses a deliberately minimized (and only gradually expanded) blast radius, and includes a way to measure results and automatically abort — an uncontrolled outage has none of that structure.",
                    [
                        new QuizOptionSeed("Nothing — they're the same thing, just with different names", false),
                        new QuizOptionSeed("It's a hypothesis-driven, blast-radius-controlled, measured, and abortable experiment", true),
                        new QuizOptionSeed("Chaos engineering only happens in staging and never touches production", false),
                        new QuizOptionSeed("It requires no metrics, since the goal is just to see what breaks", false),
                    ]),
                new QuizQuestionSeed(
                    "Why should a chaos experiment begin with a small, controlled blast radius instead of testing the whole production system at once?",
                    "A small blast radius limits real user impact if the hypothesis turns out to be wrong or uncovers a genuine bug, and lets the team build confidence incrementally — expanding scope only after smaller experiments have already passed safely.",
                    [
                        new QuizOptionSeed("It limits real user impact if something goes wrong, and lets confidence build incrementally before expanding scope", true),
                        new QuizOptionSeed("Small-scale experiments are the only kind current tooling supports", false),
                        new QuizOptionSeed("A large blast radius always produces more statistically valid results", false),
                        new QuizOptionSeed("It's purely a cost-saving measure with no safety implications", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("Principles of Chaos Engineering", "https://principlesofchaos.org/", LinkType.OfficialDocs),
                new ReferenceLinkSeed("Chaos Toolkit: Core Concepts", "https://chaostoolkit.org/reference/concepts/", LinkType.OfficialDocs),
            ]);

        var lesson2Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson2.Slug,
        [
            "Write a concrete steady-state hypothesis for a service you know (e.g., a latency/error-rate threshold under a specific failure)",
            "Design one minimal-blast-radius chaos experiment, including its automated abort condition",
            "Explain why chaos engineering requires a hypothesis and measurement, not just 'turning things off'",
        ]);

        var module = BuildModule(topicId, "infrastructure-as-code-and-reliability", "Infrastructure as Code & Reliability Engineering",
            "Managing infrastructure as versioned, reviewable code with Terraform's state file, module, and plan/apply workflow — then proving that resilience by deliberately injecting controlled failure instead of waiting for production to fail on its own.",
            75, [lesson1, lesson2], sortOrder: 2);

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

        var lesson3 = BuildLesson(
            slug: "domain-driven-design-basics",
            title: "Domain-Driven Design Basics: Entities, Value Objects & Bounded Contexts",
            summary: "Modeling the domain with entities, value objects, and aggregates, and drawing bounded-context boundaries between subdomains.",
            estimatedMinutes: 45,
            objectives:
            [
                "Distinguish an entity from a value object and explain why the distinction matters",
                "Explain what an aggregate root is and why it's the only valid entry point into an aggregate",
                "Identify a bounded-context boundary between two subdomains in a real system",
                "Recognize when DDD's tactical patterns are worth the complexity vs. overkill for a simple CRUD app",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    **Domain-Driven Design (DDD)** is a set of practices for modeling software so its structure mirrors the real business domain, using a shared **ubiquitous language** that developers and domain experts both use, unambiguously, in conversation and in code.

                    Two core building blocks:

                    - **Entity** — has a persistent identity that survives attribute changes over time (a `Customer` is still the same customer after changing their email address). Identity, not attributes, defines equality.
                    - **Value Object** — has no identity of its own; it's fully defined by its attributes and is typically immutable (`Money(50, "USD")` equals any other `Money(50, "USD")` — there's no "which one" to ask about).

                    An **aggregate** is a cluster of entities and value objects treated as one consistency boundary. The **aggregate root** is the single entity other code is allowed to reference from outside — it's the only door in, and it's responsible for enforcing every invariant across the whole cluster.

                    A **bounded context** is an explicit boundary (often a service, module, or team) within which a specific model and ubiquitous language apply consistently. The same word can mean different things in different bounded contexts — "Customer" in a Sales context (who they are, what they've bought) is a very different model from "Customer" in a Shipping context (where to deliver, what's in transit) — and that's fine, as long as the boundary is explicit.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    An entity is like a person's Social Security Number — they can change their name, address, even their appearance, and it's still legally the same person. A value object is like a dollar bill — any two $1 bills are completely interchangeable; nobody asks "which specific dollar bill do you mean?"

                    An aggregate root is like a shipping container's manifest — you don't reach into the container and grab an individual crate directly; you go through the manifest, which is the only thing that knows and enforces what's allowed to be in there together.

                    A bounded context is like two departments in the same company both talking about "the customer" in a meeting — Sales means "who's paying us," Shipping means "where does the box go" — and both are correct, because each department's model only has to make sense within its own walls.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Entity vs. Value Object**

                    - Entity — has identity, mutable over time, equality by ID
                    - Value Object — no identity, usually immutable, equality by attribute values

                    **Aggregate rules**

                    - One aggregate root per aggregate; it's the only externally-referenceable entity
                    - Everything inside the boundary is loaded/saved together, as one consistency unit
                    - Reference other aggregates by ID only, never by direct object reference

                    **Bounded context**

                    - Same term can have different models in different contexts — that's expected, not a bug
                    - Draw the boundary where the ubiquitous language would otherwise start conflicting
                    """, 3),
                Block(BlockType.CodeSnippet, "A Value Object and an Aggregate Root Enforcing an Invariant", BodyFormat.PlainText, """
                    // Value object: no identity, immutable, equality by value (records give this for free).
                    public record Money(decimal Amount, string Currency)
                    {
                        public static Money operator +(Money a, Money b) =>
                            a.Currency != b.Currency
                                ? throw new InvalidOperationException("Cannot add different currencies.")
                                : a with { Amount = a.Amount + b.Amount };
                    }

                    // Aggregate root: the ONLY entry point into the Order/OrderLine cluster.
                    public class Order
                    {
                        private readonly List<OrderLine> _lines = [];

                        public int Id { get; private set; }
                        public bool IsShipped { get; private set; }
                        public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();

                        public void AddLine(string sku, int quantity, Money unitPrice)
                        {
                            if (IsShipped)
                            {
                                throw new InvalidOperationException("Cannot modify a shipped order.");
                            }

                            _lines.Add(new OrderLine(sku, quantity, unitPrice));
                        }
                    }

                    public record OrderLine(string Sku, int Quantity, Money UnitPrice);
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "Aggregate Boundary vs. Bounded Context Boundary", BodyFormat.AsciiArt, """
                    Aggregate boundary (one consistency unit):

                        Order (aggregate root)
                          +-- OrderLine
                          +-- OrderLine
                          (external code only ever talks to Order, never an OrderLine directly)

                    Bounded context boundary (same word, different models):

                        [ Sales Context ]        [ Shipping Context ]
                          Customer                  Customer
                          - billingAddress          - deliveryAddress
                          - loyaltyTier             - deliveryInstructions

                    Order references a Customer by ID only across the boundary —
                    never by holding a direct reference into another context's model.
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Keep aggregates small — one aggregate root guarding a handful of tightly-related entities. A large aggregate that pulls in half the domain graph turns every save into a broad lock and a merge-conflict magnet.

                    Reference other aggregates by ID (`CustomerId`, not a `Customer` object reference) — this keeps aggregate boundaries honest and avoids accidentally loading (and locking) far more of the object graph than the operation actually needs.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked to explain DDD, don't lead with the vocabulary — lead with an example: "we had a `Customer` that meant something different to billing than it did to support, so we drew a bounded context boundary between them and stopped trying to force one shared model to do both jobs." Naming entities/value objects/aggregates only after the example lands is far more convincing than reciting definitions.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Building an **anemic domain model** — entities that are just public-getter/setter data bags with all the actual logic living in separate "service" classes. This throws away the main point of DDD: behavior and invariants should live on the objects that own the data they protect.

                    Also common: one giant aggregate that spans the whole domain "to keep everything consistent," and never drawing bounded-context boundaries at all, so the same term silently means five different things across the codebase depending on which file you're reading.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "What's the key difference between an entity and a value object?",
                    "An entity has a persistent identity that survives changes to its attributes over time; a value object has no identity at all and is fully defined by (and equal based on) its attribute values.",
                    [
                        new QuizOptionSeed("Entities are always immutable; value objects are always mutable", false),
                        new QuizOptionSeed("An entity has identity that persists through change; a value object is defined entirely by its attributes", true),
                        new QuizOptionSeed("Value objects can only hold numeric data", false),
                        new QuizOptionSeed("There is no real difference — the terms are interchangeable", false),
                    ]),
                new QuizQuestionSeed(
                    "Why should external code reference an aggregate root instead of reaching into its internal entities directly?",
                    "The aggregate root is the only place invariants across the whole cluster are enforced. Bypassing it to modify an internal entity directly can leave the aggregate in an inconsistent state that violates rules the root exists to protect.",
                    [
                        new QuizOptionSeed("It's purely a naming convention with no functional purpose", false),
                        new QuizOptionSeed("The aggregate root is the only entity that enforces invariants across the whole cluster", true),
                        new QuizOptionSeed("Internal entities are always private and can't be referenced regardless", false),
                        new QuizOptionSeed("It improves database query performance automatically", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("DDD Aggregate (Martin Fowler)", "https://martinfowler.com/bliki/DDD_Aggregate.html", LinkType.FurtherReading),
                new ReferenceLinkSeed("Design a DDD-oriented microservice (.NET architecture docs)", "https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/ddd-oriented-microservice", LinkType.OfficialDocs),
            ],
            prerequisites: [lesson1]);

        var lesson3Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson3.Slug,
        [
            "Find one entity in your own code that's actually being used as a value object (or vice versa), and describe the fix",
            "Identify the aggregate root for one cluster of related classes in your own code, and name the invariant it enforces",
            "Describe two bounded contexts in a system you know where the same term means something different in each",
        ]);

        var lesson4 = BuildLesson(
            slug: "microservices-vs-monoliths",
            title: "Microservices vs. Monoliths: Trade-offs and the Distributed Monolith Trap",
            summary: "When splitting a monolith into services actually pays off, how services should talk to each other, and how to avoid building a distributed monolith.",
            estimatedMinutes: 45,
            objectives:
            [
                "State the core trade-off between a monolith and microservices in one sentence",
                "Choose synchronous vs. asynchronous communication for a given cross-service scenario",
                "Use bounded contexts to decide where a real service boundary should go",
                "Recognize a 'distributed monolith' and explain why it has the downsides of both worlds",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    A **monolith** is a single deployable application — one codebase, one build, one deployment unit (even if it's internally well-layered). A **microservices** architecture splits an application into multiple independently deployable services, each typically owning its own data store, communicating over the network.

                    The core trade-off: microservices buy you independent deployability, independent scaling, and team autonomy (each team owns and ships its own service) — at the cost of real operational complexity: network calls that can fail or time out, distributed transactions (or the need to avoid them), service discovery, and eventual consistency between services instead of a single ACID database transaction.

                    Service boundaries should follow **bounded contexts**, not org charts or technical layers — a service should own one coherent piece of the domain model (e.g., "Shipping"), not be "the database layer" or "the reporting team's stuff." Splitting along the wrong lines is how teams end up with services that must change together anyway.

                    Communication between services is either **synchronous** (HTTP/gRPC request-response, when the caller genuinely needs an answer right now) or **asynchronous** (events/messages via a queue or broker, when the caller can proceed without waiting and just needs the side effect to happen eventually).
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    A monolith is like one big kitchen making every dish on the menu — simple to coordinate, but the whole kitchen shuts down together, and any change to the salad station risks disrupting the grill.

                    Microservices are like a food court — each stall owns its own ingredients, its own staff, and can change its menu without asking the stall next door. But now the stalls have to coordinate handoffs (an order that spans two stalls), and one stall being closed doesn't have to take down the whole food court.

                    A **distributed monolith** is a food court where every stall still has to call over to every other stall and wait for a response before it can plate a single dish — you've paid for separate kitchens, separate rent, and network overhead between them, but none of the actual independence you were trying to buy.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Lean toward microservices when**

                    - The domain has clear bounded contexts owned by different teams
                    - Different parts of the system need very different scaling profiles or release cadences
                    - You're already paying the operational cost of distributed systems for other reasons

                    **Lean toward a monolith when**

                    - The team is small, or the domain boundaries aren't well understood yet
                    - You're early-stage and speed of iteration matters more than independent deployability

                    **Communication choice**

                    - Synchronous (HTTP/gRPC) — caller needs the answer now, and can tolerate the callee being temporarily down
                    - Asynchronous (queue/event bus) — caller doesn't need to wait, and wants resilience against the callee being temporarily unavailable
                    """, 3),
                Block(BlockType.CodeSnippet, "Decoupling Services with an Asynchronous Event", BodyFormat.PlainText, """
                    // Published by the Order service after it commits to ITS OWN database —
                    // it does not call the Shipping service directly or wait for it.
                    public record OrderPlacedIntegrationEvent(int OrderId, string CustomerEmail, decimal Total);

                    public class OrderService(AppDbContext db, IEventPublisher publisher)
                    {
                        public async Task PlaceOrderAsync(Order order)
                        {
                            db.Orders.Add(order);
                            await db.SaveChangesAsync();

                            // Shipping reacts to this asynchronously; if Shipping is down
                            // right now, placing the order still succeeds.
                            await publisher.PublishAsync(
                                new OrderPlacedIntegrationEvent(order.Id, order.CustomerEmail, order.Total));
                        }
                    }
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "Microservices vs. a Distributed Monolith", BodyFormat.AsciiArt, """
                    Healthy microservices (async, own data each):

                        [ Order Service ] --event--> [ Message Bus ] --event--> [ Shipping Service ]
                              |                                                        |
                          (own DB)                                                (own DB)

                    Distributed monolith (synchronous call chain, shared data):

                        [ Order Service ] --HTTP,waits--> [ Shipping Service ] --HTTP,waits--> [ Billing Service ]
                              \\_____________________ shared database _____________________/

                    Every service must be up AND fast for any request to succeed —
                    all the network overhead of microservices, none of the independence.
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Give each service its own datastore — a shared database across services is one of the fastest ways to end up with a distributed monolith, since it silently couples every service's schema to every other service.

                    Default to asynchronous events for cross-service side effects (send a confirmation email, update a search index) and reserve synchronous calls for the cases where the caller genuinely cannot proceed without an immediate answer.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    When asked "would you use microservices for this system?", resist the urge to answer with blanket enthusiasm. A strong answer is conditional: "it depends on team size, whether the domain boundaries are clear yet, and whether we actually need independent deploy cadences — for a small team early on, I'd start with a well-layered monolith and split later, along bounded contexts, once a real need for independence shows up."
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Splitting services along technical layers ("the API service," "the database service") instead of bounded contexts — this guarantees the "services" must be deployed together anyway, since a single business change touches all of them at once.

                    Also common: building a **distributed monolith** — services that are deployed separately but share one database, or that chain synchronous calls (A waits on B waits on C) for every request. This has all of microservices' network latency and operational overhead with none of the independent-deployability benefit, and one slow or down service cascades failures through the whole chain.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "What's the defining problem with a 'distributed monolith'?",
                    "It's deployed as separate services but remains tightly coupled — often via a shared database or synchronous call chains that must all succeed together — so it carries microservices' network and operational overhead without gaining any real independent deployability.",
                    [
                        new QuizOptionSeed("It has too few services to be considered real microservices", false),
                        new QuizOptionSeed("It's deployed as separate services but stays tightly coupled, gaining overhead without independence", true),
                        new QuizOptionSeed("It uses a message queue instead of HTTP", false),
                        new QuizOptionSeed("It only happens when using SQLite instead of a distributed database", false),
                    ]),
                new QuizQuestionSeed(
                    "When is asynchronous (event/queue-based) communication a better fit than a synchronous HTTP call between two services?",
                    "When the caller doesn't need an immediate response and would rather stay available even if the other service is temporarily down — trading immediate consistency for resilience and decoupling, e.g. triggering a confirmation email after an order is placed.",
                    [
                        new QuizOptionSeed("Whenever the two services are owned by the same team", false),
                        new QuizOptionSeed("When the caller can proceed without waiting and wants resilience against the callee being unavailable", true),
                        new QuizOptionSeed("Only when the payload is larger than a few kilobytes", false),
                        new QuizOptionSeed("Asynchronous communication is always strictly better than synchronous", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("MonolithFirst (Martin Fowler)", "https://martinfowler.com/bliki/MonolithFirst.html", LinkType.FurtherReading),
                new ReferenceLinkSeed("Microservices architecture (.NET architecture docs)", "https://learn.microsoft.com/en-us/dotnet/architecture/microservices/architect-microservice-container-applications/", LinkType.OfficialDocs),
            ],
            prerequisites: [lesson3]);

        var lesson4Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson4.Slug,
        [
            "Describe one bounded context in a system you know that would make a reasonable service boundary, and why",
            "Find (or imagine) one synchronous call chain in a system you know and identify whether it should be async instead",
            "Explain the difference between microservices and a distributed monolith using your own words and an example",
        ]);

        var module = BuildModule(topicId, "software-architecture-fundamentals", "Software Architecture Fundamentals",
            "Clean Architecture's dependency rule, the design patterns that earn their complexity, DDD's tactical modeling tools, and when to split (or not split) a monolith.",
            210, [lesson1, lesson2, lesson3, lesson4]);

        return (module, [lesson1Checklist, lesson2Checklist, lesson3Checklist, lesson4Checklist]);
    }

    private static (Module, List<ChecklistSeed>) BuildEventDrivenArchitectureModule(int topicId)
    {
        var lesson1 = BuildLesson(
            slug: "event-driven-architecture-and-cqrs",
            title: "Event-Driven Architecture & CQRS",
            summary: "Decoupling services with events, replaying history with event sourcing, and separating read models from write models with CQRS — and knowing when that complexity actually pays off.",
            estimatedMinutes: 45,
            objectives:
            [
                "Explain the difference between event-driven messaging, event sourcing, and CQRS — related but independent ideas",
                "Describe how an event store lets you rebuild current state by replaying history",
                "Explain why CQRS separates read and write models and what problem that solves",
                "Recognize when the added complexity of CQRS/event sourcing pays off vs. when a simple CRUD model is enough",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    **Event-driven architecture (EDA)** is a style where services communicate by publishing and reacting to **events** — immutable facts about something that already happened ("OrderPlaced", "PaymentCaptured") — rather than calling each other directly. Producers don't know or care who (if anyone) is listening; an **event bus/broker** (Kafka, RabbitMQ, Azure Service Bus, ...) decouples them.

                    **Event sourcing** is a persistence style, not just a messaging style: instead of storing only the current state of an entity ("Order: Shipped"), you store the full sequence of events that produced it ("OrderCreated -> OrderPaid -> OrderShipped") as the system of record. Current state is a *derived* value, computed by replaying events from the beginning (or from the last snapshot).

                    **CQRS (Command Query Responsibility Segregation)** separates the model used to write data (**commands** — validate business rules, produce state changes/events) from the model used to read data (**queries** — a shape optimized purely for display, often denormalized and only eventually consistent with the write side).

                    These three ideas are related but independent: you can have an event bus with no event sourcing (just notifying other services), event sourcing with no CQRS (store events, but query the same replayed aggregate), or CQRS with no event sourcing (separate read/write models, but the write side is still a normal mutable table). They're often combined because each solves a bordering problem, but none requires the other two.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Event sourcing is like a bank ledger: your bank doesn't overwrite a single "balance" field for your account — it records every deposit and withdrawal as an immutable line item, and your current balance is *calculated* by summing the ledger. If a dispute arises, the bank can replay the entire transaction history to explain how you got there, not just show you "the number."

                    CQRS is like a restaurant's front of house and kitchen: waitstaff (the command side) take orders, enforce rules ("kitchen's closed for that dish"), and send tickets back — a very different job, with different tools, than the menu board or table-status display (the query side) that customers and hosts read from, which is just an optimized view for browsing, not for placing an order.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **When the complexity pays off**

                    - Event-driven messaging — services need to react to things happening elsewhere without tight, synchronous coupling ("when an order ships, notify billing, the warehouse, and analytics" without OrderService knowing any of those three exist)
                    - Event sourcing — a full audit trail/history is a first-class requirement (financial ledgers, compliance), or you need to reconstruct past state, or you want multiple projections derived from the same history
                    - CQRS — read and write workloads have genuinely different shapes or scaling needs (complex validation on write, high-volume denormalized reads for a dashboard)

                    **When it's not worth it**

                    - A standard CRUD resource with no audit requirement and no read/write scaling mismatch — one model with a repository is simpler and correct
                    - "We might need it later" — introduce these patterns when the specific pain (coupling, audit gaps, read/write contention) actually shows up
                    """, 3),
                Block(BlockType.CodeSnippet, "Event-Sourced Aggregate Behind a CQRS Command/Query Split", BodyFormat.PlainText, """
                    // --- Event sourcing: events are the source of truth, not a mutable row ---
                    public abstract record OrderEvent(int OrderId, DateTime OccurredUtc);
                    public record OrderCreated(int OrderId, DateTime OccurredUtc, string Sku, int Quantity) : OrderEvent(OrderId, OccurredUtc);
                    public record OrderShipped(int OrderId, DateTime OccurredUtc, string TrackingNumber) : OrderEvent(OrderId, OccurredUtc);

                    public class Order
                    {
                        public int Id { get; private set; }
                        public bool IsShipped { get; private set; }
                        private readonly List<OrderEvent> _uncommittedEvents = [];
                        public IReadOnlyList<OrderEvent> UncommittedEvents => _uncommittedEvents.AsReadOnly();

                        public static Order Create(int id, string sku, int quantity)
                        {
                            var order = new Order();
                            order.Apply(new OrderCreated(id, DateTime.UtcNow, sku, quantity));
                            return order;
                        }

                        public void Ship(string trackingNumber)
                        {
                            if (IsShipped) throw new InvalidOperationException("Already shipped.");
                            Apply(new OrderShipped(Id, DateTime.UtcNow, trackingNumber));
                        }

                        // Rebuilds current state by replaying history — this IS what "event sourcing" means.
                        public static Order Rehydrate(IEnumerable<OrderEvent> history)
                        {
                            var order = new Order();
                            foreach (var e in history) order.Apply(e, isNew: false);
                            return order;
                        }

                        private void Apply(OrderEvent e, bool isNew = true)
                        {
                            switch (e)
                            {
                                case OrderCreated c: Id = c.OrderId; break;
                                case OrderShipped: IsShipped = true; break;
                            }
                            if (isNew) _uncommittedEvents.Add(e);
                        }
                    }

                    // --- CQRS write side: validates rules, appends events ---
                    public class ShipOrderCommandHandler(IEventStore eventStore)
                    {
                        public async Task HandleAsync(int orderId, string trackingNumber)
                        {
                            var history = await eventStore.LoadAsync(orderId);
                            var order = Order.Rehydrate(history);
                            order.Ship(trackingNumber);                          // domain rule enforced here
                            await eventStore.AppendAsync(orderId, order.UncommittedEvents);
                        }
                    }

                    // --- CQRS read side: a separate, denormalized model built purely for display ---
                    public record OrderSummaryDto(int OrderId, string Status, string? TrackingNumber);

                    public class OrderSummaryQueryService(OrderReadDbContext readDb)
                    {
                        // Reads a projection table kept up to date by an event handler —
                        // never the Order aggregate or its event stream directly.
                        public async Task<OrderSummaryDto?> GetAsync(int orderId)
                        {
                            var row = await readDb.OrderSummaries.FindAsync(orderId);
                            return row is null ? null : new OrderSummaryDto(row.OrderId, row.Status, row.TrackingNumber);
                        }
                    }
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "CQRS: Different Models for Writes and Reads", BodyFormat.AsciiArt, """
                    Write side (commands)                        Read side (queries)

                    Client --command--> Command Handler          Client --query--> Read Model
                                            |                                          ^
                                            v                                          |
                                       Order Aggregate                        Denormalized
                                       (validates rules)                      Projection Table
                                            |                                          ^
                                            v                                          |
                                        Event Store  -------- events applied ---------+
                                     (OrderCreated, OrderShipped, ...)

                    The write side and read side use DIFFERENT models. The read side is
                    updated asynchronously as events are applied — it can lag slightly
                    behind the write side (eventual consistency).
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Start with a single shared read/write model (plain CRUD) and only split into CQRS when a specific pain shows up — the read side needs a shape wildly different from the write side, or read and write load need to scale independently. Introducing the split preemptively adds two models, a projection pipeline, and eventual-consistency bugs for a problem you don't have yet.

                    If you do event-source an aggregate, snapshot periodically once event streams get long (store "state as of event #500" alongside the events) so rehydration doesn't mean replaying an ever-growing history from event #1 on every load. Version your event schemas from day one (`OrderCreatedV2`) — old events on disk can never be edited, only superseded, so plan for that up front.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked "would you use CQRS/event sourcing for this system," the strong answer names the specific pain that justifies it ("the audit/compliance requirement means we need full history, not just current state" or "the dashboard's read pattern is nothing like the transactional write pattern") rather than reaching for it as a default. Interviewers listen for judgment about when NOT to use a pattern as much as when to use it.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Treating "eventually consistent" as a free property instead of a real UX and correctness concern — if a user ships an order and immediately queries the read model before the projection catches up, they may see stale state. Either design the UI to tolerate that lag, or read from the write side immediately after a write when strict consistency is required.

                    Also common: conflating the three concepts covered here — saying "we do event-driven architecture" to imply event sourcing and CQRS are also in place, when a team might just have a message bus notifying other services with no event store and no separate read model at all. Being precise about which of the three is actually in use avoids confusing an interviewer or a teammate.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "What problem does CQRS's read/write model separation primarily solve?",
                    "CQRS is worth it when read and write workloads have genuinely different shapes or scaling needs, so each side can use a model optimized for its own job. It commonly introduces eventual consistency between the two sides rather than guaranteeing strong consistency.",
                    [
                        new QuizOptionSeed("Read and write workloads that have genuinely different shapes or scaling needs can each use a model optimized for that job", true),
                        new QuizOptionSeed("It makes relational databases unnecessary", false),
                        new QuizOptionSeed("It removes the need for validation on writes", false),
                        new QuizOptionSeed("It guarantees strong consistency between reads and writes", false),
                    ]),
                new QuizQuestionSeed(
                    "In event sourcing, what is the actual 'source of truth' for an entity's state?",
                    "The full ordered sequence of events is the source of truth. Current state (or a snapshot) is a derived value, reconstructed by replaying those events — it is never itself the record of what happened.",
                    [
                        new QuizOptionSeed("A single row holding the latest state", false),
                        new QuizOptionSeed("The full ordered sequence of events; current state is derived by replaying them", true),
                        new QuizOptionSeed("A cached DTO returned by the last query", false),
                        new QuizOptionSeed("Whatever the read model currently shows", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("CQRS (Martin Fowler)", "https://martinfowler.com/bliki/CQRS.html", LinkType.FurtherReading),
                new ReferenceLinkSeed("Event Sourcing pattern (Azure Architecture Center)", "https://learn.microsoft.com/en-us/azure/architecture/patterns/event-sourcing", LinkType.OfficialDocs),
            ]);

        var lesson1Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson1.Slug,
        [
            "Identify one place in a system you know where an event bus decouples two services, and name what would break if they called each other directly instead",
            "Describe a scenario in your domain where full event-sourced history (not just current state) would be a real requirement, not a nice-to-have",
            "Explain, in your own words, why CQRS's read model is allowed to be eventually consistent, and what UX decision that forces",
        ]);

        var lesson2 = BuildLesson(
            slug: "api-design-rest-graphql-grpc",
            title: "API Design: REST, GraphQL & gRPC",
            summary: "Designing resource-oriented REST APIs, recognizing when GraphQL solves genuine over/under-fetching problems, and when gRPC's performance characteristics justify leaving JSON-over-HTTP behind.",
            estimatedMinutes: 45,
            objectives:
            [
                "Apply REST resource-design conventions (nouns not verbs, correct HTTP methods/status codes) to a real endpoint",
                "Explain the over-fetching/under-fetching problem GraphQL solves, and the N+1 problem it can introduce",
                "Explain what makes gRPC fast (HTTP/2 multiplexing, Protobuf binary serialization, streaming) and when that speed matters",
                "Choose the right API style for a given scenario based on client diversity, performance needs, and contract stability",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    **REST** models an API as a set of **resources** identified by URLs (nouns: `/orders/42`, not verbs: `/getOrder?id=42`), manipulated with standard HTTP methods (`GET` read, `POST` create, `PUT`/`PATCH` update, `DELETE` remove) and standard status codes (`200`, `201`, `404`, `409`, ...). The Richardson Maturity Model ranges from Level 0 ("HTTP as a tunnel for RPC") up to Level 3 (full hypermedia/HATEOAS) — most real APIs stop at Level 2 (resources + verbs + status codes) without full hypermedia, and that's a legitimate, pragmatic stopping point.

                    **GraphQL** exposes a single endpoint and a typed schema; the *client* specifies exactly which fields it wants in a query, and a set of **resolvers** on the server fetch each field. This directly solves REST's **over-fetching** (getting back fields you don't need) and **under-fetching** (needing several round trips to assemble one screen) problems — at the cost of shifting complexity server-side (resolver performance, the N+1 query problem, no free HTTP caching by URL).

                    **gRPC** is a contract-first RPC framework: you define services and messages in a `.proto` file, and code generation produces strongly-typed client/server stubs in many languages. It runs over **HTTP/2** (multiplexed streams over one connection, unlike HTTP/1.1's per-request overhead) and serializes with **Protocol Buffers** (a compact binary format, smaller and faster to parse than JSON), and natively supports streaming (client, server, or bidirectional). The trade-off: it isn't natively browser-friendly (needs gRPC-Web/a proxy) and payloads aren't human-readable on the wire.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    REST is like ordering from a diner's printed menu: a fixed, well-known set of dishes (resources), each with a name (URL) and a small number of standard ways to interact with it (verbs) — simple and predictable, but every dish comes as a whole plate whether you wanted all of it or not.

                    GraphQL is like a buffet where you build your own plate: you walk down the line and take exactly the items you want in a single pass, instead of ordering three separate fixed plates from three counters to get the toppings you actually wanted. But the kitchen (the server) now has to handle far more distinct plate combinations behind the scenes.

                    gRPC is like a dedicated courier contract between two companies that have already agreed on exact packaging, routes, and paperwork (the `.proto` contract, code-generated on both ends) — extremely fast and efficient between the two parties who signed the contract, but not something a random public customer can just walk up and use without the same paperwork.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Choose REST when**
                    - Public/third-party API, broad unknown client base, wide tooling/caching support matters (HTTP caching, browsers, curl-ability)

                    **Choose GraphQL when**
                    - Many different clients (web, mobile, various screens) need different slices of the same underlying data, and over/under-fetching is a real, measured problem — not just a theoretical one

                    **Choose gRPC when**
                    - Service-to-service calls inside your own infrastructure, where both ends can regenerate stubs from the same `.proto`, and low latency/high throughput or streaming genuinely matters

                    **Red flags regardless of style**
                    - Verbs in REST URLs (`/getUser`, `/createOrder`) — RPC-style thinking wearing a REST costume
                    - A GraphQL resolver making one database query per item in a list (N+1) instead of batching
                    - Choosing gRPC for a public browser-facing API without accounting for the extra proxy layer needed
                    """, 3),
                Block(BlockType.CodeSnippet, "REST Endpoint, Contrasted with GraphQL and gRPC Contracts", BodyFormat.PlainText, """
                    // REST: resource-oriented Minimal API endpoint (nouns + HTTP verbs + status codes)
                    app.MapGet("/orders/{id:int}", async (int id, IOrderRepository repo) =>
                        await repo.FindAsync(id) is { } order
                            ? Results.Ok(new OrderDto(order.Id, order.Status))
                            : Results.NotFound());

                    app.MapPost("/orders", async (CreateOrderRequest req, IOrderRepository repo) =>
                    {
                        var order = await repo.CreateAsync(req.Sku, req.Quantity);
                        return Results.Created($"/orders/{order.Id}", new OrderDto(order.Id, order.Status));
                    });

                    // GraphQL (for contrast): the client specifies exactly the fields it needs,
                    // fetching an order AND its customer's name in one round trip:
                    //
                    //   query {
                    //     order(id: 42) {
                    //       status
                    //       customer { name }
                    //     }
                    //   }

                    // gRPC (for contrast): contract-first .proto — client and server both
                    // generate strongly-typed stubs from this single definition:
                    //
                    //   service OrderService {
                    //     rpc GetOrder (GetOrderRequest) returns (Order);
                    //     rpc StreamOrderUpdates (GetOrderRequest) returns (stream OrderStatusUpdate);
                    //   }
                    //
                    //   message Order {
                    //     int32 id = 1;
                    //     string status = 2;
                    //   }
                    """, 4, language: "csharp"),
                Block(BlockType.Diagram, "Over-Fetching and Under-Fetching: REST vs. GraphQL", BodyFormat.AsciiArt, """
                    REST (fixed shape per endpoint):

                      GET /orders/42     -> { id, status, total, items[], customer_id, ... }
                                             (over-fetching: fields the screen never uses)
                      GET /customers/7   -> a second round trip just to get the customer's name
                                             (under-fetching: needed data wasn't in the first call)

                    GraphQL (client-specified shape, one round trip):

                      query { order(id: 42) { status customer { name } } }
                           -> { "order": { "status": "Shipped", "customer": { "name": "Grace" } } }

                      Exactly the fields asked for, from both "resources," in a single request.
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    For REST: version your API from day one (`/v1/orders`) even before you think you need it — the day you need a breaking change without a version prefix is a bad day. Support pagination and rate limiting on any collection endpoint before it's a production incident, not after.

                    For GraphQL: use a batching/caching layer (the DataLoader pattern) in resolvers that fetch related data, or a single list-of-100-orders query will silently issue 100+ individual database round trips (the N+1 problem) instead of one batched query.

                    For gRPC: only reach for it when both ends of the call are under your control (or willing to consume a `.proto` contract) — internal service-to-service calls are the sweet spot; a public API consumed by arbitrary browser clients is not, without an extra gRPC-Web proxy layer.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If asked "REST or GraphQL or gRPC for this system," anchor the answer in the actual client and performance constraints in the question — number of distinct client types, whether over/under-fetching is a measured problem, whether the calls are public or internal, whether raw throughput/latency is a stated requirement — rather than picking a favorite. Naming the specific trade-off you're accepting ("gRPC's speed, at the cost of needing a proxy for browser clients") reads as far more senior than a one-word answer.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Building an "RPC in REST's clothing" API — URLs full of verbs (`/api/getUserOrders`, `/api/createOrderForUser`) and everything as `POST`, ignoring HTTP methods and status codes entirely. It looks like REST at a glance but gets none of REST's actual benefits (cacheability, predictable semantics, tooling that understands HTTP verbs).

                    Also common: adopting GraphQL to "fix" over-fetching without ever addressing resolver-level N+1 queries, trading one performance problem (too many fields) for a worse one (too many database round trips) — and adopting gRPC for a public API only to discover, late, that browsers can't call it directly without an added proxy layer.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "What specific problem does GraphQL solve that plain REST endpoints often don't?",
                    "GraphQL lets the client specify exactly which fields it needs, across related resources, in a single request — directly addressing REST's tendency toward over-fetching (unused fields) and under-fetching (extra round trips).",
                    [
                        new QuizOptionSeed("It lets the client specify exactly which fields it needs across related resources in a single request, avoiding over-fetching and under-fetching", true),
                        new QuizOptionSeed("It replaces the need for a database", false),
                        new QuizOptionSeed("It's always faster than REST regardless of use case", false),
                        new QuizOptionSeed("It removes the need for authentication", false),
                    ]),
                new QuizQuestionSeed(
                    "What primarily makes gRPC faster than typical JSON-over-HTTP/1.1 REST calls?",
                    "gRPC runs over HTTP/2, which multiplexes many requests over a single connection, and serializes messages with Protocol Buffers, a compact binary format that's smaller and faster to parse than JSON — together, not either alone, driving the speed advantage.",
                    [
                        new QuizOptionSeed("HTTP/2 multiplexing plus compact binary Protobuf serialization", true),
                        new QuizOptionSeed("gRPC uses a different, faster version of JSON", false),
                        new QuizOptionSeed("gRPC bypasses TCP entirely", false),
                        new QuizOptionSeed("gRPC never validates request data", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("RESTful web API design (Azure Architecture Center)", "https://learn.microsoft.com/en-us/azure/architecture/best-practices/api-design", LinkType.OfficialDocs),
                new ReferenceLinkSeed("Introduction to gRPC (official docs)", "https://grpc.io/docs/what-is-grpc/introduction/", LinkType.FurtherReading),
            ]);

        var lesson2Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson2.Slug,
        [
            "Rewrite one verb-in-the-URL endpoint from your own code (or a public API you've used) as proper REST nouns + HTTP verbs",
            "Identify one screen in an app you use that likely over-fetches or under-fetches data with its current API, and describe how GraphQL would change that one request",
            "Explain, in your own words, why gRPC is a strong fit for internal service-to-service calls but a poor default for a public browser-facing API",
        ]);

        var module = BuildModule(topicId, "event-driven-architecture-and-api-design", "Event-Driven Architecture & API Design",
            "Decoupling services with events and CQRS, and choosing the right API style — REST, GraphQL, or gRPC — for a given client and performance profile.",
            90, [lesson1, lesson2], sortOrder: 2);

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

        var lesson3 = BuildLesson(
            slug: "handling-conflict-difficult-conversations",
            title: "Handling Conflict & Difficult Conversations at Work",
            summary: "Disagreeing constructively, giving upward feedback to a manager, and resolving team friction before it festers.",
            estimatedMinutes: 30,
            objectives:
            [
                "Disagree with a decision or approach without making it personal",
                "Give upward feedback to a manager or senior teammate respectfully",
                "Open a hard conversation with an observation instead of an accusation",
                "Recognize when a conflict needs a direct 1:1 conversation vs. escalation to a manager or HR",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    Conflict on an engineering team is normal, not a failure state — technical disagreements, competing priorities, and personality friction all surface naturally whenever people care about the work. What separates a healthy team from a dysfunctional one isn't the *absence* of conflict, it's whether conflict gets addressed early, directly, and respectfully, or left to compound silently.

                    The single most useful habit is **separating the person from the problem**: "this API design will be hard to version" is a critique of a decision; "you never think about backward compatibility" is an attack on a person. The first invites a conversation; the second invites defensiveness.

                    Most workplace conflict falls into one of three buckets, each needing a different response:

                    - **A genuine misunderstanding** — usually resolved by just asking a clarifying question.
                    - **A disagreement over approach** — resolved by stating your reasoning and evidence, hearing theirs, and deciding (sometimes via "disagree and commit").
                    - **A pattern of behavior** — the hardest kind, usually needing a direct, private conversation rather than being raised in front of the team.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Unaddressed friction is like a pressure-relief valve on a boiler: a small hiss of steam released early keeps the system stable, but capping it off "to keep the peace" doesn't make the pressure disappear — it just guarantees a much bigger, much louder problem later, at a worse time.

                    Giving upward feedback to a manager is like adjusting the GPS route for a driver, not grabbing the wheel — you're offering better information ("there's traffic ahead on this approach"), not taking control of the decision, and a good driver welcomes that input.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Before raising a conflict, check:**

                    - Have I separated the behavior/decision from the person?
                    - Am I raising this privately, not in front of the team?
                    - Do I have a specific example, not just a general feeling?
                    - What outcome am I actually asking for?

                    **Openers that de-escalate instead of accuse**

                    - "I noticed X — can you help me understand the reasoning?" (not "why would you do X")
                    - "I have a different read on this, want to walk through it?" (not "that's wrong")
                    - "I want to raise something that's been on my mind — is now an okay time?" (not ambushing mid-standup)
                    """, 3),
                Block(BlockType.CodeSnippet, "Opening an Upward-Feedback Conversation", BodyFormat.PlainText, """
                    // Weak opener: vague, accusatory, easy to get defensive about.
                    "You keep changing requirements on us and it's really frustrating."

                    // Stronger opener: specific, observation-first, names an impact,
                    // and ends by inviting their perspective instead of a verdict.
                    "I wanted to raise something — on the last two sprints, requirements
                    changed after we'd started building, and it cost us rework time both
                    times. Can we figure out together how to lock scope earlier, or is
                    there context I'm missing about why it changed?"
                    """, 4),
                Block(BlockType.Diagram, "Resolving Friction Before It Escalates", BodyFormat.StructuredSteps, """
                    [{"label":"Notice friction","note":"a comment, a decision, a pattern"},{"label":"Pause and reflect privately","note":"is this a one-off or a pattern?"},{"label":"Request a private conversation","note":"never ambush in a group setting"},{"label":"State the observation + impact","note":"specific, not accusatory"},{"label":"Listen to their perspective fully","note":"before responding"},{"label":"Agree on a concrete path forward"},{"label":"Follow up later","note":"confirm it actually changed"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Raise friction while it's still small and specific, in a private 1:1, rather than letting it accumulate until you're venting about a general pattern in a group setting — the earlier and narrower the conversation, the less defensive the other person needs to be.

                    When giving upward feedback to a manager, frame it as information they'd want to have, tied to a concrete example and (if you have one) a suggested next step — "here's what I noticed, here's the impact, here's an idea" lands far better than an open-ended complaint.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    "Tell me about a conflict with a coworker" is one of the most common behavioral questions, and interviewers are specifically listening for whether you take any ownership of your side of it — a strong answer names what you did to resolve it, not just what the other person did wrong, and ends with the relationship intact or improved, not burned.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Raising a grievance in a group meeting or group chat instead of privately — even a completely valid point lands as a public callout, which triggers defensiveness and often escalates the exact conflict you were trying to resolve.

                    Also common: letting a small friction sit unaddressed for weeks "to avoid making it awkward," until it resurfaces as a much bigger blowup over something that would have been a two-minute conversation if raised early.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "What's the key difference between 'this API design will be hard to version' and 'you never think about backward compatibility'?",
                    "The first critiques a decision or artifact, inviting a conversation about the design itself. The second attacks the person's character or pattern of behavior, which triggers defensiveness instead of problem-solving — the core idea of separating the person from the problem.",
                    [
                        new QuizOptionSeed("There's no meaningful difference — both convey the same concern", false),
                        new QuizOptionSeed("The first critiques the decision; the second attacks the person, which invites defensiveness", true),
                        new QuizOptionSeed("The second is more specific and therefore more useful feedback", false),
                        new QuizOptionSeed("The first is too vague to be actionable", false),
                    ]),
                new QuizQuestionSeed(
                    "Where should you raise a pattern of behavior that's been bothering you about a teammate's decisions?",
                    "Patterns of behavior are the hardest kind of conflict and should be raised privately, one-on-one — raising them in front of the team turns a resolvable conversation into a public callout, which tends to escalate rather than resolve the underlying issue.",
                    [
                        new QuizOptionSeed("In the next team standup, so everyone is aware", false),
                        new QuizOptionSeed("Privately, in a direct 1:1 conversation", true),
                        new QuizOptionSeed("In a group Slack channel, so there's a written record", false),
                        new QuizOptionSeed("You should let it go entirely rather than risk conflict", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("HBR: How to Handle Difficult Conversations at Work", "https://hbr.org/2015/01/how-to-handle-difficult-conversations-at-work", LinkType.FurtherReading),
                new ReferenceLinkSeed("Crucial Conversations: Tools for Talking When Stakes Are High (overview)", "https://cruciallearning.com/crucial-conversations/", LinkType.FurtherReading),
            ],
            prerequisites: [lesson2]);

        var lesson3Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson3.Slug,
        [
            "Draft an observation-plus-impact opener for a real piece of friction you've been sitting on",
            "Identify one upward-feedback conversation you've been avoiding, and plan how you'd open it",
            "Practice separating a decision/behavior from the person in a critique you'd normally phrase personally",
        ]);

        var lesson4 = BuildLesson(
            slug: "negotiating-job-offers",
            title: "Negotiating Job Offers",
            summary: "Understanding total-comp components, negotiating respectfully with leverage, and avoiding the mistakes that quietly cost candidates money.",
            estimatedMinutes: 35,
            objectives:
            [
                "Break a FAANG-style offer down into its actual components, not just base salary",
                "Negotiate using genuine leverage and rationale, without ultimatums or bluffing",
                "Sequence a negotiation correctly relative to competing offers and deadlines",
                "Recognize the negotiation mistakes that cost candidates real compensation",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    A job offer is rarely just a base salary number — it's a package with several independently negotiable pieces:

                    - **Base salary** — fixed annual cash, usually the least flexible piece at large companies with structured pay bands.
                    - **Annual bonus target** — a percentage of base, often somewhat negotiable, sometimes not.
                    - **Equity/RSUs** — a grant vesting over time (commonly 4 years, sometimes with a 1-year cliff or a back-loaded schedule) — often the single largest and most negotiable lever at big tech companies.
                    - **Sign-on bonus** — usually cash, often front-loaded specifically to offset unvested equity you're walking away from at your current employer.

                    Negotiate the **whole package**, not just base — a company that's rigid on base salary bands often has real flexibility on equity or sign-on bonus instead. Your real leverage is a competing offer, a clear articulation of your value, or both — vague requests ("can you do better?") get vague, weaker responses than specific, well-reasoned ones.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Negotiating an offer is like negotiating a home purchase, not a garage sale: you research comparable data (comparable offers, market bands) before naming a number, you know your walk-away point (your BATNA — best alternative to a negotiated agreement) before the conversation starts, and you make a reasoned ask backed by evidence, not an emotional appeal or a bluff you can't back up.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Total comp components to negotiate**

                    - Base salary, bonus target %, equity grant size, vesting schedule, sign-on bonus, start-date-tied refreshers

                    **Before you negotiate**

                    - Know your walk-away point (BATNA) before the call, not during it
                    - Gather real data points: other offers, levels.fyi bands, recruiter-shared ranges
                    - Decide your top 1-2 asks — asking for everything at once dilutes all of them

                    **Respectful negotiation phrases**

                    - "I'm genuinely excited about this offer — I do have [other data point], is there flexibility on [specific component]?"
                    - "Could we look at the equity/sign-on to help bridge that gap?"
                    """, 3),
                Block(BlockType.CodeSnippet, "A Respectful Negotiation Email", BodyFormat.PlainText, """
                    // Weak: vague, no rationale, sounds like a bluff.
                    "I need more money or I can't accept this offer."

                    // Stronger: enthusiastic, specific, backed by a real data
                    // point, and asks rather than demands.
                    "Thank you again for the offer — I'm genuinely excited about
                    the team and the role. I do have a competing offer with a
                    higher total comp (details attached), and this role is my
                    first choice. Is there flexibility on the equity grant or
                    sign-on bonus to help close that gap? Happy to hop on a
                    call to discuss."
                    """, 4),
                Block(BlockType.Diagram, "The Offer Negotiation Flow", BodyFormat.StructuredSteps, """
                    [{"label":"Receive verbal/written offer"},{"label":"Express enthusiasm, ask for time","note":"a few business days is standard"},{"label":"Gather leverage","note":"competing offers, market data"},{"label":"Decide your top 1-2 asks"},{"label":"Make the ask, with rationale","note":"to the recruiter, not the hiring manager"},{"label":"Get the revised offer in writing"},{"label":"Confirm and respond by the deadline"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Negotiate through the recruiter, not the hiring manager — it's the recruiter's job to close the deal, and routing hard numbers talk through them keeps your future day-to-day relationship with the hiring manager free of any negotiation friction.

                    Always get a revised offer in writing before making a final decision or resigning from a current role — a verbal "we can probably do that" is not a commitment until it's on the written offer letter.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    If a recruiter asks for your current salary or expectations early in the process, it's reasonable to redirect rather than answer directly: "I'd rather focus on finding the right fit first — once there's mutual interest, I'm happy to discuss compensation ranges." Anchoring too early, before the company has decided it wants you, usually works against the candidate.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Accepting or rejecting an offer on the spot, on the same call it's presented — always ask for a few business days, even when you're fairly sure of your answer; a company that pressures you not to take time to think is itself a signal worth noting.

                    Also common: negotiating only on base salary while ignoring equity and sign-on bonus, which at many tech companies are the components with the most real flexibility — and bluffing about a competing offer that doesn't exist, which can (and does) get discovered and burns trust permanently.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "At many large tech companies, which offer component is often the LEAST flexible to negotiate?",
                    "Base salary is usually tied to structured pay bands and level, making it the most rigid piece. Equity and sign-on bonus tend to have more real negotiating room, which is why negotiating the whole package matters more than fixating on base alone.",
                    [
                        new QuizOptionSeed("Base salary", true),
                        new QuizOptionSeed("Equity/RSU grant size", false),
                        new QuizOptionSeed("Sign-on bonus", false),
                        new QuizOptionSeed("Start date", false),
                    ]),
                new QuizQuestionSeed(
                    "Who should you typically negotiate compensation with, and why?",
                    "The recruiter's job is to close the deal, so routing negotiation through them is standard practice — it also keeps your future working relationship with the hiring manager free of any negotiation-related friction.",
                    [
                        new QuizOptionSeed("The hiring manager, since they have final say", false),
                        new QuizOptionSeed("The recruiter, since closing the deal is their role", true),
                        new QuizOptionSeed("Whoever you have the most rapport with on the team", false),
                        new QuizOptionSeed("HR only, and never mention it to the recruiter", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("levels.fyi: Salary Negotiation Guide", "https://www.levels.fyi/blog/negotiation-guide.html", LinkType.FurtherReading),
                new ReferenceLinkSeed("HBR: How to Negotiate Your Next Salary", "https://hbr.org/2021/01/how-to-negotiate-your-next-salary", LinkType.FurtherReading),
            ],
            prerequisites: [lesson3]);

        var lesson4Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson4.Slug,
        [
            "Write out your total-comp breakdown for a past or hypothetical offer, component by component",
            "Draft a respectful negotiation email using a real or plausible data point",
            "Identify your walk-away point (BATNA) before your next real negotiation conversation",
        ]);

        var module = BuildModule(topicId, "interview-readiness-fundamentals", "Interview & Communication Readiness",
            "Structuring behavioral answers, understanding the interview loop, communicating like a senior engineer, handling workplace conflict, and negotiating job offers.",
            160, [lesson1, lesson2, lesson3, lesson4]);

        return (module, [lesson1Checklist, lesson2Checklist, lesson3Checklist, lesson4Checklist]);
    }

    private static (Module, List<ChecklistSeed>) BuildLeadershipAndCareerGrowthModule(int topicId)
    {
        var lesson1 = BuildLesson(
            slug: "mentoring-and-leading-without-authority",
            title: "Mentoring & Technical Leadership Without Authority",
            summary: "Leading through influence instead of authority, mentoring engineers without creating dependency, and giving code review feedback that builds skill.",
            estimatedMinutes: 30,
            objectives:
            [
                "Distinguish leading through influence from leading through positional authority, and identify the levers you actually have as an individual contributor",
                "Mentor a less experienced engineer in a way that builds their independent judgment instead of creating dependency on you",
                "Give code review feedback that is specific, calibrated to risk, and paired with a concrete suggestion",
                "Recognize when to step in directly on a struggling teammate's problem versus when to let them struggle productively",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    Technical leadership on most engineering teams has almost nothing to do with a title — the majority of "leading" a senior engineer does happens without any formal authority over the people involved. You can't order a teammate to adopt your design, and you usually can't performance-manage a peer. What you actually have is **influence**: technical credibility, the quality of your reasoning, relationships built over time, and the example you set through your own work.

                    **Mentoring** is a specific, high-leverage form of this influence. The goal of mentoring someone isn't to make them dependent on your answers — it's to build their own judgment so they need you less over time. A mentor who always just fixes the bug themselves feels helpful in the moment but is quietly making the mentee worse at debugging.

                    **Code review** is where most engineers exercise technical leadership day to day, and it's easy to get wrong in two opposite directions: rubber-stamping everything (no real leadership happening) or nitpicking everything as if every comment carries equal weight (feedback fatigue, and the reviewer starts to look like a gatekeeper rather than a collaborator). The fix is **calibration** — distinguishing a must-fix blocking issue from a stylistic preference from a genuine question, and saying explicitly which one each comment is.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Mentoring well is like scaffolding on a building under construction: the scaffolding supports work that can't yet stand on its own, but a good builder deliberately removes sections as each part becomes self-supporting — leaving it up forever doesn't protect the building, it just means it never learns to bear its own weight. Leading through influence, meanwhile, is more like being a rudder than an engine: a rudder contributes no propulsion of its own, but it can steer a huge amount of momentum other people are already generating, provided the crew trusts the person holding it.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **Levers of influence you have without formal authority**

                    - Technical credibility earned through past decisions being right
                    - The quality and clarity of your reasoning, written down where others can evaluate it
                    - Relationships and trust built before you need to spend them
                    - Setting the example yourself, publicly, before asking others to follow it

                    **Calibrating a code review comment — say which one it is**

                    - **Blocking** — must be fixed before merge (a real bug, a security issue, a broken contract)
                    - **Suggestion** — worth considering, author's call whether to take it
                    - **Nit** — pure style/preference, explicitly non-blocking (prefix with "nit:")
                    - **Question** — you genuinely don't understand something, not a disguised criticism
                    """, 3),
                Block(BlockType.CodeSnippet, "Code Review Feedback: Weak vs. Calibrated", BodyFormat.PlainText, """
                    // Vague, unprioritized — the author can't tell what's a blocker
                    // and what's a preference, so everything reads as equally urgent.
                    "This isn't how I'd do it. Also this variable name is bad. Also
                    what about errors here?"

                    // Calibrated — each comment says what kind of comment it is and
                    // why, so the author can triage in seconds.
                    "blocking: this doesn't handle the case where `items` is empty —
                    line 42 will throw. suggestion: consider extracting this into a
                    helper, it's reused in two other places. nit: `tmp` -> `pendingItems`
                    would read more clearly. question: is retry-on-failure intentional
                    here, or should this bubble up the error?"
                    """, 4),
                Block(BlockType.Diagram, "The Mentoring Loop", BodyFormat.StructuredSteps, """
                    [{"label":"Let them attempt it first","note":"resist doing it for them"},{"label":"Ask a guiding question","note":"instead of giving the answer"},{"label":"Let them struggle a bit","note":"productive struggle builds judgment"},{"label":"Step in only if truly stuck","note":"or the cost of struggle exceeds the learning"},{"label":"Debrief afterward","note":"what worked, what you'd try next time"},{"label":"Widen their scope next time","note":"gradually remove the scaffolding"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Before jumping in to solve a struggling teammate's problem for them, ask one guiding question first ("what have you tried so far?" or "what does the error actually say?") — most of the time it's enough to unstick them while leaving the learning, and the actual solution, theirs.

                    In code review, say out loud (in the comment itself) which category a piece of feedback is — "nit:", "blocking:", "question:" — so the author isn't left guessing how much weight to give each comment.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    "Tell me about a time you led without formal authority" or "tell me about mentoring someone" are both extremely common behavioral prompts — a strong answer names a specific person and situation, describes what you did to build their capability (not just what you personally fixed), and ideally closes with evidence that they could handle something similar independently afterward.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Rewriting a mentee's pull request yourself instead of commenting on it — it's faster in the moment, but it teaches them that struggling gets rescued rather than resolved, and it quietly erodes their ownership of the code.

                    Also common: burying a genuinely blocking issue in a wall of nitpicks with no signal of severity, so the author either misses the real problem or dismisses the whole review as excessive.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "A junior engineer is stuck debugging a failing test. What's the risk of just fixing it yourself instead of walking them through it?",
                    "It creates dependency — the junior engineer doesn't build the debugging judgment needed to solve similar problems on their own next time, even though the immediate bug gets fixed either way.",
                    [
                        new QuizOptionSeed("It takes slightly longer than fixing it yourself", false),
                        new QuizOptionSeed("It creates dependency and the junior doesn't build the underlying debugging judgment", true),
                        new QuizOptionSeed("There is no real risk, since the bug gets fixed either way", false),
                        new QuizOptionSeed("It will make you look less capable to your manager", false),
                    ]),
                new QuizQuestionSeed(
                    "Why explicitly label a code review comment as 'nit:', 'blocking:', or 'question:'?",
                    "It tells the author how much weight to give each comment, so a real blocking issue doesn't get lost among stylistic nitpicks, and a genuine question isn't mistaken for a hidden criticism.",
                    [
                        new QuizOptionSeed("It makes the reviewer look more thorough", false),
                        new QuizOptionSeed("It lets the author triage feedback correctly instead of treating every comment as equally urgent", true),
                        new QuizOptionSeed("It is required by most version control systems", false),
                        new QuizOptionSeed("It removes the need to explain the reasoning behind a comment", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("HBR: The Necessary Art of Persuasion", "https://hbr.org/1998/05/the-necessary-art-of-persuasion", LinkType.FurtherReading),
                new ReferenceLinkSeed("Google Engineering Practices: How to Do a Code Review", "https://google.github.io/eng-practices/review/reviewer/", LinkType.FurtherReading),
            ]);

        var lesson1Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson1.Slug,
        [
            "Identify one task you currently do yourself that you could instead teach a teammate to own, and plan the handoff",
            "Rewrite one of your own recent code review comments to explicitly label it (blocking/suggestion/nit/question) and pair it with a concrete suggestion",
            "Next time a teammate asks you how to fix something, practice asking one guiding question before giving the answer",
        ]);

        var lesson2 = BuildLesson(
            slug: "promotion-packets-and-self-advocacy",
            title: "Career Growth: Promotion Packets & Self-Advocacy",
            summary: "Documenting impact continuously, building a promotion case around scope and influence rather than activity, and advocating for yourself without over-selling.",
            estimatedMinutes: 30,
            objectives:
            [
                "Explain why continuous impact documentation beats a pre-cycle scramble",
                "Structure a promotion case around scope, ambiguity, and influence rather than raw activity or hours",
                "Self-advocate for a promotion or raise using calibrated, evidence-based language instead of over-selling or under-selling",
                "Recognize the leveling signals that typically separate adjacent levels at most tech companies",
            ],
            blocks:
            [
                Block(BlockType.Notes, null, BodyFormat.MiniMarkdown, """
                    A promotion is very rarely decided in the room where you present your case — by the time a manager or promotion committee formally reviews it, the outcome mostly depends on evidence collected *throughout* the year, not assembled the week before the cycle. The single highest-leverage habit is keeping a running **impact log** (sometimes called a "brag document"): a few sentences, added continuously, every time you ship something, unblock someone, or make a decision that mattered.

                    At most tech companies, the difference between adjacent levels isn't "worked more hours" or "closed more tickets" — it's a shift along three axes:

                    - **Scope** — the size and boundary of what you own (a function vs. a service vs. a whole system)
                    - **Ambiguity** — how well-defined the problem was when it landed on your desk
                    - **Influence** — how far your impact reached beyond your own immediate task (your team, other teams, the org)

                    A strong promotion packet is a narrative built from real entries mapped onto these three axes, not a list of everything you did.
                    """, 1),
                Block(BlockType.RealWorldAnalogy, null, BodyFormat.MiniMarkdown, """
                    Building a promotion case is like building a legal case, not writing a memoir: a lawyer collects evidence continuously as events happen, because reconstructing it all from memory months later is unreliable and misses things — while a memoir just recalls whatever feels most vivid in hindsight, which is exactly the recency bias that makes a pre-cycle scramble unreliable.
                    """, 2),
                Block(BlockType.CheatSheet, null, BodyFormat.MiniMarkdown, """
                    **A good impact log entry (write it the week it happens)**

                    - What the situation/problem was, briefly
                    - What you specifically did (not "we")
                    - The measurable or observable outcome
                    - Which axis it demonstrates: scope, ambiguity, or influence

                    **Leveling signals that typically separate adjacent levels**

                    - Scope: owns a component -> owns a service -> owns a system/roadmap area
                    - Ambiguity: given a spec -> given a rough goal -> defines the goal itself
                    - Influence: affects own PRs -> affects the team -> affects other teams/org
                    """, 3),
                Block(BlockType.CodeSnippet, "Impact Log Entry: Weak vs. Strong", BodyFormat.PlainText, """
                    // Weak: describes activity, not impact — hard to map to any
                    // leveling signal, and easy to forget the specifics by promo time.
                    "Worked on the checkout service this quarter. Fixed several bugs
                    and helped onboard a new hire."

                    // Strong: specific, attributes the action to you, states the
                    // outcome, and implicitly demonstrates scope + influence.
                    "Redesigned the checkout retry logic after diagnosing a
                    production incident that was costing ~2% of failed orders;
                    partnered with the payments team on the fix; the change reduced
                    failed-order rate by 90% and was later adopted by a second
                    service with a similar pattern."
                    """, 4),
                Block(BlockType.Diagram, "Building a Promotion Case", BodyFormat.StructuredSteps, """
                    [{"label":"Log impact continuously","note":"weekly, not the week before the cycle"},{"label":"Map each entry to scope, ambiguity, influence"},{"label":"Get early calibration from your manager","note":"months before the formal cycle"},{"label":"Identify and proactively close gaps","note":"e.g. no cross-team influence entries"},{"label":"Draft the packet as a narrative","note":"not a bullet dump of everything"},{"label":"Self-advocate in the conversation","note":"calibrated, evidence-based, not inflated"}]
                    """, 5),
                Block(BlockType.BestPractice, null, BodyFormat.MiniMarkdown, """
                    Add to your impact log the same week something happens, in two or three sentences — waiting until the cycle guarantees you'll forget your best early-year work and over-weight whatever is most recent.

                    Ask your manager for informal calibration well before the formal cycle opens ("does this feel like next-level scope to you?") — a surprise "not yet" during the actual review is almost always a sign this conversation should have happened months earlier.
                    """, 6),
                Block(BlockType.InterviewTip, null, BodyFormat.MiniMarkdown, """
                    The habit of documenting specific, outcome-focused impact throughout the year pays off twice: it builds your internal promotion packet, and the same entries are usually ready-made material for STAR-style behavioral interview stories when you eventually interview elsewhere — a well-kept impact log is a stockpile of concrete stories, not just a promotion artifact.
                    """, 7),
                Block(BlockType.CommonMistake, null, BodyFormat.MiniMarkdown, """
                    Waiting until the week before the cycle to reconstruct your impact from memory — recency bias means your best work from ten months ago quietly disappears, and what's left overweights whatever shipped most recently regardless of actual significance.

                    Equally common in both directions: over-selling with inflated claims that don't survive a follow-up question ("I redesigned the whole architecture" when you changed one component), and under-selling out of modesty by describing real, scope-expanding work in passive, activity-only language ("helped out with the migration") instead of naming your specific contribution and its outcome.
                    """, 8),
            ],
            quiz:
            [
                new QuizQuestionSeed(
                    "Why keep a running impact log throughout the year instead of reconstructing your accomplishments right before a promotion cycle?",
                    "Reconstructing from memory suffers from recency bias — early-year work gets forgotten and recent work gets overweighted regardless of its actual significance. A continuous log also lets you notice and close scope gaps while there's still time.",
                    [
                        new QuizOptionSeed("It's required paperwork at most companies", false),
                        new QuizOptionSeed("It avoids recency bias and lets you notice scope gaps early enough to still close them", true),
                        new QuizOptionSeed("It replaces the need for a manager conversation entirely", false),
                        new QuizOptionSeed("It only matters for engineers who are bad at self-promotion", false),
                    ]),
                new QuizQuestionSeed(
                    "Which factors typically distinguish one engineering level from the next, more than raw hours worked or tickets closed?",
                    "Scope (the size and boundary of what you own) and ambiguity (how well-defined the problem was when it reached you), along with influence beyond your own immediate task, are the signals leveling committees actually look for — not activity volume.",
                    [
                        new QuizOptionSeed("Years of tenure and number of programming languages known", false),
                        new QuizOptionSeed("Scope of ownership and how much ambiguity you resolved", true),
                        new QuizOptionSeed("Number of meetings attended and lines of code written", false),
                        new QuizOptionSeed("How many hours you work per week", false),
                    ]),
            ],
            referenceLinks:
            [
                new ReferenceLinkSeed("levels.fyi: Compare Engineering Levels Across Companies", "https://www.levels.fyi/", LinkType.FurtherReading),
                new ReferenceLinkSeed("HBR: Managing Oneself", "https://hbr.org/2005/01/managing-oneself", LinkType.FurtherReading),
            ],
            prerequisites: [lesson1]);

        var lesson2Checklist = new ChecklistSeed(ChecklistOwnerKind.Lesson, lesson2.Slug,
        [
            "Start (or backfill) an impact log with 3 real entries from the last few months, each mapped to scope, ambiguity, or influence",
            "Draft one paragraph of a promotion-case narrative using a real or plausible impact log entry",
            "Identify one gap in your current scope (e.g., no cross-team influence entries) and plan a concrete way to close it this quarter",
        ]);

        var module = BuildModule(topicId, "leadership-and-career-growth", "Leadership & Career Growth",
            "Leading through influence instead of authority, mentoring engineers effectively, and building an evidence-based case for promotion and self-advocacy.",
            75, [lesson1, lesson2], sortOrder: 2);

        return (module, [lesson1Checklist, lesson2Checklist]);
    }

    // ============================== Shared builders ==============================

    private static Module BuildModule(
        int topicId, string slug, string title, string description, int estimatedMinutes,
        List<Lesson> lessons, CapstoneProject? capstone = null, int sortOrder = 1)
    {
        var now = DateTime.UtcNow;
        return new Module
        {
            TopicId = topicId,
            Slug = slug,
            Title = title,
            Description = description,
            SortOrder = sortOrder,
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
