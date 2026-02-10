using System; // Basic language types used by the program.

namespace Day5 // Logical grouping for this demo.
{
    public class Resource // Simple class used to demonstrate GC.
    {
        public string Name { get; set; } // Stores the resource name.

        public Resource(string name) // Constructor runs when object is created.
        {
            Name = name; // Store the provided name.
            // Print a message so we can see object creation.
            Console.WriteLine($"{Name} created");
        }

        // Destructor (Finalizer)
        ~Resource() // Runs when the GC reclaims this object.
        {
            // Print a message when the object is collected.
            Console.WriteLine($"{Name} destroyed by GC");
        }
    }
}
