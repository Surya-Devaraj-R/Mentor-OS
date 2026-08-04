// LESSON 12: Inheritance (one class building on another)

Dog myDog = new Dog("Rex", 3);
myDog.Bark();     // Dog's own, special method
myDog.Describe(); // borrowed from Animal, without Dog rewriting it!

Cat myCat = new Cat("Whiskers", 2);
myCat.Meow();     // Cat's own, special method
myCat.Describe(); // same borrowed method, still works here too

// --- The base blueprint, shared by every kind of animal ---
class Animal
{
    public string Name;
    public int Age;

    public Animal(string name, int age)
    {
        Name = name;
        Age = age;
    }

    public void Describe()
    {
        Console.WriteLine($"{Name} is {Age} years old.");
    }
}

// "class Dog : Animal" means "a Dog IS AN Animal."
// Dog automatically gets Name, Age, and Describe() for free -- for free,
// meaning we never typed that code again inside Dog.
class Dog : Animal
{
    // ": base(name, age)" means "before anything else, run Animal's OWN
    // constructor with these same values" -- Animal is the one that
    // actually knows how to set up Name and Age.
    public Dog(string name, int age) : base(name, age)
    {
    }

    // This method exists ONLY on Dog. Cat does not have this.
    public void Bark()
    {
        Console.WriteLine($"{Name} says: Woof!");
    }
}

class Cat : Animal
{
    public Cat(string name, int age) : base(name, age)
    {
    }

    // This method exists ONLY on Cat. Dog does not have this.
    public void Meow()
    {
        Console.WriteLine($"{Name} says: Meow!");
    }
}
