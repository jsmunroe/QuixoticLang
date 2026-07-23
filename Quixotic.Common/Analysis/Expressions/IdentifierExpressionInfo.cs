using Quixotic.Common.Expressions;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Analysis.Expressions
{
    public class IdentifierExpressionInfo(QxType expressionType, QxExpression expression) : ExpressionInfo(expressionType, expression)
    {
        public required string Name { get; init; }

        protected override IEnumerable<AnalysisInfo> GetChildren()
        {
            return [];
        }
    }
}
