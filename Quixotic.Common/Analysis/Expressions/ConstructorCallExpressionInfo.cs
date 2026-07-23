using Quixotic.Common.Expressions;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Analysis.Expressions
{
    public class ConstructorCallExpressionInfo(QxType expressionType, QxExpression expression) : ExpressionInfo(expressionType, expression)
    {
        public IReadOnlyList<ExpressionInfo> Arguments { get; init; } = [];

        protected override IEnumerable<AnalysisInfo> GetChildren()
        {
            return [.. Arguments];
        }

    }
}
