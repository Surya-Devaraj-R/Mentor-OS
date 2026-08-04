// LESSON 13: Reading input from the user

// "Console.Write" (no "Line") prints text WITHOUT moving to a new line
// afterward -- so whatever the user types appears right after our prompt,
// on the same line. Compare to "Console.WriteLine", which always jumps
// to a new line.
Console.Write("What is your name? ");

// "Console.ReadLine()" pauses the program and waits for the user to
// type something and press Enter. Whatever they typed comes back as text.
// The "?? """ part means: "if ReadLine somehow gives back nothing at all,
// use an empty piece of text instead." (This is just a safety net.)
string name = Console.ReadLine() ?? "";

Console.Write("How old are you? ");
string ageText = Console.ReadLine() ?? "";

// ReadLine ALWAYS gives back text (a string), even if the user typed
// numbers. So, just like Lesson 10, we use int.Parse to convert that
// text into a real number we can do math with.
int age = int.Parse(ageText);

Console.WriteLine($"Hello, {name}! In 10 years you will be {age + 10}.");
