using Quixotic.Common.Contracts;
using Quixotic.Common.Expressions;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Analysis.Expressions
{
    public abstract class ExpressionInfo(QxType expressionType, QxExpression expression) : AnalysisInfo(expression.Span), IHasType
    {
        public QxType ExpressionType { get; } = expressionType;

        public QxExpression Expression { get; } = expression;

        QxType IHasType.Type => ExpressionType;
    }
}
