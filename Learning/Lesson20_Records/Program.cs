// LESSON 20: Records (a short way to hold data that compares by value)

// A "record" is a short way to write a class that's mainly just about
// holding data. This ONE line replaces a whole class with fields,
// a constructor, AND gives you nice printing and comparison for free.
Person p1 = new Person("Surya", 28);
Person p2 = new Person("Surya", 28);

Console.WriteLine(p1); // records print themselves nicely, automatically
Console.WriteLine(p1 == p2); // True! Records compare by VALUE -- same data means "equal"

Console.WriteLine("---");

// Compare that to an ordinary class with the SAME data:
Dog d1 = new Dog("Rex");
Dog d2 = new Dog("Rex");

Console.WriteLine(d1 == d2); // False! Classes compare by IDENTITY (are these
                              // the exact same object in memory?), not by
                              // their data -- even though d1 and d2 hold
                              // identical information, they're two separate
                              // objects, so they are NOT considered equal.

record Person(string Name, int Age); // this one line IS the whole record

class Dog
{
    public string Name;

    public Dog(string name)
    {
        Name = name;
    }
}
