using System; // Basic language types used by the program.

namespace Day5 // Logical grouping for this demo.
{
    class Program // Entry class for the console app.
    {
        public static void Main() // Program starts executing here.
        {
            // Create two managed objects to demonstrate GC behavior.
            var res1 = new Resource("Res1"); // first object
            var res2 = new Resource("Res2"); // second object

            // Remove the reference to Res1 so it becomes eligible for collection.
            res1 = null;
            // Res2 is still referenced, so it should not be collected yet.

            // Force a garbage collection (only for demonstration).
            GC.Collect();
            GC.WaitForPendingFinalizers(); // Wait for finalizers to complete.

            Console.WriteLine("GC completed"); // End of demo output.
        }
    }
}
