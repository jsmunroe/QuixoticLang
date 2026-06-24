using Quixotic.Common.Operations;

namespace Quixotic.Common.Expressions
{

    public class QxBinaryExpression(Operator @operator, QxExpression left, QxExpression right) : QxExpression
    {
        public Operator Operator { get; } = @operator;

        public QxExpression Left { get; } = left;

        public QxExpression Right { get; } = right;

        public override ExpressionKind Kind
        {
            get
            {
                if (Left.Kind == Right.Kind)
                    return Left.Kind;

                if (Operator == Operator.And ||
                    Operator == Operator.Or ||
                    Operator == Operator.EqualTo ||
                    Operator == Operator.NotEqualTo ||
                    Operator == Operator.GreaterThan ||
                    Operator == Operator.GreaterThanOrEqualTo ||
                    Operator == Operator.LessThan ||
                    Operator == Operator.LessThanOrEqualTo)
                    return ExpressionKind.Boolean;

                return ExpressionKind.Unknown;
            }
        }

    }
}
