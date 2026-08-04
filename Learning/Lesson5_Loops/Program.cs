// LESSON 5: Loops (repeating instructions without copy-pasting them)

// --- The "for" loop ---
// This prints the numbers 1 through 5, one per line.
for (int i = 1; i <= 5; i++)
{
    Console.WriteLine(i);
}

Console.WriteLine("---"); // just a separator line, so the output is easier to read

// --- The "while" loop ---
// This counts down from 3 to 1, then says "Liftoff!"
int count = 3;
while (count > 0)
{
    Console.WriteLine("Countdown: " + count);
    count--; // same as: count = count - 1
}
Console.WriteLine("Liftoff!");
