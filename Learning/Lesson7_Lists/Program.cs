// LESSON 7: Lists (holding many things at once)

// A List is a box that holds MANY values of the same type, in order.
List<string> names = new List<string> { "Surya", "Ann", "Raj" };

// Each item has a position number, called an "index". Counting starts at 0, not 1!
Console.WriteLine(names[0]); // the FIRST item -- prints "Surya"
Console.WriteLine(names[1]); // the SECOND item -- prints "Ann"

Console.WriteLine(names.Count); // how many items are in the list right now

names.Add("New Friend"); // add a new item to the end of the list

Console.WriteLine("---");

// "foreach" walks through EVERY item in the list, one at a time,
// without you needing to know how many there are.
foreach (string name in names)
{
    Console.WriteLine(name);
}
