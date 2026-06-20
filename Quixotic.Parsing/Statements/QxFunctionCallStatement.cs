using Quixotic.Parsing.Expressions;

namespace Quixotic.Parsing.Statements
{
    public class QxFunctionCallStatement(string name) : QxStatement
    {
        public string Name { get; } = name;

        public List<QxExpression> Arguments { get; init; } = [];
    }
}
