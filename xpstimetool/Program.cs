using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Globalization;
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

        static bool WarningAreYouSure(String msg = "This action might be destructive.")
        {
            bool _return = false;
            WriteColorLine("WARNING!", ConsoleColor.Red, ConsoleColor.Yellow);
            WriteColorLine(msg, ConsoleColor.Yellow);
            Console.WriteLine("Are you sure you want to continue? (y/N)");
            Console.Write(": ");
            ConsoleKey _k = Console.ReadKey().Key;
            switch (_k)
            {
                case ConsoleKey.Y:
                    WriteColorLine(" Continuing.", ConsoleColor.Yellow);
                    _return = true;
                    break;

                default:
                    WriteColorLine(" Not continuing.", ConsoleColor.Green);
                    break;
            }
            return _return;
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
            Console.WriteLine("--Main Menu--");
            Console.WriteLine("Please choose an option.");
            Console.WriteLine("1) Show the Date.");
            Console.WriteLine("2) Set the Date.");
            Console.WriteLine("3) Get the date from an NTP server.");
            Console.WriteLine("0) Exit.");
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
                    WriteColorLine("UTC Date and Time:", ConsoleColor.Blue);
                    Console.WriteLine(DateTime.UtcNow.ToString());
                    WriteColorLine("Remaining days in year:", ConsoleColor.Blue);
                    Console.WriteLine($"{_days - DateTime.Now.DayOfYear} days left. (Day {DateTime.Now.DayOfYear})");
                    WriteColorLine("Leap year:", ConsoleColor.Blue);
                    Console.WriteLine($"{DateTime.IsLeapYear(DateTime.Now.Year).ToString()} ({_days.ToString()} days)");
                    Console.WriteLine();
                    PressAnyKey();
                    break;

                case ConsoleKey.D2:
                    Console.WriteLine("Do you want to set your own date or query a NTP server?.");
                    Console.WriteLine("1) Set manually.");
                    Console.WriteLine("2) Sync with NTP.");
                    Console.WriteLine("0) Go back.");
                    Console.Write(": ");
                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.D1:
                            Console.WriteLine();
                            
                            break;

                        case ConsoleKey.D2:
                            Console.WriteLine();
                            Console.WriteLine("Do you want to use your own NTP server? (y/N)");
                            Console.Write("(default: time.windows.com): ");
                            switch (Console.ReadKey().Key)
                            {
                                case ConsoleKey.Y:
                                    Console.WriteLine();
                                    Console.WriteLine("Please type your custom NTP server.");
                                    string _server = Console.ReadLine();
                                    if (WarningAreYouSure("After you press Y, this program will query the server and set the system time to whatever it brings back." + Environment.NewLine + "This has the potential to screw up applications and other stuff."))
                                    {
                                        if (_server == null)
                                        {
                                            WriteColor("Server is empty! ", ConsoleColor.Red);
                                            Console.WriteLine("Using default...");
                                            _server = "time.windows.com";
                                        }
                                        Console.WriteLine("Querying...");
                                        DateTime? ntpTime = GetNetworkTime(_server);
                                        if (ntpTime == null)
                                        {
                                            return;
                                        }
                                        WriteColorLine(ntpTime.ToString(), ConsoleColor.Green);
                                    }
                                    break;

                                default:
                                    if (WarningAreYouSure("After you press Y, this program will query the server and set the system time to whatever it brings back." + Environment.NewLine + "This has the potential to screw up applications and other stuff."))
                                    {
                                        Console.WriteLine(" Querying...");
                                        DateTime? _ntpTime = GetNetworkTime();
                                        if (_ntpTime == null)
                                        {
                                            return;
                                        }
                                        WriteColorLine(_ntpTime.ToString(), ConsoleColor.Green);
                                    }
                                    break;
                            }
                            break;

                        default:
                            break;
                    }
                    Console.WriteLine();
                    break;

                case ConsoleKey.D3:
                    Console.WriteLine("Do you want to use your own NTP server? (y/N)");
                    Console.Write("(default: time.windows.com): ");
                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.Y:
                            Console.WriteLine();
                            Console.WriteLine("Please type your custom NTP server.");
                            string _server = Console.ReadLine();
                            if (_server == null)
                            {
                                WriteColor("Server is empty! ", ConsoleColor.Red);
                                Console.WriteLine("Using default...");
                                _server = "time.windows.com";
                            }
                            Console.WriteLine("Querying...");
                            DateTime? ntpTime = GetNetworkTime(_server);
                            if (ntpTime == null)
                            {
                                return;
                            }
                            WriteColorLine(ntpTime.ToString(), ConsoleColor.Green);

                            break;

                        default:
                            Console.WriteLine(" Querying...");
                            DateTime? _ntpTime = GetNetworkTime();
                            if (_ntpTime == null)
                            {
                                return;
                            }
                            WriteColorLine(_ntpTime.ToString(), ConsoleColor.Green);
                            break;
                    }
                    break;

                default:
                    WriteColor('\r' + "Please choose a valid option.", ConsoleColor.Red);
                    break;
            }
        }

        public static DateTime? GetNetworkTime(string ntpServer = "time.windows.com")
        {
            var ntpData = new byte[48];
            ntpData[0] = 0x1B;
            var addresses = Dns.GetHostEntry(ntpServer).AddressList;
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            using (var _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                try
                {
                    _socket.Connect(ipEndPoint);
                    _socket.ReceiveTimeout = 5000;
                    _socket.Send(ntpData);
                    _socket.Receive(ntpData);
                    _socket.Close();
                }
                catch
                {
                    WriteColorLine("An error has occured while connecting to the server.", ConsoleColor.Red);
                    Console.WriteLine("Trying again...");
                    using (var __socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp))
                    {
                        try
                        {
                            __socket.Connect(ipEndPoint);
                            __socket.ReceiveTimeout = 5000;
                            __socket.Send(ntpData);
                            __socket.Receive(ntpData);
                            __socket.Close();
                        }
                        catch
                        {
                            WriteColorLine("An error has occured while connecting to the server.", ConsoleColor.Red);
                            Console.WriteLine("No more tries...");
                            return null;
                        }
                    }
                }
            }
            const byte serverReplyTime = 40;
            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);
            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);
            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
            var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

            return networkDateTime.ToLocalTime();
        }

        static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
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
            Console.WriteLine("Thanks for using my application!" + Environment.NewLine + "Want to give it a spin or add your own contributions? If so then contribute in ");
            WriteColor("https://github.com/xproot/xpstimetool", ConsoleColor.Blue);
        }
    }
}
