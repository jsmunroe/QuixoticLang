using Quixotic.Common.Expressions;

namespace Quixotic.Common.Statements
{
    public class QxMethodCallStatement(QxMethodCallExpression call) : QxStatement
    {
        public QxMethodCallExpression Call { get; } = call;
    }
}
