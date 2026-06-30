namespace Quixotic.Common.Expressions
{
    public sealed class QxNumberLiteralExpression(double value) : QxExpression
    {
        public double Value { get; } = value;
    }
}
