using Quixotic.Common.Exceptions.Interpret;
using Quixotic.InterpretTests.TestImplementations;
using Quixotic.Parsing;
using QuixoticLang.Lexer;

namespace Quixotic.InterpretTests
{
    [TestClass]
    public sealed class InterpreterTests
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertHasPrinted("Hello, windmill!");
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertHasPrinted("262144");
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertHasPrinted("393216");
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertHasPrinted("327680");
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertHasPrinted("9");
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertHasPrinted("34359738370");
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertHasPrinted("42");
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert 
            runtime.AssertHasPrinted("Hello, first windmill!");
            runtime.AssertHasPrinted("Hello, second windmill!");
            runtime.AssertHasPrinted("Hello, third windmill!");
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert 
            runtime.AssertVariableHasValue("windmills", 5);
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert 
            runtime.AssertVariableHasValue("windmills", 5);
            runtime.AssertVariableHasValue("w", 5);
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert 
            runtime.AssertVariableIsNull("windmills");
            runtime.AssertHasPrinted("nada");
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert 
            runtime.AssertVariableHasValue("windmills", 8);
            runtime.AssertHasPrinted("nada");
            runtime.AssertHasPrinted("8");
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert 
            runtime.AssertVariableHasValue("windmills", 16);
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert 
            runtime.AssertVariableHasValue("windmills", 5);
            runtime.AssertHasPrinted("5");
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertVariableHasValue("cost", -5);
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert 
            runtime.AssertVariableHasValue("cost", -1);
            runtime.AssertHasPrinted("-1");
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertVariableHasValue("c", 5);
            runtime.AssertHasPrinted("c is greater than 3");
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertVariableHasValue("c", 2);
            runtime.AssertHasPrinted("c is less than or equal to 3");
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertVariableHasValue("c", 2);
            runtime.AssertHasPrinted("c is greater than 1");
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertFunctionDeclared("sayHello");

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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertFunctionDeclared("sayHello");
            runtime.AssertHasPrinted("Hello, windmills!");
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertFunctionDeclared("sayHello");
            runtime.AssertHasPrinted("6");
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertFunctionDeclared("sayHello");
            runtime.AssertHasPrinted("6");
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute & Assert
            interpreter.Execute(parser.Parse());

            runtime.AssertFunctionDeclared("sayHello");
            runtime.AssertHasPrinted("6");

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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute & Assert
            Assert.Throws<TypeMismatchException>(() => interpreter.Execute(parser.Parse()));
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute & Assert
            Assert.Throws<UndefinedFunctionException>(() => interpreter.Execute(parser.Parse())); // There is no function with signature 'sayHello(string, number, number)'
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertFunctionDeclared("sayHello");
            runtime.AssertHasPrinted("Hello, windmills!");
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertHasPrinted("0");
            runtime.AssertHasPrinted("1");
            runtime.AssertHasPrinted("2");
            runtime.AssertHasPrinted("3");
            runtime.AssertHasPrinted("4");
            runtime.AssertHasPrinted("5");
            runtime.AssertHasPrinted("6");
            runtime.AssertHasPrinted("7");
            runtime.AssertHasPrinted("8");
            runtime.AssertHasPrinted("9");

            runtime.AssertVariableHasValue("i", 10);
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertHasPrinted("0");
            runtime.AssertHasPrinted("1");
            runtime.AssertHasPrinted("2");
            runtime.AssertHasPrinted("3");
            runtime.AssertHasPrinted("4");
            runtime.AssertHasPrinted("5");
            runtime.AssertHasPrinted("6");
            runtime.AssertHasPrinted("7");
            runtime.AssertHasPrinted("8");
            runtime.AssertHasPrinted("9");

            runtime.AssertVariableHasValue("i", 10);
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertHasPrinted("0");
            runtime.AssertHasPrinted("1");
            runtime.AssertHasPrinted("2");
            runtime.AssertHasPrinted("3");
            runtime.AssertHasPrinted("4");
            runtime.AssertHasPrinted("5");
            runtime.AssertHasPrinted("6");
            runtime.AssertHasPrinted("7");
            runtime.AssertHasPrinted("8");
            runtime.AssertHasPrinted("9");

            runtime.AssertVariableHasValue("i", 10);

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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertHasPrinted("0");
            runtime.AssertHasPrinted("1");
            runtime.AssertHasPrinted("2");
            runtime.AssertHasPrinted("3");
            runtime.AssertHasPrinted("4");
            runtime.AssertHasPrinted("5");
            runtime.AssertHasPrinted("6");
            runtime.AssertHasPrinted("7");
            runtime.AssertHasPrinted("8");
            runtime.AssertHasPrinted("9");

