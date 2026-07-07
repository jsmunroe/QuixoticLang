using Quixotic.Analysis.Contracts;
using Quixotic.Analysis.Environment;
using Quixotic.Analysis.Exceptions;
using Quixotic.Common.Expressions;
using Quixotic.Common.Operations;
using Quixotic.Common.Statements;
using Quixotic.Common.Symbols;
using Quixotic.Common.TypeSystem.Types;
using Quixotic.Common.Utilities;
using Quixotic.Parsing;

namespace Quixotic.Analysis.Semantics
{

    public class SemanticAnalyzer
    {
        private readonly static MethodIndexer<Action<SemanticAnalyzer, QxStatement>> _statementIndexer = new(typeof(SemanticAnalyzer), "AnalyzeStatement");

        private readonly static MethodIndexer<Func<SemanticAnalyzer, QxExpression, QxType>> _expressionIndexer = new(typeof(SemanticAnalyzer), "AnalyzeExpression");

        private IFrame Frame { get; set; } = new GlobalFrame();

        public List<SemanticException> Issues { get; } = [];

        public IEnumerable<SemanticException> Errors => Issues.Where(issue => issue.Severity == Severity.Error);

        public IEnumerable<SemanticException> Warnings => Issues.Where(issue => issue.Severity == Severity.Warning);

        public void PushBlockFrame()
        {
            Frame = new BlockFrame(Frame);
        }

        public void PushLoopFrame()
        {
            Frame = new LoopFrame(Frame);
        }

        public void PushFunctionFrame(SignatureSymbol signatureSymbol)
        {
            Frame = new FunctionFrame(Frame, signatureSymbol);
        }

        public void PopFrame()
        {
            if (Frame.Parent is null)
                throw new InvalidOperationException("Cannot pop the global frame.");

            Frame = Frame.Parent;
        }

        public IEnumerable<QxStatement> Analyze(Parser parser)
        {
            var statements = parser.Parse();
            return Analyze(statements);
        }

        public IEnumerable<QxStatement> Analyze(IEnumerable<QxStatement> statements)
        {
            foreach (var statement in statements)
            {
                try
                {
                    AnalyzeStatement(statement);
                }
                catch (SemanticException ex)
                {
                    Issues.Add(ex);
                }

                yield return statement;
            }
        }

        private void AnalyzeBlock(Block block)
        {
            PushBlockFrame();

            foreach (var statement in block)
            {
                try
                {
                    AnalyzeStatement(statement);
                }
                catch (SemanticException ex)
                {
                    Issues.Add(ex);
                }
            }

            PopFrame();
        }

        private void AnalyzeLoopBlock(Block block, params VariableTypeSymbol[] symbols)
        {
            PushLoopFrame();

            foreach (var symbol in symbols)
                Frame.Symbols.TryDefineVariable(symbol.Name, symbol.Type);

            foreach (var statement in block)
            {
                try
                {
                    AnalyzeStatement(statement);
                }
                catch (SemanticException ex)
                {
                    Issues.Add(ex);
                }
            }

            PopFrame();
        }


        private void AnalyzeStatement(QxStatement statement)
        {
            if (Frame.RedirectionType != FrameRedirectionType.None)
                Issues.Add(new UnreachableCodeException(statement.Span));

            var statementType = statement.GetType();

            if (!_statementIndexer.TryGetDelegate(statementType, out var action))
                throw new NotImplementedException($"No analyzer implemented for statement type: {statementType.Name}");

            action(this, statement);
        }

        private IEnumerable<QxType> AnalyzeExpressions(IEnumerable<QxExpression> expressions)
        {
            foreach (var expression in expressions)
                yield return AnalyzeExpression(expression);
        }

