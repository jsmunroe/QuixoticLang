using Quixotic.Analysis.Contracts;
using Quixotic.Analysis.Environment;
using Quixotic.Analysis.Exceptions;
using Quixotic.Analysis.Extensions;
using Quixotic.Analysis.Sessions;
using Quixotic.Common.Analysis.Expressions;
using Quixotic.Common.Analysis.Statements;
using Quixotic.Common.Contracts;
using Quixotic.Common.Expressions;
using Quixotic.Common.Operations;
using Quixotic.Common.Statements;
using Quixotic.Common.Symbols;
using Quixotic.Common.Symbols.Functions;
using Quixotic.Common.Syntax.Casing;
using Quixotic.Common.Tokens;
using Quixotic.Common.Types;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;
using Quixotic.Common.Utilities;
using Quixotic.Parsing;

namespace Quixotic.Analysis.Semantics
{

    public class SemanticAnalyzer
    {
        private readonly static MethodIndexer<Func<SemanticAnalyzer, QxStatement, StatementInfo>, QxStatement> _statementIndexer = new(typeof(SemanticAnalyzer), "AnalyzeStatement");

        private readonly static MethodIndexer<Func<SemanticAnalyzer, QxExpression, ExpressionInfo>, QxExpression> _expressionIndexer = new(typeof(SemanticAnalyzer), "AnalyzeExpression");

        private IFrame Frame { get; set; } = new GlobalFrame();

        private SymbolTable Symbols => Frame.Symbols;

        public List<SemanticException> Issues { get; } = [];

        public IEnumerable<SemanticException> Errors => Issues.Where(issue => issue.Severity == Severity.Error);

        public IEnumerable<SemanticException> Warnings => Issues.Where(issue => issue.Severity == Severity.Warning);

        public ISource? Source { get; private set; }

        public SourceDatabase? SourceDatabase { get; private set; }

        public IEnumerable<StatementInfo> Analyze(Parser parser)
        {
            return Analyze(parser.ParseSession());
        }

        public IEnumerable<StatementInfo> Analyze(Session session)
        {
            Source = session.Source;
            SourceDatabase = session.SourceDatabase;

            foreach (var statement in session.Root)
            {
                StatementInfo? statementInfo = null;

                try
                {
                    statementInfo = AnalyzeStatement(statement);
                }
                catch (SemanticException ex)
                {
                    Issues.Add(ex);
                }

                if (statementInfo is not null)
                    yield return statementInfo;
            }
        }


        private void PushBlockFrame()
        {
            Frame = new BlockFrame(Frame);
        }

        private void PushLoopFrame()
        {
            Frame = new LoopFrame(Frame);
        }

        private void PushFunctionFrame(FunctionSymbol function, SymbolTable? otherState)
        {
            Frame = new FunctionFrame(Frame, function, otherState);

            foreach (var parameter in function.Function.Parameters)
            {
                if (!Symbols.TryDefineVariable(parameter.Name, parameter.Type, parameter.Type))
                    throw new AlreadyDefinedIdentifierException(parameter.Name, Span.Empty);

            }
        }

        private void PushTypeFrame(QxType type)
        {
            Frame = new TypeFrame(Frame, type);
        }

        private IFrame PopFrame()
        {
            if (Frame.Parent is null)
                throw new InvalidOperationException("Cannot pop the global frame.");

            var frame = Frame;

            Frame = Frame.Parent;

            return frame;
        }

        private List<StatementInfo> AnalyzeBlockStatements(Block block)
        {
            List<StatementInfo> statementInfos = [];

            foreach (var statement in block)
            {
                try
                {
                    statementInfos.Add(AnalyzeStatement(statement));
                }
                catch (SemanticException ex)
                {
                    Issues.Add(ex);
                }
            }

            return statementInfos;
        }

