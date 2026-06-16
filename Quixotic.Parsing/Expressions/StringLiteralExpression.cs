namespace Quixotic.Parsing.Expressions
{
    public sealed class StringLiteralExpression(string value) : Expression
    {
        public string Value { get; } = value;
    }

    public sealed class NumberLiteralExpression(double value) : Expression
    {
        public double Value { get; } = value;
    }
}