        private QxType AnalyzeExpression(QxExpression expression)
        {
            var expressionType = expression.GetType();

            if (!_expressionIndexer.TryGetDelegate(expressionType, out var func))
                throw new NotImplementedException($"No analyzer implemented for expression type: {expressionType.Name}");

            var type = func(this, expression);
            expression.SemanticInfo = expression.SemanticInfo is null ? new Common.Expressions.SemanticInfo(type) : expression.SemanticInfo with { Type = type };
            return type;
        }

        // Example statement analyzers
        protected void AnalyzeStatement(QxPrintStatement statement)
        {
            // Analyze the expression to get its type
            var expressionType = AnalyzeExpression(statement.Expression);

            // Print statements don't have additional semantic checks
        }

        // Example statement analyzers
        protected void AnalyzeStatement(QxVariableDeclarationStatement statement)
        {
            var name = statement.Name;

            var valueType = statement.Value is not null ? AnalyzeExpression(statement.Value) : null;

            QxType? declaredType = null;
            if (statement.TypeName is not null)
            {
                if (!QxType.TryParse(statement.TypeName, out declaredType))
                    throw new UnrecognizedTypeException(statement.TypeName, statement.Span);
            }

            if (declaredType is not null)
            {
                if (!Frame.Symbols.TryDefineVariable(name, declaredType))
                    throw new AlreadyDefinedIdentifierException(name, statement.Span);
            }
            else if (valueType is not null)
            {
                if (!Frame.Symbols.TryDefineVariable(name, valueType))
                    throw new AlreadyDefinedIdentifierException(name, statement.Span);
            }
            else
                throw new UntypedVariableDeclarationException(name, statement.Span);

            if (declaredType is not null && valueType is not null && !declaredType.IsAssignableFrom(valueType))
                throw new AssignmentTypeMismatchException(name, declaredType, valueType, statement.Span);
        }

        protected void AnalyzeStatement(QxAssignmentStatement statement)
        {
            var target = AnalyzeExpression(statement.Target);

            var valueType = AnalyzeExpression(statement.Value);

            if (!target.IsAssignableFrom(valueType))
                throw new AssignmentTypeMismatchException(target, valueType, statement.Span);
        }

        protected void AnalyzeStatement(QxIfStatement statement)
        {
            AnalyzeExpression(statement.Condition);

            AnalyzeBlock(statement.ThenBlock);

            foreach (var elseIfClause in statement.ElseIfClauses)
            {
                AnalyzeExpression(elseIfClause.Condition);

                AnalyzeBlock(elseIfClause.Block);
            }

            if (statement.ElseBlock is not null)
                AnalyzeBlock(statement.ElseBlock);
        }

        protected void AnalyzeStatement(QxDoStatement statement)
        {
            if (statement.EntryControlled)
                AnalyzeExpression(statement.Condition);

            AnalyzeLoopBlock(statement.Block);

            if (statement.ExitControlled)
                AnalyzeExpression(statement.Condition);
        }

        protected void AnalyzeStatement(QxForStatement statement)
        {
            var iterator = statement.Iterator; // TODO: Validate iterator name

            var fromType = AnalyzeExpression(statement.From);

            if (fromType is not NumberType)
                throw new ForLoopRangeTypeException(fromType, "from", statement.From.Span);

            var toType = AnalyzeExpression(statement.To);

            if (toType is not NumberType)
                throw new ForLoopRangeTypeException(toType, "to", statement.To.Span);

            var stepType = AnalyzeExpression(statement.Step);

            if (stepType is not NumberType)
                throw new ForLoopRangeTypeException(stepType, "step", statement.Step.Span);

            AnalyzeLoopBlock(statement.Block, new VariableTypeSymbol(iterator, QxType.Number));
        }

        protected void AnalyzeStatement(QxFunctionDeclarationStatement statement)
        {
            var name = statement.Name;

            var returnType = QxType.Parse(statement.ReturnType);

            var parameters = statement.Parameters.Select(p => QxType.Parse(p.TypeName));

            if (!Frame.Symbols.TryDefineSignature(name, returnType, [.. parameters]))
                throw new AlreadyDefinedSignatureException(name, statement.Span);
        }

