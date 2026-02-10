using System;

namespace Day2Assignments
{
    public static class Section12_Loops
    {
        public static void Run()
        {
            Console.WriteLine("Section 12");
            int[] arr = { 1, 2, 3 };
            for (int i = 0; i < arr.Length; i++) Console.Write(arr[i]);
            Console.WriteLine();
            foreach (var item in arr) Console.Write(item);
            Console.WriteLine();
            int i2 = 0;
            while (i2++ < 3) { }
            Console.WriteLine();
        }
    }
}