        private List<StatementInfo> AnalyzeBlock(Block block)
        {
            PushBlockFrame();

            var statements = AnalyzeBlockStatements(block);

            PopFrame();

            return statements;
        }

        private List<StatementInfo> AnalyzeLoopBlock(Block block, params IdentifierSymbol[] symbols)
        {
            PushLoopFrame();

            foreach (var symbol in symbols)
                Symbols.TryDefineVariable(symbol.Name, symbol.Type, symbol.ValueType);

            var statements = AnalyzeBlockStatements(block);

            PopFrame();

            return statements;
        }

        private List<StatementInfo> AnalyzeTypeBlock(Block block, QxType type)
        {
            PushTypeFrame(type);

            var statements = AnalyzeBlockStatements(block);

            PopFrame();

            return statements;
        }

        private List<StatementInfo> AnalyzeFunctionBlock(Block block, FunctionSymbol function, SymbolTable otherState)
        {
            PushFunctionFrame(function, otherState);

            var statements = AnalyzeBlockStatements(block);

            PopFrame();

            return statements;
        }

        /// <summary>
        /// Find and call statement analyze method for given statement.
        /// </summary>
        private StatementInfo AnalyzeStatement(QxStatement statement)
        {
            if (Frame.RedirectionType != FrameRedirectionType.None)
                Issues.Add(new UnreachableCodeException(statement.Span));

            if (!_statementIndexer.TryGetMethod(statement, out var action))
                throw new NotImplementedException($"No analyzer implemented for statement type: {statement.GetType().Name}");

            try
            {
                var statementInfo = action(this, statement);
                statement.Info = statementInfo;
                SourceDatabase?.Add(statementInfo);
            }
            catch (Exception ex)
            {
                statement.Info = new StatementErrorInfo(ex, statement);
                SourceDatabase?.Add(statement.Info);
                throw;
            }

            return statement.Info;
        }

        /// <summary>
        /// Find and call expression analyze method for given expression.
        /// </summary>
        private ExpressionInfo AnalyzeExpression(QxExpression expression)
        {
            if (!_expressionIndexer.TryGetMethod(expression, out var func))
                throw new NotImplementedException($"No analyzer implemented for expression type: {expression.GetType().Name}");

            try
            {
                var expressionInfo = func(this, expression);
                expression.Info = expressionInfo;
                SourceDatabase?.Add(expressionInfo);
            }
            catch (Exception ex)
            {
                expression.Info = new ExpressionErrorInfo(ex, expression);
                SourceDatabase?.Add(expression.Info);
                throw;
            }

            return expression.Info;
        }
        private IEnumerable<ExpressionInfo> AnalyzeExpressions(IEnumerable<QxExpression> expressions)
        {
            foreach (var expression in expressions)
                yield return AnalyzeExpression(expression);
        }

        protected StatementInfo AnalyzeStatement(QxPrintStatement statement)
        {
            // Analyze the expression to get its type
            var expressionType = AnalyzeExpression(statement.Expression);

            // Print statements don't have additional semantic checks

            return new PrintStatementInfo(statement)
            {
                Expression = expressionType
            };
        }

