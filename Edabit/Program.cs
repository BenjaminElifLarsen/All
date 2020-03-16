using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edabit
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(IsIsogram("char"));
            Console.WriteLine(IsIsogram("Test"));
            Console.WriteLine(IsIsogram("test"));
            Console.WriteLine(IsIsogram("abc"));
            Console.WriteLine(IsIsogram("yrmer"));
            Console.WriteLine(IsIsogram("why"));



            Console.ReadLine();
        }

        static bool IsIsogram(string str)
        {
            str = str.ToLower();
            List<char> usedChar = new List<char>();
            foreach (char chr in str)
            {
                if (usedChar.Count == 0)
                    usedChar.Add(chr);
                else
                {
                    foreach (char chrUsed in usedChar)
                        if (chrUsed == chr)
                            return false;
                    usedChar.Add(chr);
                }

            }


            return true;
        }


    }
}
