namespace Quixotic.Common.Contracts
{
    public interface IConsoleWriter
    {
        void Clear();
        string ReadLine(string? prompt = null, ConsoleColor foreground = ConsoleColor.Gray);
        void Write(object text, ConsoleColor foreground = ConsoleColor.Gray);
        void WriteError(object text, Exception? exception = null, ConsoleColor foreground = ConsoleColor.Red);
        void WriteLine();
        void WriteLine(object text, ConsoleColor foreground = ConsoleColor.Gray);
        void WriteWarning(object text, ConsoleColor foreground = ConsoleColor.Yellow);
    }
}