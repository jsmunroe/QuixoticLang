using Quixotic.Common.Analysis.Statements;
using Quixotic.Common.Contracts;
using Quixotic.Common.Environment;
using Quixotic.Common.Expressions;
using Quixotic.Common.Operations;
using Quixotic.Common.Symbols;
using Quixotic.Common.Symbols.Functions;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Common.Analysis.Expressions
{
    public class ExpressionInfo(QxType expressionType) : IHasType
    {
        public QxType ExpressionType { get; } = expressionType;

        QxType IHasType.Type => ExpressionType;
    }

    public class LiteralExpressionInfo(QxType expressionType) : ExpressionInfo(expressionType)
    { }

    public class CollectionExpressionInfo(QxType elementType, QxType expressionType) : ExpressionInfo(expressionType)
    {
        public QxType ElementType { get; } = elementType;

        public IReadOnlyList<ExpressionInfo> Elements { get; init; } = [];
    }

    public class ArrayExpressionInfo(QxType elementType) : CollectionExpressionInfo(elementType, QxType.Array.MakeGenericType(elementType))
    { }

    public class SetExpressionInfo(QxType elementType) : CollectionExpressionInfo(elementType, QxType.Set.MakeGenericType(elementType))
    { }

    public class VoidExpressionInfo() : ExpressionInfo(QxType.Void.Type)
    { }

    public class BinaryExpressionInfo(QxType expressionType) : ExpressionInfo(expressionType)
    {
        public required Operator Operator { get; init; }
        public required ExpressionInfo Left { get; init; }
        public required ExpressionInfo Right { get; init; }

        public required SignatureSymbol SignatureSymbol { get; init; }
    }

    public class UnaryExpressionInfo(QxType expressionType) : ExpressionInfo(expressionType)
    {
        public required Operator Operator { get; init; }
        public required ExpressionInfo Operand { get; init; }

        public required SignatureSymbol SignatureSymbol { get; init; }
    }

    public class IdentifierExpressionInfo(QxType expressionType) : ExpressionInfo(expressionType)
    {
        public required string Name { get; init; }
    }

    public class IndexerExpressionInfo(QxType expressionType) : ExpressionInfo(expressionType)
    {
        public required ExpressionInfo Target { get; init; }
        public required ExpressionInfo Index { get; init; }
    }

    public class IsComparisonExpressionInfo() : ExpressionInfo(QxType.Boolean)
    {
        public required ExpressionInfo Target { get; init; }

        public required QxType Type { get; init; }

        [MemberNotNullWhen(true, nameof(PatternIdentifier))]
        public required bool Result { get; init; }

        public required IdentifierSymbol? PatternIdentifier { get; init; }
    }

    public class FunctionExpressionInfo(QxType returnType, params Parameter[] parameters) : ExpressionInfo(QxType.Function.MakeFunctionType(returnType, [.. parameters]))
    {
        public required SignatureSymbol SignatureSymbol { get; init; }

        public QxType ReturnType { get; init; } = returnType;

        public IReadOnlyList<Parameter> Parameters { get; init; } = [.. parameters];

        public IReadOnlyCollection<StatementInfo> BodyStatements { get; init; } = [];

        public required ClosureCapture? Closure { get; init; }
        public required CallType CallType { get; init; }
    }

    public class FunctionCallExpressionInfo(QxType expressionType) : ExpressionInfo(expressionType)
    {
        public required string Name { get; init; }
        public IReadOnlyList<ExpressionInfo> Arguments { get; init; } = [];
    }

    public class MethodCallExpressionInfo(QxType expressionType) : ExpressionInfo(expressionType)
    {
        public required ExpressionInfo Target { get; init; }

        public required string MethodName { get; init; }

        public required CallType FunctionCallType { get; init; }

        public required bool IsInstanceCall { get; init; }
        public required bool IsStaticCall { get; init; }
        public required bool IsDynamic { get; set; }
        public required bool IsDeferred { get; set; }

        public IReadOnlyList<ExpressionInfo> Arguments { get; init; } = [];
    }

    public class ConstructorCallExpressionInfo(QxType expressionType) : ExpressionInfo(expressionType)
    {
        public IReadOnlyList<ExpressionInfo> Arguments { get; init; } = [];
    }

    public class BaseConstructorCallExpressionInfo(QxType expressionType) : ExpressionInfo(expressionType)
    {
        public required QxType BaseType { get; init; }
        public List<QxExpression> Arguments { get; init; } = []; // Arguments are not evaluated until the base type is called.
        public required BaseConstructor BaseConstructor { get; init; }
    }

    public class ExpressionErrorInfo(Exception exception) : ExpressionInfo(QxType.Nada.Type)
    {
        public Exception Exception { get; } = exception;
    }
}
