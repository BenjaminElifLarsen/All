using System;

namespace Palindrome
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine(PalindromeChecker("boob"));
            Console.WriteLine(PalindromeChecker("Boob"));
            Console.WriteLine(PalindromeChecker("Check"));
            Console.WriteLine(PalindromeChecker("check"));
            Console.WriteLine(PalindromeChecker("reviver"));
            Console.WriteLine(PalindromeChecker("Reviver"));
            Console.ReadLine();
        }



        static bool PalindromeChecker(string str)
        {
            str = str.ToLower();
            string reverseStr = "";
            char[] chars = str.ToCharArray();
            for (int i = str.Length-1; i >= 0; i--)
                reverseStr += chars[i];
            if (reverseStr == str)
                return true;
            else
                return false;
        }

    }


}

