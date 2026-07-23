using Quixotic.Analysis.Errors;
using Quixotic.Analysis.Extensions;
using Quixotic.Analysis.Semantics;
using Quixotic.Common.Contracts;
using Quixotic.Common.Environment;
using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Expressions;
using Quixotic.Common.Operations;
using Quixotic.Common.Source;
using Quixotic.Common.Statements;
using Quixotic.Common.Symbols;
using Quixotic.Common.Symbols.Functions;
using Quixotic.Common.Syntax.Casing;
using Quixotic.Common.Tokens;
using Quixotic.Common.Types;
using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;
using Quixotic.Common.Utilities;
using Quixotic.Interpret.FlowControl;
using Quixotic.Parsing;
using Quixotic.Runtime.Contracts;
using Quixotic.Runtime.Environment;
using Quixotic.Runtime.References;
using Quixotic.Runtime.Values;
using QuixoticLang.Lexer;

namespace Quixotic.Interpret
{
    public class Interpreter(IRuntime runtime, IConsoleWriter? consoleWriter = null)
    {
        private readonly static MethodIndexer<Action<Interpreter, QxStatement>, QxStatement> _executes = new(typeof(Interpreter), "Execute");

        private readonly static MethodIndexer<Func<Interpreter, QxExpression, Instance>, QxExpression> _evaluates = new(typeof(Interpreter), "Evaluate");

        private readonly IConsoleWriter _consoleWriter = consoleWriter ?? new ConsoleWriter();

        public Interpreter(IConsoleWriter? consoleWriter = null)
            : this(new Runtime.Environment.Runtime(), consoleWriter)
        { }

        public Scope Scope => runtime.Frame.Scope;

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
                var session = parser.ParseSession();

                var analyzer = new SemanticAnalyzer();
                analyzer.Analyze(session);

                foreach (var issue in analyzer.Issues)
                {
                    var message = formatter.Describe(issue, source);
                    message.Output(_consoleWriter);
                }

                if (analyzer.Errors.Any())
                    return;

