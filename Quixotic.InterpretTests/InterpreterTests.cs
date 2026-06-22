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
                let windmills
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
                let windmills
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
                function sayHello first, second, third
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
            var runtime = new TestRuntime();
            var interpreter = new Interpret.Interpreter(runtime);

            // Execute
            interpreter.Execute(parser.Parse());

            // Assert
            runtime.AssertHasPrinted("Attack!");
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
            var source = @"
                let keepItUp := true

                do while keepItUp
                    print ""before continue""

                    keepItUp := false
                    continue

                    print ""after continue""
                loop
            ";

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

    }
}
