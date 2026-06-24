using Quixotic.Common.Expressions;

namespace Quixotic.Common.Statements
{
    public class QxFunctionCallStatement(string name) : QxStatement
    {
        public string Name { get; } = name;

        public List<QxExpression> Arguments { get; init; } = [];
    }
}
