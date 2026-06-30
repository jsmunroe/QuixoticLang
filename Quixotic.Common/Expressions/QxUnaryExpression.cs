using Quixotic.Common.Operations;

namespace Quixotic.Common.Expressions
{
    public class QxUnaryExpression(Operator @operator, QxExpression operand) : QxExpression
    {
        public Operator Operator { get; } = @operator;
        public QxExpression Operand { get; } = operand;
    }
}
