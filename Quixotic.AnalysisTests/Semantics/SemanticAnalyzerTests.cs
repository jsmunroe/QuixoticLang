using Quixotic.Analysis.Errors;
using Quixotic.Analysis.Exceptions;
using Quixotic.Analysis.Semantics;
using Quixotic.Common.Analysis.Expressions;
using Quixotic.Common.Analysis.Statements;
using Quixotic.Common.Contracts;
using Quixotic.Parsing;
using QuixoticLang.Lexer;
using System.Text;

namespace Quixotic.AnalysisTests.Semantics
{
    [TestClass]
    [DoNotParallelize] // Quixotic's compiler/runtime currently assumes single-threaded execution. Tests must not execute concurrently.
    public class SemanticAnalyzerTests
    {
        [TestMethod]
        public void Execute_print_statement_with_lexer()
        {
            // Setup
            var source = """
                print "Hello, windmill!"
            """;
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);

            Assert.IsNotNull(analyzer.SourceDatabase);

            var analysisInfos = analyzer.SourceDatabase.Query(1, 11);

            Assert.HasCount(2, analysisInfos);

            Assert.IsInstanceOfType<StatementInfo>(analysisInfos[0]);
            Assert.IsInstanceOfType<ExpressionInfo>(analysisInfos[1]);
        }

