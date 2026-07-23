using Quixotic.Common.Expressions;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Analysis.Expressions
{
    public class CollectionExpressionInfo(QxType elementType, QxType expressionType, QxExpression expression) : ExpressionInfo(expressionType, expression)
    {
        public QxType ElementType { get; } = elementType;

        public IReadOnlyList<ExpressionInfo> Elements { get; init; } = [];
    }
}
