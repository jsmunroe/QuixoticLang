using Quixotic.Common.Exceptions.Parsing;
using Quixotic.Common.Expressions;
using Quixotic.Common.Operations;
using Quixotic.Common.Statements;
using Quixotic.Common.Tokens;
using Quixotic.Parsing;
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
                new Token { Type = TokenType.Print, Value = "print", Span = Span.Empty },
                new Token { Type = TokenType.StringLiteral, Value = "Hello, windmill!", Span = Span.Empty },
                new Token { Type = TokenType.Eof, Value = string.Empty, Span = Span.Empty,},
            ];
            var parser = new Parser(tokens);

            // Execute 
            var statements = parser.Parse().ToList();

            // Assert
            Assert.HasCount(1, statements);

            AssertPrint(statements[0], "Hello, windmill!");
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

            AssertPrint(statements[0], "Hello, windmill!");
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

            AssertPrint(statements[0], 262144);
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

            AssertPrint(statements[0], (262144, "+", 131072));
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

            AssertPrint(statements[0], (262144, "+", (131072, "/", 2)));
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

            AssertPrint(statements[0], ((1, "+", 2), "*", 3));
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

            AssertPrint(statements[0], ((262144, "*", 131072), "+", 2));
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

            AssertPrint(statements[0], (((1, "+", 2), "*", 4), "+", (17, "+", (7, "+", (4, "+", 2)))));
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
            var exception = Assert.Throws<UnexpectedTokenException>(() => parser.Parse().ToList());

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

            Assert.AreEqual(TokenType.CloseParen, exception.Token.Type);
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

            Assert.AreEqual(TokenType.CloseParen, exception.Token.Type);
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

            AssertPrint(statements[0], "Hello, first windmill!");
            AssertPrint(statements[1], "Hello, second windmill!");
            AssertPrint(statements[2], "Hello, third windmill!");
        }

        [TestMethod]
        public void Parse_identifier_statement()
        {
            // NOTE: The interpreter cannot execute the code in this test in isolation because 'windmills' is not properly defined.

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
            // NOTE: The interpreter cannot execute the code in this test in isolation because 'windmills' is not properly defined.

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
            // NOTE: The interpreter cannot execute the code in this test in isolation because 'windmills' is not properly defined.

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
        public void Parse_identifier_statement_with_from_other_identifier()
        {
            // Setup
            var source = @"
                let windmills := 5
                let w := windmills
                print w
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute
            var statements = parser.Parse().ToList();

            // Assert 
            Assert.HasCount(3, statements);

            AssertVariableDeclaration(statements[0], "windmills", 5);
            AssertVariableDeclaration(statements[1], "w", new TestIdentifierExpression("windmills"));
            AssertPrint(statements[2], new TestIdentifierExpression("w"));
        }

        [TestMethod]
        public void Parse_unary_subtract_expression()
        {
            // NOTE: The interpreter cannot execute the code in this test in isolation because 'cost' is not properly defined.

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

            AssertAssignment(statements[0], "cost", -5);
        }

        [TestMethod]
        public void Parse_unary_subtract_expression_in_exrpression_series()
        {
            // NOTE: The interpreter cannot execute the code in this test in isolation because 'cost' is not properly defined.

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

            AssertAssignment(statements[0], "cost", (-5, "+", (2, "*", (3, "-", 1))));
        }

        [TestMethod]
        public void Parse_if_statement()
        {
            // NOTE: The interpreter cannot execute the code in this test in isolation because 'c' is not properly defined.

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

            AssertAssignment(statements[0], "c", 5);

            AssertIf(statements[1], ("[c]", ">", 3),
                thenBlock: block =>
                {
                    AssertPrint(block[0], "c is greater than 3");
                });
        }

        [TestMethod]
        public void Parse_if_statement_with_else()
        {
            // NOTE: The interpreter cannot execute the code in this test in isolation because 'c' is not properly defined.

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
            AssertAssignment(statements[0], "c", 5);

            AssertIf(statements[1], ("[c]", ">", 3),
                thenBlock: block =>
                {
                    AssertPrint(block[0], "c is greater than 3");
                },
                elseBlock: block =>
                {
                    AssertPrint(block[0], "c is less than or equal to 3");
                });
        }


        [TestMethod]
        public void Parse_if_statement_with_else_if()
        {
            // NOTE: The interpreter cannot execute the code in this test in isolation because 'c' is not properly defined.

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

            AssertAssignment(statements[0], "c", 5);

            AssertIf(statements[1], (("[c]", ">", 3), "and", ("[c]", "<", 10)),
                thenBlock: block =>
                {
                    AssertPrint(block[0], "c is greater than 3");
                },
                elseIfBlock: (block, i) =>
                {
                    AssertPrint(block[0], "c is greater than 1");
                },
                elseBlock: block =>
                {
                    AssertPrint(block[0], "I can see clearly now the rain is gone.");
                    AssertPrint(block[1], "I can see all obsticles in my way.");
                    AssertPrint(block[2], "Gone are the dark clouds that had my blind.");
                    AssertPrint(block[3], "It's gonna be a bright, bright, bright sunshiney day!");
                });

        }

        [TestMethod]
        public void Parse_function_declaration_statement()
        {
            // Setup
            var source = @"
                function sayHello
                    print ""Hello, windmills!""
                end function
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute
            var statements = parser.Parse().ToList();

            // Assert
            Assert.HasCount(1, statements);

            AssertFunctionDeclaration(statements[0], "sayHello",
                body: block =>
                {
                    AssertPrint(block[0], "Hello, windmills!");
                });
        }

        [TestMethod]
        public void Parse_function_declaration_statement_with_call()
        {
            // Setup
            var source = @"
                function sayHello
                    print ""Hello, windmills!""
                end function

                sayHello()
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute
            var statements = parser.Parse().ToList();

            // Assert
            Assert.HasCount(2, statements);
            AssertFunctionDeclaration(statements[0], "sayHello", body =>
            {
                AssertPrint(body[0], "Hello, windmills!");
            });

            AssertFunctionCall(statements[1], "sayHello");
        }


        [TestMethod]
        public void Parse_function_call_with_return_value()
        {
            // Setup
            var source = @"
                function sayHello
                    return ""Hello, windmills!""
                end function

                let hello := sayHello()
                print hello
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute
            var statements = parser.Parse().ToList();

            // Assert
            Assert.HasCount(3, statements);

            AssertFunctionDeclaration(statements[0], "sayHello", body =>
            {
                AssertReturn(body[0], "Hello, windmills!");
            });

            AssertVariableDeclaration(statements[1], "hello", new TestFunctionCallExpression("sayHello"));

            AssertPrint(statements[2], new TestIdentifierExpression("hello"));
        }

        [TestMethod]
        public void Parse_do_while_loop()
        {
            // Setup
            var source = @"
                let i := 0

                do while i < 10
                    print i
                    i := i + 1
                loop
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute
            var statements = parser.Parse().ToList();

            // Assert
            Assert.HasCount(2, statements);

            AssertVariableDeclaration(statements[0], "i", 0);

            AssertDo(statements[1], ("[i]", "<", 10), isEntryControl: true,
                block: block =>
                {
                    AssertPrint(block[0], new TestIdentifierExpression("i"));
                    AssertAssignment(block[1], "i", ("[i]", "+", 1.0));
                });
        }

        [TestMethod]
        public void Parse_do_until_loop()
        {
            // Setup
            var source = @"
                let i := 0

                do until i >= 10
                    print i
                    i := i + 1
                loop
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute
            var statements = parser.Parse().ToList();

            // Assert
            Assert.HasCount(2, statements);

            AssertVariableDeclaration(statements[0], "i", 0);

            AssertDo(statements[1], new TestUnaryExpression(Common.Operations.Operator.Not, ("[i]", ">=", 10)), isEntryControl: true,
                block: block =>
                {
                    AssertPrint(block[0], new TestIdentifierExpression("i"));
                    AssertAssignment(block[1], "i", ("[i]", "+", 1.0));
                });
        }

        [TestMethod]
        public void Parse_do_loop_while()
        {
            // Setup
            var source = @"
                let i := 0

                do
                    print i
                    i := i + 1
                loop while i < 10
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute
            var statements = parser.Parse().ToList();

            // Assert
            Assert.HasCount(2, statements);

            AssertVariableDeclaration(statements[0], "i", 0);

            AssertDo(statements[1], ("[i]", "<", 10), isEntryControl: false,
                block: block =>
                {
                    AssertPrint(block[0], new TestIdentifierExpression("i"));
                    AssertAssignment(block[1], "i", ("[i]", "+", 1.0));
                });
        }

        [TestMethod]
        public void Parse_do_loop_until()
        {
            // Setup
            var source = @"
                let i := 0

                do
                    print i
                    i := i + 1
                loop until i >= 10
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute
            var statements = parser.Parse().ToList();

            // Assert
            Assert.HasCount(2, statements);

            AssertVariableDeclaration(statements[0], "i", 0);

            AssertDo(statements[1], new TestUnaryExpression(Common.Operations.Operator.Not, ("[i]", ">=", 10)), isEntryControl: false,
                block: block =>
                {
                    AssertPrint(block[0], new TestIdentifierExpression("i"));
                    AssertAssignment(block[1], "i", ("[i]", "+", 1.0));
                });
        }

        [TestMethod]
        public void Parse_assignment_with_boolean_literal_true()
        {
            // Setup
            var source = @"
                let isAGiant := true

                if isAGiant then
                    print ""Attack!""
                end if
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute
            var statements = parser.Parse().ToList();

            // Assert
            Assert.HasCount(2, statements);

            AssertVariableDeclaration(statements[0], "isAGiant", true);

            AssertIf(statements[1], new TestIdentifierExpression("isAGiant"),
                thenBlock: block =>
                {
                    AssertPrint(block[0], "Attack!");
                });
        }

        [TestMethod]
        public void Parse_assignment_with_boolean_literal_false()
        {
            // Setup
            var source = @"
                let isAWindmill := false

                if not isAWindmill then
                    print ""Attack!""
                end if
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute
            var statements = parser.Parse().ToList();

            // Assert
            Assert.HasCount(2, statements);

            AssertVariableDeclaration(statements[0], "isAWindmill", false);

            AssertIf(statements[1], new TestUnaryExpression(Operator.Not, new TestIdentifierExpression("isAWindmill")),
                thenBlock: block =>
                {
                    AssertPrint(block[0], "Attack!");
                });
        }

        [TestMethod]
        public void Parse_do_loop_with_break()
        {
            // Setup
            var source = @"
                do while true
                    break
                loop
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute
            var statements = parser.Parse().ToList();

            // Assert
            Assert.HasCount(1, statements);

            AssertDo(statements[0], true, isEntryControl: true,
                block: block =>
                {
                    AssertBreak(block[0]);
                });
        }

        [TestMethod]
        public void Parse_do_loop_with_continue()
        {
            // Setup
            var source = @"
                do while true
                    continue
                loop
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute
            var statements = parser.Parse().ToList();

            // Assert
            Assert.HasCount(1, statements);

            AssertDo(statements[0], true, isEntryControl: true,
                block: block =>
                {
                    AssertContinue(block[0]);
                });
        }

        [TestMethod]
        public void Parse_for_loop()
        {
            // Setup
            var source = @"
                for i := 0 to 10
                    print i
                next
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute
            var statements = parser.Parse().ToList();

            // Assert
            Assert.HasCount(1, statements);

            AssertFor(statements[0],
                iterator: "i",
                from: 0,
                to: 10,
                block: block =>
                {
                    AssertPrint(block[0], "[i]");
                });
        }

        [TestMethod]
        public void Parse_for_loop_to_compute_pi()
        {
            // Setup
            var source = @"
                let denominator := 1
                let piOverFour := 0
                let sign := 1

                for i := 0 to 1000
                    piOverFour := piOverFour + 1/denominator

                    print piOverFour * 4

                    denominator := denominator + 2
                    sign := sign * -1
                next
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute
            var statements = parser.Parse().ToList();
        }

        [TestMethod]
        public void Parse_for_loop_with_comments()
        {
            // Setup
            var source = @"
                ' This is a for loop
                for i := 0 to 10 ' from 1 to 10
                    print i ' printe each number
                    ' this is a comment inside the for loop block!
                next ' end the loop!
                ' so long '''
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            // Execute
            var statements = parser.Parse().ToList();

            // Assert
            Assert.HasCount(1, statements);

            AssertFor(statements[0],
                iterator: "i",
                from: 0,
                to: 10,
                block: block =>
                {
                    AssertPrint(block[0], "[i]");
                });
        }

        private QxVariableDeclarationStatement AssertVariableDeclaration(QxStatement statement, string name, TestExpression? expression = null)
        {
            var variableDeclarationStatement = Assert.IsInstanceOfType<QxVariableDeclarationStatement>(statement);

            Assert.AreEqual(name, variableDeclarationStatement.Name);

            if (expression is not null)
            {
                Assert.IsNotNull(variableDeclarationStatement.Value);
                expression.Assert(variableDeclarationStatement.Value);
            }

            return variableDeclarationStatement;
        }

        private QxFunctionDeclarationStatement AssertFunctionDeclaration(QxStatement statement, string name, Action<Block>? body = null)
        {
            var functionStatement = Assert.IsInstanceOfType<QxFunctionDeclarationStatement>(statement);

            Assert.AreEqual(name, functionStatement.Name);

            if (body is not null)
                body(functionStatement.Body);

            return functionStatement;
        }

        private QxIfStatement AssertIf(QxStatement statement, TestExpression? condition = null, Action<Block>? thenBlock = null, Action<Block, int>? elseIfBlock = null, Action<Block>? elseBlock = null)
        {
            var ifStatement = Assert.IsInstanceOfType<QxIfStatement>(statement);

            if (condition is not null)
                condition.Assert(ifStatement.Condition);

            if (thenBlock is not null)
                thenBlock(ifStatement.ThenBlock);

            if (elseIfBlock is not null)
            {
                for (var i = 0; i < ifStatement.ElseIfClauses.Count; i++)
                {
                    var elseIfClause = ifStatement.ElseIfClauses[i];
                    elseIfBlock(elseIfClause.Block, i);
                }
            }

            if (elseBlock is not null)
                elseBlock(ifStatement.ElseBlock);

            return ifStatement;
        }

        private QxForStatement AssertFor(QxStatement statement, string? iterator = null, TestExpression? from = null, TestExpression? to = null, TestExpression? step = null, Action<Block>? block = null)
        {
            var forStatement = Assert.IsInstanceOfType<QxForStatement>(statement);

            if (iterator is not null)
                Assert.AreEqual(iterator, forStatement.Iterator.Name);

            if (from is not null)
                from.Assert(forStatement.From);

            if (to is not null)
                to.Assert(forStatement.To);

            if (step is not null)
            {
                Assert.IsNotNull(forStatement.Step);
                step.Assert(forStatement.Step);
            }

            if (block is not null)
                block(forStatement.Block);

            return forStatement;

        }

        private QxDoStatement AssertDo(QxStatement statement, TestExpression? condition = null, bool? isEntryControl = null, Action<Block>? block = null)
        {
            var doStatement = Assert.IsInstanceOfType<QxDoStatement>(statement);

            if (condition is not null)
                condition.Assert(doStatement.Condition);

            if (isEntryControl is not null)
                Assert.AreEqual(isEntryControl, doStatement.EntryControlled);

            if (block is not null)
                block(doStatement.Block);

            return doStatement;
        }

        private QxFunctionCallStatement AssertFunctionCall(QxStatement statement, string name)
        {
            var functionCallStatement = Assert.IsInstanceOfType<QxFunctionCallStatement>(statement);

            Assert.AreEqual(name, functionCallStatement.Name);

            return functionCallStatement;
        }

        private QxAssignmentStatement AssertAssignment(QxStatement statement, string name, TestExpression? expression = null)
        {
            var assignmentStatement = Assert.IsInstanceOfType<QxAssignmentStatement>(statement);

            var identifierExpression = Assert.IsInstanceOfType<QxIdentifierExpression>(assignmentStatement.Target);

            Assert.AreEqual(name, identifierExpression.Name);

            if (expression is not null)
                expression.Assert(assignmentStatement.Value);

            return assignmentStatement;
        }

        private QxPrintStatement AssertPrint(QxStatement statement, TestExpression expression)
        {
            var printStatement = Assert.IsInstanceOfType<QxPrintStatement>(statement);

            expression.Assert(printStatement.Expression);

            return printStatement;
        }

        private QxReturnStatement AssertReturn(QxStatement statement, TestExpression? expression = null)
        {
            var returnStatement = Assert.IsInstanceOfType<QxReturnStatement>(statement);

            if (expression is not null)
            {
                Assert.IsNotNull(returnStatement.Expression);
                expression.Assert(returnStatement.Expression);
            }

            return returnStatement;
        }

        private QxBreakStatement AssertBreak(QxStatement statement)
        {
            var breakStatement = Assert.IsInstanceOfType<QxBreakStatement>(statement);
            return breakStatement;
        }

        private QxContinueStatement AssertContinue(QxStatement statement)
        {
            var continueStatement = Assert.IsInstanceOfType<QxContinueStatement>(statement);
            return continueStatement;
        }
    }
}
