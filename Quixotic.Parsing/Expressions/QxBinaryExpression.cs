using Quixotic.Parsing.Operations;

namespace Quixotic.Parsing.Expressions
{

    public class QxBinaryExpression(Operator @operator, QxExpression left, QxExpression right) : QxExpression
    {
        public Operator Operator { get; } = @operator;

        public QxExpression Left { get; } = left;

        public QxExpression Right { get; } = right;
    }
}