        protected void AnalyzeStatement(QxFunctionCallStatement statement)
        {
            var returnType = AnalyzeExpression(statement.Call);
        }

        protected void AnalyzeStatement(QxBreakStatement statement)
        {
            if (Frame.GetLoopFrame() is null)
                throw new BreakOutsideLoopException(statement.Span);

            Frame.RedirectionType = FrameRedirectionType.Break;
        }

        protected void AnalyzeStatement(QxContinueStatement statement)
        {
            if (Frame.GetLoopFrame() is null)
                throw new ContinueOutsideLoopException(statement.Span);

            Frame.RedirectionType = FrameRedirectionType.Continue;
        }

        protected void AnalyzeStatement(QxReturnStatement statement)
        {
            var functionFrame = Frame.GetFunctionFrame() ?? throw new ReturnOutsideFunctionException(statement.Span);

            var returnValueType = statement.Expression is null ? QxType.Void.Type : AnalyzeExpression(statement.Expression);

            if (!functionFrame.Function.ReturnType.IsAssignableFrom(returnValueType))
                throw new ReturnTypeMismatchException(functionFrame.Function.Name, returnValueType, statement.Span);

            Frame.RedirectionType = FrameRedirectionType.Return;
        }

        protected QxType AnalyzeExpression(QxNumberLiteralExpression expression)
        {
            return QxType.Number;
        }

        protected QxType AnalyzeExpression(QxStringLiteralExpression expression)
        {
            return QxType.String;
        }

        protected QxType AnalyzeExpression(QxBooleanLiteralExpression expression)
        {
            return QxType.Boolean;
        }

        protected QxType AnalyzeExpression(QxArrayExpression expression)
        {
            var elementTypes = AnalyzeExpressions(expression.Elements);
            var commonElementType = QxType.GetCommonBase(elementTypes);

            return QxType.Array(commonElementType);
        }

        protected QxType AnalyzeExpression(QxBinaryExpression expression)
        {
            var operatorValue = OperationMetadata.GetOperatorValue(expression.Operator) ?? throw new UnrecognizedOperatorException(expression.Operator, expression.Span);

            var leftType = AnalyzeExpression(expression.Left);
            var rightType = AnalyzeExpression(expression.Right);

            var signature = Frame.Symbols.GetSignature(operatorValue, leftType, rightType) ?? throw new UnrecognizedFunctionSignatureException(new Signature(operatorValue, leftType, rightType), expression.Span);

            signature = signature.ReplaceGenerics(leftType, rightType);

            return signature.ReturnType;
        }

        protected QxType AnalyzeExpression(QxUnaryExpression expression)
        {
            var operand = AnalyzeExpression(expression.Operand);

            return operand;
        }

        protected QxType AnalyzeExpression(QxIdentifierExpression expression)
        {
            var name = expression.Name;

            var type = Frame.Symbols.GetInstance(name) ?? throw new UnrecognizedIdentifierException(name, expression.Span);

            return type;
        }

        protected QxType AnalyzeExpression(QxIndexerExpression expression)
        {
            var targetType = AnalyzeExpression(expression.Target);

            if (targetType is not ArrayType arrayType)
                throw new InvalidIndexerTargetException(targetType, expression.Span);

            var indexType = AnalyzeExpression(expression.Index);

            if (indexType != QxType.Number)
                throw new IndexTypeException(indexType, expression.Span);

            return arrayType.ElementType;
        }

        protected QxType AnalyzeExpression(QxFunctionCallExpression expression)
        {
            var name = expression.Name;

            var arguments = AnalyzeExpressions(expression.Arguments);

            var signature = new Signature(name, [.. arguments]);

            var functionSignature = Frame.Symbols.GetSignature(name, [.. arguments]) ?? throw new UnrecognizedFunctionSignatureException(signature, expression.Span);

            return functionSignature.ReturnType;
        }

    }
}
