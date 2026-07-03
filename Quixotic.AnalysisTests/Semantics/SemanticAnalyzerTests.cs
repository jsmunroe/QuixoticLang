using Quixotic.Analysis.Exceptions;
using Quixotic.Analysis.Semantics;
using Quixotic.Parsing;
using QuixoticLang.Lexer;

namespace Quixotic.AnalysisTests.Semantics
{
    [TestClass]
    public class SemanticAnalyzerTests
    {
        [TestMethod]
        public void Execute_print_statement_with_lexer()
        {
            // Setup
            var source = @"
                print ""Hello, windmill!""
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_print_statement_with_number()
        {
            // Setup
            var source = @"
                print 262144
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_print_statement_with_addition_expression()
        {
            // Setup
            var source = @"
                print 262144 + 131072
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_print_statement_with_addition_expression_with_parentheses()
        {
            // Setup
            var source = @"
                print 262144 + (131072 / 2)
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_print_statement_where_parentheses_override_precedence()
        {
            // Setup
            var source = @"
                print (1 + 2) * 3
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_print_statement_with_arithmetic_series()
        {
            // Setup
            var source = @"
                print 262144 * 131072 + 2
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_print_statement_with_multi_tier_parenthetical_series()
        {
            // Setup
            var source = @"
                print ((1 + 2) * 4) + (17 + (7 + (4 + 2)))
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_multiple_print_statements()
        {
            // Setup
            var source = @"
                print ""Hello, first windmill!""
                print ""Hello, second windmill!""
                print ""Hello, third windmill!""
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_identifier_statement()
        {
            // Setup
            var source = @"
                let windmills := 5
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_identifier_statement_from_identifier()
        {
            // Setup
            var source = @"
                let windmills := 5
                let w := windmills
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_identifier_statement_without_assignment()
        {
            // Setup
            var source = @"
                let windmills: number
                print windmills
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_identifier_assign_to_variable_without_assignment()
        {
            // Setup
            var source = @"
                let windmills: number
                print windmills
                windmills := 8
                print windmills
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }


        [TestMethod]
        public void Execute_identifier_statement_with_parenthetical_expression()
        {
            // Setup
            var source = @"
                let windmills := (5 + 3) * 2
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }


        [TestMethod]
        public void Execute_identifier_statement_with_print_statement()
        {
            // Setup
            var source = @"
                let windmills := 5
                print windmills
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_unary_subtract_expression()
        {
            // Setup
            var source = @"
                let cost := -5
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_unary_subtract_expression_in_exrpression_series()
        {
            // Setup
            var source = @"
                let cost := -5 + 2 * (3 - 1)
                print cost
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_if_statement()
        {
            // Setup
            var source = @"
                let c := 5
                if c > 3
                    print ""c is greater than 3""
                end if
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_if_statement_with_else()
        {
            // Setup
            var source = @"
                let c := 2
                if c > 3
                    print ""c is greater than 3""
                else
                    print ""c is less than or equal to 3""
                end if
            ";
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }


        [TestMethod]
        public void Execute_if_statement_with_else_if()
        {
            // Setup
            var source = @"
                let c := 2
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
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_function_declaration_statement()
        {
            // Setup
            var source = @"
                function sayHello
                    print ""Hello, windmills!""
                end function
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_function_declaration_statement_with_call()
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
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_function_declaration_statement_with_call_with_parameters()
        {
            // Setup
            var source = @"
                function sayHello(first: number, second: number, third: number): void
                    print first + second + third
                end function

                sayHello(1, 2, 3)
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_function_declaration_statement_with_call_with_parameters_that_returns_value()
        {
            // Setup
            var source = @"
                function sayHello(first: number, second: number, third: number): number
                    return first + second + third
                end function

                print sayHello(1, 2, 3)
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_function_declaration_statement_with_call_with_parameters_that_returns_value_with_call_in_assignment()
        {
            // Setup
            var source = @"
                function sayHello(first: number, second: number, third: number): number
                    return first * second * third
                end function

                let value: number

                value := sayHello(1, 2, 3)

                print value
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_function_declaration_statement_with_call_with_parameters_that_returns_value_with_call_in_type_missmatched_assignment()
        {
            // Setup
            var source = @"
                function sayHello(first: number, second: number, third: number): number
                    return first + second + third
                end function

                let value: string

                value := sayHello(1, 2, 3)
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            Assert.HasCount(1, analyzer.Issues);

            Assert.IsInstanceOfType<AssignmentTypeMismatchException>(analyzer.Issues[0]);
        }

        [TestMethod]
        public void Execute_function_declaration_statement_with_call_with_parameters_that_returns_value_with_call_with_wrong_typed_arguments()
        {
            // Setup
            var source = @"
                function sayHello(first: number, second: number, third: number): number
                    return first + second + third
                end function

                let value: string

                value := sayHello(""1"", 2, 3)
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            Assert.HasCount(1, analyzer.Issues);

            Assert.IsInstanceOfType<UnrecognizedFunctionSignatureException>(analyzer.Issues[0]);
        }


        [TestMethod]
        public void Execute_function_call_with_return_value()
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
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }


        [TestMethod]
        public void Execute_do_while_loop()
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
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_do_until_loop()
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
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_do_loop_while()
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
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_do_loop_until()
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
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }


        [TestMethod]
        public void Execute_and_short_circuiting()
        {
            // Setup
            var source = @"
                function boom 
                    print ""Explosions and distruction!!!""
                    return false
                end function

                let i := 1

                if i = 0 and boom() then
                    print ""Important stuff!""
                end if
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_or_short_circuiting()
        {
            // Setup
            var source = @"
                function boom 
                    print ""Explosions and distruction!!!""
                    return false
                end function

                let i := 1

                if i = 1 or boom() then
                    print ""Important stuff!""
                end if
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_assignment_with_boolean_literal_true()
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
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_assignment_with_boolean_literal_false()
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
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }


        [TestMethod]
        public void Execute_do_loop_with_break()
        {
            // Setup
            var source = @"
                do while true
                    print ""before break""
                    break
                    print ""after break""
                loop
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            Assert.HasCount(1, analyzer.Issues);

            Assert.IsInstanceOfType<UnreachableCodeException>(analyzer.Issues[0]);
        }

        [TestMethod]
        public void Execute_for_loop()
        {
            // Setup
            var source = @"
                for i := 0 to 10
                    print i
                next
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_for_loop_backwards()
        {
            // Setup
            var source = @"
                for i := 10 to 0 step -1
                    print i
                next
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_for_loop_with_step()
        {
            // Setup
            var source = @"
                for i := 0 to 10 step 2
                    print i
                next
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_for_loop_with_comments()
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
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_array_assignment()
        {
            // Setup
            var source = @"
                let array := [1, 2, 3, 4, 5]
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_array_assignment_add_element()
        {
            // Setup
            var source = @"
                let array := [1, 2, 3, 4, 5]

                array := array + 6
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_array_indexing()
        {
            // Setup
            var source = @"
                let array := [1, 2, 3, 4, 5]

                let element := array[2]
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_array_index_assignment()
        {
            // Setup
            var source = @"
                let array := [1, 2, 3, 4, 5]

                array[2] := 10
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_array_with_element_type_missmatch()
        {
            // Setup
            var source = @"
                let array: number[] := [""1"", ""2"", ""3""]

                array[2] := 10
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            Assert.HasCount(1, analyzer.Issues);

            Assert.IsInstanceOfType<AssignmentTypeMismatchException>(analyzer.Issues[0]);
        }

        [TestMethod]
        public void Execute_array_index_assignment_with_element_type_mismatch()
        {
            // Setup
            var source = @"
                let array := [1, 2, 3, 4, 5]

                array[2] := ""10""
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser);

            // Assert
            Assert.HasCount(1, analyzer.Issues);

            Assert.IsInstanceOfType<AssignmentTypeMismatchException>(analyzer.Issues[0]);
        }

        public void AssertNoIssues(SemanticAnalyzer analyzer)
        {
            if (analyzer.Issues.Count > 0)
            {
                var issues = string.Join(Environment.NewLine, analyzer.Issues.Select(i => $"{i.GetType().Name}: {i.Message}"));
                Assert.Fail($"Expected no issues, but found {analyzer.Issues.Count} issues: {Environment.NewLine}{issues}");
            }
        }
    }
}
