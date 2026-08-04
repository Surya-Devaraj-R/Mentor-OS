// LESSON 8: Classes (building your own "things" with data and actions)

// Make TWO separate dogs from the same blueprint.
Dog myDog = new Dog();
myDog.Name = "Rex";
myDog.Age = 3;
myDog.Bark();

Dog anotherDog = new Dog();
anotherDog.Name = "Bella";
anotherDog.Age = 5;
anotherDog.Bark();

Console.WriteLine($"{myDog.Name} is {myDog.Age} years old.");
Console.WriteLine($"{anotherDog.Name} is {anotherDog.Age} years old.");

// --- The blueprint itself ---
// A "class" is a blueprint for making your own kind of "thing."
// It bundles DATA (fields) and ACTIONS (methods) together in one place.
class Dog
{
    // These are FIELDS -- data that belongs to each individual dog.
    public string Name = "";
    public int Age;

    // This is a METHOD -- notice it does NOT have the word "static" this time.
    // Without "static", this method belongs to ONE specific dog, and uses
    // THAT dog's own Name when it runs.
    public void Bark()
    {
        Console.WriteLine($"{Name} says: Woof!");
    }
}
