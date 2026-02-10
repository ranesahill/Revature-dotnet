using System;

namespace Day2Assignments
{
    public static class Section07_Conditionals
    {
        public static void Run()
        {
            Console.WriteLine("Section 7");
            int x = -1;
            if (x > 0) Console.WriteLine("pos");
            else if (x < 0) Console.WriteLine("neg");
            else Console.WriteLine("zero");

            string result = x switch
            {
                0 => "zero",
                > 0 => "positive",
                < 0 => "negative"
            };
            Console.WriteLine(result);
            Console.WriteLine();
        }
    }
}
