using Quixotic.Parsing.Expressions;

namespace Quixotic.Parsing.Statements
{
    public sealed class QxPrintStatement(QxExpression expression) : QxStatement
    {
        public QxExpression Expression { get; } = expression;
    }
}
