using Quixotic.Parsing.Operations;

namespace Quixotic.Parsing.Expressions
{
    public class UnaryExpression(Operator @operator, Expression operand) : Expression
    {
        public Operator Operator { get; } = @operator;
        public Expression Operand { get; } = operand;
    }
}
