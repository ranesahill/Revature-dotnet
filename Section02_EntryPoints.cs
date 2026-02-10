using System;

namespace Day2Assignments
{
    public static class Section02_EntryPoints
    {
        public static void Run()
        {
            Console.WriteLine("Section 2");
            string[] args = Array.Empty<string>();
            Console.WriteLine(args.Length > 0 ? args[0] : "no args");
            Console.WriteLine();
        }
    }
}
