// LESSON 14: Dictionaries (look things up by a key, not a position)

// A List looks things up by POSITION (index 0, 1, 2...).
// A Dictionary looks things up by a KEY you choose -- here, a person's name.
Dictionary<string, string> phoneBook = new Dictionary<string, string>
{
    { "Surya", "555-1234" },
    { "Ann", "555-5678" }
};

Console.WriteLine(phoneBook["Surya"]); // look up the value that belongs to the key "Surya"

phoneBook["Raj"] = "555-9999"; // this both ADDS a new entry (since "Raj" doesn't exist yet)

Console.WriteLine(phoneBook.ContainsKey("Ann")); // True  -- "Ann" IS a key in this dictionary
Console.WriteLine(phoneBook.ContainsKey("Bob")); // False -- "Bob" is NOT a key in this dictionary

Console.WriteLine("---");

// Walking through every key-value PAIR in the dictionary, one at a time.
foreach (KeyValuePair<string, string> entry in phoneBook)
{
    Console.WriteLine($"{entry.Key}: {entry.Value}");
}
