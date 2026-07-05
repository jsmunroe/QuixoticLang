using Quixotic.Common.Utilities;
using Quixotic.Interpret;
using Quixotic.Interpret.Environment;
using Quixotic.Parsing;

namespace Quixotic.Cli
{
    internal class Program
    {
        private static ConsoleWriter Console { get; } = new();

        static void Main(string[] args)
        {
            var runtime = new Runtime();
            var interpreter = new Interpreter(runtime);

            if (args.Length > 0)
                InterpretFile(interpreter, args[0]);
            else
                RunRepl(interpreter);
        }

        private static void InterpretFile(Interpreter interpreter, string path)
        {
            if (!File.Exists(path))
                Console.WriteError($"File not found. {path}");

            try
            {
                var fileStream = File.OpenRead(path);

                interpreter.Execute(fileStream);
            }
            catch (Exception ex)
            {
                Console.WriteError($"[{ex.GetType().Name}]", ex);
            }
        }

        private static void RunRepl(Interpreter interpreter)
        {
            Console.Clear();
            Console.Write("Quixotic ", ConsoleColor.Cyan);
            Console.WriteLine("v0.1", ConsoleColor.White);
            Console.WriteLine("========", ConsoleColor.White);
            Console.WriteLine();

            while (true)
            {
                Console.Write("> ", ConsoleColor.Cyan);

                var source = Console.ReadLine();

                if (source is null)
                    continue;

                try
                {
                    Execute(interpreter, source);
                }
                catch (Exception ex)
                {
                    Console.WriteError($"[{ex.GetType().Name}]", ex);
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
