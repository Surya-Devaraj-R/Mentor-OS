using MentorOS.Models;
using MentorOS.Models.Enums;

namespace MentorOS.Data.Seed;

// A real, interview-representative exercise bank with genuine solutions —
// self-assessment only, no execution (per the confirmed two-phase plan: a
// real code-execution service is a separate, deferred initiative with its
// own security design pass).
public static class ExerciseSeedData
{
    public static List<Exercise> BuildExercises(IReadOnlyDictionary<string, int> lessonIdBySlug)
    {
        var tags = new Dictionary<string, Tag>();

        return
        [
            BuildValidAnagram(lessonIdBySlug["two-pointer-hash-map-patterns"], tags),
            BuildContainsDuplicate(lessonIdBySlug["two-pointer-hash-map-patterns"], tags),
            BuildTwoSumSorted(lessonIdBySlug["two-pointer-hash-map-patterns"], tags),
            BuildBinarySearch(lessonIdBySlug["binary-search-patterns"], tags),
            BuildSecondHighestSalary(lessonIdBySlug["aggregations-subqueries-window-functions"], tags),
            BuildDuplicateEmails(lessonIdBySlug["select-join-query-fundamentals"], tags),
            BuildPalindromeCheck(lessonIdBySlug["variables-types-control-flow"], tags),
            BuildShapeAreaCalculator(lessonIdBySlug["oop-interfaces-solid-basics"], tags),
            BuildRateLimiterDesign(lessonIdBySlug["scaling-load-balancing-caching"], tags),
            BuildCacheAsideImplementation(lessonIdBySlug["scaling-load-balancing-caching"], tags),
            BuildResolveMergeConflict(lessonIdBySlug["collaborative-git-prs-rebasing-conflicts"], tags),
            BuildCiPipelineYaml(lessonIdBySlug["cicd-pipelines-automated-testing"], tags),
            BuildFizzBuzz(lessonIdBySlug["csharp-basics-loops"], tags),
            BuildSumOfAList(lessonIdBySlug["csharp-basics-lists"], tags),
            BuildCountVowels(lessonIdBySlug["csharp-basics-string-manipulation"], tags),
            BuildRectangleAreaClass(lessonIdBySlug["csharp-basics-classes"], tags),
            BuildFilterEvenNumbersLinq(lessonIdBySlug["csharp-basics-linq"], tags),
        ];
    }

    private static ExerciseTag Tagged(Dictionary<string, Tag> tags, string name)
    {
        if (!tags.TryGetValue(name, out var tag))
        {
            tag = new Tag { Name = name };
            tags[name] = tag;
        }
        return new ExerciseTag { Tag = tag };
    }

    private static Exercise BuildValidAnagram(int lessonId, Dictionary<string, Tag> tags)
    {
        var now = DateTime.UtcNow;
        return new Exercise
        {
            LessonId = lessonId,
            Slug = "valid-anagram",
            Title = "Valid Anagram",
            Prompt = "Given two strings `s` and `t`, return `true` if `t` is an anagram of `s` (uses the exact same characters, with the same counts, in any order), and `false` otherwise.",
            DifficultyLevel = DifficultyLevel.Easy,
            ExerciseType = ExerciseType.Coding,
            StarterCode = """
                public bool IsAnagram(string s, string t)
                {
                    // Your code here
                }
                """,
            Language = "csharp",
            IsInterviewChallenge = true,
            FollowUpQuestions = "What would change if the strings could contain Unicode characters, not just lowercase a-z?",
            SortOrder = 1,
            CreatedUtc = now,
            UpdatedUtc = now,
            ExerciseTags = [Tagged(tags, "hashing"), Tagged(tags, "strings")],
            Hints =
            [
                new ExerciseHint { Text = "If the lengths differ, they can't possibly be anagrams — check that first.", SortOrder = 1 },
                new ExerciseHint { Text = "Counting character frequencies avoids the O(n log n) cost of sorting both strings.", SortOrder = 2 },
            ],
            Solutions =
            [
                new ExerciseSolution
                {
                    ApproachTitle = "Sorting",
                    Explanation = "Sort both strings and compare — if they're anagrams, their sorted character sequences are identical.",
                    SolutionCode = """
                        public bool IsAnagram(string s, string t)
                        {
                            if (s.Length != t.Length) return false;

                            var sChars = s.ToCharArray();
                            var tChars = t.ToCharArray();
                            Array.Sort(sChars);
                            Array.Sort(tChars);

                            return sChars.AsSpan().SequenceEqual(tChars);
                        }
                        """,
                    Language = "csharp",
                    TimeComplexity = "O(n log n)",
                    SpaceComplexity = "O(n)",
                    SortOrder = 1,
                },
                new ExerciseSolution
                {
                    ApproachTitle = "Character Counting (Optimal)",
                    Explanation = "Count character frequencies in one pass over `s`, then decrement those same counts while scanning `t`. If every count returns to zero, the strings are anagrams.",
                    SolutionCode = """
                        public bool IsAnagram(string s, string t)
                        {
                            if (s.Length != t.Length) return false;

                            Span<int> counts = stackalloc int[26];
                            foreach (var c in s) counts[c - 'a']++;
                            foreach (var c in t) counts[c - 'a']--;

                            foreach (var count in counts)
                            {
                                if (count != 0) return false;
                            }
                            return true;
                        }
                        """,
                    Language = "csharp",
                    TimeComplexity = "O(n)",
                    SpaceComplexity = "O(1) — fixed 26-slot array",
                    SortOrder = 2,
                },
            ],
        };
    }

