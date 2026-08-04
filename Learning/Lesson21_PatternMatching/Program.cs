// LESSON 21: Pattern matching (is, switch expressions)

object item = 42; // "object" can hold ANYTHING -- a number, text, a Dog, anything

// "is int number" does TWO things in one step:
// 1) checks "is this actually a whole number?"
// 2) if yes, captures it into a brand new box called "number" you can use right away
if (item is int number)
{
    Console.WriteLine($"It's a number: {number}");
}

Console.WriteLine("---");

// A "switch expression" is a compact way to turn an input into an output,
// without writing "case" / "break" / "return" over and over.
Console.WriteLine(Describe(-5));
Console.WriteLine(Describe(0));
Console.WriteLine(Describe(5));

static string Describe(int n) => n switch
{
    < 0 => "negative",  // "< 0" means "matches any number less than 0"
    0 => "zero",        // matches exactly 0
    > 0 => "positive"   // matches any number greater than 0
};
