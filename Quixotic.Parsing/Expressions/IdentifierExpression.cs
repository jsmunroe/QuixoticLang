namespace Quixotic.Parsing.Expressions
{
    public class IdentifierExpression(string name) : Expression
    {
        public string Name { get; } = name;
    }
}
