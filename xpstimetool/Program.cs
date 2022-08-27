using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace xpstimetool
{
    internal class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetSystemTime(ref SYSTEMTIME st);

        public static string newDate = null;

        public struct SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
        }

        #region Functions
        static void WriteColor(String str, ConsoleColor? fg = null, ConsoleColor? bg = null)
        {
            if (fg != null)
                Console.ForegroundColor = (ConsoleColor)fg;
            if (bg != null)
                Console.BackgroundColor = (ConsoleColor)bg;
            Console.Write(str);
            Console.ResetColor();
        }

        static void WriteColorLine(String line, ConsoleColor? fg = null , ConsoleColor? bg = null)
        {
            if (fg != null)
                Console.ForegroundColor = (ConsoleColor)fg;
            if (bg != null)
                Console.BackgroundColor = (ConsoleColor)bg;
            Console.WriteLine(line);
            Console.ResetColor();
        }
        #endregion

        static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("========================");
            WriteColor("xp", ConsoleColor.Green); WriteColor("root", ConsoleColor.Red); Console.WriteLine("'s Time Tool");
            Console.WriteLine();
            Console.WriteLine("Simple TUI Application for managing the time");
            WriteColorLine("http://xproot.pw", ConsoleColor.DarkBlue);
            Console.WriteLine("---");
            Console.WriteLine($"Current date is {DateTime.Today.ToLongDateString()}");
            Console.WriteLine("========================");
            Console.WriteLine();
            WriteColorLine("Please choose an option.", ConsoleColor.Red);
            Console.WriteLine("1) Show the Date.");
            Console.WriteLine(Console.ReadKey().ToString());
            while (true)
            {
                Console.Write('\r' + $"{DateTime.Now.ToString()}    ");
                Thread.Sleep(100);
            }
        }
    }
}
