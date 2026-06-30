using Quixotic.Common.Operations;

namespace Quixotic.Common.Expressions
{

    public class QxBinaryExpression(Operator @operator, QxExpression left, QxExpression right) : QxExpression
    {
        public Operator Operator { get; } = @operator;

        public QxExpression Left { get; } = left;

        public QxExpression Right { get; } = right;

    }
}
