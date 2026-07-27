using MentorOS.Models;
using MentorOS.Models.Enums;

namespace MentorOS.Data.Seed;

// A handful of real, interview-representative exercises with genuine
// solutions — self-assessment only, no execution (per the confirmed
// two-phase plan: a real code-execution service is a separate, deferred
// initiative with its own security design pass).
public static class ExerciseSeedData
{
    public static List<Exercise> BuildExercises(IReadOnlyDictionary<string, int> lessonIdBySlug)
    {
        var dsaLessonId = lessonIdBySlug["two-pointer-hash-map-patterns"];
        var sqlLessonId = lessonIdBySlug["select-join-query-fundamentals"];

        return
        [
            BuildValidAnagram(dsaLessonId),
            BuildContainsDuplicate(dsaLessonId),
            BuildSecondHighestSalary(sqlLessonId),
        ];
    }

    private static Exercise BuildValidAnagram(int lessonId)
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
            SortOrder = 1,
            CreatedUtc = now,
            UpdatedUtc = now,
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

    private static Exercise BuildContainsDuplicate(int lessonId)
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
            SortOrder = 2,
            CreatedUtc = now,
            UpdatedUtc = now,
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

    private static Exercise BuildSecondHighestSalary(int lessonId)
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
            SortOrder = 3,
            CreatedUtc = now,
            UpdatedUtc = now,
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
            ],
        };
    }
}
