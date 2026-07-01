using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Expressions;
using Quixotic.Common.Operations;
using Quixotic.Common.Statements;
using Quixotic.Interpret.Contracts;
using Quixotic.Interpret.Environment;
using Quixotic.Interpret.FlowControl;
using Quixotic.Interpret.Symbols;
using Quixotic.Interpret.Symbols.Instances;
using Quixotic.Interpret.Symbols.Types;
using Quixotic.Interpret.Symbols.Values;
using Quixotic.Parsing;
using QuixoticLang.Lexer;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Quixotic.Interpret
{
    public class Interpreter(IRuntime runtime)
    {
        private readonly static Dictionary<Type, Action<Interpreter, QxStatement>> _executeMap = [];

        private readonly static Dictionary<Type, Func<Interpreter, QxExpression, Instance>> _evaluateMap = [];

        static Interpreter() => LoadMapEntries();

        public void Execute(string source)
        {
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var statements = parser.Parse();

            Execute(statements);
        }

        public void Execute(Stream source)
        {
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var statements = parser.Parse();

            Execute(statements);
        }

        public void Execute(IEnumerable<QxStatement> statements)
        {
            // Executes statements in global space

            foreach (var statement in statements)
                Execute(statement);
        }

        public void Execute(Block statements, RuntimeFrameType frameType, List<Argument>? arguments = null)
        {
            runtime.PushBlock(frameType);

            if (arguments is not null)

                foreach (var argument in arguments)
                    runtime.Frame.Scope.DefineVariable(argument.Name, argument.Value);

            try
            {
                foreach (var statement in statements)
                    Execute(statement);
            }
            catch (FlowControlException)
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
            catch (FlowControlException)
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

        private Instance Evaluate(QxExpression expression)
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
            Instance value = Instance.Nada;
            if (statement.Value is not null)
                value = Evaluate(statement.Value);

            QxType? type = null;
            if (statement.TypeName is not null)
            {
                if (!QxType.TryParse(statement.TypeName, out type))
                    throw new UnrecognizedTypeException(statement.TypeName);
            }

            if (!value.IsNada && type is not null)
            {
                if (!type.IsAssignableFrom(value.Type))
                    throw new TypeMismatchException(value.Type, type);

                runtime.Frame.Scope.DefineVariable(statement.Name, value);
            }
            else if (!value.IsNada)
            {
                runtime.Frame.Scope.DefineVariable(statement.Name, value);
            }
            else if (type is not null)
            {
                runtime.Frame.Scope.DefineVariable(statement.Name, type);
            }
            else
            {
                throw new UntypedVariableDeclarationException();
            }
        }

        public void Execute(QxFunctionDeclarationStatement statement)
        {
            List<Parameter> parameters = [];
            foreach (var parameter in statement.Parameters)
            {
                if (!QxType.TryParse(parameter.TypeName, out var type))
                    throw new UnrecognizedTypeException(parameter.TypeName);

                parameters.Add(new(parameter.Name, type));
            }

            var function = new Function(statement.Body) { Parameters = [.. parameters] };
            runtime.Frame.Scope.DefineFunction(statement.Name, function);
        }

        public void Execute(QxFunctionCallStatement statement)
        {
            var call = statement.Call;

            var name = call.Name;

            var function = runtime.Frame.Scope.GetFunction(name);

            var arguments = BindArguments(name, function, call.Arguments);

            try
            {
                Execute(function, arguments);
            }
            catch (ReturnException) { } // Ignore returned value;
        }

        public void Execute(QxReturnStatement statement)
        {
            Instance? instance = null;
            if (statement.Expression is not null)
                instance = Evaluate(statement.Expression);

            throw new ReturnException(instance);
        }

        public static void Execute(QxContinueStatement statement)
        {
            throw new ContinueException();
        }

        public static void Execute(QxBreakStatement statement)
        {
            throw new BreakException();
        }

        public void Execute(QxAssignmentStatement statement)
        {
            var value = Evaluate(statement.Value);

            if (statement.Target is QxIdentifierExpression identifierExpression)
            {
                var name = identifierExpression.Name;

                runtime.Frame.Scope.AssignVariable(name, value);
            }
            else if (statement.Target is QxIndexerExpression indexerExpression)
            {
                var targetValue = Evaluate(indexerExpression.Target);
                var indexValue = Evaluate(indexerExpression.Index);

                if (targetValue is ArrayInstance array && indexValue is NumberValue indexNumber)
                    array.Set(indexNumber, value);
                else
                    throw new IndexerTargetException(targetValue.Type);
            }
            else
            {
                var target = Evaluate(statement.Target);
                throw new InvalidAssignmentTargetException(target.Type.Name);
            }
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

        public void Execute(QxForStatement statement)
        {
            var iterator = statement.Iterator.Name;

            var from = ExpectType<NumberValue>(Evaluate(statement.From));
            var to = ExpectType<NumberValue>(Evaluate(statement.To));

            double start() => from.Value;

            bool check(double i)
            {
                if (from.Value < to.Value)
                    return i <= to.Value;

                return i >= to.Value;
            }

            NumberValue step;
            double iterate(double i) => i += step.Value;

            for (var i = start(); check(i); i = iterate(i))
            {
                from = ExpectType<NumberValue>(Evaluate(statement.From));
                to = ExpectType<NumberValue>(Evaluate(statement.To));
                step = ExpectType<NumberValue>(Evaluate(statement.Step));

                Execute(statement.Block, RuntimeFrameType.ForBlock, [new(iterator, new NumberValue(i))]);
            }
        }

        public void Execute(QxDoStatement statement)
        {
            while (true)
            {
                if (statement.EntryControlled && !IsTruthy(Evaluate(statement.Condition)))
                    break;

                try
                {
                    Execute(statement.Block, RuntimeFrameType.Loop);
                }
                catch (BreakException)
                {
                    break;
                }
                catch (ContinueException)
                {
                    continue;
                }

                if (!statement.EntryControlled && !IsTruthy(Evaluate(statement.Condition)))
                    break;
            }
        }

        protected static Instance Evaluate(QxNumberLiteralExpression expression)
        {
            return new NumberValue(expression.Value);
        }

        protected static Instance Evaluate(QxStringLiteralExpression expression)
        {
            return new StringValue(expression.Value);
        }

        protected static Instance Evaluate(QxBooleanLiteralExpression expression)
        {
            return new BooleanValue(expression.Value);
        }

        protected Instance Evaluate(QxArrayExpression expression)
        {
            List<Instance> elements = [.. expression.Elements.Select(Evaluate)];

            var baseType = QxType.GetCommonBase(elements);

            return new ArrayInstance(baseType, elements);
        }

        protected Instance Evaluate(QxIdentifierExpression expression)
        {
            var name = expression.Name;
            var value = runtime.Frame.Scope.GetInstance(name);

            return value;
        }

        protected Instance Evaluate(QxIndexerExpression expression)
        {
            var target = Evaluate(expression.Target);

            var index = Evaluate(expression.Index);

            if (target is ArrayInstance array)
            {
                if (index is not NumberValue number)
                    throw new IndexTypeException(array.Type, index.Type);

                return array.Get(number);
            }

            throw new IndexerTargetException(target.Type);
        }

        protected Instance Evaluate(QxUnaryExpression expression)
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

        protected Instance Evaluate(QxBinaryExpression expression)
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

                _ => throw new BinaryOperatorException(left, operatorValue, right),
            };
        }

        protected Instance Evaluate(QxFunctionCallExpression expression)
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

        private static bool IsTruthy(Instance instance)
        {
            return instance.IsTruthy();
        }

        private TValue ExpectType<TValue>(Instance instance)
            where TValue : Instance
        {
            if (instance is not TValue expectedValue)
                throw new UnexpectedTypeException(typeof(TValue).Describe(), instance.Type);

            return expectedValue;
        }

        private Instance Add(QxExpression left, QxExpression right)
        {
            var leftValue = Evaluate(left);
            var rightValue = Evaluate(right);

            return leftValue.Add(rightValue);
        }

        private Instance Subtract(QxExpression left, QxExpression right)
        {
            var leftValue = Evaluate(left);
            var rightValue = Evaluate(right);

            return leftValue.Subtract(rightValue);
        }

        private Instance Multiply(QxExpression left, QxExpression right)
        {
            var leftValue = Evaluate(left);
            var rightValue = Evaluate(right);

            return leftValue.Multiply(rightValue);
        }

        private Instance Divide(QxExpression left, QxExpression right)
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
                var instance = Evaluate(expression);

                if (!parameter.Type.IsAssignableFrom(instance.Type))
                    throw new TypeMismatchException(instance.Type, parameter.Type);

                arguments.Add(new Argument(parameter.Name, instance));
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

        private static bool TryCreateEvaluateMapEntry(MethodInfo method, [NotNullWhen(true)] out Func<Interpreter, QxExpression, Instance>? action)
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
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<Interpreter, QxExpression, Instance>>(call, instance, parameter);

            action = lambda.Compile();

            return true;
        }
    }
}
