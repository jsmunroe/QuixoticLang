using Quixotic.Common.Expressions;

namespace Quixotic.Common.Statements
{
    public class QxAssignmentStatement(QxAssignableExpression target, QxExpression value) : QxStatement
    {
        public QxAssignableExpression Target { get; } = target;
        public QxExpression Value { get; } = value;
    }
}
