using EquationAnalyzer.EquationParts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationAnalyzer
{
    internal class Program
    {
        private static char[] doubleChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.' };

        private static void Main(string[] args)
        {
            Console.WriteLine("Enter a Term");
            Console.Write("  > ");
            string term = Console.ReadLine();
            //Term scannedTerm = readTerm(term);
            int offset = 0;
            Console.WriteLine(readDouble(term, ref offset));
            offset += 4;
            Console.WriteLine(readDouble(term, ref offset));
            Console.ReadLine();
        }

        private static string readDouble(string str, ref int offset)
        {
            string doubleString = "";
            for (; offset < str.Length; offset++)
            {
                if (!doubleChars.Contains(str[offset]))
                {
                    break;
                }
                doubleString += str[offset];
            }
            return doubleString;
        }

        private static Term readTerm(string termString, ref int offset)
        {
            Term term = new Term();

            while (offset < termString.Length)
            {
                offset++;

                if (termString[offset] == '(')
                {
                    readTerm(termString, ref offset);
                }
                else if (termString[offset] == ')')
                {
                    break;
                }
            }

            return term;
        }
    }
}