        protected StatementInfo AnalyzeStatement(QxVariableDeclarationStatement statement)
        {
            var name = statement.Name;

            var value = statement.Value is not null ? AnalyzeExpression(statement.Value) : null;

            QxType? declaredType = null;
            if (statement.TypeName is not null)
            {
                if (CaseRule.Current.Equals(statement.TypeName, "function")) // Defer a type function without any arguments or return type
                    declaredType = new DeferredType($"Function type was given without arguments or return type.", ContextDependency.VariableAssignment) { NecessaryBaseType = QxType.Function };

                else if (!Symbols.TryGetType(statement.TypeName, out declaredType))
                    throw new UnrecognizedTypeException(statement.TypeName, statement.Span);
            }

            var expressionType = value?.ExpressionType ?? declaredType;

            if (expressionType is null)
                throw new UntypedVariableDeclarationException(name, statement.Span);

            IdentifierSymbol? identifierSymbol = null;
            SignatureSymbol? getterSymbol = null;
            SignatureSymbol? setterSymbol = null;

            var typeFrame = Frame as TypeFrame;

            // Variable is member of type
            if (typeFrame is not null)
            {
                var type = typeFrame.Type;
                (getterSymbol, setterSymbol) = type.RegisterProperty(name, expressionType);
            }
            else if (value is not null)
            {
                if (!Symbols.TryDefineVariable(name, value.ExpressionType, value.ExpressionType, out identifierSymbol))
                    throw new AlreadyDefinedIdentifierException(name, statement.Span);
            }
            else if (declaredType is not null)
            {
                if (!Symbols.TryDefineVariable(name, declaredType, null, out identifierSymbol))
                    throw new AlreadyDefinedIdentifierException(name, statement.Span);

            }
            else
                throw new UntypedVariableDeclarationException(name, statement.Span);

            if (declaredType is not null && value is not null && !declaredType.IsAssignableFrom(value.ExpressionType))
                throw new AssignmentTypeMismatchException(name, declaredType, value.ExpressionType, statement.Span);

            return new VariableDeclarationStatementInfo(statement)
            {
                Name = name,
                Value = value,
                DeclaredType = declaredType,
                IdentifierSymbol = identifierSymbol,
                IsPropertyMember = typeFrame is not null,
                MemberOf = typeFrame?.Type,
                GetterSymbol = getterSymbol,
                SetterSymbol = setterSymbol,
            };
        }

        protected StatementInfo AnalyzeStatement(QxAssignmentStatement statement)
        {
            var target = AnalyzeExpression(statement.Target);

            var value = AnalyzeExpression(statement.Value);

            if (target is IdentifierExpressionInfo identifierExpressionInfo)
            {
                if (!Symbols.TryAssignVariable(identifierExpressionInfo.Name, value.ExpressionType))
                    throw new AssignmentTypeMismatchException(target.ExpressionType, value.ExpressionType, statement.Span);
            }
            else
            {
                if (!target.ExpressionType.IsAssignableFrom(value.ExpressionType))
                    throw new AssignmentTypeMismatchException(target.ExpressionType, value.ExpressionType, statement.Span);
            }

            return new AssignmentStatementInfo(statement)
            {
                Target = target,
                Value = value,
            };
        }

        protected StatementInfo AnalyzeStatement(QxIfStatement statement)
        {
            var ifCondition = AnalyzeExpression(statement.Condition);

            var thenStatements = AnalyzeBlock(statement.ThenBlock);

            List<ElseIfBlockInfo> elseIfBlocks = [];
            foreach (var elseIfClause in statement.ElseIfClauses)
            {
                var elseIfCondition = AnalyzeExpression(elseIfClause.Condition);

                var elseIfStatements = AnalyzeBlock(elseIfClause.Block);

                elseIfBlocks.Add(new ElseIfBlockInfo
                {
                    Condition = elseIfCondition,
                    Statements = [.. elseIfStatements],
                });
            }

            ElseBlockInfo? elseInfo = null;

            if (statement.ElseBlock is not null)
            {
                var elseStatements = AnalyzeBlock(statement.ElseBlock);

                elseInfo = new()
                {
                    Statements = [.. elseStatements],
                };
            }

            return new IfStatementInfo(statement)
            {
                Condition = ifCondition,
                IfStatements = [.. thenStatements],
                ElseIfBlocks = [.. elseIfBlocks],
                ElseBlock = elseInfo,
            };
        }

