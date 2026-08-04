// LESSON 9: Arrays (a fixed-size box for many values)

// An array holds many values, like a List did in Lesson 7.
// The BIG difference: an array's size is FIXED the moment you create it.
// You cannot add a 4th value to this later. It will always hold exactly 3.
int[] numbers = { 10, 20, 30 };

Console.WriteLine(numbers[0]);   // first item -- index 0, same rule as List
Console.WriteLine(numbers.Length); // arrays use ".Length", NOT ".Count" -- just a naming difference to remember

Console.WriteLine("---");

for (int i = 0; i < numbers.Length; i++)
{
    Console.WriteLine(numbers[i]);
}
