// LESSON 15: Enums & switch (naming a fixed set of options)

Day today = Day.Wednesday;

Console.WriteLine(today); // enums print their own name -- prints "Wednesday"

// A "switch" checks ONE value against many possible cases -- a tidier
// alternative to a long chain of "if / else if / else if / ...".
switch (today)
{
    case Day.Saturday:
    case Day.Sunday:
        // Two "case" lines stacked together with no code between them
        // means: "either of these leads to the same result."
        Console.WriteLine("It's the weekend!");
        break; // "break" means: stop here, don't fall into the next case

    default:
        // "default" runs when NONE of the cases above matched.
        Console.WriteLine("It's a weekday.");
        break;
}

// An "enum" is a fixed, named list of allowed values. Instead of using
// a plain number (0, 1, 2...) or text ("Monday") that could be misspelled,
// you get a real, safe, named option the computer understands.
enum Day
{
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday,
    Sunday
}
