using Quixotic.Interpret.Contracts;
using Quixotic.Interpret.Environment;
using Quixotic.Interpret.Exceptions;
using Quixotic.Interpret.Symbols;
using Quixotic.Parsing;
using Quixotic.Parsing.Expressions;
using Quixotic.Parsing.Operations;
using Quixotic.Parsing.Statements;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Quixotic.Interpret
{
    public class Interpreter(IRuntime runtime)
    {
        private readonly static Dictionary<Type, Action<Interpreter, QxStatement>> _executeMap = [];

        private readonly static Dictionary<Type, Func<Interpreter, QxExpression, Value>> _evaluateMap = [];

        static Interpreter() => LoadMapEntries();

        public void Execute(IEnumerable<QxStatement> statements)
        {
            // Executes statements in global space

            foreach (var statement in statements)
                Execute(statement);
        }

        public void Execute(Block statements, RuntimeFrameType frameType)
        {
            runtime.PushBlock(frameType);

            try
            {
                foreach (var statement in statements)
                    Execute(statement);
            }
            catch (ReturnException)
            {
                runtime.Pop(); // Need to pop out of runtime frame since we are returning. Returned value will be caught higher up the stack.
                throw;
            }

            runtime.Pop();
        }

        public void Execute(Function function, List<Argument> arguments)
        {
            runtime.PushFunction();

            foreach (var argument in arguments)
                runtime.Frame.Scope.DefineVariable(argument.Name, argument.Value);

            try
            {

                foreach (var statement in function.Body)
                    Execute(statement);
            }
            catch (ReturnException)
            {
                runtime.Pop(); // Need to pop out of runtime frame since we are returning. Returned value will be caught higher up the stack.
                throw;
            }

            runtime.Pop();
        }

        public void Execute(QxStatement statement)
        {
            var statementType = statement.GetType();
            if (!_executeMap.TryGetValue(statementType, out var action))
                throw new NotSupportedException($"Unsupported statement type: {statementType.Name}");

            action(this, statement);
        }

        private Value Evaluate(QxExpression expression)
        {
            var expressionType = expression.GetType();
            if (!_evaluateMap.TryGetValue(expressionType, out var function))
                throw new NotSupportedException($"Unsupported expression type: {expressionType.Name}");

            return function(this, expression);
        }


        public void Execute(QxPrintStatement statement)
        {
            var value = Evaluate(statement.Expression);
            runtime.ExecutePrint(value);
        }

        public void Execute(QxVariableDeclarationStatement statement)
        {
            Value value = NadaValue.Value;

            if (statement.Value is not null)
                value = Evaluate(statement.Value);

            runtime.Frame.Scope.DefineVariable(statement.Name, value);
        }

        public void Execute(QxFunctionDeclarationStatement statement)
        {
            List<Parameter> parameters = [.. statement.Parameters.Select(p => new Parameter(p.Name))];

            var function = new Function(statement.Body) { Parameters = [.. parameters] };
            runtime.Frame.Scope.DefineFunction(statement.Name, function);
        }

        public void Execute(QxFunctionCallStatement statement)
        {
            var name = statement.Name;

            var function = runtime.Frame.Scope.GetFunction(name);

            var arguments = BindArguments(name, function, statement.Arguments);

            try
            {
                Execute(function, arguments);
            }
            catch (ReturnException) { } // Ignore returned value;
        }

        public void Execute(QxReturnStatement statement)
        {
            Value? value = null;
            if (statement.Expression is not null)
                value = Evaluate(statement.Expression);

            throw new ReturnException(value);
        }

        public void Execute(QxAssignmentStatement statement)
        {
            var value = Evaluate(statement.Value);
            var name = statement.Target.Name;

            runtime.Frame.Scope.AssignVariable(name, value);

        }

        public void Execute(QxIfStatement statement)
        {
            if (IsTruthy(Evaluate(statement.Condition)))
            {
                Execute(statement.ThenBlock, RuntimeFrameType.IfBlock);
                return;
            }

            foreach (var elseIfClasue in statement.ElseIfClauses)
            {
                if (IsTruthy(Evaluate(elseIfClasue.Condition)))
                {
                    Execute(elseIfClasue.Block, RuntimeFrameType.IfBlock);
                    return;
                }
            }

            Execute(statement.ElseBlock, RuntimeFrameType.IfBlock);
        }

        public void Execute(QxDoStatement statement)
        {
            while (true)
            {
                if (statement.EntryControlled && !IsTruthy(Evaluate(statement.Condition)))
                    break;

                Execute(statement.Block);

                if (!statement.EntryControlled && !IsTruthy(Evaluate(statement.Condition)))
                    break;
            }
        }

        protected static Value Evaluate(QxNumberLiteralExpression expression)
        {
            return new NumberValue(expression.Value);
        }

        protected static Value Evaluate(QxStringLiteralExpression expression)
        {
            return new StringValue(expression.Value);
        }

        protected static Value Evaluate(QxBooleanLiteralExpression expression)
        {
            return new BooleanValue(expression.Value);
        }

        protected Value Evaluate(QxIdentifierExpression expression)
        {
            var name = expression.Name;
            var value = runtime.Frame.Scope.GetValue(name);

            return value;
        }

        protected Value Evaluate(QxUnaryExpression expression)
        {
            var operand = Evaluate(expression.Operand);

            var operatorValue = OperationMetadata.GetOperatorValue(expression.Operator);

            return expression.Operator switch
            {
                Operator.Not => new BooleanValue(!IsTruthy(operand)),
                Operator.Subtract when operand is NumberValue number => new NumberValue(-number.Value),
                Operator.Subtract when operand is not NumberValue => throw new UnaryOperatorException($"Cannot apply negative operator to operand that is type {operand.Type}."),
                _ => throw new UnaryOperatorException($"{operatorValue} is not supported as a unary operator."),
            };
        }

        protected Value Evaluate(QxBinaryExpression expression)
        {
            var left = expression.Left;
            var right = expression.Right;

            var operatorValue = OperationMetadata.GetOperatorValue(expression.Operator) ?? string.Empty;

            return expression.Operator switch
            {
                Operator.Add => Add(left, right),
                Operator.Subtract => Subtract(left, right),
                Operator.Multiply => Multiply(left, right),
                Operator.Divide => Divide(left, right),

                Operator.EqualTo => IsEqualTo(left, right),
                Operator.NotEqualTo => IsNotEqualTo(left, right),
                Operator.LessThan => IsLessThan(left, right),
                Operator.LessThanOrEqualTo => IsLessThanOrEqualTo(left, right),
                Operator.GreaterThan => IsGreaterThan(left, right),
                Operator.GreaterThanOrEqualTo => IsGreaterThanOrEqualTo(left, right),

                Operator.And => And(left, right),
                Operator.Or => Or(left, right),

                _ => throw new BinaryOperatorException(left.Kind, operatorValue, right.Kind),
            };
        }

        protected Value Evaluate(QxFunctionCallExpression expression)
        {
            var name = expression.Name;

            var function = runtime.Frame.Scope.GetFunction(name);

            var arguments = BindArguments(name, function, expression.Arguments);

            try
            {
                Execute(function, arguments);
            }
            catch (ReturnException returnException)
            {
                if (returnException.Value is not null)
                    return returnException.Value;
            }

            throw new ExpectedReturnValueException(name);
        }

        private static bool IsTruthy(Value value)
        {
            return value.IsTruthy();
        }

        private Value Add(QxExpression left, QxExpression right)
        {
            var leftValue = Evaluate(left);
            var rightValue = Evaluate(right);

            return leftValue.Add(rightValue);
        }

        private Value Subtract(QxExpression left, QxExpression right)
        {
            var leftValue = Evaluate(left);
            var rightValue = Evaluate(right);

            return leftValue.Subtract(rightValue);
        }

        private Value Multiply(QxExpression left, QxExpression right)
        {
            var leftValue = Evaluate(left);
            var rightValue = Evaluate(right);

            return leftValue.Multiply(rightValue);
        }

        private Value Divide(QxExpression left, QxExpression right)
        {
            var leftValue = Evaluate(left);
            var rightValue = Evaluate(right);

            return leftValue.Divide(rightValue);
        }

        private BooleanValue IsEqualTo(QxExpression left, QxExpression right)
        {
            var leftValue = Evaluate(left);
            var rightValue = Evaluate(right);

            return leftValue.IsEqualTo(rightValue);
        }

        private BooleanValue IsNotEqualTo(QxExpression left, QxExpression right)
        {
            var leftValue = Evaluate(left);
            var rightValue = Evaluate(right);

            return leftValue.IsNotEqualTo(rightValue);
        }

        private BooleanValue IsLessThan(QxExpression left, QxExpression right)
        {
            var leftValue = Evaluate(left);
            var rightValue = Evaluate(right);

            return leftValue.IsLessThan(rightValue);
        }

        private BooleanValue IsLessThanOrEqualTo(QxExpression left, QxExpression right)
        {
            var leftValue = Evaluate(left);
            var rightValue = Evaluate(right);

            return leftValue.IsLessThanOrEqualTo(rightValue);
        }

        private BooleanValue IsGreaterThan(QxExpression left, QxExpression right)
        {
            var leftValue = Evaluate(left);
            var rightValue = Evaluate(right);

            return leftValue.IsGreaterThan(rightValue);
        }

        private BooleanValue IsGreaterThanOrEqualTo(QxExpression left, QxExpression right)
        {
            var leftValue = Evaluate(left);
            var rightValue = Evaluate(right);

            return leftValue.IsGreaterThanOrEqualTo(rightValue);
        }

        private BooleanValue And(QxExpression left, QxExpression right)
        {
            var leftValue = Evaluate(left);

            if (!IsTruthy(leftValue))
                return BooleanValue.False;

            var rightValue = Evaluate(right);

            return IsTruthy(rightValue) ? BooleanValue.True : BooleanValue.False;
        }

        private BooleanValue Or(QxExpression left, QxExpression right)
        {
            var leftValue = Evaluate(left);

            if (IsTruthy(leftValue))
                return BooleanValue.True;

            var rightValue = Evaluate(right);

            return IsTruthy(rightValue) ? BooleanValue.True : BooleanValue.False;
        }

        public List<Argument> BindArguments(string name, Function function, List<QxExpression> expressions)
        {
            var parameters = function.Parameters;

            if (parameters.Count != expressions.Count)
                throw new ParameterCountException(name, parameters.Count, expressions.Count);

            List<Argument> arguments = [];

            // Push function parameters into 
            foreach (var (parameter, expression) in parameters.Zip(expressions))
            {
                var value = Evaluate(expression);
                arguments.Add(new Argument(parameter.Name, value));
            }

            return arguments;
        }

        private static void LoadMapEntries()
        {
            foreach (var method in typeof(Interpreter).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                var methodParameters = method.GetParameters();

                if (methodParameters.Length < 1)
                    continue;

                var methodParameter = method.GetParameters()[0];
                var parameterType = methodParameter.ParameterType;

                if (TryCreateExecuteMapEntry(method, out var execute))
                    _executeMap[parameterType] = execute;

                if (TryCreateEvaluateMapEntry(method, out var evaluate))
                    _evaluateMap[parameterType] = evaluate;
            }
        }

        private static bool TryCreateExecuteMapEntry(MethodInfo method, [NotNullWhen(true)] out Action<Interpreter, QxStatement>? action)
        {
            action = null;

            if (method.Name != "Execute")
                return false;

            var parameters = method.GetParameters();

            if (parameters.Length != 1)
                return false;

            var parameterType = parameters[0].ParameterType;

            if (!typeof(QxStatement).IsAssignableFrom(parameterType))
                return false;

            var isStatic = method.IsStatic;

            var instance = System.Linq.Expressions.Expression.Parameter(typeof(Interpreter), "instance");
            var parameter = System.Linq.Expressions.Expression.Parameter(typeof(QxStatement), "statement");
            var castParameter = System.Linq.Expressions.Expression.Convert(parameter, parameterType);
            var call = System.Linq.Expressions.Expression.Call(isStatic ? null : instance, method, castParameter);
            var lambda = System.Linq.Expressions.Expression.Lambda<Action<Interpreter, QxStatement>>(call, instance, parameter);

            action = lambda.Compile();

            return true;
        }

        private static bool TryCreateEvaluateMapEntry(MethodInfo method, [NotNullWhen(true)] out Func<Interpreter, QxExpression, Value>? action)
        {
            action = null;

            if (method.Name != "Evaluate")
                return false;

            var parameters = method.GetParameters();

            if (parameters.Length != 1)
                return false;

            var parameterType = parameters[0].ParameterType;

            if (!typeof(QxExpression).IsAssignableFrom(parameterType))
                return false;

            if (!method.ReturnType.IsAssignableFrom(typeof(Value)))
                return false;

            var isStatic = method.IsStatic;

            var instance = System.Linq.Expressions.Expression.Parameter(typeof(Interpreter), "instance");
            var parameter = System.Linq.Expressions.Expression.Parameter(typeof(QxExpression), "expression");
            var castParameter = System.Linq.Expressions.Expression.Convert(parameter, parameterType);
            var call = System.Linq.Expressions.Expression.Call(isStatic ? null : instance, method, castParameter);
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<Interpreter, QxExpression, Value>>(call, instance, parameter);

            action = lambda.Compile();

            return true;
        }
    }
}
