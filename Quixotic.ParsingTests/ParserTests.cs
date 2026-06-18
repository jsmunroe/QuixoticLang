using Quixotic.Lexography.Tokens;
using Quixotic.Parsing;
using Quixotic.Parsing.Exceptions;
using Quixotic.Parsing.Expressions;
using Quixotic.Parsing.Operations;
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

            var printStatement = Assert.IsInstanceOfType<QxPrintStatement>(statements[0]);

            var stringLiteralExpression = Assert.IsInstanceOfType<QxStringLiteralExpression>(printStatement.Expression);

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

            var printStatement = Assert.IsInstanceOfType<QxPrintStatement>(statements[0]);

            var stringLiteralExpression = Assert.IsInstanceOfType<QxStringLiteralExpression>(printStatement.Expression);

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

            var printStatement = Assert.IsInstanceOfType<QxPrintStatement>(statements[0]);

            var numberLiteralExpression = Assert.IsInstanceOfType<QxNumberLiteralExpression>(printStatement.Expression);

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

            Assert.IsInstanceOfType<QxPrintStatement>(statements[0]);

            var printStatement = (QxPrintStatement)statements[0];

            var binaryExpression = Assert.IsInstanceOfType<QxBinaryExpression>(printStatement.Expression);

            TestBinaryExpression testBinaryExpression = new(262144, "+", 131072);

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

            Assert.IsInstanceOfType<QxPrintStatement>(statements[0]);

            var printStatement = (QxPrintStatement)statements[0];

            var binaryExpression = Assert.IsInstanceOfType<QxBinaryExpression>(printStatement.Expression);

            TestBinaryExpression testBinaryExpression = new(262144, "+", (131072, "/", 2));

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

            Assert.IsInstanceOfType<QxPrintStatement>(statements[0]);

            var printStatement = (QxPrintStatement)statements[0];

            var binaryExpression = Assert.IsInstanceOfType<QxBinaryExpression>(printStatement.Expression);

            TestBinaryExpression testBinaryExpression = new((1, "+", 2), "*", 3);

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

            Assert.IsInstanceOfType<QxPrintStatement>(statements[0]);

            var printStatement = (QxPrintStatement)statements[0];

            var binaryExpression = Assert.IsInstanceOfType<QxBinaryExpression>(printStatement.Expression);

            TestBinaryExpression testBinaryExpression = new((262144, "*", 131072), "+", 2);

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

            Assert.IsInstanceOfType<QxPrintStatement>(statements[0]);

            var printStatement = (QxPrintStatement)statements[0];

            var binaryExpression = Assert.IsInstanceOfType<QxBinaryExpression>(printStatement.Expression);

            TestBinaryExpression testBinaryExpression = new(((1, "+", 2), "*", 4), "+", (17, "+", (7, "+", (4, "+", 2))));

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

            var printStatement1 = Assert.IsInstanceOfType<QxPrintStatement>(statements[0]);
            TestExpression.Create("Hello, first windmill!").Assert(printStatement1.Expression);

            var printStatement2 = Assert.IsInstanceOfType<QxPrintStatement>(statements[1]);
            TestExpression.Create("Hello, second windmill!").Assert(printStatement2.Expression);

            var printStatement3 = Assert.IsInstanceOfType<QxPrintStatement>(statements[2]);
            TestExpression.Create("Hello, third windmill!").Assert(printStatement3.Expression);
        }

        [TestMethod]
        public void Parse_identifier_statement()
        {
            // Setup
            var source = @"
                windmills := 5
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute
            var statements = parser.Parse().ToList();

            // Assert 
            Assert.HasCount(1, statements);

            var assignmentStatement = Assert.IsInstanceOfType<QxAssignmentStatement>(statements[0]);

            var identifierExpression = Assert.IsInstanceOfType<QxIdentifierExpression>(assignmentStatement.Target);

            Assert.AreEqual("windmills", identifierExpression.Name);

            var numberLiteralExpression = Assert.IsInstanceOfType<QxNumberLiteralExpression>(assignmentStatement.Value);

            Assert.AreEqual(5, numberLiteralExpression.Value);
        }

        [TestMethod]
        public void Parse_identifier_statement_with_parenthetical_expression()
        {
            // Setup
            var source = @"
                windmills := (5 + 3) * 2
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute
            var statements = parser.Parse().ToList();

            // Assert 
            Assert.HasCount(1, statements);

            var assignmentStatement = Assert.IsInstanceOfType<QxAssignmentStatement>(statements[0]);

            var identifierExpression = Assert.IsInstanceOfType<QxIdentifierExpression>(assignmentStatement.Target);

            Assert.AreEqual("windmills", identifierExpression.Name);

            TestExpression.Create(((5, "+", 3), "*", 2)).Assert(assignmentStatement.Value);
        }


        [TestMethod]
        public void Parse_identifier_statement_with_print_statement()
        {
            // Setup
            var source = @"
                windmills := 5
                print windmills
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute
            var statements = parser.Parse().ToList();

            // Assert 
            Assert.HasCount(2, statements);

            var assignmentStatement = Assert.IsInstanceOfType<QxAssignmentStatement>(statements[0]);

            var identifierExpression = Assert.IsInstanceOfType<QxIdentifierExpression>(assignmentStatement.Target);

            Assert.AreEqual("windmills", identifierExpression.Name);

            var numberLiteralExpression = Assert.IsInstanceOfType<QxNumberLiteralExpression>(assignmentStatement.Value);

            Assert.AreEqual(5, numberLiteralExpression.Value);

            var printStatement = Assert.IsInstanceOfType<QxPrintStatement>(statements[1]);

            var identifierExpressionInPrint = Assert.IsInstanceOfType<QxIdentifierExpression>(printStatement.Expression);

            Assert.AreEqual("windmills", identifierExpressionInPrint.Name);
        }

        [TestMethod]
        public void Parse_unary_subtract_expression()
        {
            // Setup
            var source = @"
                cost := -5
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute
            var statements = parser.Parse().ToList();

            // Assert 
            Assert.HasCount(1, statements);

            var assignmentStatement = Assert.IsInstanceOfType<QxAssignmentStatement>(statements[0]);

            var identifierExpression = Assert.IsInstanceOfType<QxIdentifierExpression>(assignmentStatement.Target);

            Assert.AreEqual("cost", identifierExpression.Name);

            var unaryExpression = Assert.IsInstanceOfType<QxUnaryExpression>(assignmentStatement.Value);

            Assert.AreEqual(Operator.Subtract, unaryExpression.Operator);

            var numberLiteralExpression = Assert.IsInstanceOfType<QxNumberLiteralExpression>(unaryExpression.Operand);

            Assert.AreEqual(5, numberLiteralExpression.Value);
        }

        [TestMethod]
        public void Parse_unary_subtract_expression_in_exrpression_series()
        {
            // Setup
            var source = @"
                cost := -5 + 2 * (3 - 1)
                print cost
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute
            var statements = parser.Parse().ToList();

            // Assert 
            Assert.HasCount(2, statements);

            var assignmentStatement = Assert.IsInstanceOfType<QxAssignmentStatement>(statements[0]);

            var identifierExpression = Assert.IsInstanceOfType<QxIdentifierExpression>(assignmentStatement.Target);

            Assert.AreEqual("cost", identifierExpression.Name);

            TestExpression.Create((-5, "+", (2, "*", (3, "-", 1)))).Assert(assignmentStatement.Value);
        }

        [TestMethod]
        public void Parse_if_statement()
        {
            // Setup
            var source = @"
                c := 5
                if c > 3
                    print ""c is greater than 3""
                end if
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute
            var statements = parser.Parse().ToList();

            // Assert
            Assert.HasCount(2, statements);

            var assignmentStatement = Assert.IsInstanceOfType<QxAssignmentStatement>(statements[0]);

            var identifierExpression = Assert.IsInstanceOfType<QxIdentifierExpression>(assignmentStatement.Target);

            Assert.AreEqual("c", identifierExpression.Name);

            var numberLiteralExpression = Assert.IsInstanceOfType<QxNumberLiteralExpression>(assignmentStatement.Value);

            Assert.AreEqual(5, numberLiteralExpression.Value);

            var ifStatement = Assert.IsInstanceOfType<QxIfStatement>(statements[1]);

            TestExpression.Create(("[c]", ">", 3)).Assert(ifStatement.Condition);

            var thenStatements = ifStatement.ThenBlock.ToList();

            Assert.HasCount(1, thenStatements);

            var printStatement = Assert.IsInstanceOfType<QxPrintStatement>(thenStatements[0]);

            var stringLiteralExpression = Assert.IsInstanceOfType<QxStringLiteralExpression>(printStatement.Expression);

            Assert.AreEqual("c is greater than 3", stringLiteralExpression.Value);
        }

        [TestMethod]
        public void Parse_if_statement_with_else()
        {
            // Setup
            var source = @"
                c := 5
                if c > 3
                    print ""c is greater than 3""
                else
                    print ""c is less than or equal to 3""
                end if
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute
            var statements = parser.Parse().ToList();

            // Assert
            Assert.HasCount(2, statements);

            var assignmentStatement = Assert.IsInstanceOfType<QxAssignmentStatement>(statements[0]);

            var identifierExpression = Assert.IsInstanceOfType<QxIdentifierExpression>(assignmentStatement.Target);

            Assert.AreEqual("c", identifierExpression.Name);

            var numberLiteralExpression = Assert.IsInstanceOfType<QxNumberLiteralExpression>(assignmentStatement.Value);

            Assert.AreEqual(5, numberLiteralExpression.Value);

            var ifStatement = Assert.IsInstanceOfType<QxIfStatement>(statements[1]);

            TestExpression.Create(("[c]", ">", 3)).Assert(ifStatement.Condition);


            var thenStatements = ifStatement.ThenBlock.ToList();

            Assert.HasCount(1, thenStatements);

            var printStatement = Assert.IsInstanceOfType<QxPrintStatement>(thenStatements[0]);

            var stringLiteralExpression = Assert.IsInstanceOfType<QxStringLiteralExpression>(printStatement.Expression);

            Assert.AreEqual("c is greater than 3", stringLiteralExpression.Value);


            var elseStatements = ifStatement.ElseBlock.ToList();

            Assert.HasCount(1, elseStatements);

            printStatement = Assert.IsInstanceOfType<QxPrintStatement>(elseStatements[0]);

            stringLiteralExpression = Assert.IsInstanceOfType<QxStringLiteralExpression>(printStatement.Expression);

            Assert.AreEqual("c is less than or equal to 3", stringLiteralExpression.Value);
        }


        [TestMethod]
        public void Parse_if_statement_with_else_if()
        {
            // Setup
            var source = @"
                c := 5
                if c > 3 and c < 10 then
                    print ""c is greater than 3""
                else if c > 1
                    print ""c is greater than 1""
                else
                    print ""I can see clearly now the rain is gone.""
                    print ""I can see all obsticles in my way.""
                    print ""Gone are the dark clouds that had my blind.""
                    print ""It's gonna be a bright, bright, bright sunshiney day!""
                end if
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute
            var statements = parser.Parse().ToList();

            // Assert
            Assert.HasCount(2, statements);

            var assignmentStatement = Assert.IsInstanceOfType<QxAssignmentStatement>(statements[0]);

            var identifierExpression = Assert.IsInstanceOfType<QxIdentifierExpression>(assignmentStatement.Target);

            Assert.AreEqual("c", identifierExpression.Name);

            var numberLiteralExpression = Assert.IsInstanceOfType<QxNumberLiteralExpression>(assignmentStatement.Value);

            Assert.AreEqual(5, numberLiteralExpression.Value);

            var ifStatement = Assert.IsInstanceOfType<QxIfStatement>(statements[1]);

            TestExpression.Create((("[c]", ">", 3), "and", ("[c]", "<", 10))).Assert(ifStatement.Condition);


            var thenStatements = ifStatement.ThenBlock.ToList();

            Assert.HasCount(1, thenStatements);

            var printStatement = Assert.IsInstanceOfType<QxPrintStatement>(thenStatements[0]);

            var stringLiteralExpression = Assert.IsInstanceOfType<QxStringLiteralExpression>(printStatement.Expression);

            Assert.AreEqual("c is greater than 3", stringLiteralExpression.Value);


            var elseIfClauses = ifStatement.ElseIfClauses.ToList();

            Assert.HasCount(1, elseIfClauses);

            TestExpression.Create(("[c]", ">", 1)).Assert(elseIfClauses[0].Condition);

            var elseIfBlockStatements = elseIfClauses[0].Block.ToList();

            Assert.HasCount(1, elseIfBlockStatements);

            printStatement = Assert.IsInstanceOfType<QxPrintStatement>(elseIfBlockStatements[0]);

            stringLiteralExpression = Assert.IsInstanceOfType<QxStringLiteralExpression>(printStatement.Expression);

            Assert.AreEqual("c is greater than 1", stringLiteralExpression.Value);


            var elseStatements = ifStatement.ElseBlock.ToList();

            Assert.HasCount(4, elseStatements);

            printStatement = Assert.IsInstanceOfType<QxPrintStatement>(elseStatements[0]);

            stringLiteralExpression = Assert.IsInstanceOfType<QxStringLiteralExpression>(printStatement.Expression);

            Assert.AreEqual("I can see clearly now the rain is gone.", stringLiteralExpression.Value);

            printStatement = Assert.IsInstanceOfType<QxPrintStatement>(elseStatements[1]);

            stringLiteralExpression = Assert.IsInstanceOfType<QxStringLiteralExpression>(printStatement.Expression);

            Assert.AreEqual("I can see all obsticles in my way.", stringLiteralExpression.Value);

            printStatement = Assert.IsInstanceOfType<QxPrintStatement>(elseStatements[2]);

            stringLiteralExpression = Assert.IsInstanceOfType<QxStringLiteralExpression>(printStatement.Expression);

            Assert.AreEqual("Gone are the dark clouds that had my blind.", stringLiteralExpression.Value);

            printStatement = Assert.IsInstanceOfType<QxPrintStatement>(elseStatements[3]);

            stringLiteralExpression = Assert.IsInstanceOfType<QxStringLiteralExpression>(printStatement.Expression);

            Assert.AreEqual("It's gonna be a bright, bright, bright sunshiney day!", stringLiteralExpression.Value);
        }

    }
}
