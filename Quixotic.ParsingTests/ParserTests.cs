using Quixotic.Lexography.Tokens;
using Quixotic.Parsing;
using Quixotic.Parsing.Exceptions;
using Quixotic.Parsing.Expressions;
using Quixotic.Parsing.Statements;
using Quixotic.ParsingTests.TestModels;
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

            TestBinaryExpression testBinaryExpression = new(262144, '+', 131072);

            testBinaryExpression.Assert(binaryExpression);
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

            TestBinaryExpression testBinaryExpression = new(262144, '+', (131072, '/', 2));

            testBinaryExpression.Assert(binaryExpression);
        }

        [TestMethod]
        public void Parse_print_statement_where_parentheses_override_precedence()
        {
            // Setup
            var source = @"
                print (1 + 2) * 3
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

            TestBinaryExpression testBinaryExpression = new((1, '+', 2), '*', 3);

            testBinaryExpression.Assert(binaryExpression);
        }

        [TestMethod]
        public void Parse_print_statement_with_arithmetic_series()
        {
            // Setup
            var source = @"
                print 262144 * 131072 + 2
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

            TestBinaryExpression testBinaryExpression = new((262144, '*', 131072), '+', 2);

            testBinaryExpression.Assert(binaryExpression);

        }

        [TestMethod]
        public void Parse_print_statement_with_multi_tier_parenthetical_series()
        {
            // Setup
            var source = @"
                print ((1 + 2) * 4) + (17 + (7 + (4 + 2)))
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

            TestBinaryExpression testBinaryExpression = new(((1, '+', 2), '*', 4), '+', (17, '+', (7, '+', (4, '+', 2))));

            testBinaryExpression.Assert(binaryExpression);
        }

        [TestMethod]
        public void Parse_print_statement_with_missing_closing_parenthesis()
        {
            // Setup
            var source = @"
                print (1 + 2
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute & Assert
            var exception = Assert.Throws<ExpectedTokenException>(() => parser.Parse().ToList());

            Assert.AreEqual(TokenType.NewLine, exception.Token.Type);
        }

        [TestMethod]
        public void Parse_print_statement_with_unexpected_closing_parenthesis()
        {
            // Setup
            var source = @"
                print 1 + 2)
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute & Assert
            var exception = Assert.Throws<UnexpectedTokenException>(() => parser.Parse().ToList());

            Assert.AreEqual(TokenType.RightParen, exception.Token.Type);
            Assert.AreEqual(")", exception.Token.Value);
        }

        [TestMethod]
        public void Parse_print_statement_with_empty_parentheses()
        {
            // Setup
            var source = @"
                print ()
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute & Assert
            var exception = Assert.Throws<UnexpectedTokenException>(() => parser.Parse().ToList());

            Assert.AreEqual(TokenType.RightParen, exception.Token.Type);
            Assert.AreEqual(")", exception.Token.Value);
        }

        [TestMethod]
        public void Parse_multiple_print_statements()
        {
            // Setup
            var source = @"
                print ""Hello, first windmill!""
                print ""Hello, second windmill!""
                print ""Hello, third windmill!""
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute
            var statements = parser.Parse().ToList();

            // Assert 
            Assert.HasCount(3, statements);

            var printStatement1 = Assert.IsInstanceOfType<PrintStatement>(statements[0]);
            TestExpression.Create("Hello, first windmill!").Assert(printStatement1.Expression);

            var printStatement2 = Assert.IsInstanceOfType<PrintStatement>(statements[1]);
            TestExpression.Create("Hello, second windmill!").Assert(printStatement2.Expression);

            var printStatement3 = Assert.IsInstanceOfType<PrintStatement>(statements[2]);
            TestExpression.Create("Hello, third windmill!").Assert(printStatement3.Expression);
        }
    }
}
