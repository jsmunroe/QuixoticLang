using Quixotic.Common.Diagnostics;
using Quixotic.Common.Diagnostics.Issues;
using Quixotic.Common.Exceptions.Parsing;
using Quixotic.Common.Tokens;
using Quixotic.Parsing;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Quixotic.ParsingTests.Context
{
    [TestClass]
    public class ParseContextTests
    {
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            WriteIndented = true,
            IndentSize = 4
        };

        [TestMethod]
        public void Parse_print_without_expression()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("print");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.Print, diagnostic.StatementType);
            Assert.IsNotNull(diagnostic.Statement);
            Assert.IsNotNull(diagnostic.Activity);
            Assert.AreEqual(ActivityType.Print, diagnostic.Activity.Type);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.NewLine, issue.Encountered.Type);
            Assert.IsNull(issue.Expected);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        [DataRow("print for", ActivityType.Print, TokenType.For, "for", null, false)]
        [DataRow("print if", ActivityType.Print, TokenType.If, "if", null, false)]
        [DataRow("print +", ActivityType.UnaryPlus, TokenType.Eof, "<EOF>", null, true)]
        [DataRow("print (", ActivityType.ParenSet, TokenType.Eof, "<EOF>", null, true)]
        [DataRow("print )", ActivityType.Print, TokenType.CloseParen, ")", null, false)]
        [DataRow("print (5", ActivityType.ParenSet, TokenType.Eof, "<EOF>", TokenType.CloseParen, true)]
        [DataRow("print 5)", ActivityType.ConsumeStatementTerminator, TokenType.CloseParen, ")", TokenType.NewLine, false)]
        [DataRow("print ((5)", ActivityType.ParenSet, TokenType.Eof, "<EOF>", TokenType.CloseParen, true)]
        [DataRow("print 5 +", ActivityType.RightOperand, TokenType.Eof, "<EOF>", null, true)]
        [DataRow("print * 5", ActivityType.Print, TokenType.Multiply, "*", null, false)]
        [DataRow("print 5 + * 4", ActivityType.RightOperand, TokenType.Multiply, "*", null, false)]
        [DataRow("print 5 and or 4", ActivityType.RightOperand, TokenType.Or, "or", null, false)]
        public void Parse_print_with_bad_expression(string source, ActivityType activityType, TokenType encounteredType, string encounteredValue, TokenType? expected, bool isEndOfLine)
        {
            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            Assert.IsNotNull(diagnostic);
            WriteDiagnostic(diagnostic);

            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.Print, diagnostic.StatementType);
            Assert.IsNotNull(diagnostic.Statement);
            Assert.IsNotNull(diagnostic.Activity);
            Assert.AreEqual(activityType, diagnostic.Activity.Type);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(encounteredType, issue.Encountered.Type);
            Assert.AreEqual(encounteredValue, issue.Encountered.Value);
            Assert.AreEqual(expected, issue.Expected);
            Assert.AreEqual(isEndOfLine, diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_let_without_identifier()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("let");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.VariableDeclaration, diagnostic.StatementType);
            Assert.IsNotNull(diagnostic.Statement);
            Assert.IsNotNull(diagnostic.Activity);
            Assert.AreEqual(ActivityType.Identifier, diagnostic.Activity.Type);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.NewLine, issue.Encountered.Type);
            Assert.AreEqual(TokenType.Identifier, issue.Expected);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_let_with_bad_identifier()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("let while");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.VariableDeclaration, diagnostic.StatementType);
            Assert.IsNotNull(diagnostic.Statement);
            Assert.IsNotNull(diagnostic.Activity);
            Assert.AreEqual(ActivityType.Identifier, diagnostic.Activity.Type);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.While, issue.Encountered.Type);
            Assert.AreEqual(TokenType.Identifier, issue.Expected);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_let_with_unexpected_token()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("let i 2");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.VariableDeclaration, diagnostic.StatementType);
            Assert.IsNotNull(diagnostic.Statement);
            Assert.IsNotNull(diagnostic.Activity);
            Assert.AreEqual(ActivityType.ConsumeStatementTerminator, diagnostic.Activity.Type);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.NumberLiteral, issue.Encountered.Type);
            Assert.AreEqual(TokenType.NewLine, issue.Expected); // Parser is expecting statement to end with just a declaration.
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_let_with_bad_assignment()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("let i := to");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.VariableDeclaration, diagnostic.StatementType);
            Assert.IsNotNull(diagnostic.Statement);
            Assert.IsNotNull(diagnostic.Activity);
            Assert.AreEqual(ActivityType.AssignedExpression, diagnostic.Activity.Type);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.To, issue.Encountered.Type);
            Assert.AreEqual("to", issue.Encountered.Value);
            Assert.IsNull(issue.Expected);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_identifier_with_no_assignment()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("i");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<StandaloneExpressionException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.StandaloneExpression, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.None, diagnostic.ActivityType);
            Assert.IsInstanceOfType<StandaloneExpression>(diagnostic.Issue);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_identifier_with_bad_assignment()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("i while");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<StandaloneExpressionException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.StandaloneExpression, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.None, diagnostic.ActivityType);
            Assert.IsInstanceOfType<StandaloneExpression>(diagnostic.Issue);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_assignment_with_no_expression()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("i := ");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.StandaloneExpression, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.RightOperand, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.NewLine, issue.Encountered.Type);
            Assert.IsNull(issue.Expected);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_assignment_with_no_identifier()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine(" := 5");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.StandaloneExpression, diagnostic.StatementType); // No identifier exists to trigger creation of assignment statement.
            Assert.AreEqual(ActivityType.StandaloneExpression, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.Assignment, issue.Encountered.Type);
            Assert.IsNull(issue.Expected);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty); // No statement is created.
        }

        [TestMethod]
        public void Parse_assignment_with_bad_expression()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("i := next");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.StandaloneExpression, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.RightOperand, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.Next, issue.Encountered.Type);
            Assert.AreEqual("next", issue.Encountered.Value);
            Assert.IsNull(issue.Expected);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_assignment_with_bad_expression_2()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("i := +");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.StandaloneExpression, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.UnaryPlus, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.NewLine, issue.Encountered.Type);
            Assert.IsNull(issue.Expected);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_if_with_no_condition()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("if ");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.If, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.IfCondition, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.NewLine, issue.Encountered.Type);
            Assert.IsNull(issue.Expected);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_if_with_bad_condition()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("if return");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.If, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.IfCondition, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.Return, issue.Encountered.Type);
            Assert.AreEqual("return", issue.Encountered.Value);
            Assert.IsNull(issue.Expected);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_if_with_no_body()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("if true");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<IncompleteSourceException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.If, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.IfThenBlock, diagnostic.ActivityType);
            Assert.IsInstanceOfType<IncompleteSource>(diagnostic.Issue);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_if_then_with_no_body()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("if true then");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<IncompleteSourceException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.If, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.IfThenBlock, diagnostic.ActivityType);
            Assert.IsInstanceOfType<IncompleteSource>(diagnostic.Issue);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_if_then_with_no_end_if()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("if true then");
            sourceBuilder.AppendLine("   print \"It's true!\"");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<IncompleteSourceException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.If, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.IfThenBlock, diagnostic.ActivityType);
            Assert.IsInstanceOfType<IncompleteSource>(diagnostic.Issue);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_if_then_with_no_ending_if()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("if true then");
            sourceBuilder.AppendLine("   print \"It's true!\"");
            sourceBuilder.AppendLine("end");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.If, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.None, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.NewLine, issue.Encountered.Type);
            Assert.AreEqual(TokenType.If, issue.Expected);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_do_while_with_no_condition()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("do while");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.Do, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.DoPrecondition, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.NewLine, issue.Encountered.Type);
            Assert.IsNull(issue.Expected);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_do_while_with_bad_condition()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("do while if ");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.Do, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.DoPrecondition, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.If, issue.Encountered.Type);
            Assert.IsNull(issue.Expected);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_do_while_without_loop()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("do while true ");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<IncompleteSourceException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.Do, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.DoBlock, diagnostic.ActivityType);
            Assert.IsInstanceOfType<IncompleteSource>(diagnostic.Issue);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }
        [TestMethod]
        public void Parse_do_until_with_no_condition()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("do until");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.Do, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.DoPrecondition, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.NewLine, issue.Encountered.Type);
            Assert.IsNull(issue.Expected);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_do_until_with_bad_condition()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("do until if ");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.Do, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.DoPrecondition, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.If, issue.Encountered.Type);
            Assert.IsNull(issue.Expected);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_do_until_without_loop()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("do until false ");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<IncompleteSourceException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.Do, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.DoBlock, diagnostic.ActivityType);
            Assert.IsInstanceOfType<IncompleteSource>(diagnostic.Issue);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_do_loop_while_with_no_condition()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("do");
            sourceBuilder.AppendLine("    print \"Abraham Lincoln is cool!\"");
            sourceBuilder.AppendLine("loop while");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.Do, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.DoPostcondition, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.NewLine, issue.Encountered.Type);
            Assert.IsNull(issue.Expected);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_do_loop_until_with_bad_condition()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("do");
            sourceBuilder.AppendLine("    print \"Abraham Lincoln is cool!\"");
            sourceBuilder.AppendLine("loop until print");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.Do, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.DoPostcondition, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.Print, issue.Encountered.Type);
            Assert.AreEqual("print", issue.Encountered.Value);
            Assert.IsNull(issue.Expected);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_do_loop_without_condition()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("do");
            sourceBuilder.AppendLine("    print \"Abraham Lincoln is cool!\"");
            sourceBuilder.AppendLine("loop");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<DoLoopNoConditionException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.Do, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.None, diagnostic.ActivityType);
            Assert.IsInstanceOfType<DoLoopNoCondition>(diagnostic.Issue);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_do_loop_with_dual_condition()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("do while true");
            sourceBuilder.AppendLine("    print \"Abraham Lincoln is cool!\"");
            sourceBuilder.AppendLine("loop until false");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<DoLoopDualConditionException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.Do, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.None, diagnostic.ActivityType);
            Assert.IsInstanceOfType<DoLoopDualCondition>(diagnostic.Issue);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_for_with_no_iterator()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.For, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.None, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.NewLine, issue.Encountered.Type);
            Assert.AreEqual(TokenType.Identifier, issue.Expected);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_for_with_bad_iterator()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for do");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.For, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.None, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.Do, issue.Encountered.Type);
            Assert.AreEqual(TokenType.Identifier, issue.Expected);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_for_with_iterator_without_assignment()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.For, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.FromValue, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.NewLine, issue.Encountered.Type);
            Assert.AreEqual(TokenType.Assignment, issue.Expected);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_for_with_iterator_with_bad_assignment()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i do");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.For, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.FromValue, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.Do, issue.Encountered.Type);
            Assert.AreEqual(TokenType.Assignment, issue.Expected);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }
        [TestMethod]
        public void Parse_for_with_iterator_without_assigned_expression()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i := ");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.For, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.FromValue, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.NewLine, issue.Encountered.Type);
            Assert.IsNull(issue.Expected);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_for_with_iterator_with_bad_assignment_expression()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i := then");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.For, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.FromValue, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.Then, issue.Encountered.Type);
            Assert.IsNull(issue.Expected);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_for_without_to()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i := 7");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.For, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.ToValue, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.NewLine, issue.Encountered.Type);
            Assert.AreEqual(TokenType.To, issue.Expected);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_for_with_bad_to()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i := 7 for ");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.For, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.ToValue, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.For, issue.Encountered.Type);
            Assert.AreEqual(TokenType.To, issue.Expected);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_for_with_to_without_expression()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i := 7 to ");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.For, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.ToValue, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.NewLine, issue.Encountered.Type);
            Assert.IsNull(issue.Expected);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_for_with_to_with_bad_expression()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i := 7 to while ");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.For, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.ToValue, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.While, issue.Encountered.Type);
            Assert.IsNull(issue.Expected);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_for_with_bad_step()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i := 7 to 15 2 ");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<StandaloneExpressionException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.StandaloneExpression, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.ForBlock, diagnostic.ActivityType); // Parser assumes that the "loop" token is part of a new statement in the for's block.
            Assert.IsInstanceOfType<StandaloneExpression>(diagnostic.Issue);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_for_with_from_and_to_without_iterator()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for := 7 to 15");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.For, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.None, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.Assignment, issue.Encountered.Type);
            Assert.AreEqual(TokenType.Identifier, issue.Expected);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_for_with_identifier_and_from_without_to()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i := 1 5");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.For, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.ToValue, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.NumberLiteral, issue.Encountered.Type);
            Assert.AreEqual("5", issue.Encountered.Value);
            Assert.AreEqual(TokenType.To, issue.Expected);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_for_with_identifier_and_from_without_to_expression()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i := 1 to");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.For, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.ToValue, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.NewLine, issue.Encountered.Type);
            Assert.IsNull(issue.Expected);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_for_without_step_expression()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i := 7 to 15 step ");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.For, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.StepValue, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.NewLine, issue.Encountered.Type);
            Assert.IsNull(issue.Expected);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_for_with_bad_step_expression()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i := 7 to 15 step ");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.For, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.StepValue, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.NewLine, issue.Encountered.Type);
            Assert.IsNull(issue.Expected);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_for_without_next()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i := 7 to 15 ");
            sourceBuilder.AppendLine("    print \"New Mexico is the new Arkansas!!!\"");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<IncompleteSourceException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.For, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.ForBlock, diagnostic.ActivityType);
            Assert.IsInstanceOfType<IncompleteSource>(diagnostic.Issue);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_function_declaration_without_function_name()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("function");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.FunctionDeclaration, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.FunctionName, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.NewLine, issue.Encountered.Type);
            Assert.AreEqual(TokenType.Identifier, issue.Expected);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_function_declaration_with_bad_function_name()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("function 2");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.FunctionDeclaration, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.FunctionName, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.NumberLiteral, issue.Encountered.Type);
            Assert.AreEqual(TokenType.Identifier, issue.Expected);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_function_declaration_bad_arguments()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("function foo(bar: number, 2)");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.FunctionDeclaration, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.Parameter, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.NumberLiteral, issue.Encountered.Type);
            Assert.AreEqual(TokenType.Identifier, issue.Expected);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_function_declaration_without_end_function()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("function foo(bar: number)");
            sourceBuilder.AppendLine("    print \"Are rice crispies just freeze-dried rice grains?\"");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<IncompleteSourceException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            Assert.IsNotNull(diagnostic);
            WriteDiagnostic(diagnostic);

            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.FunctionDeclaration, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.FunctionBody, diagnostic.ActivityType);
            Assert.IsInstanceOfType<IncompleteSource>(diagnostic.Issue);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_function_declaration_without_without_parameter_type()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("function foo(bar)");
            sourceBuilder.AppendLine("    print \"Are rice crispies just freeze-dried rice grains?\"");
            sourceBuilder.AppendLine("end function");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            Assert.IsNotNull(diagnostic);
            WriteDiagnostic(diagnostic);

            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.FunctionDeclaration, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.Parameter, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.CloseParen, issue.Encountered.Type);
            Assert.AreEqual(TokenType.Type, issue.Expected);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_function_declaration_missing_first_parameter_name()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("function foo(, bar:number ,ro: number)");
            sourceBuilder.AppendLine("    print \"Are rice crispies just freeze-dried rice grains?\"");
            sourceBuilder.AppendLine("end function");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.FunctionDeclaration, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.Parameter, diagnostic.ActivityType); // Parser believed function had no parameters because it didn't encounter an identifier
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.Comma, issue.Encountered.Type);
            Assert.AreEqual(TokenType.Identifier, issue.Expected);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_function_declaration_missing_middle_parameter_name()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("function foo(bar: number,,ro: number)");
            sourceBuilder.AppendLine("    print \"Are rice crispies just freeze-dried rice grains?\"");
            sourceBuilder.AppendLine("end function");


            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.FunctionDeclaration, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.Parameter, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.Comma, issue.Encountered.Type);
            Assert.AreEqual(TokenType.Identifier, issue.Expected);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_function_declaration_missing_last_parameter_name()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("function foo(bar: number, ro: number, )");
            sourceBuilder.AppendLine("    print \"Are rice crispies just freeze-dried rice grains?\"");
            sourceBuilder.AppendLine("end function");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.FunctionDeclaration, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.Parameter, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.CloseParen, issue.Encountered.Type);
            Assert.AreEqual(TokenType.Identifier, issue.Expected);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_function_call_without_close_paren()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("foo (bar,ro");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.StandaloneExpression, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.Argument, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.NewLine, issue.Encountered.Type);
            Assert.AreEqual(TokenType.Comma, issue.Expected);
            Assert.IsTrue(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_function_call_missing_argument()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("foo(a,)");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.StandaloneExpression, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.Argument, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.CloseParen, issue.Encountered.Type);
            Assert.AreEqual(TokenType.Identifier, issue.Expected);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }

        [TestMethod]
        public void Parse_function_call_missing_comma()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("foo(1 2)");

            var source = sourceBuilder.ToString();

            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            WriteDiagnostic(diagnostic);

            Assert.IsNotNull(diagnostic);
            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(StatementType.StandaloneExpression, diagnostic.StatementType);
            Assert.AreEqual(ActivityType.Argument, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(TokenType.NumberLiteral, issue.Encountered.Type);
            Assert.AreEqual("2", issue.Encountered.Value);
            Assert.AreEqual(TokenType.Comma, issue.Expected);
            Assert.IsFalse(diagnostic.IsEndOfLine);
            Assert.IsFalse(diagnostic.Span.IsEmpty);
        }


        [TestMethod]
        [DataRow("end if", StatementType.StandaloneExpression, ActivityType.StandaloneExpression, TokenType.End, "end", null, false, false)]
        [DataRow("loop", StatementType.StandaloneExpression, ActivityType.StandaloneExpression, TokenType.Loop, "loop", null, false, false)]
        [DataRow("next", StatementType.StandaloneExpression, ActivityType.StandaloneExpression, TokenType.Next, "next", null, false, false)]
        [DataRow("else\r\nprint 5", StatementType.StandaloneExpression, ActivityType.StandaloneExpression, TokenType.Else, "else", null, false, false)]
        [DataRow("else if x", StatementType.StandaloneExpression, ActivityType.StandaloneExpression, TokenType.Else, "else", null, false, false)]
        [DataRow("if x then\r\nelse\r\nelse\r\nend if", StatementType.If, ActivityType.None, TokenType.Else, "else", TokenType.End, false, false)]
        public void Parse_construct_closers_without_construct(string source, StatementType statementType, ActivityType activityType, TokenType encountered, string encounteredValue, TokenType? expected, bool isEndOfLine, bool isSpanEmpty)
        {
            // Execute
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            // Assert
            var diagnostic = exception.Diagnostic;

            Assert.IsNotNull(diagnostic);
            WriteDiagnostic(diagnostic);

            Assert.AreEqual(ContextType.Parsing, diagnostic.ContextType);
            Assert.AreEqual(statementType, diagnostic.StatementType);
            Assert.AreEqual(activityType, diagnostic.ActivityType);
            var issue = Assert.IsInstanceOfType<UnexpectedToken>(diagnostic.Issue);
            Assert.AreEqual(encountered, issue.Encountered.Type);
            Assert.AreEqual(encounteredValue, issue.Encountered.Value);
            Assert.AreEqual(expected, issue.Expected);
            Assert.AreEqual(isEndOfLine, diagnostic.IsEndOfLine);
            Assert.AreEqual(isSpanEmpty, diagnostic.Span.IsEmpty);
        }

        public void WriteDiagnostic(Diagnostic diagnostic)
        {
            var json = JsonSerializer.Serialize(diagnostic, _jsonOptions);

            Console.Write(json);
        }
    }
}
