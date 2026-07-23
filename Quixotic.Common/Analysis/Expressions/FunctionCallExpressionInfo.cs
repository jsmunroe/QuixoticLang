using Quixotic.Common.Expressions;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Analysis.Expressions
{
    public class FunctionCallExpressionInfo(QxType expressionType, QxExpression expression) : ExpressionInfo(expressionType, expression)
    {
        public required string Name { get; init; }
        public IReadOnlyList<ExpressionInfo> Arguments { get; init; } = [];
        protected override IEnumerable<AnalysisInfo> GetChildren()
        {
            return [.. Arguments];
        }

    }
}
