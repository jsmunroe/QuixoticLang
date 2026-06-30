using Quixotic.Analysis.Errors;
using Quixotic.Common.Diagnostics;
using Quixotic.Common.Exceptions.Parsing;
using Quixotic.Parsing;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Quixotic.AnalysisTests.Errors
{
    [TestClass]
    public class ErrorMessageFormatterTests
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

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());
            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected end of line was encountered while parsing the print statement.", result);

        }

        [TestMethod]
        [DataRow("print for", "An unexpected keyword 'for' was encountered while parsing the print statement.")]
        [DataRow("print if", "An unexpected keyword 'if' was encountered while parsing the print statement.")]
        [DataRow("print +", "An unexpected end of line was encountered while parsing the print statement. A unary plus operator (+n) was encountered without an operand.")]
        [DataRow("print (", "An unexpected end of line was encountered while parsing the print statement. An open parenthesis has been left without a matching close parenthesis.")]
        [DataRow("print )", "An unexpected close parenthesis ')' was encountered while parsing the print statement.")]
        [DataRow("print (5", "An unexpected end of line was encountered while parsing the print statement. An open parenthesis has been left without a matching close parenthesis. The parser expected a close parenthesis.")]
        [DataRow("print 5)", "An unexpected close parenthesis ')' was encountered while parsing the statement terminator. The parser expected an end of the line.")]
        [DataRow("print ((5)", "An unexpected end of line was encountered while parsing the print statement. An open parenthesis has been left without a matching close parenthesis. The parser expected a close parenthesis.")]
        [DataRow("print 5 +", "An unexpected end of line was encountered while parsing the print statement. The expression '5 +' may not be finished.")]
        [DataRow("print * 5", "An unexpected operator '*' was encountered while parsing the print statement.")]
        [DataRow("print 5 + * 4", "An unexpected operator '*' was encountered while parsing the print statement. Operator '*' is an invalid operand in the expresssion '5 + *'.")]
        [DataRow("print 5 and or 4", "An unexpected keyword 'or' was encountered while parsing the print statement. Keyword 'or' is an invalid operand in the expresssion '5 and or'.")]
        public void Parse_print_with_bad_expression(string source, string expectedMessage)
        {
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual(expectedMessage, result);
        }

        [TestMethod]
        public void Parse_let_without_identifier()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("let");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

        }

        [TestMethod]
        public void Parse_let_with_bad_identifier()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("let while");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected keyword 'while' was encountered while parsing the identifier. The parser expected an identifier.", result);

        }

        [TestMethod]
        public void Parse_let_with_unexpected_token()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("let i 2");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected number literal '2' was encountered while parsing the statement terminator. The parser expected an end of the line.", result);
        }

        [TestMethod]
        public void Parse_let_with_bad_assignment()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("let i := to");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected keyword 'to' was encountered while parsing the assignment of identifier 'i'.", result);
        }

        [TestMethod]
        public void Parse_identifier_with_no_assignment()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("i");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<StandaloneExpressionException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("Standalone expressions are not allowed.", result);
        }

        [TestMethod]
        public void Parse_identifier_with_bad_assignment()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("i while");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<StandaloneExpressionException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("Standalone expressions are not allowed.", result);
        }

        [TestMethod]
        public void Parse_assignment_with_no_expression()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("i := ");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected end of line was encountered while parsing the expression. The expression 'i :=' may not be finished.", result);
        }

        [TestMethod]
        public void Parse_assignment_with_no_identifier()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine(" := 5");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected operator ':=' was encountered while parsing the expression.", result);
        }

        [TestMethod]
        public void Parse_assignment_with_bad_expression()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("i := next");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected keyword 'next' was encountered while parsing the expression. Keyword 'next' is an invalid operand in the expresssion 'i := next'.", result);
        }

        [TestMethod]
        public void Parse_assignment_with_bad_expression_2()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("i := +");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected end of line was encountered while parsing the expression. A unary plus operator (+n) was encountered without an operand.", result);
        }

        [TestMethod]
        public void Parse_if_with_no_condition()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("if ");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected end of line was encountered while parsing the if condition. The if condition expression may not be finished.", result);
        }

        [TestMethod]
        public void Parse_if_with_bad_condition()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("if return");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected keyword 'return' was encountered while parsing the if condition.", result);
        }

        [TestMethod]
        public void Parse_if_with_no_body()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("if true");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<IncompleteSourceException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("Encountered end of file before if block has been properly terminated (end if).", result);
        }

        [TestMethod]
        public void Parse_if_then_with_no_body()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("if true then");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<IncompleteSourceException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("Encountered end of file before if block has been properly terminated (end if).", result);
        }

        [TestMethod]
        public void Parse_if_then_with_no_body_in_function()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("function checkTrue");
            sourceBuilder.AppendLine("    if true then");
            sourceBuilder.AppendLine("        print \"In the end of time, there was a man who knew the road...\"");
            sourceBuilder.AppendLine("end function");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected keyword 'function' was encountered while parsing the if statement. The parser expected a keyword 'if'.", result);
        }

        [TestMethod]
        public void Parse_if_then_with_no_end_if()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("if true then");
            sourceBuilder.AppendLine("   print \"It's true!\"");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<IncompleteSourceException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("Encountered end of file before if block has been properly terminated (end if).", result);
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

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected end of line was encountered while parsing the if statement. The parser expected a keyword 'if'.", result);
        }

        [TestMethod]
        public void Parse_do_while_with_no_condition()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("do while");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected end of line was encountered while parsing the precondition of the do statement.", result);
        }

        [TestMethod]
        public void Parse_do_while_with_bad_condition()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("do while if ");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected keyword 'if' was encountered while parsing the precondition of the do statement.", result);
        }

        [TestMethod]
        public void Parse_do_while_without_loop()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("do while true ");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<IncompleteSourceException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("Encountered end of file before do block has been properly terminated (end do).", result);
        }

        [TestMethod]
        public void Parse_do_until_with_no_condition()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("do until");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected end of line was encountered while parsing the precondition of the do statement.", result);
        }

        [TestMethod]
        public void Parse_do_until_with_bad_condition()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("do until if ");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected keyword 'if' was encountered while parsing the precondition of the do statement.", result);
        }

        [TestMethod]
        public void Parse_do_until_without_loop()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("do until false ");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<IncompleteSourceException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("Encountered end of file before do block has been properly terminated (end do).", result);
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

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected end of line was encountered while parsing the postcondition of the do statement.", result);
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

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected keyword 'print' was encountered while parsing the postcondition of the do statement.", result);
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

            var exception = Assert.Throws<DoLoopNoConditionException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("A do statement is lacking a condition. A do loop statement must have either a precondition (do while/until) or a postcondition (loop while/until) followed by a Boolean expression.", result);
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

            var exception = Assert.Throws<DoLoopDualConditionException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("A do statement cannot have both a precondition and postcondition expression.", result);
        }

        [TestMethod]
        public void Parse_for_with_no_iterator()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected end of line was encountered while parsing the iterator of the for statement. The parser expected an identifier.", result);
        }

        [TestMethod]
        public void Parse_for_with_bad_iterator()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for do");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected keyword 'do' was encountered while parsing the iterator of the for statement. The parser expected an identifier.", result);
        }

        [TestMethod]
        public void Parse_for_with_iterator_without_assignment()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected end of line was encountered while parsing the from value of the for statement. The parser expected an operator ':='.", result);
        }

        [TestMethod]
        public void Parse_for_with_iterator_with_bad_assignment()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i do");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected keyword 'do' was encountered while parsing the from value of the for statement. The parser expected an operator ':='.", result);
        }

        [TestMethod]
        public void Parse_for_with_iterator_without_assigned_expression()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i := ");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected end of line was encountered while parsing the from value of the for statement.", result);
        }

        [TestMethod]
        public void Parse_for_with_iterator_with_bad_assignment_expression()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i := then");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected keyword 'then' was encountered while parsing the from value of the for statement.", result);
        }

        [TestMethod]
        public void Parse_for_without_to()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i := 7");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected end of line was encountered while parsing the to value of the for statement. The parser expected a keyword 'to'.", result);
        }

        [TestMethod]
        public void Parse_for_with_bad_to()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i := 7 for ");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected keyword 'for' was encountered while parsing the to value of the for statement. The parser expected a keyword 'to'.", result);
        }

        [TestMethod]
        public void Parse_for_with_to_without_expression()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i := 7 to ");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected end of line was encountered while parsing the to value of the for statement.", result);
        }

        [TestMethod]
        public void Parse_for_with_to_with_bad_expression()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i := 7 to while ");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected keyword 'while' was encountered while parsing the to value of the for statement.", result);
        }

        [TestMethod]
        public void Parse_for_with_bad_step()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i := 7 to 15 2 ");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<StandaloneExpressionException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            // TODO: Enable the parser to be more tuned to new lines so as not to cut the 2 here of as it's own standalone expression

            Assert.AreEqual("Standalone expressions are not allowed.", result);
        }

        [TestMethod]
        public void Parse_for_with_from_and_to_without_iterator()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for := 7 to 15");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected operator ':=' was encountered while parsing the iterator of the for statement. The parser expected an identifier.", result);
        }

        [TestMethod]
        public void Parse_for_with_identifier_and_from_without_to()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i := 1 5");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected number literal '5' was encountered while parsing the to value of the for statement. The parser expected a keyword 'to'.", result);
        }

        [TestMethod]
        public void Parse_for_with_identifier_and_from_without_to_expression()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i := 1 to");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected end of line was encountered while parsing the to value of the for statement.", result);
        }

        [TestMethod]
        public void Parse_for_without_step_expression()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i := 7 to 15 step ");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected end of line was encountered while parsing the step value of the for statement.", result);
        }

        [TestMethod]
        public void Parse_for_with_bad_step_expression()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i := 7 to 15 step ");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected end of line was encountered while parsing the step value of the for statement.", result);
        }

        [TestMethod]
        public void Parse_for_without_next()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("for i := 7 to 15 ");
            sourceBuilder.AppendLine("    print \"New Mexico is the new Arkansas!!!\"");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<IncompleteSourceException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("Encountered end of file before for block has been properly terminated (next).", result);
        }

        [TestMethod]
        public void Parse_function_declaration_without_function_name()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("function");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected end of line was encountered while parsing the function name. The parser expected an identifier.", result);
        }

        [TestMethod]
        public void Parse_function_declaration_with_bad_function_name()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("function 2");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected number literal '2' was encountered while parsing the function name. The parser expected an identifier.", result);
        }

        [TestMethod]
        public void Parse_function_declaration_bad_arguments()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("function foo(bar: number, 2)");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected number literal '2' was encountered while parsing the parameter expression. The parser expected an identifier.", result);
        }

        [TestMethod]
        public void Parse_function_declaration_without_end_function()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("function foo(bar: number)");
            sourceBuilder.AppendLine("    print \"Are rice crispies just freeze-dried rice grains?\"");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<IncompleteSourceException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("Encountered end of file before function declaration has been properly terminated (end function).", result);
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

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected close parenthesis ')' was encountered while parsing the parameter expression. The parser expected a type.", result);
        }

        [TestMethod]
        public void Parse_function_declaration_missing_first_parameter_name()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("function foo( ,bar: number, ro: number)");
            sourceBuilder.AppendLine("    print \"Are rice crispies just freeze-dried rice grains?\"");
            sourceBuilder.AppendLine("end function");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected comma ',' was encountered while parsing the parameter expression. The parser expected an identifier.", result);
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

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected comma ',' was encountered while parsing the parameter expression. The parser expected an identifier.", result);
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

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected close parenthesis ')' was encountered while parsing the parameter expression. The parser expected an identifier.", result);
        }

        [TestMethod]
        public void Parse_function_call_without_close_paren()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("foo (bar,ro");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected end of line was encountered while parsing the argument expression. The parser expected a comma.", result);
        }

        [TestMethod]
        public void Parse_function_call_missing_argument()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("foo(a,)");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected close parenthesis ')' was encountered while parsing the argument expression. The parser expected an identifier.", result);
        }

        [TestMethod]
        public void Parse_function_call_missing_comma()
        {
            // Setup
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("foo(1 2)");

            var source = sourceBuilder.ToString();

            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual("An unexpected number literal '2' was encountered while parsing the argument expression. The parser expected a comma.", result);
        }


        [TestMethod]
        [DataRow("end if", "An unexpected keyword 'end' was encountered while parsing the expression.")]
        [DataRow("loop", "An unexpected keyword 'loop' was encountered while parsing the expression.")]
        [DataRow("next", "An unexpected keyword 'next' was encountered while parsing the expression.")]
        [DataRow("else\r\nprint 5", "An unexpected keyword 'else' was encountered while parsing the expression.")]
        [DataRow("else if x", "An unexpected keyword 'else' was encountered while parsing the expression.")]
        [DataRow("if x then\r\nelse\r\nelse\r\nend if", "An unexpected keyword 'else' was encountered while parsing the if statement. The parser expected a keyword 'end'.")]
        public void Parse_construct_closers_without_construct(string source, string expectedMessage)
        {
            var exception = Assert.Throws<UnexpectedTokenException>(() => Parser.Parse(source).ToList());

            var errorMessageFormatter = new ErrorMessageFormatter();

            // Execute
            var result = errorMessageFormatter.Describe(exception);

            // Assert
            Console.WriteLine(result);
            WriteDiagnostic(exception.Diagnostic);

            Assert.AreEqual(expectedMessage, result);
        }

        public void WriteDiagnostic(Diagnostic diagnostic)
        {
            var json = JsonSerializer.Serialize(diagnostic, _jsonOptions);

            Console.Write(json);
        }
    }
}
