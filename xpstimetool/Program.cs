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
        public static bool run = true;

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

        static void PressAnyKey(String msg = "Press any key to continue. ")
        {
            Console.Write(msg);
            Console.ReadKey();
            Console.WriteLine();
        }

        static void OptionChooser()
        {
            Console.WriteLine();
            Console.WriteLine("Please choose an option.");
            Console.WriteLine("1) Show the Date.");
            Console.WriteLine("0) Exit.");
            Console.WriteLine();
            Console.Write(": ");
            ConsoleKey _k = Console.ReadKey().Key;
            Console.WriteLine();
            Console.WriteLine();
            switch (_k)
            {
                case ConsoleKey.D0:
                    run = false;
                    break;

                case ConsoleKey.D1:
                    int _days = 365;
                    if (DateTime.IsLeapYear(DateTime.Now.Year))
                        _days = 366;

                    WriteColorLine("Current Date and Time:", ConsoleColor.Blue);
                    Console.WriteLine(DateTime.Now.ToString());
                    WriteColorLine("Remaining days in year:", ConsoleColor.Blue);
                    Console.WriteLine($"{_days - DateTime.Now.DayOfYear} days left. (Day {DateTime.Now.DayOfYear})");
                    WriteColorLine("Leap year:", ConsoleColor.Blue);
                    Console.WriteLine($"{DateTime.IsLeapYear(DateTime.Now.Year).ToString()} ({_days.ToString()} days)");
                    Console.WriteLine();
                    PressAnyKey();
                    break;

                default:
                    WriteColor('\r' + "Please choose a valid option.", ConsoleColor.Red);
                    break;
            }
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
            while (run)
                OptionChooser();
        }
    }
}