    private static Exercise BuildContainsDuplicate(int lessonId, Dictionary<string, Tag> tags)
    {
        var now = DateTime.UtcNow;
        return new Exercise
        {
            LessonId = lessonId,
            Slug = "contains-duplicate",
            Title = "Contains Duplicate",
            Prompt = "Given an integer array `nums`, return `true` if any value appears at least twice, and `false` if every element is distinct.",
            DifficultyLevel = DifficultyLevel.Easy,
            ExerciseType = ExerciseType.Coding,
            StarterCode = """
                public bool ContainsDuplicate(int[] nums)
                {
                    // Your code here
                }
                """,
            Language = "csharp",
            IsInterviewChallenge = true,
            FollowUpQuestions = "Could you solve this with O(1) extra space if you were allowed to sort the input array in place?",
            SortOrder = 2,
            CreatedUtc = now,
            UpdatedUtc = now,
            ExerciseTags = [Tagged(tags, "hashing"), Tagged(tags, "arrays")],
            Hints =
            [
                new ExerciseHint { Text = "A hash set's Add method tells you whether the value was already present.", SortOrder = 1 },
            ],
            Solutions =
            [
                new ExerciseSolution
                {
                    ApproachTitle = "Hash Set (Optimal)",
                    Explanation = "Add each value to a hash set; if `Add` reports the value was already present, a duplicate exists.",
                    SolutionCode = """
                        public bool ContainsDuplicate(int[] nums)
                        {
                            var seen = new HashSet<int>();
                            foreach (var num in nums)
                            {
                                if (!seen.Add(num)) return true;
                            }
                            return false;
                        }
                        """,
                    Language = "csharp",
                    TimeComplexity = "O(n)",
                    SpaceComplexity = "O(n)",
                    SortOrder = 1,
                },
            ],
        };
    }

    private static Exercise BuildTwoSumSorted(int lessonId, Dictionary<string, Tag> tags)
    {
        var now = DateTime.UtcNow;
        return new Exercise
        {
            LessonId = lessonId,
            Slug = "two-sum-sorted-array",
            Title = "Two Sum on a Sorted Array",
            Prompt = "Given a SORTED integer array `nums` and a `target`, return the 1-indexed positions of the two numbers that add up to `target`, using O(1) extra space.",
            DifficultyLevel = DifficultyLevel.Easy,
            ExerciseType = ExerciseType.Coding,
            StarterCode = """
                public int[] TwoSumSorted(int[] nums, int target)
                {
                    // Your code here
                }
                """,
            Language = "csharp",
            IsInterviewChallenge = true,
            FollowUpQuestions = "How would the approach change if the array were NOT sorted, but you still needed O(1) extra space?",
            SortOrder = 3,
            CreatedUtc = now,
            UpdatedUtc = now,
            ExerciseTags = [Tagged(tags, "two-pointers"), Tagged(tags, "arrays")],
            Hints =
            [
                new ExerciseHint { Text = "The array is sorted — that's a strong hint two pointers apply here, not a hash map.", SortOrder = 1 },
                new ExerciseHint { Text = "If the current pair's sum is too big, which pointer should move — left or right?", SortOrder = 2 },
            ],
            Solutions =
            [
                new ExerciseSolution
                {
                    ApproachTitle = "Two Pointers (Optimal)",
                    Explanation = "Start pointers at both ends. If the sum is too big, move the right pointer left (decreasing the sum); if too small, move the left pointer right (increasing it) — sorted order guarantees this always converges correctly.",
                    SolutionCode = """
                        public int[] TwoSumSorted(int[] nums, int target)
                        {
                            var left = 0;
                            var right = nums.Length - 1;

                            while (left < right)
                            {
                                var sum = nums[left] + nums[right];
                                if (sum == target) return [left + 1, right + 1];
                                if (sum < target) left++;
                                else right--;
                            }

                            throw new ArgumentException("No two numbers sum to the target.");
                        }
                        """,
                    Language = "csharp",
                    TimeComplexity = "O(n)",
                    SpaceComplexity = "O(1)",
                    SortOrder = 1,
                },
            ],
        };
    }

