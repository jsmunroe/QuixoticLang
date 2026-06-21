using Quixotic.Parsing.Expressions;

namespace Quixotic.Parsing.Statements
{
    public class QxReturnStatement(QxExpression? expression) : QxStatement
    {
        public QxExpression? Expression { get; } = expression;
    }
}
