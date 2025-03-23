using NetworkSniffer.Interfaces;

namespace NetworkSniffer.Loggers
{
    internal class ConsoleLogger : ILogger
    {
        public void Log(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