    private static Exercise BuildBinarySearch(int lessonId, Dictionary<string, Tag> tags)
    {
        var now = DateTime.UtcNow;
        return new Exercise
        {
            LessonId = lessonId,
            Slug = "classic-binary-search",
            Title = "Classic Binary Search",
            Prompt = "Given a sorted array of distinct integers `nums` and a `target`, return the index of `target`, or -1 if it isn't present. Must run in O(log n).",
            DifficultyLevel = DifficultyLevel.Easy,
            ExerciseType = ExerciseType.Coding,
            StarterCode = """
                public int Search(int[] nums, int target)
                {
                    // Your code here
                }
                """,
            Language = "csharp",
            IsInterviewChallenge = true,
            FollowUpQuestions = "How would you find the FIRST occurrence of a target if the array had duplicates?",
            SortOrder = 4,
            CreatedUtc = now,
            UpdatedUtc = now,
            ExerciseTags = [Tagged(tags, "binary-search"), Tagged(tags, "arrays")],
            Hints =
            [
                new ExerciseHint { Text = "Compute mid as left + (right - left) / 2 to avoid overflow.", SortOrder = 1 },
            ],
            Solutions =
            [
                new ExerciseSolution
                {
                    ApproachTitle = "Standard Binary Search",
                    Explanation = "Narrow [left, right] by comparing the midpoint to the target each time, discarding the half that can't contain it.",
                    SolutionCode = """
                        public int Search(int[] nums, int target)
                        {
                            var left = 0;
                            var right = nums.Length - 1;

                            while (left <= right)
                            {
                                var mid = left + (right - left) / 2;
                                if (nums[mid] == target) return mid;
                                if (nums[mid] < target) left = mid + 1;
                                else right = mid - 1;
                            }

                            return -1;
                        }
                        """,
                    Language = "csharp",
                    TimeComplexity = "O(log n)",
                    SpaceComplexity = "O(1)",
                    SortOrder = 1,
                },
            ],
        };
    }

    private static Exercise BuildSecondHighestSalary(int lessonId, Dictionary<string, Tag> tags)
    {
        var now = DateTime.UtcNow;
        return new Exercise
        {
            LessonId = lessonId,
            Slug = "second-highest-salary",
            Title = "Second Highest Salary",
            Prompt = "Given an `employees` table with columns `id` and `salary`, write a query that returns the second-highest distinct salary. Return `NULL` if it doesn't exist.",
            DifficultyLevel = DifficultyLevel.Medium,
            ExerciseType = ExerciseType.Coding,
            StarterCode = "-- Your query here",
            Language = "sql",
            IsInterviewChallenge = true,
            FollowUpQuestions = "How would you generalize this to find the Nth highest salary, for an arbitrary N?",
            SortOrder = 5,
            CreatedUtc = now,
            UpdatedUtc = now,
            ExerciseTags = [Tagged(tags, "sql"), Tagged(tags, "subqueries")],
            Hints =
            [
                new ExerciseHint { Text = "What does MAX() return when it aggregates over zero rows? That's the key to handling the 'no second salary' case cleanly.", SortOrder = 1 },
                new ExerciseHint { Text = "DENSE_RANK() ordered by salary descending gives you an alternative, window-function-based approach.", SortOrder = 2 },
            ],
            Solutions =
            [
                new ExerciseSolution
                {
                    ApproachTitle = "OFFSET/FETCH",
                    Explanation = "Sort distinct salaries descending and skip the first row. Simple, but doesn't automatically return NULL if there's no second distinct value — it just returns no rows instead.",
                    SolutionCode = """
                        SELECT DISTINCT salary
                        FROM employees
                        ORDER BY salary DESC
                        OFFSET 1 ROW FETCH NEXT 1 ROW ONLY;
                        """,
                    Language = "sql",
                    TimeComplexity = "O(n log n) — the sort",
                    SpaceComplexity = "O(n)",
                    SortOrder = 1,
                },
                new ExerciseSolution
                {
                    ApproachTitle = "Subquery with MAX (handles the NULL case correctly)",
                    Explanation = "Find the highest salary that is strictly less than the overall highest. If only one distinct salary exists, the WHERE clause matches zero rows, and MAX() over zero rows correctly evaluates to NULL — satisfying the requirement without a separate NULL check.",
                    SolutionCode = """
                        SELECT MAX(salary) AS second_highest
                        FROM employees
                        WHERE salary < (SELECT MAX(salary) FROM employees);
                        """,
                    Language = "sql",
                    TimeComplexity = "O(n)",
                    SpaceComplexity = "O(1)",
                    SortOrder = 2,
                },
                new ExerciseSolution
                {
                    ApproachTitle = "DENSE_RANK Window Function",
                    Explanation = "Rank distinct salaries descending with DENSE_RANK(), then filter to rank 2 — this generalizes cleanly to 'Nth highest' by changing one number.",
                    SolutionCode = """
                        SELECT salary AS second_highest
                        FROM (
                            SELECT salary, DENSE_RANK() OVER (ORDER BY salary DESC) AS rnk
                            FROM (SELECT DISTINCT salary FROM employees) AS distinct_salaries
                        ) ranked
                        WHERE rnk = 2;
                        """,
                    Language = "sql",
                    TimeComplexity = "O(n log n)",
                    SpaceComplexity = "O(n)",
                    SortOrder = 3,
                },
            ],
        };
    }