        protected StatementInfo AnalyzeStatement(QxDoStatement statement)
        {
            if (statement.IsEntryControlled && statement.IsExitControlled)
                throw new DoStatementHasDualConditionException(statement.Span);

            if (!statement.IsEntryControlled && !statement.IsExitControlled)
                throw new DoStatementMissingConditionException(statement.Span);

            var condition = AnalyzeExpression(statement.Condition);

            var statements = AnalyzeLoopBlock(statement.Block);

            return new DoStatementInfo(statement)
            {
                IsEntryControlled = statement.IsEntryControlled,
                IsExitControlled = statement.IsExitControlled,
                Condition = condition,
                BlockStatements = [.. statements],
            };
        }

        protected StatementInfo AnalyzeStatement(QxForStatement statement)
        {
            var iterator = statement.Iterator; // TODO: Validate iterator name

            var from = AnalyzeExpression(statement.From);

            if (from.ExpressionType is not NumberType)
                throw new ForLoopRangeTypeException(from, "from", statement.From.Span);

            var to = AnalyzeExpression(statement.To);

            if (to.ExpressionType is not NumberType)
                throw new ForLoopRangeTypeException(to, "to", statement.To.Span);

            var step = statement.Step is not null ? AnalyzeExpression(statement.Step) : null;

            if (step is not null && step.ExpressionType is not NumberType)
                throw new ForLoopRangeTypeException(step, "step", statement.Step!.Span);

            var statements = AnalyzeLoopBlock(statement.Block, new IdentifierSymbol(iterator, QxType.Number, QxType.Number));

            return new ForStatementInfo(statement)
            {
                IteratorName = iterator,
                From = from,
                To = to,
                Step = step,
                BlockStatements = [.. statements],
            };
        }

        protected StatementInfo AnalyzeStatement(QxForInStatement statement)
        {
            var iterator = statement.Iterator;

            var collection = AnalyzeExpression(statement.Collection);

            if (collection.ExpressionType is not CollectionType collectionType)
                throw new ForInLoopCollectionTypeException(collection.ExpressionType, statement.Collection.Span);

            var statements = AnalyzeLoopBlock(statement.Block, new IdentifierSymbol(iterator, collectionType.ElementType, collectionType.ElementType));

            return new ForInStatementInfo(statement)
            {
                IteratorName = iterator,
                Collection = collection,
                BlockStatements = [.. statements],
            };
        }

        protected StatementInfo AnalyzeStatement(QxFunctionDeclarationStatement statement)
        {
            var name = statement.Name;

            var call = (FunctionExpressionInfo)AnalyzeExpression(statement.Expression);

            var typeFrame = Frame as TypeFrame;

            // Variable is member of type
            if (typeFrame is not null)
            {
                var type = typeFrame.Type;

                type.RegisterMethod(name, new Function(Block.Empty, call.ReturnType, call.CallType)
                {
                    Parameters = [.. call.Parameters]
                });
            }
            else
            {
                if (!Symbols.TryDefineSignature(name, call.SignatureSymbol))
                    throw new AlreadyDefinedSignatureException(name, statement.Span);
            }

            return new FunctionDeclarationStatementInfo(statement)
            {
                Name = name,
                Parameters = call.Parameters,
                ReturnType = call.ReturnType,
                SignatureSymbol = call.SignatureSymbol,
                BodyStatements = call.BodyStatements,
            };
        }

        protected StatementInfo AnalyzeStatement(QxFunctionCallStatement statement)
        {
            var call = AnalyzeExpression(statement.Call);

            return new FunctionCallStatementInfo(statement)
            {
                Call = call,
            };
        }

        protected StatementInfo AnalyzeStatement(QxMethodCallStatement statement)
        {
            var call = AnalyzeExpression(statement.Call);

            return new MethodCallStatementInfo(statement)
            {
                Call = call,
            };
        }

        protected StatementInfo AnalyzeStatement(QxBreakStatement statement)
        {
            if (Frame.GetLoopFrame() is null)
                throw new BreakOutsideLoopException(statement.Span);

            Frame.RedirectionType = FrameRedirectionType.Break;

            return new BreakStatementInfo(statement);
        }

