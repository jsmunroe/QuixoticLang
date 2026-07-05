using Quixotic.Common.Expressions;

namespace Quixotic.Common.Statements
{
    public class QxForInStatement(string iterator, QxExpression collection) : QxStatement
    {
        public string Iterator { get; } = iterator;

        public QxExpression Collection { get; } = collection;

        public Block Block { get; init; } = [];
    }
}