    private static Exercise BuildDuplicateEmails(int lessonId, Dictionary<string, Tag> tags)
    {
        var now = DateTime.UtcNow;
        return new Exercise
        {
            LessonId = lessonId,
            Slug = "find-duplicate-emails",
            Title = "Find Duplicate Emails",
            Prompt = "Given a `people` table with columns `id` and `email`, write a query that returns every email address that appears more than once.",
            DifficultyLevel = DifficultyLevel.Easy,
            ExerciseType = ExerciseType.Coding,
            StarterCode = "-- Your query here",
            Language = "sql",
            IsInterviewChallenge = true,
            FollowUpQuestions = "How would you write a query to DELETE the duplicate rows, keeping only the lowest id for each email?",
            SortOrder = 6,
            CreatedUtc = now,
            UpdatedUtc = now,
            ExerciseTags = [Tagged(tags, "sql"), Tagged(tags, "joins")],
            Hints =
            [
                new ExerciseHint { Text = "GROUP BY email, then filter groups (not rows) with HAVING.", SortOrder = 1 },
            ],
            Solutions =
            [
                new ExerciseSolution
                {
                    ApproachTitle = "GROUP BY + HAVING",
                    Explanation = "Group rows by email, then keep only the groups whose count is greater than one.",
                    SolutionCode = """
                        SELECT email
                        FROM people
                        GROUP BY email
                        HAVING COUNT(*) > 1;
                        """,
                    Language = "sql",
                    TimeComplexity = "O(n log n)",
                    SpaceComplexity = "O(n)",
                    SortOrder = 1,
                },
            ],
        };
    }

    private static Exercise BuildPalindromeCheck(int lessonId, Dictionary<string, Tag> tags)
    {
        var now = DateTime.UtcNow;
        return new Exercise
        {
            LessonId = lessonId,
            Slug = "palindrome-check-pattern-matching",
            Title = "Palindrome Check Using Pattern Matching",
            Prompt = "Write a method that returns `true` if a string reads the same forwards and backwards (ignoring case), using a C# pattern-matching-friendly style rather than a manual index loop.",
            DifficultyLevel = DifficultyLevel.Easy,
            ExerciseType = ExerciseType.Coding,
            StarterCode = """
                public bool IsPalindrome(string s)
                {
                    // Your code here
                }
                """,
            Language = "csharp",
            IsInterviewChallenge = false,
            FollowUpQuestions = "How would you adapt this to ignore spaces and punctuation, not just casing?",
            SortOrder = 7,
            CreatedUtc = now,
            UpdatedUtc = now,
            ExerciseTags = [Tagged(tags, "csharp"), Tagged(tags, "strings")],
            Hints =
            [
                new ExerciseHint { Text = "String has a built-in way to compare against its own reversed characters.", SortOrder = 1 },
            ],
            Solutions =
            [
                new ExerciseSolution
                {
                    ApproachTitle = "Reverse and Compare",
                    Explanation = "Build the reversed string using a span-based reverse, then compare case-insensitively.",
                    SolutionCode = """
                        public bool IsPalindrome(string s)
                        {
                            var chars = s.ToCharArray();
                            Array.Reverse(chars);
                            return string.Equals(s, new string(chars), StringComparison.OrdinalIgnoreCase);
                        }
                        """,
                    Language = "csharp",
                    TimeComplexity = "O(n)",
                    SpaceComplexity = "O(n)",
                    SortOrder = 1,
                },
            ],
        };
    }

