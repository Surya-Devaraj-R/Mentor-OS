// LESSON 2: Variables (boxes that hold information)

// A "variable" is like a labeled box. You put one piece of information
// inside it, give the box a name, and later you can look inside the box
// again by using its name.

// Here we make a box named "age". It can only hold a whole number (int).
// We put the number 25 inside it.
int age = 25;

// Here we make a box named "name". It holds text (string).
// Text always goes inside double quotes " ".
string name = "Surya";

// Now we print what is inside each box.
Console.WriteLine(age);
Console.WriteLine(name);

// We can also mix text and a box's value together in one line.
// The $ before the quotes turns on a special feature called
// "string interpolation" -- it means: "if you see { } inside this text,
// look inside those curly braces, find a box name, and put that box's
// value there instead."
Console.WriteLine($"My name is {name} and I am {age} years old.");
