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
