using System;

namespace Day3Assignments
{
    public static class TryFindMaxProgram
    {
        public static void Run()
        {
            int[] numbers = { 3, 9, 2, 15, 6 };

            if (TryFindMax(numbers, out int max))
            {
                Console.WriteLine($"TryFindMax -> max = {max}");
            }

            var (found, max2) = TryFindMaxTuple(numbers);
            Console.WriteLine($"TryFindMaxTuple -> found = {found}, max = {max2}");
        }

        public static bool TryFindMax(int[] numbers, out int max)
        {
            max = 0;
            if (numbers == null || numbers.Length == 0) return false;

            max = numbers[0];
            for (int i = 1; i < numbers.Length; i++)
            {
                if (numbers[i] > max) max = numbers[i];
            }
            return true;
        }

        public static (bool found, int max) TryFindMaxTuple(int[] numbers)
        {
            if (!TryFindMax(numbers, out int max)) return (false, 0);
            return (true, max);
        }
    }
}
