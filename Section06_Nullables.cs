using System;

namespace Day2Assignments
{
    public static class Section06_Nullables
    {
        public static void Run()
        {
            Console.WriteLine("Section 6");
            int? n = null;
            int value = n ?? -1;
            string? s = null;
            int? length = s?.Length;
            Console.WriteLine($"value={value}, length={length}");
            Console.WriteLine();
        }
    }
}
