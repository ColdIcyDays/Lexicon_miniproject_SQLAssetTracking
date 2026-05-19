using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexicon_ConsoleWriter
{
    static internal class LexiConsoleWriter
    {
        public static void LexiWriteLine(string aText, ConsoleColor aForegroundColor = ConsoleColor.White, ConsoleColor aBackgroundColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = aForegroundColor;
            Console.BackgroundColor = aBackgroundColor;
            Console.WriteLine(aText);
            Console.ResetColor();
        }

        public static void LexiWrite(string aText, ConsoleColor aForegroundColor = ConsoleColor.White, ConsoleColor aBackgroundColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = aForegroundColor;
            Console.BackgroundColor = aBackgroundColor;
            Console.Write(aText);
            Console.ResetColor();
        }
    }
}
