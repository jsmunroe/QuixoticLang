using Quixotic.Common.Expressions;

namespace Quixotic.Common.Statements
{
    public class QxReturnStatement(QxExpression? expression) : QxStatement
    {
        public QxExpression? Expression { get; } = expression;
    }
}