        protected StatementInfo AnalyzeStatement(QxContinueStatement statement)
        {
            if (Frame.GetLoopFrame() is null)
                throw new ContinueOutsideLoopException(statement.Span);

            Frame.RedirectionType = FrameRedirectionType.Continue;

            return new ContinueStatementInfo(statement);
        }

        protected StatementInfo AnalyzeStatement(QxReturnStatement statement)
        {
            var functionFrame = Frame.GetFunctionFrame() ?? throw new ReturnOutsideFunctionException(statement.Span);

            var returnValue = statement.Expression is null ? new VoidExpressionInfo(statement.Expression!) : AnalyzeExpression(statement.Expression);

            if (functionFrame.Function.ReturnType is DeferredType deferredType)
                deferredType.AddAlternative(returnValue.ExpressionType);
            else if (!functionFrame.Function.ReturnType.IsAssignableFrom(returnValue.ExpressionType))
                throw new ReturnTypeMismatchException(functionFrame.Function.Name, returnValue.ExpressionType, statement.Span);

            Frame.RedirectionType = FrameRedirectionType.Return;

            return new ReturnStatementInfo(statement)
            {
                ReturnValue = returnValue,
            };
        }

        protected StatementInfo AnalyzeStatement(QxTypeDeclarationStatement statement)
        {
            var name = statement.Name;

            if (!CaseRule.Current.TypeNames.IsMatch(name))
                Issues.Add(new TypeNameCasingException(name, statement.Span));

            var baseTypeName = statement.BaseName;

            if (!Symbols.TryGetType(baseTypeName, out var baseType))
                throw new UnrecognizedTypeException(baseTypeName, statement.Span);

            var type = new DefinedType(name, baseType);

            var statements = AnalyzeTypeBlock(statement.Body, type);

            if (!Symbols.TryDefineType(name, type))
                throw new AlreadyDefinedTypeException(name, statement.Span);

            return new TypeDeclarationStatementInfo(statement)
            {
                Type = type,
                BaseType = baseType,
                MemberStatements = [.. statements],
            };
        }

        protected StatementInfo AnalyzeStatement(QxConstructorDeclarationStatement statement)
        {
            if (Frame is not TypeFrame typeFrame)
                throw new ConstructorOutsideOfTypeException(statement.Span);

            List<Parameter> parameters = [];
            foreach (var parameter in statement.Parameters)
            {
                if (!Symbols.TryGetType(parameter.TypeName, out var parameterType))
                    throw new UnrecognizedTypeException(parameter.TypeName, statement.Span);

                parameters.Add(new Parameter(parameter.Name, parameterType));
            }

            BaseConstructorCallExpressionInfo? baseCall = statement.BaseCall is null ? null : (BaseConstructorCallExpressionInfo?)AnalyzeExpression(statement.BaseCall);

            var type = typeFrame.Type;

            var signatureSymbol = type.RegisterConstructor(new Constructor(type, statement.Body)
            {
                Base = baseCall?.BaseConstructor,
                Parameters = parameters,
            });

            return new ConstructorDeclarationStatementInfo(statement)
            {
                Type = typeFrame.Type,
                Parameters = parameters.AsReadOnly(),
                BaseCall = baseCall,
                SignatureSymbol = signatureSymbol,
            };
        }

        protected StatementInfo AnalyzeStatement(QxImportStatement statement)
        {
            Symbols.Import(statement.Namespace);

            return new ImportStatementInfo(statement)
            {
                Namespace = statement.Namespace
            };
        }

        protected ExpressionInfo AnalyzeExpression(QxNumberLiteralExpression expression)
        {
            return new LiteralExpressionInfo(QxType.Number, expression);
        }

        protected ExpressionInfo AnalyzeExpression(QxStringLiteralExpression expression)
        {
            return new LiteralExpressionInfo(QxType.String, expression);
        }

