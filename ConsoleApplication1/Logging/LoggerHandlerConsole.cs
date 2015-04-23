using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApplication1.Logging
{
    public class LoggerHandlerConsole : ILoggerListener
    {
        public LoggerHandlerConsole()
        { }

        public void LogError(DateTime now, string message, int? ident)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLine("ERR " + now + " ", message, ident);
            Console.ForegroundColor = color;
        }

        public void LogMessage(DateTime now, string message, int? ident)
        {
            WriteLine("MSG " + now + " ", message, ident);
        }

        public void LogWarning(DateTime now, string message, int? ident)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            WriteLine("WRN " + now + " ", message, ident);
            Console.ForegroundColor = color;
        }

        private void WriteLine(string header, string message, int? ident)
        {
            Console.Write(header);

            if (ident.HasValue)
            {
                for (int i = 0; i < ident.Value; i++)
                {
                    Console.Write("    ");
                }
            }

            Console.WriteLine(message);
        }
    }
}
