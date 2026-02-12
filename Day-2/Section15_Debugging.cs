using System;
using System.Diagnostics;

namespace Day2Assignments
{
    public static class Section15_Debugging
    {
        public static void Run()
        {
            Console.WriteLine("Section 15");
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 10000; i++) { }
            sw.Stop();
            Console.WriteLine($"Elapsed ms: {sw.ElapsedMilliseconds}");
            Console.WriteLine();
        }
    }
}