        protected ExpressionInfo AnalyzeExpression(QxBooleanLiteralExpression expression)
        {
            return new LiteralExpressionInfo(QxType.Boolean, expression);
        }

        protected ExpressionInfo AnalyzeExpression(QxArrayExpression expression)
        {
            var elements = AnalyzeExpressions(expression.Elements);
            var commonElementType = QxType.GetCommonBase(elements.GetTypes());

            return new ArrayExpressionInfo(commonElementType, expression)
            {
                Elements = [.. elements],
            };
        }

        protected ExpressionInfo AnalyzeExpression(QxSetExpression expression)
        {
            var elements = AnalyzeExpressions(expression.Elements);
            var commonElementType = QxType.GetCommonBase(elements.GetTypes());

            return new SetExpressionInfo(commonElementType, expression)
            {
                Elements = [.. elements],
            };
        }

        protected ExpressionInfo AnalyzeExpression(QxBinaryExpression expression)
        {
            var operatorValue = OperationMetadata.GetOperatorValue(expression.Operator) ?? throw new UnrecognizedOperatorException(expression.Operator, expression.Span);

            var left = AnalyzeExpression(expression.Left);
            var right = AnalyzeExpression(expression.Right);

            var signature = Symbols.GetSignature(operatorValue, left.ExpressionType, right.ExpressionType)
                ?? Symbols.GetSignatureFromType(left.ExpressionType, operatorValue, left.ExpressionType, right.ExpressionType)
                ?? throw new UnrecognizedFunctionSignatureException(new Signature(operatorValue, left.ExpressionType, right.ExpressionType), expression.Span);

            return new BinaryExpressionInfo(signature.ReturnType, expression)
            {
                Operator = expression.Operator,
                Left = left,
                Right = right,
                SignatureSymbol = signature,
            };
        }

        protected ExpressionInfo AnalyzeExpression(QxUnaryExpression expression)
        {
            if (expression.Operator == Operator.New)
                return AnalyzeExpression(expression.Operand);

            var operatorValue = OperationMetadata.GetOperatorValue(expression.Operator) ?? throw new UnrecognizedOperatorException(expression.Operator, expression.Span);

            var operand = AnalyzeExpression(expression.Operand);

            var signature = Symbols.GetSignature(operatorValue, operand.ExpressionType) ?? throw new UnrecognizedFunctionSignatureException(new Signature(operatorValue, operand.ExpressionType), expression.Span);

            return new UnaryExpressionInfo(signature.ReturnType, expression)
            {
                Operator = expression.Operator,
                Operand = operand,
                SignatureSymbol = signature,
            };
        }

        protected ExpressionInfo AnalyzeExpression(QxIdentifierExpression expression)
        {
            var name = expression.Name;

            QxType? variableType = null;

            if (Symbols.TryGetVariable(name, out var identifierSymbol))
                variableType = identifierSymbol.Type;
            else if (Symbols.TryGetSignatureByName(name, out var signatureIdentifier))
                variableType = QxType.Function.MakeFunctionType(signatureIdentifier.ReturnType, [.. signatureIdentifier.ParameterTypes.ToParameters()]);
            else if (Symbols.TryGetType(name, out var type))
                variableType = type;
            else
                throw new UnrecognizedIdentifierException(name, expression.Span);

            return new IdentifierExpressionInfo(variableType, expression)
            {
                Name = name,
            };
        }

        protected ExpressionInfo AnalyzeExpression(QxIndexerExpression expression)
        {
            var target = AnalyzeExpression(expression.Target);

            if (target.ExpressionType is not ArrayType arrayType)
                throw new InvalidIndexerTargetException(target, expression.Span);

            var index = AnalyzeExpression(expression.Index);

            if (index.ExpressionType != QxType.Number)
                throw new IndexTypeException(index, expression.Span);

            return new IndexerExpressionInfo(arrayType.ElementType, expression)
            {
                Target = target,
                Index = index,
            };
        }

