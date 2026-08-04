// LESSON 17: String manipulation (Split, Trim, Contains, Substring)

string messy = "  Hello, World!  ";
Console.WriteLine($"[{messy.Trim()}]"); // Trim() removes spaces from the START and END only

string text = "Hello, World!";
Console.WriteLine(text.ToUpper()); // every letter becomes CAPITAL
Console.WriteLine(text.ToLower()); // every letter becomes lowercase

Console.WriteLine(text.Contains("World")); // True -- "World" DOES appear somewhere inside text
Console.WriteLine(text.Contains("Bananas")); // False -- it does not appear

Console.WriteLine("---");

// Split() breaks ONE string into MANY pieces, wherever a chosen
// character shows up -- here, every time there's a comma.
string sentence = "one,two,three";
string[] parts = sentence.Split(',');
foreach (string part in parts)
{
    Console.WriteLine(part);
}

Console.WriteLine("---");

// Substring() grabs a piece of a string using position numbers.
Console.WriteLine(text.Substring(7));    // starting at position 7, take the REST of the string -> "World!"
Console.WriteLine(text.Substring(0, 5)); // starting at position 0, take exactly 5 characters -> "Hello"
