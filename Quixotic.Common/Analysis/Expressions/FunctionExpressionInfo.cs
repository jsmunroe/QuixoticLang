using Quixotic.Common.Analysis.Statements;
using Quixotic.Common.Environment;
using Quixotic.Common.Expressions;
using Quixotic.Common.Symbols;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Analysis.Expressions
{
    public class FunctionExpressionInfo(QxType returnType, Parameter[] parameters, QxExpression expression) : ExpressionInfo(QxType.Function.MakeFunctionType(returnType, [.. parameters]), expression)
    {
        public required SignatureSymbol SignatureSymbol { get; init; }

        public QxType ReturnType { get; init; } = returnType;

        public IReadOnlyList<Parameter> Parameters { get; init; } = [.. parameters];

        public IReadOnlyCollection<StatementInfo> BodyStatements { get; init; } = [];

        public required ClosureCapture? Closure { get; init; }
        public required CallType CallType { get; init; }
    }
}
