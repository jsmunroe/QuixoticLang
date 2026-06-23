namespace Quixotic.Cli
{
    public class ConsoleWriter
    {
        public void Clear() => Console.Clear();

        public void Write(object text, ConsoleColor foreground = ConsoleColor.Gray)
        {
            Console.ForegroundColor = foreground;
            Console.Write(text);
            Console.ResetColor();
        }

        public void WriteLine(object text, ConsoleColor foreground = ConsoleColor.Gray)
        {
            Console.ForegroundColor = foreground;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public void WriteLine()
        {
            Console.WriteLine();
        }

        public void WriteError(object text, Exception? exception = null, ConsoleColor foreground = ConsoleColor.Gray)
        {
            Console.ForegroundColor = foreground;
            Console.WriteLine(text);

            if (exception is not null)
            {
                Console.WriteLine();
                Console.WriteLine(exception);
                Console.WriteLine();
            }

            Console.ResetColor();
        }

        public string ReadLine(string? prompt = null, ConsoleColor foreground = ConsoleColor.Gray)
        {
            if (prompt is not null)
                Write(prompt, foreground);

            return Console.ReadLine() ?? string.Empty;
        }
    }
}
