using System;

namespace Day2Assignments
{
    public static class Section09_LogicalOps
    {
        public static void Run()
        {
            Console.WriteLine("Section 9");
            int x = 50;
            if (x > 0 && x < 100) Console.WriteLine("in range");
            bool isReady = false;
            bool hasData = false;
            if (!(isReady || hasData)) Console.WriteLine("not ready");
            Console.WriteLine();
        }
    }
}