    private static Exercise BuildShapeAreaCalculator(int lessonId, Dictionary<string, Tag> tags)
    {
        var now = DateTime.UtcNow;
        return new Exercise
        {
            LessonId = lessonId,
            Slug = "shape-area-calculator",
            Title = "Extensible Shape Area Calculator",
            Prompt = "Design a small set of types so that a `TotalArea` function can sum the area of any mix of shapes (at least Circle and Rectangle), without ever needing to change when a new shape is added.",
            DifficultyLevel = DifficultyLevel.Medium,
            ExerciseType = ExerciseType.Coding,
            StarterCode = """
                // Design your interface and shape types, then implement TotalArea.
                public double TotalArea(IEnumerable<object> shapes)
                {
                    // Your code here
                }
                """,
            Language = "csharp",
            IsInterviewChallenge = false,
            FollowUpQuestions = "How would your design change if shapes needed a Perimeter calculation too, alongside Area?",
            SortOrder = 8,
            CreatedUtc = now,
            UpdatedUtc = now,
            ExerciseTags = [Tagged(tags, "csharp"), Tagged(tags, "oop")],
            Hints =
            [
                new ExerciseHint { Text = "This is the Open/Closed Principle in practice — extend via new types, not by modifying TotalArea.", SortOrder = 1 },
            ],
            Solutions =
            [
                new ExerciseSolution
                {
                    ApproachTitle = "Interface + Polymorphism",
                    Explanation = "Define an IShapeAreaCalculator interface; TotalArea depends only on that interface, so adding a new shape never requires changing TotalArea itself.",
                    SolutionCode = """
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

                        public double TotalArea(IEnumerable<IShapeAreaCalculator> shapes) =>
                            shapes.Sum(s => s.CalculateArea());
                        """,
                    Language = "csharp",
                    TimeComplexity = "O(n)",
                    SpaceComplexity = "O(1) extra",
                    SortOrder = 1,
                },
            ],
        };
    }

    private static Exercise BuildRateLimiterDesign(int lessonId, Dictionary<string, Tag> tags)
    {
        var now = DateTime.UtcNow;
        return new Exercise
        {
            LessonId = lessonId,
            Slug = "design-a-rate-limiter",
            Title = "Design a Rate Limiter",
            Prompt = "Design (in writing — no code required) a rate limiter that allows at most N requests per user per minute. Describe the data structure, what happens at the boundary between minutes, and how it would work across multiple app server instances.",
            DifficultyLevel = DifficultyLevel.Medium,
            ExerciseType = ExerciseType.Conceptual,
            IsInterviewChallenge = true,
            FollowUpQuestions = "How would your design change if the limit needed to be 'N requests per rolling 60 seconds' instead of per fixed-clock minute?",
            SortOrder = 9,
            CreatedUtc = now,
            UpdatedUtc = now,
            ExerciseTags = [Tagged(tags, "system-design")],
            Hints =
            [
                new ExerciseHint { Text = "A fixed-window counter is the simplest approach — but think about what happens right at the window boundary.", SortOrder = 1 },
                new ExerciseHint { Text = "Multiple app servers can't each keep their own in-memory counter — where does shared state need to live?", SortOrder = 2 },
            ],
            Solutions =
            [
                new ExerciseSolution
                {
                    ApproachTitle = "Fixed Window Counter in a Shared Cache",
                    Explanation = "Store a counter per user per minute-bucket (e.g., key `ratelimit:{userId}:{minuteTimestamp}`) in a shared cache like Redis, incrementing on each request and rejecting once the count exceeds N. Using a shared cache (not per-server memory) means the limit is enforced correctly regardless of which app server instance handles the request. The main weakness: a burst right at the window boundary (end of minute 1, start of minute 2) can briefly allow close to 2N requests in a short span — a sliding-window-log or sliding-window-counter algorithm fixes this at the cost of more storage/complexity.",
                    SolutionCode = "N/A — conceptual design exercise, no code required.",
                    Language = "text",
                    TimeComplexity = "O(1) per request",
                    SpaceComplexity = "O(active users)",
                    SortOrder = 1,
                },
            ],
        };
    }