        protected ExpressionInfo AnalyzeExpression(QxIsComparisonExpression expression)
        {
            var target = AnalyzeExpression(expression.Target);

            var typeName = expression.TypeName;

            if (!Symbols.TryGetType(typeName, out var type))
                throw new UnrecognizedTypeException(typeName, expression.Span);

            var result = target.ExpressionType.IsAssignableFrom(type);

            IdentifierSymbol? patternIdentifier = null;
            if (result && expression.PatternIdentifier is not null)
            {
                if (!Symbols.TryDefineVariable(expression.PatternIdentifier, type, type, out patternIdentifier))
                    throw new AlreadyDefinedIdentifierException(expression.PatternIdentifier, expression.Span);
            }

            return new IsComparisonExpressionInfo(expression)
            {
                Target = target,
                Type = type,
                Result = result,
                PatternIdentifier = patternIdentifier,
            };
        }

        protected ExpressionInfo AnalyzeExpression(QxFunctionCallExpression expression)
        {
            var name = expression.Name;

            var arguments = AnalyzeExpressions(expression.Arguments);

            var signature = new Signature(name, [.. arguments.GetTypes()]);

            QxType returnType;

            if (Symbols.TryGetSignature(name, [.. arguments.GetTypes()], out var signatureSymbol))
                returnType = signatureSymbol.ReturnType;
            else if (Symbols.TryGetVariable(name, out var identifierSymbol) && identifierSymbol.ValueType is FunctionType functionType)
                returnType = functionType.ReturnType;
            else
                throw new UnrecognizedFunctionSignatureException(signature, expression.Span);

            return new FunctionCallExpressionInfo(returnType, expression)
            {
                Name = name,
                Arguments = [.. arguments],
            };

        }

        protected ExpressionInfo AnalyzeExpression(QxFunctionExpression expression)
        {
            var typeFrame = Frame as TypeFrame;

            QxType? returnType;
            if (CaseRule.Current.Equals(expression.ReturnType, "function")) // Defer a type function without any arguments or return type
                returnType = new DeferredType($"Function type was given without arguments or return type.", ContextDependency.ReturnedValuesAnalyzed) { NecessaryBaseType = QxType.Function };
            else
                returnType = Symbols.GetType(expression.ReturnType);

            List<Parameter> parameters = [];
            foreach (var parameter in expression.Parameters)
            {
                if (!Symbols.TryGetType(parameter.TypeName, out var parameterType))
                    throw new UnrecognizedTypeException(parameter.TypeName, expression.Span);

                parameters.Add(new(parameter.Name, parameterType));
            }

            var function = new Function(expression.Body, returnType, expression.CallType) { Parameters = [.. parameters.Select(p => new Parameter(p.Name, p.Type))] };
            var functionSymbol = new FunctionSymbol(expression.Name, function);

            var symbols = new SymbolTable();

            if (expression is QxLambdaFunctionExpression)
            {
                symbols = Symbols;
            }
            else if (expression.WithClosure is not null)
            {
                symbols = Symbols.Capture(expression.WithClosure);
            }

            if (!expression.IsGlobalOrStatic)
            {
                if (typeFrame is not null)
                {
                    var type = typeFrame.Type;
                    symbols.TryDefineVariable("this", typeFrame.Type, typeFrame.Type);

                    var baseType = type.BaseType;
                    if (baseType is not null)
                        symbols.TryDefineVariable("base", baseType, baseType);
                }
                else
                {
                    var thisType = new DeferredType("A this variable is supplied to the scope of the inline function and is deferred in lieu of assignment to a member.", ContextDependency.AssignmentToMember);
                    symbols.TryDefineVariable("this", thisType, thisType);
                }
            }

            var statements = AnalyzeFunctionBlock(expression.Body, functionSymbol, symbols);

            if (function.ReturnType is DeferredType deferredType)
            {
                returnType = deferredType.SelectAlternative(QxType.Void.Type);

                function = new Function(expression.Body, returnType, expression.CallType) { Parameters = [.. parameters.Select(p => new Parameter(p.Name, p.Type))] };
                functionSymbol = new FunctionSymbol(expression.Name, function);

                expression.ReturnType = returnType.Name;
            }

            return new FunctionExpressionInfo(returnType, [.. parameters], expression)
            {
                SignatureSymbol = functionSymbol,
                BodyStatements = [.. statements],
                CallType = expression.CallType,
                Closure = expression.WithClosure,
            };
        }

