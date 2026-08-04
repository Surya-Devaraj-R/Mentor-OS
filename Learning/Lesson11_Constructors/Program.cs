// LESSON 11: Constructors (setting up an object the moment it's built)

// In Lesson 8, we built a Dog, then filled in Name and Age line by line
// AFTER building it. That means it's possible to forget to set one of them.
// A constructor fixes this: it forces you to hand over the values
// the moment you build the object -- no incomplete dogs allowed.

Dog myDog = new Dog("Rex", 3);
myDog.Bark();

Dog anotherDog = new Dog("Bella", 5);
anotherDog.Bark();

Console.WriteLine($"{myDog.Name} is {myDog.Age} years old.");

class Dog
{
    public string Name;
    public int Age;

    // A CONSTRUCTOR is a special method: same name as the class, no
    // return type at all (not even "void"). It runs AUTOMATICALLY,
    // exactly once, the instant "new Dog(...)" is called.
    public Dog(string name, int age)
    {
        // Note: "name" (lowercase) is the INPUT we were just handed.
        // "Name" (uppercase, with a capital N) is the dog's own FIELD.
        // C# treats these as two completely different things -- capital
        // letters matter. This line copies the input into the real field.
        Name = name;
        Age = age;
    }

    public void Bark()
    {
        Console.WriteLine($"{Name} says: Woof!");
    }
}
