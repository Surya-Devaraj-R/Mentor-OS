// LESSON 6: Methods (reusable blocks of instructions)

// A method with no input, and no answer to give back.
// "void" means: "this method does something, but hands nothing back."
static void SayHello()
{
    Console.WriteLine("Hello there!");
}

// A method that takes ONE input (called a "parameter").
// Still "void" -- it does something, but still gives nothing back.
static void Greet(string name)
{
    Console.WriteLine($"Hello, {name}!");
}

// A method that takes TWO inputs, and DOES give something back.
// "int" before the name means: "this method hands back a whole number."
static int Add(int a, int b)
{
    return a + b; // "return" means: hand this value back to whoever called us.
}

// --- Now we actually USE (call) the methods above ---

SayHello();
Greet("Surya");

int result = Add(4, 7);
Console.WriteLine(result);
