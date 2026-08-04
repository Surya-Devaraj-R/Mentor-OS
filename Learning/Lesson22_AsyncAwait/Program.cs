// LESSON 22: Async/await basics (waiting for slow things without freezing)

// Some operations take real time -- downloading something from the
// internet, reading a big file, waiting on a database. "async" and
// "await" are how C# handles "wait for this, without freezing everything else."

Console.WriteLine("Starting...");

DateTime start = DateTime.Now;
await DoSomethingSlowAsync();
DateTime end = DateTime.Now;

Console.WriteLine($"Done! That took about {(end - start).TotalSeconds:0.0} seconds.");

// "async" on a method means: "this method might need to pause and wait
// partway through." "Task" (instead of "void") is what a method returns
// when it's async and doesn't hand back a specific value.
static async Task DoSomethingSlowAsync()
{
    Console.WriteLine("Working... (pretend this is downloading a file)");

    // "await" means: "pause right here until this finishes -- but don't
    // freeze the whole program while waiting." Task.Delay(2000) is a
    // fake, safe way to pretend "this takes 2 real seconds," standing
    // in for something like a real network call.
    await Task.Delay(2000);

    Console.WriteLine("Finished working.");
}
