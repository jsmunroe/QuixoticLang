using Quixotic.Common.Expressions;

namespace Quixotic.Common.Statements
{
    public class QxForStatement(string iterator, QxExpression from, QxExpression to) : QxStatement
    {
        public string Iterator { get; } = iterator;

        public QxExpression From { get; } = from;
        public QxExpression To { get; } = to;

        public QxExpression Step { get; init; } = new QxNumberLiteralExpression(1);

        public Block Block { get; init; } = [];
    }
}
