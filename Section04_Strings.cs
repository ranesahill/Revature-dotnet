using System;
using System.Text;

namespace Day2Assignments
{
    public static class Section04_Strings
    {
        public static void Run()
        {
            Console.WriteLine("Section 4");
            string s = "Hello" + " World";
            string template = $"User: {Environment.UserName}, Date: {DateTime.Today:d}";
            var sb = new StringBuilder();
            sb.Append("Line1").AppendLine();
            sb.AppendFormat("{0} items", 5);
            string result = sb.ToString();
            Console.WriteLine(s);
            Console.WriteLine(template);
            Console.WriteLine(result);
            Console.WriteLine();
        }
    }
}
