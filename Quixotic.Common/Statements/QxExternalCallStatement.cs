using Quixotic.Common.Expressions;

namespace Quixotic.Common.Statements
{
    public class QxExternalCallStatement(QxExternalCallExpression expression) : QxStatement
    {
        public QxExternalCallExpression Expression { get; } = expression;
    }
}