        [TestMethod]
        public void Execute_print_statement_with_number()
        {
            // Setup
            var source = """
                print 262144
            """;
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_print_statement_with_addition_expression()
        {
            // Setup
            var source = """
                print 262144 + 131072
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_print_statement_with_addition_expression_with_parentheses()
        {
            // Setup
            var source = """
                print 262144 + (131072 / 2)
            """;
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_print_statement_where_parentheses_override_precedence()
        {
            // Setup
            var source = """
                print (1 + 2) * 3
            """;
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_print_statement_with_arithmetic_series()
        {
            // Setup
            var source = """
                print 262144 * 131072 + 2
            """;
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_print_statement_with_multi_tier_parenthetical_series()
        {
            // Setup
            var source = """
                print ((1 + 2) * 4) + (17 + (7 + (4 + 2)))
            """;
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_multiple_print_statements()
        {
            // Setup
            var source = """
                print "Hello, first windmill!"
                print "Hello, second windmill!"
                print "Hello, third windmill!"
            """;
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_identifier_statement()
        {
            // Setup
            var source = """
                let windmills := 5
            """;
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_identifier_statement_from_identifier()
        {
            // Setup
            var source = """
                let windmills := 5
                let w := windmills
            """;
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_identifier_statement_without_assignment()
        {
            // Setup
            var source = """
                let windmills: number
                print windmills
            """;
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_identifier_assign_to_variable_without_assignment()
        {
            // Setup
            var source = """
                let windmills: number
                print windmills
                windmills := 8
                print windmills
            """;
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }


        [TestMethod]
        public void Execute_identifier_statement_with_parenthetical_expression()
        {
            // Setup
            var source = """
                let windmills := (5 + 3) * 2
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }


        [TestMethod]
        public void Execute_identifier_statement_with_print_statement()
        {
            // Setup
            var source = """
                let windmills := 5
                print windmills
            """;
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_unary_subtract_expression()
        {
            // Setup
            var source = """
                let cost := -5
            """;
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_unary_subtract_expression_in_exrpression_series()
        {
            // Setup
            var source = """
                let cost := -5 + 2 * (3 - 1)
                print cost
            """;
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_if_statement()
        {
            // Setup
            var source = """
                let c := 5
                if c > 3
                    print "c is greater than 3"
                end if
            """;
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_if_statement_with_else()
        {
            // Setup
            var source = """
                let c := 2
                if c > 3
                    print "c is greater than 3"
                else
                    print "c is less than or equal to 3"
                end if
            """;
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }


        [TestMethod]
        public void Execute_if_statement_with_else_if()
        {
            // Setup
            var source = """
                let c := 2
                if c > 3 and c < 10 then
                    print "c is greater than 3"
                else if c > 1
                    print "c is greater than 1"
                else
                    print "I can see clearly now the rain is gone."
                    print "I can see all obsticles in my way."
                    print "Gone are the dark clouds that had my blind."
                    print "It's gonna be a bright, bright, bright sunshiney day!"
                end if
            """;
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_function_declaration_statement()
        {
            // Setup
            var source = """
                function sayHello
                    print "Hello, windmills!"
                end function
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_function_declaration_statement_with_call()
        {
            // Setup
            var source = """
                function sayHello
                    print "Hello, windmills!"
                end function

                sayHello()
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_function_declaration_statement_with_call_with_parameters()
        {
            // Setup
            var source = """
                function sayHello(first: number, second: number, third: number): void
                    print first + second + third
                end function

                sayHello(1, 2, 3)
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_function_declaration_statement_with_call_with_parameters_that_returns_value()
        {
            // Setup
            var source = """
                function sayHello(first: number, second: number, third: number): number
                    return first + second + third
                end function

                print sayHello(1, 2, 3)
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_function_declaration_statement_with_call_with_parameters_that_returns_value_with_call_in_assignment()
        {
            // Setup
            var source = """
                function sayHello(first: number, second: number, third: number): number
                    return first * second * third
                end function

                let value: number

                value := sayHello(1, 2, 3)

                print value
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_function_declaration_statement_with_call_with_parameters_that_returns_value_with_call_in_type_missmatched_assignment()
        {
            // Setup
            var source = """
                function sayHello(first: number, second: number, third: number): number
                    return first + second + third
                end function

                let value: string

                value := sayHello(1, 2, 3)
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertHasIssues(analyzer, typeof(AssignmentTypeMismatchException));

        }

        [TestMethod]
        public void Execute_function_declaration_statement_with_call_with_parameters_that_returns_value_with_call_with_wrong_typed_arguments()
        {
            // Setup
            var source = """
                function sayHello(first: number, second: number, third: number): number
                    return first + second + third
                end function

                let value: string

                value := sayHello("1", 2, 3)
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertHasIssues(analyzer, typeof(UnrecognizedFunctionSignatureException));

        }


        [TestMethod]
        public void Execute_function_call_with_return_value()
        {
            // Setup
            var source = """
                function sayHello
                    return "Hello, windmills!"
                end function

                let hello := sayHello()
                print hello
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }


        [TestMethod]
        public void Execute_do_while_loop()
        {
            // Setup
            var source = """
                let i := 0

                do while i < 10
                    print i
                    i := i + 1
                loop
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_do_until_loop()
        {
            // Setup
            var source = """
                let i := 0

                do until i >= 10
                    print i
                    i := i + 1
                loop
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_do_loop_while()
        {
            // Setup
            var source = """
                let i := 0

                do
                    print i
                    i := i + 1
                loop while i < 10
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_do_loop_until()
        {
            // Setup
            var source = """
                let i := 0

                do
                    print i
                    i := i + 1
                loop until i >= 10
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }


        [TestMethod]
        public void Execute_and_short_circuiting()
        {
            // Setup
            var source = """
                function boom :boolean
                    print "Explosions and distruction!!!"
                    return false
                end function

                let i := 1

                if i = 0 and boom() then
                    print "Important stuff!"
                end if
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_or_short_circuiting()
        {
            // Setup
            var source = """
                function boom 
                    print "Explosions and distruction!!!"
                    return false
                end function

                let i := 1

                if i = 1 or boom() then
                    print "Important stuff!"
                end if
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_assignment_with_boolean_literal_true()
        {
            // Setup
            var source = """
                let isAGiant := true

                if isAGiant then
                    print "Attack!"
                end if
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_assignment_with_boolean_literal_false()
        {
            // Setup
            var source = """
                let isAWindmill := false

                if not isAWindmill then
                    print "Attack!"
                end if
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }


        [TestMethod]
        public void Execute_do_loop_with_break()
        {
            // Setup
            var source = """
                do while true
                    print "before break"
                    break
                    print "after break"
                loop
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertHasIssues(analyzer, typeof(UnreachableCodeException));

        }

        [TestMethod]
        public void Execute_for_loop()
        {
            // Setup
            var source = """
                for i := 0 to 10
                    print i
                next
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_for_loop_backwards()
        {
            // Setup
            var source = """
                for i := 10 to 0 step -1
                    print i
                next
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_for_loop_with_step()
        {
            // Setup
            var source = """
                for i := 0 to 10 step 2
                    print i
                next
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_for_loop_with_comments()
        {
            // Setup
            var source = """
                ' This is a for loop
                for i := 0 to 10 ' from 1 to 10
                    print i ' printe each number
                    ' this is a comment inside the for loop block!
                next ' end the loop!
                ' so long '''
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_array_assignment()
        {
            // Setup
            var source = """
                let array := [1, 2, 3, 4, 5]
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_array_assignment_add_element()
        {
            // Setup
            var source = """
                let array := [1, 2, 3, 4, 5]

                array := array + 6
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_array_indexing()
        {
            // Setup
            var source = """
                let array := [1, 2, 3, 4, 5]

                let element := array[2]
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_array_index_assignment()
        {
            // Setup
            var source = """
                let array := [1, 2, 3, 4, 5]

                array[2] := 10
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_array_with_element_type_missmatch()
        {
            // Setup
            var source = """
                let array: number[] := ["1", "2", "3"]

                array[2] := 10
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertHasIssues(analyzer, typeof(AssignmentTypeMismatchException));
        }

        [TestMethod]
        public void Execute_array_index_assignment_with_element_type_mismatch()
        {
            // Setup
            var source = """
                let array := [1, 2, 3, 4, 5]

                array[2] := "10"
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertHasIssues(analyzer, typeof(AssignmentTypeMismatchException));

        }


        [TestMethod]
        public void Execute_for_loop_with_array()
        {
            // Setup
            var source = """
                let array := [1, 2, 3, 4, 5]

                for i in array
                    print i
                next
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_array_length_property()
        {
            // Setup
            var source = """
                let array := [1, 2, 3, 4, 5]
                
                print array.length
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_array_set_length_property()
        {
            // Setup
            var source = """
                let array := [1, 2, 3, 4, 5]
                
                array.length := 7 ' This shouldn't be possible
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertHasIssues(analyzer, typeof(UnrecognizedPropertySignatureException));

        }


        [TestMethod]
        public void Execute_set_assignment()
        {
            // Setup
            var source = """
                let set := {1, 2, 3, 4, 5}
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_set_assignment_add_element()
        {
            // Setup
            var source = """
                let set := {1, 2, 3, 4, 5}

                set := set + 6
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_set_assignment_add_duplicate_element()
        {
            // Setup
            var source = """
                let set := {1, 2, 3, 4, 5}

                set := set + 3
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_set_with_in_operator()
        {
            // Setup
            var source = """
                let set := {1, 2, 3, 4, 5}

                let hasElement := 2 in set
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_set_add_element_in_variable_declaration()
        {
            // Setup
            var source = """
                let set := {1, 2, 3, 4, 5}

                let newSet := set + 6
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_set_add_duplicate_element_in_variable_declaratio()
        {
            // Setup
            var source = """
                let set := {1, 2, 3, 4, 5}

                let newSet := set + 2
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_for_loop_with_set()
        {
            // Setup
            var source = """
                let set := {1, 2, 3, 4, 5}

                for i in set
                    print i
                next
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_set_length_property()
        {
            // Setup
            var source = """
                let set := {1, 2, 3, 4, 5}
                
                print set.length
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_set_set_length_property()
        {
            // Setup
            var source = """
                let set := {1, 2, 3, 4, 5}
                
                set.length := 7 ' This shouldn't be possible
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertHasIssues(analyzer, typeof(UnrecognizedPropertySignatureException));
        }

        [TestMethod]
        public void Execute_type_declaration()
        {
            // Setup
            var source = """
                type Person

                    let Name : string := ""

                    let Age : number := 0

                    function Greet()
                        print "Hello " + this.Name
                    end function

                end type
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_type_instantiation()
        {
            // Setup
            var source = """
                type Person

                    let Name : string := "Meow"

                    let Age : number := 42

                    function Greet()
                        print "Hello " + this.Name
                    end function

                end type

                let me := new Person
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_type_instance_get_member()
        {
            // Setup
            var source = """
                type Person

                    let Name : string := "Meow"

                    let Age : number := 42

                    function Greet()
                        print "Hello " + this.Name
                    end function

                end type

                let me := new Person

                print me.Name
                print me.Age
                me.Greet()


                me.Name := "Woof"
                me.Greet()
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_type_construct()
        {
            // Setup
            var source = """
                type Person
                    construct(name: string, age: number)
                        this.Name := name
                        this.Age := age
                    end construct

                    let Name : string

                    let Age : number

                    function Greet()
                        print "Hello " + this.Name
                    end function

                end type

                let me := new Person("Woof", 99)

                print me.Name
                print me.Age
                me.Greet()
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_type_inheritance()
        {
            // Setup
            var source = """
                type Animal 
                    let noise: string

                    function makeNoise() 
                        print this.noise + "!"
                    end function

                end type


                type Dog is Animal
                    construct
                        this.noise := "woof"
                    end construct

                end type

                let dog: Dog := new Dog()

                dog.makeNoise()
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }


        [TestMethod]
        public void Execute_type_inheritance_with_construction()
        {
            // Setup
            var source = """
                type Animal 
                    construct(name: string, noise: string)
                        this.name := name
                        this.noise := noise
                    end construct

                    let noise: string
                    let name: string

                    function makeNoise() 
                        print this.noise + "!"
                    end function

                end type


                type Dog is Animal
                    construct(name: string) : base(name, "woof")
                    end construct
                end type

                let dog: Animal := new Dog("Rover")

                print dog.name
                dog.makeNoise()
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_type_inheritance_base_method_call()
        {
            // Setup
            var source = """
                type Animal 
                    construct(name: string, noise: string)
                        this.name := name
                        this.noise := noise
                    end construct

                    let noise: string
                    let name: string

                    function makeNoise() 
                        print this.noise + "!"
                    end function

                end type


                type Dog is Animal
                    construct(name: string) : base(name, "woof")
                    end construct

                    function makeNoise()
                        print "The dog " + this.name + " says..."
                        base.makeNoise()
                    end function

                end type

                let dog: Animal := new Dog("Rover")

                print dog.name
                dog.makeNoise()
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_type_complete_inheritance()
        {
            // Setup
            var source = """
                type Animal 
                    construct(noise: string)
                        this.noise := noise
                    end construct

                    let noise: string

                    function makeNoise() 
                        print this.noise + "!"
                    end function

                end type


                type Dog is Animal
                    construct(noise: string, name: string) : base(noise)
                        this.name := name
                    end construct

                    let name: string

                    function makeNoise()
                        print "The dog " + this.name + " says..."
                        base.makeNoise()
                    end function
                end type

                let animal: Animal := new Dog("woof", "Fido")

                animal.makeNoise() ' prints "The dog Fido says...\nwoof!"

                if animal is Dog dog
                    dog.name := "Penny"
                    dog.makeNoise() ' prints "The dog Penny says...\nwoof!"
                end if

                animal.makeNoise() ' prints "The dog Penny says...\nwoof!"
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_declare_dynamic()
        {
            // Setup
            var source = """
                let thing := new dynamic
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_dynamic_assign_member()
        {
            // Setup
            var source = """
                let thing := new dynamic

                thing.fish := "Salmon"

                print thing.fish
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_dynamic_with_a_function()
        {
            // Setup
            var source = """
                let thing := new dynamic

                thing.fish := "Salmon"

                thing.isFish := function(fish: string): boolean
                    return fish = this.fish
                end function

                print thing.fish
                print thing.isFish("Salmon") ' true
                print thing.isFish("Trout") ' false
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_dynamic_assign_member_and_check_if_only_that_member_is_assigned()
        {
            // Setup
            var source = """
                let thingA := new dynamic
                let thingB := new dynamic

                thingA.fish := "Salmon"

                print thingB.fish
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_function_as_variable()
        {
            // Setup
            var source = """
                let hello := function(name: string)
                    print "Hello, " + name + "!"
                end function

                hello("Bob")
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }


        [TestMethod]
        public void Execute_function_as_variable_with_closure()
        {
            // Setup
            var source = """
                let hello :function(string)

                if true then
                    let greeting := "Hi"

                    hello := function(name: string)
                        print greeting + ", " + name + "!"
                    end function
                end if

                hello("Bob")
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_function_as_variable_without_closure()
        {
            // Setup
            var source = """
                let hello :function

                if true then
                    let greeting := "Hi"

                    function hi(name: string)
                        print greeting + ", " + name + "!"
                    end function

                    hello := hi
                end if

                hello("Bob")
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertHasIssues(analyzer, typeof(UnrecognizedIdentifierException));

        }

        [TestMethod]
        public void Execute_function_as_variable_with_explicit_closure_capture()
        {
            // Setup
            var source = """
                let hello :function

                if true then
                    let greeting := "Hi"

                    function hi(name: string) with greeting
                        print greeting + ", " + name + "!"
                    end function

                    hello := hi
                end if

                hello("Bob")
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);

        }

        [TestMethod]
        public void Execute_function_as_variable_with_explicit_all_closure_capture()
        {
            // Setup
            var source = """
                function countDown(one: number, two: number, three: number): function
                    
                    function addUp() with closure
                        return one + two + three
                    end function

                    return addUp
                end function

                let result := countDown(1, 1, 2)
                
                print result()
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_function_as_variable_with_explicit_all_closure_capture_2()
        {
            // Setup
            var source = """
                function makeCounter(): function
                    let count := 0

                    function increment() with count
                        count := count + 1
                        return count
                    end function

                    return increment
                end function

                let counter := makeCounter()

                print counter()
                print counter()
                print counter()
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_function_stored_in_variable()
        {
            // Setup
            var source = """
                function hi(name: string)
                    print "Hello, " + name + "!"
                end function

                let hello := hi

                hello("Greg")
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_import_standard_library_use_Complex()
        {
            // Setup
            var source = """
                import Quixotic.Standard.Math

                let complex := new Complex(1, 1)

                print complex.magnitudeSqr

                let complex2 := new Complex(2, 4)
                let complex3 := complex + complex2
                
                print complex3
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_import_standard_library_use_Math()
        {
            // Setup
            var source = """
                import Quixotic.Standard.Math
                
                let result := Math.min(100, 10)

                print result
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertNoIssues(analyzer);
        }

        [TestMethod]
        public void Execute_array_append()
        {
            // Setup
            var source = """
                let a := [1,2,3]
                let b := ["a","b"]

                let c := a + 4      ' ✓
                let d := b + "c"    ' ✓
                let e := a + "c"    ' should fail
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertHasIssues(analyzer, typeof(UnrecognizedFunctionSignatureException));

        }

        [TestMethod]
        public void Execute_array_concat()
        {
            // Setup
            var source = """
                let a := [1]
                let b := ["a"]

                let c := a + b        ' should fail
            """;

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var analyzer = new SemanticAnalyzer();

            // Execute
            analyzer.Analyze(parser).ToList();

            // Assert
            AssertHasIssues(analyzer, typeof(UnrecognizedFunctionSignatureException)); // No function exists that adds a number array to a number array.

        }

        private string Format(Exception exception, ISource? source)
        {
            var formatter = new ErrorMessageFormatter();

            var builder = new StringBuilder();

            var message = formatter.Describe(exception, source);

            return message.ToString();
        }

        public void AssertNoIssues(SemanticAnalyzer analyzer)
        {
            var source = analyzer.Source;
            var message = new StringBuilder();

            if (analyzer.Issues.Count > 0)
            {
                message.AppendLine($"Expected no issues, but found {analyzer.Issues.Count} issues: ");

                foreach (var issue in analyzer.Issues)
                {
                    var type = issue.GetType();
                    message.AppendLine($"{type.Name}: {Format(issue, source)}");
                    message.AppendLine(issue.StackTrace);
                    message.AppendLine();
                }

                Assert.Fail(message.ToString());
            }
        }

        public void AssertHasIssues(SemanticAnalyzer analyzer, params Type[] types)
        {
            var source = analyzer.Source;
            var message = new StringBuilder();

            List<SemanticException> semanticIssues = [.. analyzer.Issues];
            List<Type> issueTypes = [.. types];
            foreach (var issue in semanticIssues.ToList()) // ToList because the underlying list will change
            {
                var type = issue.GetType();

                if (issueTypes.Contains(type))
                    semanticIssues.Remove(issue);
            }

            if (semanticIssues.Count == 0)
                return;

            if (issueTypes.Count > 0)
                message.AppendLine($"Expected issues of type(s) {string.Join(", ", issueTypes.Select(t => t.Name))}.");

            message.AppendLine($"Found {semanticIssues.Count} issues not expected: ");

            foreach (var issue in semanticIssues)
            {
                var type = issue.GetType();
                message.AppendLine($"{type.Name}: {Format(issue, source)}");
                message.AppendLine(issue.StackTrace);
                message.AppendLine();
            }

            Assert.Fail(message.ToString());
        }
    }
}
