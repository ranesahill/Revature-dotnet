using System;

namespace Day3Assignments
{
    public static class StringExtensions
    {
        public static bool IsPalindrome(this string s)
        {
            if (s == null) return false;

            int left = 0;
            int right = s.Length - 1;

            while (left < right)
            {
                char l = char.ToLowerInvariant(s[left]);
                char r = char.ToLowerInvariant(s[right]);

                if (!char.IsLetterOrDigit(l)) { left++; continue; }
                if (!char.IsLetterOrDigit(r)) { right--; continue; }
                if (l != r) return false;

                left++;
                right--;
            }

            return true;
        }
    }

    public static class PalindromeProgram
    {
        public static void Run()
        {
            string text = "Madam, I'm Adam";
            Console.WriteLine($"\"{text}\" IsPalindrome? {text.IsPalindrome()}");

            int multiplier = 2;
            int[] numbers = { 3, 9, 2, 15, 6 };

            int MultiplyAndSum(int[] values)
            {
                int total = 0;
                foreach (var v in values)
                {
                    total += v * multiplier;
                }

                return total;
            }

            int sum = MultiplyAndSum(numbers);
            Console.WriteLine($"MultiplyAndSum (multiplier={multiplier}) -> {sum}");
        }
    }
}
