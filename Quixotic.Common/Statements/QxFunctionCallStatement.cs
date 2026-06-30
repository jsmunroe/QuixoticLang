using Quixotic.Common.Expressions;

namespace Quixotic.Common.Statements
{
    public class QxFunctionCallStatement(QxFunctionCallExpression call) : QxStatement
    {
        public QxFunctionCallExpression Call { get; } = call;
    }
}
