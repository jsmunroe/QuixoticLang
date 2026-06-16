using Quixotic.Lexography.Exceptions;
using Quixotic.Lexography.Tokens;
using QuixoticLang.Lexer;

namespace Quixotic.LexographyTests
{
    [TestClass]
    public sealed class LexerTests
    {
        [TestMethod]
        public void Lex_Print_statement()
        {
            // Setup
            var source = "print \"Hello, windmill!\"";
            var lexer = new Lexer(source);

            // Execute
            var result = lexer.Run().ToList();

            // Assert
            Assert.HasCount(3, result);

            Assert.AreEqual(TokenType.Print, result[0].Type);
            Assert.AreEqual("print", result[0].Value);

            Assert.AreEqual(TokenType.StringLiteral, result[1].Type);
            Assert.AreEqual("Hello, windmill!", result[1].Value);

            Assert.AreEqual(TokenType.Eof, result[2].Type);
            Assert.AreEqual(string.Empty, result[2].Value);
        }

        [TestMethod]
        public void Lex_Print_statement_with_escape_quotes()
        {
            // Setup
            var source = @"print ""Hello, \""windmill!\""";
            var lexer = new Lexer(source);

            // Execute
            var result = lexer.Run().ToList();

            // Assert
            Assert.HasCount(3, result);

            Assert.AreEqual(TokenType.Print, result[0].Type);
            Assert.AreEqual("print", result[0].Value);

            Assert.AreEqual(TokenType.StringLiteral, result[1].Type);
            Assert.AreEqual(@"Hello, ""windmill!""", result[1].Value);

            Assert.AreEqual(TokenType.Eof, result[2].Type);
            Assert.AreEqual(string.Empty, result[2].Value);
        }

        [TestMethod]
        public void Lex_Print_statement_with_bad_character()
        {
            // Setup 
            var source = "print ¶Hello, windmill!";
            var lexer = new Lexer(source);

            // Execute & Assert
            var ex = Assert.Throws<LexerUnexpectedCharacterException>(() => lexer.Run().ToList());

            Assert.AreEqual('¶', ex.Character);
            Assert.AreEqual(1, ex.Position.Line);
            Assert.AreEqual(7, ex.Position.Column);
        }
    }

}

