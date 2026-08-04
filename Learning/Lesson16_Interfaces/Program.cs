// LESSON 16: Interfaces (a contract a class promises to follow)

// This method works with ANY object, as long as it promises to be
// "something that can MakeSound()" -- it doesn't care if it's a Dog,
// a Cat, or anything else, as long as that promise is kept.
static void AnnounceSound(IMakesSound animal)
{
    animal.MakeSound();
}

AnnounceSound(new Dog());
AnnounceSound(new Cat());

// An "interface" is a CONTRACT, not real code. It only lists WHAT
// methods a class must have -- with no bodies, no actual instructions.
// Compare this to Lesson 12's "Animal" base class, which gave Dog and
// Cat REAL, shared code (Describe). An interface gives NO code at all --
// only a promise that a certain method will exist.
interface IMakesSound
{
    void MakeSound(); // just a promise: "whoever implements me must have this method"
}

// "class Dog : IMakesSound" means "Dog PROMISES to provide everything
// IMakesSound requires." If Dog forgot to write MakeSound(), this would
// not compile at all -- the promise is enforced by the compiler.
class Dog : IMakesSound
{
    public void MakeSound()
    {
        Console.WriteLine("Woof!");
    }
}

class Cat : IMakesSound
{
    public void MakeSound()
    {
        Console.WriteLine("Meow!");
    }
}