    private static Exercise BuildCacheAsideImplementation(int lessonId, Dictionary<string, Tag> tags)
    {
        var now = DateTime.UtcNow;
        return new Exercise
        {
            LessonId = lessonId,
            Slug = "implement-cache-aside",
            Title = "Implement the Cache-Aside Pattern",
            Prompt = "Write a method `GetProductAsync(int id)` that implements cache-aside: check the cache first, fall back to the database on a miss, and populate the cache before returning.",
            DifficultyLevel = DifficultyLevel.Easy,
            ExerciseType = ExerciseType.Coding,
            StarterCode = """
                public async Task<Product> GetProductAsync(int id)
                {
                    // Your code here
                }
                """,
            Language = "csharp",
            IsInterviewChallenge = false,
            FollowUpQuestions = "How would you handle cache invalidation when the product is updated elsewhere in the system?",
            SortOrder = 10,
            CreatedUtc = now,
            UpdatedUtc = now,
            ExerciseTags = [Tagged(tags, "system-design"), Tagged(tags, "caching")],
            Hints =
            [
                new ExerciseHint { Text = "Check the cache before touching the database at all — that's the whole point of the pattern.", SortOrder = 1 },
            ],
            Solutions =
            [
                new ExerciseSolution
                {
                    ApproachTitle = "Cache-Aside",
                    Explanation = "On a cache hit, return immediately with no database call. On a miss, load from the database, populate the cache with a TTL, then return.",
                    SolutionCode = """
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
                        """,
                    Language = "csharp",
                    TimeComplexity = "O(1) on hit, O(query cost) on miss",
                    SpaceComplexity = "O(1) per cached item",
                    SortOrder = 1,
                },
            ],
        };
    }

    private static Exercise BuildResolveMergeConflict(int lessonId, Dictionary<string, Tag> tags)
    {
        var now = DateTime.UtcNow;
        return new Exercise
        {
            LessonId = lessonId,
            Slug = "resolve-a-merge-conflict",
            Title = "Resolve a Merge Conflict",
            Prompt = """
                You're merging `feature/increase-retries` into `main` and get a conflict in `Config.cs`:

                ```
                <<<<<<< HEAD
                public const int MaxRetries = 3;
                =======
                public const int MaxRetries = 5;
                >>>>>>> feature/increase-retries
                ```

                Write the correct final content of this file after resolving the conflict in favor of the feature branch's intent, and describe the exact commands you'd run afterward.
                """,
            DifficultyLevel = DifficultyLevel.Easy,
            ExerciseType = ExerciseType.Conceptual,
            IsInterviewChallenge = false,
            FollowUpQuestions = "What would you do differently if you realized BOTH values were wrong, and the correct answer was actually 4?",
            SortOrder = 11,
            CreatedUtc = now,
            UpdatedUtc = now,
            ExerciseTags = [Tagged(tags, "git")],
            Hints =
            [
                new ExerciseHint { Text = "All three marker lines (<<<<<<<, =======, >>>>>>>) must be deleted, not just the losing side's content.", SortOrder = 1 },
            ],
            Solutions =
            [
                new ExerciseSolution
                {
                    ApproachTitle = "Keep the Feature Branch's Value",
                    Explanation = "Delete all three conflict marker lines, keep the value that reflects the intended change (5 retries, since that's the point of the feature branch), then stage and commit the resolution.",
                    SolutionCode = """
                        public const int MaxRetries = 5;

                        // Then:
                        git add Config.cs
                        git commit
                        """,
                    Language = "csharp",
                    SortOrder = 1,
                },
            ],
        };
    }

    private static Exercise BuildCiPipelineYaml(int lessonId, Dictionary<string, Tag> tags)
    {
        var now = DateTime.UtcNow;
        return new Exercise
        {
            LessonId = lessonId,
            Slug = "write-a-ci-pipeline",
            Title = "Write a Basic CI Pipeline",
            Prompt = "Write a GitHub Actions workflow that runs on every push and pull request, restores dependencies, builds a .NET project, and runs its test suite — failing the pipeline if any step fails.",
            DifficultyLevel = DifficultyLevel.Easy,
            ExerciseType = ExerciseType.Coding,
            StarterCode = "# Your workflow YAML here",
            Language = "yaml",
            IsInterviewChallenge = false,
            FollowUpQuestions = "How would you add a step that only deploys to production when a push lands specifically on the main branch, not on every branch?",
            SortOrder = 12,
            CreatedUtc = now,
            UpdatedUtc = now,
            ExerciseTags = [Tagged(tags, "devops"), Tagged(tags, "ci-cd")],
            Hints =
            [
                new ExerciseHint { Text = "Each step runs in order; if one fails, later steps in the same job don't run by default.", SortOrder = 1 },
            ],
            Solutions =
            [
                new ExerciseSolution
                {
                    ApproachTitle = "Minimal GitHub Actions Workflow",
                    Explanation = "Trigger on push and pull_request, then checkout, set up .NET, restore, build, and test in order.",
                    SolutionCode = """
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
                        """,
                    Language = "yaml",
                    SortOrder = 1,
                },
            ],
        };
    }

