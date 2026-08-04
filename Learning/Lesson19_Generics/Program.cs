// LESSON 19: Generics (one method that works with any type)

List<int> numbers = new List<int> { 10, 20, 30 };
List<string> names = new List<string> { "Surya", "Ann" };

Console.WriteLine(GetFirst(numbers)); // works with a List of int
Console.WriteLine(GetFirst(names));   // the SAME method also works with a List of string

// "<T>" is a placeholder for "whatever type you use me with."
// T stands for "Type" -- it's not a real type itself, just a stand-in.
// This ONE method works for List<int>, List<string>, or a list of
// anything else you can think of -- no copy-pasting a version per type.
static T GetFirst<T>(List<T> list)
{
    return list[0];
}
