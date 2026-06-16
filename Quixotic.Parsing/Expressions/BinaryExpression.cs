using Quixotic.Parsing.Operations;

namespace Quixotic.Parsing.Expressions
{

    public class BinaryExpression(Operator @operator, Expression left, Expression right) : Expression
    {
        public Operator Operator { get; } = @operator;

        public Expression Left { get; } = left;

        public Expression Right { get; } = right;
    }
}
