using Quixotic.Common.Expressions;

namespace Quixotic.Common.Statements
{
    public class QxBaseConstructorCallExpression : QxExpression
    {
        public List<QxExpression> Arguments { get; init; } = [];
    }
}
