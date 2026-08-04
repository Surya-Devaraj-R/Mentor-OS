// LESSON 10: Handling errors (try / catch)

// int.Parse tries to turn text into a whole number.
// If the text is NOT a real number, the program would normally CRASH.
// "try/catch" lets us catch that crash and handle it gracefully instead.

string badInput = "abc";
string goodInput = "42";

// --- Attempt #1: text that is NOT a valid number ---
try
{
    int number = int.Parse(badInput); // this line will fail (throw an error)
    Console.WriteLine("You entered: " + number); // this line never runs, because the line above failed
}
catch (FormatException)
{
    // The program jumps straight here the moment something inside "try" fails.
    Console.WriteLine("That wasn't a valid number!");
}

// --- Attempt #2: text that IS a valid number ---
try
{
    int number = int.Parse(goodInput); // this line succeeds
    Console.WriteLine("You entered: " + number); // so this line DOES run
}
catch (FormatException)
{
    Console.WriteLine("That wasn't a valid number!"); // skipped entirely -- nothing failed
}

Console.WriteLine("Program continues normally after this.");
