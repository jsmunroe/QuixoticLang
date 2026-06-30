namespace Quixotic.Common.Expressions
{
    public sealed class QxBooleanLiteralExpression(bool value) : QxExpression
    {
        public bool Value { get; } = value;
    }
}
