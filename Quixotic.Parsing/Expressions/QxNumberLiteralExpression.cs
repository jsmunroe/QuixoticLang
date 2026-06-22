namespace Quixotic.Parsing.Expressions
{
    public sealed class QxNumberLiteralExpression(double value) : QxExpression
    {
        public double Value { get; } = value;

        public override ExpressionKind Kind => ExpressionKind.Number;
    }
}
