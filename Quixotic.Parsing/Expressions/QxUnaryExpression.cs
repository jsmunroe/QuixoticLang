using Quixotic.Parsing.Operations;

namespace Quixotic.Parsing.Expressions
{
    public class QxUnaryExpression(Operator @operator, QxExpression operand) : QxExpression
    {
        public Operator Operator { get; } = @operator;
        public QxExpression Operand { get; } = operand;
    }
}
