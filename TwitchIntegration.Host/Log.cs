using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchIntegration.Host
{
    public static class Log
    {
        private static void Write(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Warning(string message)
        {
            Write(message, ConsoleColor.Yellow);
        }

        public static void Success(string message)
        {
            Write(message, ConsoleColor.Green);
        }

        public static void Error(string message)
        {
            Write(message, ConsoleColor.Red);
        }

        public static void Info(string message)
        {
            Write(message, ConsoleColor.White);
        }

        public static void Status(string message)
        {
            Write(message, ConsoleColor.Cyan);
        }

        public static void Exception(string message)
        {
            Write(message, ConsoleColor.Magenta);
        }
    }
}
