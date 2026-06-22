using Quixotic.Interpret;
using Quixotic.Interpret.Environment;
using Quixotic.Parsing;

namespace Quixotic.Cli
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // TODO: If given a file path in args, interpet and run the file.

            // Run REPL
            var runtime = new Runtime();
            var interpeter = new Interpreter(runtime);

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Quixotic");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("========");
            Console.WriteLine();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("> ");
                Console.ResetColor();
                var source = Console.ReadLine();

                if (source is null)
                    continue;

                try
                {
                    Execute(interpeter, source);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{ex.GetType().Name}] {ex.Message}");
                    Console.ResetColor();
                }
            }
        }

        private static void Execute(Interpreter interpreter, string source)
        {
            while (!Parser.IsSourceComplete(source))
            {
                Console.Write("+ ");
                var continuation = Console.ReadLine();

                source += "\r\n" + continuation;
            }

            interpreter.Execute(source);
        }
    }
}
