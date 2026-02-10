using System;
using System.Collections.Generic;
using System.Linq;

namespace Day2Assignments
{
    public static class Section11_Collections
    {
        public static void Run()
        {
            Console.WriteLine("Section 11");
            int[] arr = new int[3] { 1, 2, 3 };
            var list = new List<string> { "a", "b" };
            list.Add("c");
            var evens = arr.Where(i => i % 2 == 0).ToArray();
            Console.WriteLine(string.Join(",", arr));
            Console.WriteLine(string.Join(",", list));
            Console.WriteLine(string.Join(",", evens));
            Console.WriteLine();
        }
    }
}
