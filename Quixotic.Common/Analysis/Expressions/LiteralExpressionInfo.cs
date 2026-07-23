using Quixotic.Common.Expressions;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Analysis.Expressions
{
    public class LiteralExpressionInfo(QxType expressionType, QxExpression expression) : ExpressionInfo(expressionType, expression)
    {
        protected override IEnumerable<AnalysisInfo> GetChildren()
        {
            return [];
        }
    }
}
