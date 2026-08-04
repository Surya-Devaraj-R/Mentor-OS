// LESSON 4: Making decisions (if / else)

int age = 20;

// A "bool" is a box that can only hold one of two values: true or false.
// Here, age >= 18 is a QUESTION: "is age 20 greater than or equal to 18?"
// The answer (true or false) gets stored in the box "isAdult".
bool isAdult = age >= 18;
Console.WriteLine(isAdult); // prints: True

if (isAdult)
{
    Console.WriteLine("You can vote.");
}
else
{
    Console.WriteLine("You cannot vote yet.");
}

// Now a decision with THREE possible outcomes, using else if.
int temperature = 15;

if (temperature > 30)
{
    Console.WriteLine("It's hot outside.");
}
else if (temperature > 15)
{
    Console.WriteLine("It's warm outside.");
}
else
{
    Console.WriteLine("It's cold outside.");
}
