using Quixotic.Interpret.Contracts;
using Quixotic.Interpret.Environment;
using Quixotic.Interpret.Exceptions;
using Quixotic.Interpret.Values;
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
            runtime.Push(RuntimeFrameType.Global);

            foreach (var statement in statements)
                Execute(statement);

            runtime.Pop();
        }

        public void Execute(IEnumerable<QxStatement> statements, RuntimeFrameType frameType)
        {
            runtime.Push(frameType);

            foreach (var statement in statements)
                Execute(statement);

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

        public void Execute(QxAssignmentStatement statement)
        {
            var value = Evaluate(statement.Value);
            var name = statement.Target.Name;
            runtime.Frame[name] = value;
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

        protected static Value Evaluate(QxNumberLiteralExpression expression)
        {
            return new NumberValue(expression.Value);
        }

        protected static Value Evaluate(QxStringLiteralExpression expression)
        {
            return new StringValue(expression.Value);
        }

        protected Value Evaluate(QxIdentifierExpression expression)
        {
            var name = expression.Name;
            var value = runtime.Frame[name];

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
            var left = Evaluate(expression.Left);
            var right = Evaluate(expression.Right);

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

                _ => throw new BinaryOperatorException(left.Type, operatorValue, right.Type),
            };
        }

        private static bool IsTruthy(Value value)
        {
            return value.IsTruthy();
        }

        private static Value Add(Value left, Value right)
        {
            return left.Add(right);
        }

        private static Value Subtract(Value left, Value right)
        {
            return left.Subtract(right);
        }

        private static Value Multiply(Value left, Value right)
        {
            return left.Multiply(right);
        }

        private static Value Divide(Value left, Value right)
        {
            return left.Divide(right);
        }

        private static BooleanValue IsEqualTo(Value left, Value right)
        {
            return left.IsEqualTo(right);
        }

        private static BooleanValue IsNotEqualTo(Value left, Value right)
        {
            return left.IsEqualTo(right).Not();
        }

        private static BooleanValue IsLessThan(Value left, Value right)
        {
            return left.IsLessThan(right);
        }

        private static BooleanValue IsLessThanOrEqualTo(Value left, Value right)
        {
            return left.IsLessThanOrEqualTo(right);
        }

        private static BooleanValue IsGreaterThan(Value left, Value right)
        {
            return left.IsGreaterThan(right).Or(left.IsEqualTo(right));
        }

        private static BooleanValue IsGreaterThanOrEqualTo(Value left, Value right)
        {
            return left.IsGreaterThanOrEqualTo(right);
        }

        private static BooleanValue And(Value left, Value right)
        {
            return left.And(right);
        }

        private static BooleanValue Or(Value left, Value right)
        {
            return left.Or(right);
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
