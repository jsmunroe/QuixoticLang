using Quixotic.Common.Expressions;

namespace Quixotic.Common.Statements
{
    public class QxAssignmentStatement(QxIdentifierExpression target, QxExpression value) : QxStatement
    {
        public QxIdentifierExpression Target { get; } = target;
        public QxExpression Value { get; } = value;
    }
}
