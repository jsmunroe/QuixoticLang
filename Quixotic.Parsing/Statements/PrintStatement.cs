using Quixotic.Parsing.Expressions;

namespace Quixotic.Parsing.Statements
{
    public sealed class PrintStatement(Expression expression) : Statement
    {
        public Expression Expression { get; } = expression;
    }
}
