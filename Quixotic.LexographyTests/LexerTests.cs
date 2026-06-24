using Quixotic.Common.Exceptions.Lexography;
using Quixotic.Common.Tokens;
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
            var result = lexer.Tokenize().ToList();

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
            var result = lexer.Tokenize().ToList();

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
            var ex = Assert.Throws<LexerUnexpectedCharacterException>(() => lexer.Tokenize().ToList());

            Assert.AreEqual('¶', ex.Character);
            Assert.AreEqual(1, ex.Position.Line);
            Assert.AreEqual(7, ex.Position.Column);
        }

        [TestMethod]
        public void Lex_multiple_print_statements()
        {

            // Setup
            var source = @"
                print ""Hello, first windmill!""
                print ""Hello, second windmill!""
                print ""Hello, third windmill!""
            ";

            var lexer = new Lexer(source);

            // Execute
            var tokens = lexer.Tokenize().ToList();

            Assert.HasCount(11, tokens);

            Assert.AreEqual(TokenType.NewLine, tokens[0].Type);

            Assert.AreEqual(TokenType.Print, tokens[1].Type);
            Assert.AreEqual(TokenType.StringLiteral, tokens[2].Type);
            Assert.AreEqual(TokenType.NewLine, tokens[3].Type);

            Assert.AreEqual(TokenType.Print, tokens[4].Type);
            Assert.AreEqual(TokenType.StringLiteral, tokens[5].Type);
            Assert.AreEqual(TokenType.NewLine, tokens[6].Type);

            Assert.AreEqual(TokenType.Print, tokens[7].Type);
            Assert.AreEqual(TokenType.StringLiteral, tokens[8].Type);
            Assert.AreEqual(TokenType.NewLine, tokens[9].Type);

            Assert.AreEqual(TokenType.Eof, tokens[10].Type);
        }

        [TestMethod]
        public void Lex_assignment_operator()
        {
            var lexer = new Lexer("x := 42");

            var tokens = lexer.Tokenize().ToList();

            Assert.AreEqual(TokenType.Identifier, tokens[0].Type);
            Assert.AreEqual(TokenType.Assignment, tokens[1].Type);
            Assert.AreEqual(TokenType.NumberLiteral, tokens[2].Type);
        }
    }

}

