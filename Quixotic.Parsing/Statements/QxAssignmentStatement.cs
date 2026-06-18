using Quixotic.Parsing.Expressions;

namespace Quixotic.Parsing.Statements
{
    public class QxAssignmentStatement(QxIdentifierExpression target, QxExpression value) : QxStatement
    {
        public QxIdentifierExpression Target { get; } = target;
        public QxExpression Value { get; } = value;
    }
}