        protected ExpressionInfo AnalyzeExpression(QxLambdaFunctionExpression expression)
        {
            return AnalyzeExpression((QxFunctionExpression)expression);
        }

        protected ExpressionInfo AnalyzeExpression(QxMethodCallExpression expression)
        {
            var target = AnalyzeExpression(expression.Target);

            var methodName = expression.MethodName;

            var functionCallType = expression.Type;

            ExpressionInfo[] arguments = [.. AnalyzeExpressions(expression.Arguments)];

            Function? method = null;
            bool isDynamic = false;
            bool isDeferred = false;

            var targetType = target.ExpressionType;
            if (targetType is DeferredType deferredType && deferredType.HasAlternative)
                targetType = deferredType.SelectedAlternative;

            if (targetType is DeferredType)
                isDeferred = true;
            else if (targetType is DynamicType)
                isDynamic = true;
            else if (!targetType.TryResolveMethod(methodName, [.. arguments.GetTypes()], out method))
            {
                if (expression.Type == CallType.Setter || expression.Type == CallType.Getter)
                    throw new UnrecognizedPropertySignatureException(target.ExpressionType, methodName, expression.Span);
                else
                    throw new UnrecognizedMethodSignatureException(target.ExpressionType, new Signature(methodName, [.. arguments.GetTypes()]), expression.Span);
            }

            var isInstanceCall = method is BindableFunction;
            var isStaticCall = !isInstanceCall;

            var expressionType = method?.ReturnType ?? target.ExpressionType;

            return new MethodCallExpressionInfo(expressionType, expression)
            {
                Target = target,
                MethodName = methodName,
                FunctionCallType = functionCallType,
                Arguments = [.. arguments],
                IsInstanceCall = isInstanceCall,
                IsStaticCall = isStaticCall,
                IsDynamic = isDynamic,
                IsDeferred = isDeferred,
            };
        }

        protected ExpressionInfo AnalyzeExpression(QxConstructorCallExpression expression)
        {
            var typeName = expression.TypeName;

            if (!Symbols.TryGetType(typeName, out var type))
                throw new UnrecognizedTypeException(typeName, expression.Span);

            ExpressionInfo[] arguments = [.. AnalyzeExpressions(expression.Arguments)];

            if (!type.TryResolveConstructor([.. arguments.GetTypes()], out var constructor))
            {
                if (arguments.Length > 0)
                    throw new UnrecognizedConstructorSignatureException(type, expression.Span);
            }

            return new ConstructorCallExpressionInfo(type, expression)
            {
                Arguments = [.. arguments],
            };
        }

        protected ExpressionInfo AnalyzeExpression(QxBaseConstructorCallExpression expression)
        {
            if (Frame is not TypeFrame typeFrame)
                throw new ConstructorOutsideOfTypeException(expression.Span);

            var thisType = typeFrame.Type;
            var baseType = thisType.BaseType ?? throw new BaseConstructorOnTypeWithoutBaseTypeException(thisType, expression.Span);

            return new BaseConstructorCallExpressionInfo(thisType, expression)
            {
                BaseType = baseType,
                BaseConstructor = new BaseConstructor(thisType, Span.Empty) { Arguments = expression.Arguments },
                Arguments = expression.Arguments, // Arguments are not evaluated until the base type is called.
            };
        }
    }
}