            runtime.AssertVariableHasValue("i", 10);
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertHasNotPrinted("Explosions and distruction!!!");
            runtime.AssertHasNotPrinted("Important stuff!");
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertHasNotPrinted("Explosions and distruction!!!");
            runtime.AssertHasPrinted("Important stuff!");
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertHasPrinted("Attack!");
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertHasPrinted("Attack!");
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertHasPrinted("before break");
            runtime.AssertHasNotPrinted("after break");
        }

        [TestMethod]
        public void Execute_do_loop_with_continue()
        {
            // Setup
            using var source = GetTestFile("do_loop_with_continue");

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertHasPrinted("before continue");
            runtime.AssertHasNotPrinted("after continue");
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertHasPrinted(0, "0");
            runtime.AssertHasPrinted(1, "1");
            runtime.AssertHasPrinted(2, "2");
            runtime.AssertHasPrinted(3, "3");
            runtime.AssertHasPrinted(4, "4");
            runtime.AssertHasPrinted(5, "5");
            runtime.AssertHasPrinted(6, "6");
            runtime.AssertHasPrinted(7, "7");
            runtime.AssertHasPrinted(8, "8");
            runtime.AssertHasPrinted(9, "9");
            runtime.AssertHasPrinted(10, "10");

            runtime.AssertVariableHasValue("i", 10);
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertHasPrinted(0, "10");
            runtime.AssertHasPrinted(1, "9");
            runtime.AssertHasPrinted(2, "8");
            runtime.AssertHasPrinted(3, "7");
            runtime.AssertHasPrinted(4, "6");
            runtime.AssertHasPrinted(5, "5");
            runtime.AssertHasPrinted(6, "4");
            runtime.AssertHasPrinted(7, "3");
            runtime.AssertHasPrinted(8, "2");
            runtime.AssertHasPrinted(9, "1");
            runtime.AssertHasPrinted(10, "0");

            runtime.AssertVariableHasValue("i", 0);
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertHasPrinted(0, "0");
            runtime.AssertHasPrinted(1, "2");
            runtime.AssertHasPrinted(2, "4");
            runtime.AssertHasPrinted(3, "6");
            runtime.AssertHasPrinted(4, "8");
            runtime.AssertHasPrinted(5, "10");

            runtime.AssertVariableHasValue("i", 10);
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertVariableHasValue("array", [1, 2, 3, 4, 5]);
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertVariableHasValue("array", [1, 2, 3, 4, 5, 6]);
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertVariableHasValue("array", [1, 2, 3, 4, 5]);
            runtime.AssertVariableHasValue("element", 3);
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertVariableHasValue("array", [1, 2, 10, 4, 5]);
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute && Assert
            Assert.Throws<TypeMismatchException>(() => interpreter.Execute(parser.Parse()));
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute && Assert
            Assert.Throws<TypeMismatchException>(() => interpreter.Execute(parser.Parse()));
        }


        [TestMethod]
        public void Execute_for_loop_with_array()
        {
            // Setup
            var source = @"
                let array := [1, 2, 3, 4, 5]

                for i in array
                    print i
                next
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertHasPrinted(0, "1");
            runtime.AssertHasPrinted(1, "2");
            runtime.AssertHasPrinted(2, "3");
            runtime.AssertHasPrinted(3, "4");
            runtime.AssertHasPrinted(4, "5");
        }

        [TestMethod]
        public void Execute_array_length_property()
        {
            // Setup
            var source = @"
                let array := [1, 2, 3, 4, 5]
                
                print array.length
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertHasPrinted("5");
        }

        [TestMethod]
        public void Execute_array_set_length_property()
        {
            // Setup
            var source = @"
                let array := [1, 2, 3, 4, 5]
                
                array.length := 7 ' This shouldn't be possible
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            Assert.Throws<UndefinedMethodException>(() => interpreter.Execute(parser.Parse()));
        }


        [TestMethod]
        public void Execute_set_assignment()
        {
            // Setup
            var source = @"
                let set := {1, 2, 3, 4, 5}
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertVariableHasValue("set", Set([1, 2, 3, 4, 5]));
        }

        [TestMethod]
        public void Execute_set_assignment_add_element()
        {
            // Setup
            var source = @"
                let set := {1, 2, 3, 4, 5}

                set := set + 6
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert

            runtime.AssertVariableHasValue("set", Set([1, 2, 3, 4, 5, 6]));

        }

        [TestMethod]
        public void Execute_set_assignment_add_duplicate_element()
        {
            // Setup
            var source = @"
                let set := {1, 2, 3, 4, 5}

                set := set + 3
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert

            runtime.AssertVariableHasValue("set", Set([1, 2, 3, 4, 5]));

        }

        [TestMethod]
        public void Execute_set_with_in_operator()
        {
            // Setup
            var source = @"
                let set := {1, 2, 3, 4, 5}

                let hasElement := 2 in set
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertVariableHasValue("set", Set([1, 2, 3, 4, 5]));
            runtime.AssertVariableHasValue("hasElement", true);
        }

        [TestMethod]
        public void Execute_set_add_element_in_variable_declaration()
        {
            // Setup
            var source = @"
                let set := {1, 2, 3, 4, 5}

                let newSet := set + 6
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertVariableHasValue("set", Set([1, 2, 3, 4, 5]));
            runtime.AssertVariableHasValue("newSet", Set([1, 2, 3, 4, 5, 6]));
        }

        [TestMethod]
        public void Execute_set_add_duplicate_element_in_variable_declaratio()
        {
            // Setup
            var source = @"
                let set := {1, 2, 3, 4, 5}

                let newSet := set + 2
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertVariableHasValue("set", Set([1, 2, 3, 4, 5]));
            runtime.AssertVariableHasValue("newSet", Set([1, 2, 3, 4, 5]));
        }

        [TestMethod]
        public void Execute_for_loop_with_set()
        {
            // Setup
            var source = @"
                let set := {1, 2, 3, 4, 5}

                for i in set
                    print i
                next
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertHasPrinted("1");
            runtime.AssertHasPrinted("2");
            runtime.AssertHasPrinted("3");
            runtime.AssertHasPrinted("4");
            runtime.AssertHasPrinted("5");

        }

        [TestMethod]
        public void Execute_set_length_property()
        {
            // Setup
            var source = @"
                let set := {1, 2, 3, 4, 5}
                
                print set.length
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Executet
            interpreter.Execute(parser.Parse());

            // Assert

            runtime.AssertHasPrinted("5");

        }

        [TestMethod]
        public void Execute_set_set_length_property()
        {
            // Setup
            var source = @"
                let set := {1, 2, 3, 4, 5}
                
                set.length := 7 ' This shouldn't be possible
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute & Assert
            Assert.Throws<UndefinedMethodException>(() => interpreter.Execute(parser.Parse()));
        }

        [TestMethod]
        public void Execute_type_declaration()
        {
            // Setup
            var source = @"
                type Person

                    let Name : string := """"

                    let Age : number := 0

                    function Greet()
                        print ""Hello "" + this.Name
                    end function

                end type
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Executet
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertTypeDeclared("Person");
        }

        [TestMethod]
        public void Execute_type_instantiation()
        {
            // Setup
            var source = @"
                type Person

                    let Name : string := ""Meow""

                    let Age : number := 42

                    function Greet()
                        print ""Hello "" + this.Name
                    end function

                end type

                let me := new Person
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Executet
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertTypeDeclared("Person");
            runtime.AssertVariableHasValue("me");
        }

        [TestMethod]
        public void Execute_type_instance_get_member()
        {
            // Setup
            var source = @"
                type Person

                    let Name : string := ""Meow""

                    let Age : number := 42

                    function Greet()
                        print ""Hello "" + this.Name
                    end function

                end type

                let me := new Person

                print me.Name
                print me.Age
                me.Greet()


                me.Name := ""Woof""
                me.Greet()
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Executet
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertTypeDeclared("Person");
            runtime.AssertVariableHasValue("me");

            runtime.AssertHasPrinted("Meow");
            runtime.AssertHasPrinted("42");
            runtime.AssertHasPrinted("Hello Meow");
            runtime.AssertHasPrinted("Hello Woof");
        }

        [TestMethod]
        public void Execute_type_construct()
        {
            // Setup
            var source = @"
                type Person
                    construct(name: string, age: number)
                        this.Name := name
                        this.Age := age
                    end construct

                    let Name : string

                    let Age : number

                    function Greet()
                        print ""Hello "" + this.Name
                    end function

                end type

                let me := new Person(""Woof"", 99)

                print me.Name
                print me.Age
                me.Greet()
            ";

            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Executet
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertTypeDeclared("Person");
            runtime.AssertVariableHasValue("me");

            runtime.AssertHasPrinted("Woof");
            runtime.AssertHasPrinted("99");
            runtime.AssertHasPrinted("Hello Woof");
        }

        private Stream GetTestFile(string name)
        {
            if (!name.EndsWith(".qx"))
                name += ".qx";

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestFiles", name);

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Test file '{name}' not found at path '{filePath}'.");

            var stream = File.OpenRead(filePath);
            return stream;
        }

        public TestSet<double> Set(double[] elements) => new(elements);
    }
}
