using Quixotic.Lexography.Tokens;
using Quixotic.Parsing;
using Quixotic.Parsing.Expressions;
using Quixotic.Parsing.Operations;
using Quixotic.Parsing.Statements;
using QuixoticLang.Lexer;

namespace Quixotic.ParsingTests
{
    [TestClass]
    public sealed class ParserTests
    {
        [TestMethod]
        public void Parse_print_statement()
        {
            // Setup
            List<Token> tokens = [
                new Token { Type = TokenType.Print, Value = "print", Position = new Position(),},
                new Token { Type = TokenType.StringLiteral, Value = "Hello, windmill!", Position = new Position(),},
                new Token { Type = TokenType.Eof, Value = string.Empty, Position = new Position(),},
            ];
            var parser = new Parser(tokens);

            // Execute 
            var statements = parser.Parse().ToList();

            // Assert
            Assert.HasCount(1, statements);

            var printStatement = Assert.IsInstanceOfType<PrintStatement>(statements[0]);

            var stringLiteralExpression = Assert.IsInstanceOfType<StringLiteralExpression>(printStatement.Expression);

            Assert.AreEqual("Hello, windmill!", stringLiteralExpression.Value);
        }

        [TestMethod]
        public void Parse_print_statement_with_lexer()
        {
            // Setup
            var source = @"
                print ""Hello, windmill!""
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute 
            var statements = parser.Parse().ToList();

            // Assert
            Assert.HasCount(1, statements);

            var printStatement = Assert.IsInstanceOfType<PrintStatement>(statements[0]);

            var stringLiteralExpression = Assert.IsInstanceOfType<StringLiteralExpression>(printStatement.Expression);

            Assert.AreEqual("Hello, windmill!", stringLiteralExpression.Value);
        }

        [TestMethod]
        public void Parse_print_statement_with_number()
        {
            // Setup
            var source = @"
                print 262144
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute 
            var statements = parser.Parse().ToList();

            // Assert
            Assert.HasCount(1, statements);

            var printStatement = Assert.IsInstanceOfType<PrintStatement>(statements[0]);

            var numberLiteralExpression = Assert.IsInstanceOfType<NumberLiteralExpression>(printStatement.Expression);

            Assert.AreEqual(262144, numberLiteralExpression.Value);
        }

        [TestMethod]
        public void Parse_print_statement_with_addition_expression()
        {
            // Setup
            var source = @"
                print 262144 + 131072
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute 
            var statements = parser.Parse().ToList();

            // Assert
            Assert.HasCount(1, statements);

            Assert.IsInstanceOfType<PrintStatement>(statements[0]);

            var printStatement = (PrintStatement)statements[0];

            var binaryExpression = Assert.IsInstanceOfType<BinaryExpression>(printStatement.Expression);

            Assert.AreEqual(Operator.Add, binaryExpression.Operator);

            var leftExpression = Assert.IsInstanceOfType<NumberLiteralExpression>(binaryExpression.Left);

            Assert.AreEqual(262144, leftExpression.Value);

            var rightExpression = Assert.IsInstanceOfType<NumberLiteralExpression>(binaryExpression.Right);

            Assert.AreEqual(131072, rightExpression.Value);
        }

        [TestMethod]
        public void Parse_print_statement_with_addition_expression_with_parentheses()
        {
            // Setup
            var source = @"
                print 262144 + (131072 / 2)
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute 
            var statements = parser.Parse().ToList();

            // Assert
            Assert.HasCount(1, statements);

            Assert.IsInstanceOfType<PrintStatement>(statements[0]);

            var printStatement = (PrintStatement)statements[0];

            var binaryExpression = Assert.IsInstanceOfType<BinaryExpression>(printStatement.Expression);

            Assert.AreEqual(Operator.Add, binaryExpression.Operator);

            var leftExpression = Assert.IsInstanceOfType<NumberLiteralExpression>(binaryExpression.Left);

            Assert.AreEqual(262144, leftExpression.Value);

            var rightExpression = Assert.IsInstanceOfType<BinaryExpression>(binaryExpression.Right);

            Assert.AreEqual(Operator.Divide, rightExpression.Operator);

            var rightLeftExpression = Assert.IsInstanceOfType<NumberLiteralExpression>(rightExpression.Left);

            Assert.AreEqual(131072, rightLeftExpression.Value);

            var rightRightExpression = Assert.IsInstanceOfType<NumberLiteralExpression>(rightExpression.Right);

            Assert.AreEqual(2, rightRightExpression.Value);
        }
    }
}
