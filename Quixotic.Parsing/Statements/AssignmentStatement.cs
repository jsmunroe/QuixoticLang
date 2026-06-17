using Quixotic.Parsing.Expressions;

namespace Quixotic.Parsing.Statements
{
    public class AssignmentStatement(IdentifierExpression target, Expression value) : Statement
    {
        public IdentifierExpression Target { get; } = target;
        public Expression Value { get; } = value;
    }
}
