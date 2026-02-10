using System;

namespace Day2Assignments
{
    public static class Section08_Patterns
    {
        public static void Run()
        {
            Console.WriteLine("Section 8");
            object o = 5;
            if (o is int i) Console.WriteLine(i + 1);

            var person = new Person("Zia", 18);
            if (person is { Age: >= 18, Name: var n })
            {
                Console.WriteLine($"Adult: {n}");
            }
            Console.WriteLine();
        }
    }
}