                Execute(session.Root);
            }
            catch (Exception ex)
            {
                var message = formatter.Describe(ex, source);
                message.Output(_consoleWriter);

                throw new ExecutionException(message.ToString(), ex);
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

        public void Execute(Block statements, List<Argument>? arguments = null)
        {
            runtime.PushBlock();

            if (arguments is not null)
            {
                foreach (var argument in arguments)
                    Scope.DefineVariable(argument.Name, argument.Value);
            }

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

        public void Evaluate(BoundConstructor constructor, List<Argument> arguments, Instance instance, ScopeState? scopeState = null)
        {
            runtime.PushFunction(constructor);

            foreach (var argument in arguments)
                Scope.DefineVariable(argument.Name, argument.Value);

            if (scopeState is not null)
                Scope.Add(scopeState);

            if (constructor.Base is BaseConstructor baseConstructor)
                Evaluate(baseConstructor, instance);

            foreach (var statement in constructor.Body)
                Execute(statement);

            runtime.Pop();
        }

        public void Evaluate(BaseConstructor baseConstructor, Instance instance)
        {
            var type = baseConstructor.Type;

            Instance[] argumentValues = [.. Evaluate(baseConstructor.Arguments)];

            if (!type.TryResolveConstructor([.. argumentValues.GetTypes()], out var constructor))
                throw new UndefinedMethodException(type, $"{type}::constructor", Span.Empty); // TODO: Figure out actual Span

            var boundConstructor = constructor.Bind(instance);

            var arguments = boundConstructor.BindArguments($"{type}::constructor", argumentValues);

            Evaluate(boundConstructor, arguments);
        }

        public Instance Evaluate(Function function, List<Argument> arguments, ScopeState? scopeState = null)
        {
            if (function is BindableFunction)
                throw new UnboundFunctionException(function);

            runtime.PushFunction(function);

            if (scopeState is not null)
                Scope.Add(scopeState);

            foreach (var argument in arguments)
                Scope.DefineVariable(argument.Name, argument.Value);

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
            return QxType.Void;
        }

        public void Execute(QxStatement statement)
        {
            try
            {
                var statementType = statement.GetType();
                if (!_executes.TryGetMethod(statementType, out var action))
                    throw new NotSupportedException($"Unsupported statement type: {statementType.Name}");

                action(this, statement);
            }
            catch (FlowControlException)
            {
                throw;
            }
            catch (Exception ex) when (ex is not IHasSpan)
            {
                throw new StatementException(ex, statement);
            }
        }

        private Instance Evaluate(QxExpression expression)
        {
            try
            {
                var expressionType = expression.GetType();
                if (!_evaluates.TryGetMethod(expressionType, out var function))
                    throw new NotSupportedException($"Unsupported expression type: {expressionType.Name}");

                return function(this, expression);
            }
            catch (FlowControlException)
            {
                throw;
            }
            catch (Exception ex) when (ex is not IHasSpan)
            {
                throw new ExpressionException(ex, expression);
            }
        }

        public void Execute(QxPrintStatement statement)
        {
            var value = Evaluate(statement.Expression);
            runtime.ExecutePrint(value);
        }

        public void Execute(QxVariableDeclarationStatement statement)
        {
            Instance value = QxType.Nada;
            if (statement.Value is not null)
                value = Evaluate(statement.Value);

            QxType? declaredType = null;
            if (statement.TypeName is not null)
            {
                if (CaseRule.Current.Equals(statement.TypeName, "function")) // Defer a type function without any arguments or return type
                    declaredType = new DeferredType($"Function type was given without arguments or return type.", ContextDependency.VariableAssignment) { NecessaryBaseType = QxType.Function };

                else if (!Scope.TryGetType(statement.TypeName, out declaredType))
                    throw new UnrecognizedTypeException(statement.TypeName, statement.Span);
            }

            if (!QxType.IsNada(value) && declaredType is not null)
            {
                if (!declaredType.IsAssignableFrom(value.Type))
                    throw new TypeMismatchException(value.Type, declaredType, statement.Value?.Span ?? statement.Span);

                Scope.DefineVariable(statement.Name, value);
            }
            else if (!QxType.IsNada(value))
            {
                Scope.DefineVariable(statement.Name, value);
            }
            else if (declaredType is not null)
            {
                Scope.DefineVariable(statement.Name, declaredType);
            }
            else
            {
                throw new UntypedVariableDeclarationException();
            }
        }

        public void Execute(QxFunctionDeclarationStatement statement)
        {
            var functionReference = Evaluate(statement.Expression);

            Scope.DefineFunction(statement.Name, functionReference.Function);
        }

        public void Execute(QxFunctionCallStatement statement)
        {
            Evaluate(statement.Call);
        }

        public void Execute(QxMethodCallStatement statement)
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

        public void Execute(QxExternalCallStatement statement)
        {
            Evaluate(statement.Expression);
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

                Scope.AssignVariable(name, value);
            }
            else if (statement.Target is QxIndexerExpression indexerExpression)
            {
                var targetValue = Evaluate(indexerExpression.Target);
                var indexValue = Evaluate(indexerExpression.Index);

                if (targetValue is not ArrayReference array || indexValue is not NumberValue indexNumber)
                {
                    throw new IndexerTargetException(targetValue.Type, indexerExpression.Span);
                }
                else
                {
                    if (!array.ElementType.IsAssignableFrom(value.Type))
                        throw new TypeMismatchException(value.Type, array.ElementType, indexerExpression.Span);

                    array.Set((int)indexNumber.Value, value);
                }
            }
            else
            {
                var target = Evaluate(statement.Target);
                throw new InvalidAssignmentTargetException(target.Type.Name, statement.Span);
            }
        }

        public void Execute(QxIfStatement statement)
        {
            if (IsTruthy(Evaluate(statement.Condition)))
            {
                Execute(statement.ThenBlock);
                return;
            }

            foreach (var elseIfClasue in statement.ElseIfClauses)
            {
                if (IsTruthy(Evaluate(elseIfClasue.Condition)))
                {
                    Execute(elseIfClasue.Block);
                    return;
                }
            }

            Execute(statement.ElseBlock);
        }

        public void Execute(QxForStatement statement)
        {
            var iterator = statement.Iterator;

            var from = ExpectNumberValue(statement.From);
            var to = ExpectNumberValue(statement.To);

            double start() => from;

            bool check(double i)
            {
                if (from < to)
                    return i <= to;

                return i >= to;
            }

            double step = statement.Step is null ? 1 : ExpectNumberValue(statement.Step);

            double iterate(double i) => i += step;

            for (var i = start(); check(i); i = iterate(i))
            {
                try
                {
                    Execute(statement.Block, [new(iterator, new NumberValue(i))]);
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

            if (collection is not CollectionReference array)
                throw new TypeMismatchException(collection.Type, QxType.Array.MakeGenericType(QxType.Any), statement.Collection.Span);

            foreach (var item in array.Elements)
                Execute(statement.Block, [new(iterator, item)]);
        }

        public void Execute(QxDoStatement statement)
        {
            while (true)
            {
                if (statement.IsEntryControlled && !IsTruthy(Evaluate(statement.Condition)))
                    break;

                try
                {
                    Execute(statement.Block);
                }
                catch (BreakException)
                {
                    break;
                }
                catch (ContinueException)
                {
                    continue;
                }

                if (!statement.IsEntryControlled && !IsTruthy(Evaluate(statement.Condition)))
                    break;
            }
        }

        public void Execute(QxTypeDeclarationStatement statement)
        {
            var baseName = statement.BaseName;

            var baseType = runtime.Frame.GlobalScope.GetType(baseName) ?? QxType.Any;

            var type = new DefinedType(statement.Name, baseType);

            runtime.PushType(type);

            foreach (var childStatement in statement.Body)
                Execute(childStatement);

            foreach (var functionSymbol in Scope.Functions)
                type.RegisterMethod(functionSymbol.Name, functionSymbol.Function);

            foreach (var variableSymbol in Scope.Variables)
            {
                if (!variableSymbol.Instance.IsNada)
                    type.RegisterProperty(variableSymbol.Name, variableSymbol.Instance);
                else
                    type.RegisterProperty(variableSymbol.Name, variableSymbol.Type);
            }

            runtime.Pop();

            runtime.Frame.GlobalScope.DefineType(type.Name, type);
        }

        public void Execute(QxImportStatement statement)
        {
            Scope.Import(statement.Namespace);
        }

        public void Execute(QxConstructorDeclarationStatement statement)
        {
            if (runtime.Frame is not TypeRuntimeFrame frame)
                throw new ConstructorOutsideOfTypeException();

            List<Parameter> parameters = [];
            foreach (var parameter in statement.Parameters)
            {
                if (!Scope.TryGetType(parameter.TypeName, out var parameterType))
                    throw new UnrecognizedTypeException(parameter.TypeName, statement.Span);

                parameters.Add(new(parameter.Name, parameterType));
            }

            BaseConstructor? baseConstructorCall = null;
            if (statement.BaseCall is QxBaseConstructorCallExpression baseConstructorCallExpression)
                baseConstructorCall = Evaluate(baseConstructorCallExpression);

            var constructor = new Constructor(frame.Type, statement.Body)
            {
                Parameters = parameters,
                Base = baseConstructorCall,
            };

            Scope.DefineConstructor(constructor);
        }

        public BaseConstructor Evaluate(QxBaseConstructorCallExpression expression)
        {
            if (runtime.Frame is not TypeRuntimeFrame frame)
                throw new ConstructorOutsideOfTypeException();

            var thisType = frame.Type;
            var baseType = thisType.BaseType ?? throw new BaseCallOnTypeWithoutBaseTypeException(thisType, expression.Span);

            return new BaseConstructor(baseType, expression.Span)
            {
                Arguments = expression.Arguments,
            };
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

            return new ArrayReference(baseType, [.. elements]);
        }

        protected Instance Evaluate(QxSetExpression expression)
        {
            List<Instance> elements = [.. expression.Elements.Select(Evaluate)];

            var baseType = QxType.GetCommonBase(elements);

            return new SetReference(baseType, [.. elements]);
        }

        protected Instance Evaluate(QxIdentifierExpression expression)
        {
            var name = expression.Name;

            if (Scope.TryGetInstance(name, out var instance))
                return instance;

            if (Scope.TryGetType(name, out var type))
                return QxType.Meta(type).Construct();

            var functions = Scope.GetFunctionsByName(name);
            if (functions.Count == 1)
                return QxType.Function.Construct(functions[0]);

            throw new UndefinedIdentifierException(name, expression.Span);
        }

        protected Instance Evaluate(QxIndexerExpression expression)
        {
            var target = Evaluate(expression.Target);

            var index = Evaluate(expression.Index);

            if (target is ArrayReference array)
            {
                if (index is not NumberValue number)
                    throw new IndexTypeException(array.Type, index.Type, expression.Target.Span);

                return array.Get(number);
            }

            throw new IndexerTargetException(target.Type, expression.Span);
        }

        protected Instance Evaluate(QxUnaryExpression expression)
        {
            var op = expression.Operator;

            return InvokeOperator(op, expression.Operand);
        }

        protected Instance Evaluate(QxBinaryExpression expression)
        {
            var left = expression.Left;
            var right = expression.Right;

            var result = InvokeOperator(expression.Operator, left, right);
            return result;
        }

        protected Instance Evaluate(QxIsComparisonExpression expression)
        {
            var instance = Evaluate(expression.Target);

            var type = Scope.GetType(expression.TypeName);

            if (instance.Is(type))
            {
                if (expression.PatternIdentifier is not null)
                {
                    Scope.DefineVariable(expression.PatternIdentifier, type);
                    Scope.AssignVariable(expression.PatternIdentifier, instance);
                }

                return BooleanType.True;
            }

            if (expression.PatternIdentifier is not null)
            {
                Scope.DefineVariable(expression.PatternIdentifier, type);
                Scope.AssignVariable(expression.PatternIdentifier, QxType.Nada);
            }

            return BooleanType.False;
        }

        protected FunctionReference Evaluate(QxFunctionExpression expression)
        {
            var function = EvaluateFunction(expression);

            var functionInstance = new FunctionReference(QxType.Function.Construct(function));

            return functionInstance;
        }

        protected FunctionReference Evaluate(QxLambdaFunctionExpression expression)
        {
            var function = new Function(EvaluateFunction(expression)) { Closure = Scope };

            var functionInstance = new FunctionReference(QxType.Function.Construct(function));

            return functionInstance;
        }

        private Function EvaluateFunction(QxFunctionExpression expression)
        {
            List<Parameter> parameters = [];
            foreach (var parameter in expression.Parameters)
            {
                if (!Scope.TryGetType(parameter.TypeName, out var type))
                    throw new UnrecognizedTypeException(parameter.TypeName, expression.Span);

                parameters.Add(new(parameter.Name, type));
            }

            var returnType = Scope.GetType(expression.ReturnType);

            Scope? closure = null;
            if (expression.WithClosure is not null)
                closure = Scope.Capture(expression.WithClosure);

            var function = new Function(expression.Body, returnType, CallType.Call)
            {
                Parameters = [.. parameters],
                Closure = closure,
            };

            return function;
        }

        protected Instance Evaluate(QxFunctionCallExpression expression)
        {
            var name = expression.Name;

            var argumentValues = Evaluate(expression.Arguments);

            if (!Scope.TryGetFunction(name, [.. argumentValues.GetTypes()], out var function))
            {
                if (Scope.TryGetInstance(name, out var instance) && instance.Type is FunctionType functionType)
                    function = functionType.GetFunction(instance);
                else
                    throw new UndefinedFunctionException(name, expression.Span);
            }

            var arguments = function.BindArguments(name, [.. argumentValues]);

            return Evaluate(function, arguments);
        }

        protected Instance Evaluate(QxMethodCallExpression expression)
        {
            var target = Evaluate(expression.Target);

            var name = expression.MethodName;

            Instance[] argumentValues = [.. Evaluate(expression.Arguments)];

            var type = target.Type;

            Function? method = null;

            if (type is QxMetaType metaType) // Static method call
            {
                if (!metaType.TypeReference.TryResolveMethod(name, [.. argumentValues.GetTypes()], out method))
                {
                    if (expression.Type == CallType.Call)
                        throw new UndefinedMethodException(metaType.TypeReference, name, expression.Span);
                    else if (expression.Type == CallType.OperatorCall)
                        throw new UndefinedOperatorException(metaType.TypeReference, name, expression.Span);
                    else
                        throw new UndefinedPropertyException(metaType.TypeReference, name, expression.Span);
                }
            }
            else
            {
                if (type is DynamicType dynamicType)
                {
                    if (expression.Type == CallType.Getter)
                        method = dynamicType.BuildPropertyGetter(target, name);
                    else if (expression.Type == CallType.Setter)
                        method = dynamicType.BuildPropertySetter(target, name, argumentValues[0].Type);
                    else
                        method = dynamicType.BuildMethodCaller(target, name, [.. argumentValues.GetTypes()]);
                }
                else if (!type.TryResolveMethod(name, [.. argumentValues.GetTypes()], out method))
                {
                    if (expression.Type == CallType.Call)
                        throw new UndefinedMethodException(target.Type, name, expression.Span);
                    else if (expression.Type == CallType.OperatorCall)
                        throw new UndefinedOperatorException(target.Type, name, expression.Span);
                    else
                        throw new UndefinedPropertyException(target.Type, name, expression.Span);
                }
            }

            method = method is BindableFunction bindableFunction ? bindableFunction.Bind(target) : method;

            var arguments = method.BindArguments(name, argumentValues);

            var scopeState = new ScopeState();
            if (target.Type.BaseType is not null)
            {
                var baseType = target.Type.BaseType;
                scopeState.Variables.Register("base", target.As(baseType));
            }

            return Evaluate(method, arguments, scopeState);
        }

        protected Instance Evaluate(QxExternalCallExpression expression)
        {
            var arguments = Evaluate(expression.Arguments);

            var result = expression.Invoke([.. arguments]);
            return result;
        }

        protected Instance Evaluate(QxConstructorCallExpression expression)
        {
            var type = Scope.GetType(expression.TypeName);

            var instance = type.Construct();

            Instance[] argumentValues = [.. Evaluate(expression.Arguments)];

            if (!type.TryResolveConstructor([.. argumentValues.GetTypes()], out var constructor))
            {
                if (argumentValues.Length == 0)
                    return instance;

                throw new UndefinedMethodException(type, $"{type}::constructor", expression.Span);
            }

            var boundConstructor = constructor.Bind(instance);

            var arguments = boundConstructor.BindArguments(argumentValues);

            Evaluate(boundConstructor, arguments, instance);

            return instance;
        }

        private static bool IsTruthy(Instance instance)
        {
            return instance.IsTruthy;
        }

        private double ExpectNumberValue(QxExpression expression)
        {
            var instance = Evaluate(expression);

            if (!instance.Type.Equals(QxType.Number))
                throw new UnexpectedTypeException(QxType.Number, instance.Type, expression.Span);

            return QxType.Number.Get(instance);
        }

        private Instance InvokeOperator(Operator op, QxExpression left)
        {
            if (op == Operator.New)
                return Evaluate(left);

            var name = OperationMetadata.GetOperatorValue(op);

            if (string.IsNullOrEmpty(name))
                throw new NotSupportedException($"Operator {op} is not supported.");

            var leftValue = Evaluate(left);

            var function = Scope.GetFunction(name, leftValue.Type);

            var arguments = function.BindArguments(name, [leftValue]);

            return Evaluate(function, arguments);
        }

        private Instance InvokeOperator(Operator op, QxExpression left, QxExpression right)
        {
            if (op == Operator.And)
                return And(left, right);

            if (op == Operator.Or)
                return Or(left, right);

            var name = OperationMetadata.GetOperatorValue(op);

            if (string.IsNullOrEmpty(name))
                throw new NotSupportedException($"Operator {op} is not supported.");

            var leftValue = Evaluate(left);
            var rightValue = Evaluate(right);

            if (!leftValue.Type.TryResolveMethod(name, [leftValue.Type, rightValue.Type], out var function)) // Check for static operator on left's type
            {
                if (!Scope.TryGetFunction(name, [leftValue.Type, rightValue.Type], out function)) // Check scope for operator
                    throw new UndefinedOperatorException(leftValue.Type, name, left.Span + right.Span);
            }

            var arguments = function.BindArguments(name, [leftValue, rightValue]);

            return Evaluate(function, arguments);
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
    }
}
