namespace NetworkSniffer.Interfaces
{
    internal interface ILogger
    {
        void Log(string message, ConsoleColor color = ConsoleColor.White);
    }
}