    private static Exercise BuildFizzBuzz(int lessonId, Dictionary<string, Tag> tags)
    {
        var now = DateTime.UtcNow;
        return new Exercise
        {
            LessonId = lessonId,
            Slug = "fizzbuzz-classic",
            Title = "FizzBuzz",
            Prompt = "Write a method that prints the numbers 1 through `n`. But for multiples of 3, print \"Fizz\" instead of the number; for multiples of 5, print \"Buzz\"; and for multiples of both 3 and 5, print \"FizzBuzz\".",
            DifficultyLevel = DifficultyLevel.Easy,
            ExerciseType = ExerciseType.Coding,
            StarterCode = """
                public void FizzBuzz(int n)
                {
                    // Your code here
                }
                """,
            Language = "csharp",
            IsInterviewChallenge = true,
            FollowUpQuestions = "How would you rewrite the body of this loop using a switch expression instead of an if/else if chain?",
            SortOrder = 13,
            CreatedUtc = now,
            UpdatedUtc = now,
            ExerciseTags = [Tagged(tags, "loops"), Tagged(tags, "csharp")],
            Hints =
            [
                new ExerciseHint { Text = "Check divisibility by 15 (3 * 5) FIRST, before checking 3 or 5 alone -- order matters in an if/else if chain.", SortOrder = 1 },
            ],
            Solutions =
            [
                new ExerciseSolution
                {
                    ApproachTitle = "if/else if Chain",
                    Explanation = "Loop from 1 to n, checking the most specific condition (divisible by both 3 and 5) before the more general ones -- otherwise a multiple of 15 would incorrectly print just \"Fizz\".",
                    SolutionCode = """
                        public void FizzBuzz(int n)
                        {
                            for (int i = 1; i <= n; i++)
                            {
                                if (i % 15 == 0) Console.WriteLine("FizzBuzz");
                                else if (i % 3 == 0) Console.WriteLine("Fizz");
                                else if (i % 5 == 0) Console.WriteLine("Buzz");
                                else Console.WriteLine(i);
                            }
                        }
                        """,
                    Language = "csharp",
                    TimeComplexity = "O(n)",
                    SpaceComplexity = "O(1)",
                    SortOrder = 1,
                },
            ],
        };
    }

    private static Exercise BuildSumOfAList(int lessonId, Dictionary<string, Tag> tags)
    {
        var now = DateTime.UtcNow;
        return new Exercise
        {
            LessonId = lessonId,
            Slug = "sum-of-a-list",
            Title = "Sum of a List",
            Prompt = "Write a method that takes a `List<int>` and returns the sum of all its elements, without using LINQ's built-in `.Sum()` method.",
            DifficultyLevel = DifficultyLevel.Easy,
            ExerciseType = ExerciseType.Coding,
            StarterCode = """
                public int SumOfList(List<int> numbers)
                {
                    // Your code here
                }
                """,
            Language = "csharp",
            IsInterviewChallenge = false,
            FollowUpQuestions = "Now solve it again using LINQ's .Sum() -- how many lines does that take compared to your loop?",
            SortOrder = 14,
            CreatedUtc = now,
            UpdatedUtc = now,
            ExerciseTags = [Tagged(tags, "lists"), Tagged(tags, "csharp")],
            Hints =
            [
                new ExerciseHint { Text = "Start a running total at 0 before the loop begins, and add each number to it as you go.", SortOrder = 1 },
            ],
            Solutions =
            [
                new ExerciseSolution
                {
                    ApproachTitle = "foreach Loop",
                    Explanation = "Keep a running total, starting at 0, and add each item in the list to it with foreach.",
                    SolutionCode = """
                        public int SumOfList(List<int> numbers)
                        {
                            int total = 0;
                            foreach (int number in numbers)
                            {
                                total += number;
                            }
                            return total;
                        }
                        """,
                    Language = "csharp",
                    TimeComplexity = "O(n)",
                    SpaceComplexity = "O(1)",
                    SortOrder = 1,
                },
            ],
        };
    }

