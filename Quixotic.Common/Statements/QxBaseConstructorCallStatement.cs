using Quixotic.Common.Expressions;

namespace Quixotic.Common.Statements
{
    public class QxBaseConstructorCallStatement : QxStatement
    {
        public List<QxExpression> Arguments { get; init; } = [];
    }
}
