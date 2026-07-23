using Quixotic.Common.Expressions;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Analysis.Expressions
{
    public class IndexerExpressionInfo(QxType expressionType, QxExpression expression) : ExpressionInfo(expressionType, expression)
    {
        public required ExpressionInfo Target { get; init; }
        public required ExpressionInfo Index { get; init; }

        protected override IEnumerable<AnalysisInfo> GetChildren()
        {
            return [Target, Index];
        }

    }
}