    private static Exercise BuildCountVowels(int lessonId, Dictionary<string, Tag> tags)
    {
        var now = DateTime.UtcNow;
        return new Exercise
        {
            LessonId = lessonId,
            Slug = "count-vowels-in-a-string",
            Title = "Count Vowels in a String",
            Prompt = "Write a method that counts how many vowels (a, e, i, o, u -- case-insensitive) appear in a given string.",
            DifficultyLevel = DifficultyLevel.Easy,
            ExerciseType = ExerciseType.Coding,
            StarterCode = """
                public int CountVowels(string text)
                {
                    // Your code here
                }
                """,
            Language = "csharp",
            IsInterviewChallenge = false,
            FollowUpQuestions = "How would you change this to count vowels only at the START of each word, not anywhere in the string?",
            SortOrder = 15,
            CreatedUtc = now,
            UpdatedUtc = now,
            ExerciseTags = [Tagged(tags, "strings"), Tagged(tags, "csharp")],
            Hints =
            [
                new ExerciseHint { Text = "Call ToLower() on the string once at the start, so you only need to check 5 lowercase letters instead of 10.", SortOrder = 1 },
            ],
            Solutions =
            [
                new ExerciseSolution
                {
                    ApproachTitle = "foreach + Contains",
                    Explanation = "Lowercase the string once, then check each character against a small string of vowels using Contains.",
                    SolutionCode = """
                        public int CountVowels(string text)
                        {
                            string vowels = "aeiou";
                            int count = 0;
                            foreach (char c in text.ToLower())
                            {
                                if (vowels.Contains(c)) count++;
                            }
                            return count;
                        }
                        """,
                    Language = "csharp",
                    TimeComplexity = "O(n)",
                    SpaceComplexity = "O(1)",
                    SortOrder = 1,
                },
            ],
        };
    }

    private static Exercise BuildRectangleAreaClass(int lessonId, Dictionary<string, Tag> tags)
    {
        var now = DateTime.UtcNow;
        return new Exercise
        {
            LessonId = lessonId,
            Slug = "rectangle-area-class",
            Title = "Rectangle Area Class",
            Prompt = "Create a `Rectangle` class with `Width` and `Height` fields set through a constructor, and a method `GetArea()` that returns the rectangle's area.",
            DifficultyLevel = DifficultyLevel.Easy,
            ExerciseType = ExerciseType.Coding,
            StarterCode = """
                public class Rectangle
                {
                    // Your code here
                }
                """,
            Language = "csharp",
            IsInterviewChallenge = false,
            FollowUpQuestions = "How would you add a GetPerimeter() method to the same class?",
            SortOrder = 16,
            CreatedUtc = now,
            UpdatedUtc = now,
            ExerciseTags = [Tagged(tags, "csharp"), Tagged(tags, "oop")],
            Hints =
            [
                new ExerciseHint { Text = "The constructor's only job is to set Width and Height -- GetArea() does the actual math separately, whenever it's called.", SortOrder = 1 },
            ],
            Solutions =
            [
                new ExerciseSolution
                {
                    ApproachTitle = "Class with Constructor",
                    Explanation = "The constructor stores the two dimensions; GetArea() multiplies them together each time it's called.",
                    SolutionCode = """
                        public class Rectangle
                        {
                            public double Width;
                            public double Height;

                            public Rectangle(double width, double height)
                            {
                                Width = width;
                                Height = height;
                            }

                            public double GetArea()
                            {
                                return Width * Height;
                            }
                        }
                        """,
                    Language = "csharp",
                    TimeComplexity = "O(1)",
                    SpaceComplexity = "O(1)",
                    SortOrder = 1,
                },
            ],
        };
    }

    private static Exercise BuildFilterEvenNumbersLinq(int lessonId, Dictionary<string, Tag> tags)
    {
        var now = DateTime.UtcNow;
        return new Exercise
        {
            LessonId = lessonId,
            Slug = "filter-even-numbers-linq",
            Title = "Filter Even Numbers with LINQ",
            Prompt = "Given a `List<int>`, use LINQ's `Where()` to return only the even numbers, as a new `List<int>`.",
            DifficultyLevel = DifficultyLevel.Easy,
            ExerciseType = ExerciseType.Coding,
            StarterCode = """
                public List<int> FilterEvenNumbers(List<int> numbers)
                {
                    // Your code here
                }
                """,
            Language = "csharp",
            IsInterviewChallenge = false,
            FollowUpQuestions = "Now chain a .Select() after your Where() to double each even number before returning it.",
            SortOrder = 17,
            CreatedUtc = now,
            UpdatedUtc = now,
            ExerciseTags = [Tagged(tags, "linq"), Tagged(tags, "csharp")],
            Hints =
            [
                new ExerciseHint { Text = "n % 2 == 0 is how you check whether a number is even inside the Where() lambda.", SortOrder = 1 },
            ],
            Solutions =
            [
                new ExerciseSolution
                {
                    ApproachTitle = "Where + ToList",
                    Explanation = "Where() keeps only the items matching the even check; ToList() turns the result back into a real List<int>.",
                    SolutionCode = """
                        public List<int> FilterEvenNumbers(List<int> numbers)
                        {
                            return numbers.Where(n => n % 2 == 0).ToList();
                        }
                        """,
                    Language = "csharp",
                    TimeComplexity = "O(n)",
                    SpaceComplexity = "O(n)",
                    SortOrder = 1,
                },
            ],
        };
    }
}
