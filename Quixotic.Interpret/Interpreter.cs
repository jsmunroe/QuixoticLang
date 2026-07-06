using Quixotic.Analysis.Errors;
using Quixotic.Analysis.Semantics;
using Quixotic.Common.Contracts;
using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Expressions;
using Quixotic.Common.Operations;
using Quixotic.Common.Source;
using Quixotic.Common.Statements;
using Quixotic.Common.Types;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;
using Quixotic.Common.Utilities;
using Quixotic.Interpret.Expressions;
using Quixotic.Interpret.FlowControl;
using Quixotic.Parsing;
using Quixotic.Runtime.Contracts;
using Quixotic.Runtime.Environment;
using Quixotic.Runtime.Instances;
using Quixotic.Runtime.References;
using Quixotic.Runtime.Symbols;
using Quixotic.Runtime.Values;
using QuixoticLang.Lexer;

namespace Quixotic.Interpret
{
    public class Interpreter(IRuntime runtime, IConsoleWriter? consoleWriter = null)
    {
        private readonly static MethodIndexer<Action<Interpreter, QxStatement>> _executes = new(typeof(Interpreter), "Execute");

        private readonly static MethodIndexer<Func<Interpreter, QxExpression, Instance>> _evaluates = new(typeof(Interpreter), "Evaluate");

        private readonly IConsoleWriter _consoleWriter = consoleWriter ?? new ConsoleWriter();

        public Interpreter(IConsoleWriter? consoleWriter = null)
            : this(new Runtime.Environment.Runtime(), consoleWriter)
        { }

        public void Execute(string source)
        {
            Execute(new StringSource(source));
        }

        public void Execute(Stream source)
        {
            Execute(StringSource.FromStream(source));
        }

        private void Execute(ISource source)
        {
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);

            var formatter = new ErrorMessageFormatter();

            try
            {
                var statements = parser.Parse();

                var analyzer = new SemanticAnalyzer();
                analyzer.Analyze(statements);

                foreach (var issue in analyzer.Issues)
                {
                    var message = formatter.Describe(issue, source);
                    message.Output(_consoleWriter);
                }

                if (analyzer.Errors.Any())
                    return;

                Execute(statements);
            }
            catch (Exception ex)
            {
                var message = formatter.Describe(ex, source);
                message.Output(_consoleWriter);
            }
        }

        public void Execute(IEnumerable<QxStatement> statements)
        {
            // Executes statements in global space

            foreach (var statement in statements)
                Execute(statement);
        }

        public IEnumerable<Instance> Evaluate(IEnumerable<QxExpression> expressions)
        {
            foreach (var expression in expressions)
                yield return Evaluate(expression);
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

        public Instance Evaluate(Function function, List<Argument> arguments)
        {
            runtime.PushFunction();

            foreach (var argument in arguments)
                runtime.Frame.Scope.DefineVariable(argument.Name, argument.Value);

            try
            {
                foreach (var statement in function.Body)
                    Execute(statement);
            }
            catch (ReturnException returnException)
            {
                runtime.Pop();
                return returnException.Value;
            }
            catch (FlowControlException)
            {
                runtime.Pop(); // Need to pop out of runtime frame since we are returning. Returned value will be caught higher up the stack.
                throw;
            }

            runtime.Pop();
            return Instance.Void;
        }

        public void Execute(QxStatement statement)
        {
            var statementType = statement.GetType();
            if (!_executes.TryGetDelegate(statementType, out var action))
                throw new NotSupportedException($"Unsupported statement type: {statementType.Name}");

            action(this, statement);
        }

        private Instance Evaluate(QxExpression expression)
        {
            var expressionType = expression.GetType();
            if (!_evaluates.TryGetDelegate(expressionType, out var function))
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

            var returnType = QxType.Parse(statement.ReturnType);

            var function = new Function(statement.Body, returnType) { Parameters = [.. parameters] };
            runtime.Frame.Scope.DefineFunction(statement.Name, function);
        }

        public void Execute(QxFunctionCallStatement statement)
        {
            Evaluate(statement.Call);
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

                if (targetValue is ArrayReference array && indexValue is NumberValue indexNumber)
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
            var iterator = statement.Iterator;

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

                try
                {
                    Execute(statement.Block, RuntimeFrameType.Loop, [new(iterator, new NumberValue(i))]);
                }
                catch (BreakException)
                {
                    break;
                }
                catch (ContinueException)
                {
                    continue;
                }
            }
        }

        public void Execute(QxForInStatement statement)
        {
            var iterator = statement.Iterator;

            var collection = Evaluate(statement.Collection);

            if (collection is not ArrayReference array)
                throw new TypeMismatchException(collection.Type, QxType.Array(QxType.Any));

            foreach (var item in array.Elements)
                Execute(statement.Block, RuntimeFrameType.Loop, [new(iterator, item)]);
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

            var baseType = Instance.GetCommonBase(elements);

            return new ArrayReference(baseType, elements);
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

            if (target is ArrayReference array)
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

            try
            {
                var result = InvokeOperator(expression.Operator, left, right);
                return result;
            }
            catch
            {
                var operatorValue = OperationMetadata.GetOperatorValue(expression.Operator) ?? "unknown";
                throw new BinaryOperatorException(left, operatorValue, right);
            }
        }

        protected Instance Evaluate(QxFunctionCallExpression expression)
        {
            var name = expression.Name;

            var argumentValues = Evaluate(expression.Arguments);

            var function = runtime.Frame.Scope.GetFunction(name, [.. argumentValues.GetTypes()]);

            var arguments = BindArguments(name, function.Parameters, [.. argumentValues]);

            return Evaluate(function, arguments);
        }

        protected Instance Evaluate(QxExternalCallExpression expression)
        {
            var arguments = Evaluate(expression.Arguments);

            var result = expression.Invoke([.. arguments]);
            return result;
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

        private Instance InvokeOperator(Operator op, QxExpression left, QxExpression right)
        {
            if (op == Operator.And)
                return And(left, right);

            if (op == Operator.Or)
                return Or(left, right);

            if (op == Operator.Dot)
                return InvokeMember(left, right);

            var name = OperationMetadata.GetOperatorValue(op);

            if (string.IsNullOrEmpty(name))
                throw new NotSupportedException($"Operator {op} is not supported.");

            var leftValue = Evaluate(left);
            var rightValue = Evaluate(right);

            var function = runtime.Frame.Scope.GetFunction(name, leftValue.Type, rightValue.Type);

            var arguments = BindArguments(name, function.Parameters, [leftValue, rightValue]);

            return Evaluate(function, arguments);
        }

        private Instance InvokeMember(QxExpression thisExpression, QxExpression memberExpression)
        {
            var thisInstance = Evaluate(thisExpression);

            if (memberExpression is QxIdentifierExpression)
            {

            }

            var member = Evaluate(memberExpression);

            return Instance.Nada;
        }

        private Instance InvokePropertyGetter(Instance instance, string propertyName)
        {

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

        public List<Argument> BindArguments(string name, List<Parameter> parameters, List<Instance> instances)
        {
            if (parameters.Count != instances.Count)
                throw new ParameterCountException(name, parameters.Count, instances.Count);

            List<Argument> arguments = [];

            // Push function parameters into 
            foreach (var (parameter, instance) in parameters.Zip(instances))
            {
                if (!parameter.Type.IsAssignableFrom(instance.Type))
                    throw new TypeMismatchException(instance.Type, parameter.Type);

                arguments.Add(new Argument(parameter.Name, instance));
            }

            return arguments;
        }
    }
}
