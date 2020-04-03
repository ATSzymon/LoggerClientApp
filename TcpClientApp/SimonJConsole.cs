using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TcpClientApp
{
    public static class SimonJConsole 
    {
        public static void WriteLineInfo(string message)
        {
            Console.WriteLine(message, Console.ForegroundColor = ConsoleColor.Cyan);
        }

        public static void WriteLineDebug(string message)
        {
            Console.WriteLine(message, Console.ForegroundColor = ConsoleColor.White);
        }

        public static void WriteLineWarn(string message)
        {
            Console.WriteLine(message, Console.ForegroundColor = ConsoleColor.Yellow);
        }

        public static void WriteLineErr(string message)
        {
            Console.WriteLine(message, Console.ForegroundColor = ConsoleColor.Red);
        }
    }
}
