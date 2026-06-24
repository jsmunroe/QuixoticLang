using Quixotic.Common.Expressions;

namespace Quixotic.Common.Statements
{
    public sealed class QxPrintStatement(QxExpression expression) : QxStatement
    {
        public QxExpression Expression { get; } = expression;
    }
}
