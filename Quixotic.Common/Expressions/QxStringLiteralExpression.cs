namespace Quixotic.Common.Expressions
{
    public sealed class QxStringLiteralExpression(string value) : QxExpression
    {
        public string Value { get; } = value;
    }
}
