using System;

namespace Day3Assignments
{
    public class Logger
    {
        public void Log(string message, int level = 0)
        {
            Console.WriteLine($"[Level {level}] {message}");
        }

        public void Log(string message, string category = "General")
        {
            Console.WriteLine($"[{category}] {message}");
        }

        public void Log(params string[] messages)
        {
            Console.WriteLine("[Batch] " + string.Join(" | ", messages));
        }

        public void Log(string message, params string[] tags)
        {
            Console.WriteLine($"[Tags: {string.Join(",", tags)}] {message}");
        }
    }

    public static class LoggerProgram
    {
        public static void Run()
        {
            var logger = new Logger();
            logger.Log("Hello", level: 2);
            logger.Log("Hello", category: "System");
            logger.Log("A", "B", "C");
            logger.Log("Hello", "tag1", "tag2");

            // If you uncomment this line, it becomes ambiguous:
            // logger.Log(message: "Hello");
        }
    }
}
