using System;
using System.Collections.Generic;

namespace Day2Assignments
{
    public static class Section05_Var
    {
        public static void Run()
        {
            Console.WriteLine("Section 5");
            var x = 10;
            var list = new List<string>();
            list.Add("item");
            Console.WriteLine($"{x}, {list[0]}");
            Console.WriteLine();
        }
    }
}
