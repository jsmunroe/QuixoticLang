namespace Quixotic.Parsing.Expressions
{
    public class QxFunctionCallExpression(string name) : QxExpression
    {
        public string Name { get; } = name;

        public List<QxExpression> Arguments { get; init; } = [];
    }
}
