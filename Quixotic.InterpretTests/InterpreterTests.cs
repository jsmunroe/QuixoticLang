using Quixotic.Interpret.Values;
using Quixotic.InterpretTests.TestImplementations;
using Quixotic.Parsing;
using QuixoticLang.Lexer;

namespace Quixotic.InterpretTests
{
    [TestClass]
    public sealed class InterpreterTests
    {
        [TestMethod]
        public void Parse_identifier_statement()
        {
            // Setup
            var source = @"
                windmills := 5
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert 
            var windmills = Assert.IsInstanceOfType<NumberValue>(runtime.AllFrames[0]["windmills"]);

            Assert.AreEqual(5, windmills.Value);
        }
    }
}
