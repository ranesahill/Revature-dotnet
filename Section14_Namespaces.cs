using System;
using System.Text;

namespace Day2Assignments
{
    public static class Section14_Namespaces
    {
        public static void Run()
        {
            Console.WriteLine("Section 14");
            var sb = new StringBuilder();
            sb.Append("using System.Text");
            Console.WriteLine(sb.ToString());
            Console.WriteLine();
        }
    }
}
