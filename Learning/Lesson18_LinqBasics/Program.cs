// LESSON 18: LINQ basics (Where, Select, OrderBy)

// LINQ gives you one-line tools for common list tasks, instead of
// writing your own loop by hand every single time.

List<int> numbers = new List<int> { 5, 2, 8, 1, 9, 3 };

// "n => n > 4" is called a LAMBDA. Read it as: "for each item (I'll
// call it n while I'm looking at it), check: is n greater than 4?"
// Where() KEEPS only the items where the answer is true.
List<int> bigNumbers = numbers.Where(n => n > 4).ToList();
Console.WriteLine("Numbers bigger than 4:");
foreach (int n in bigNumbers)
{
    Console.WriteLine(n);
}

Console.WriteLine("---");

// Select() TRANSFORMS every item into something new -- here, doubling it.
List<int> doubled = numbers.Select(n => n * 2).ToList();
Console.WriteLine("Every number doubled:");
foreach (int n in doubled)
{
    Console.WriteLine(n);
}

Console.WriteLine("---");

// OrderBy() SORTS the list -- smallest to largest, using whatever you give it.
List<int> sorted = numbers.OrderBy(n => n).ToList();
Console.WriteLine("Sorted smallest to largest:");
foreach (int n in sorted)
{
    Console.WriteLine(n);
}
