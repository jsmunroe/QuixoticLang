using Quixotic.Common.Expressions;

namespace Quixotic.Common.Statements
{
    public class QxForStatement(QxIdentifierExpression iterator, QxExpression from, QxExpression to) : QxStatement
    {
        public QxIdentifierExpression Iterator { get; } = iterator;

        public QxExpression From { get; } = from;
        public QxExpression To { get; } = to;

        public QxExpression Step { get; init; } = new QxNumberLiteralExpression(1);

        public Block Block { get; init; } = [];
    }
}
