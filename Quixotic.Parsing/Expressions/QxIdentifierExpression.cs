namespace Quixotic.Parsing.Expressions
{
    public class QxIdentifierExpression(string name) : QxExpression
    {
        public string Name { get; } = name;

        public override ExpressionKind Kind => ExpressionKind.Unknown;
    }
}
