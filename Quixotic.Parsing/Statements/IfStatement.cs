namespace Quixotic.Parsing.Statements
{
    public class IfStatement(Expressions.Expression condition) : Statement
    {
        public Expressions.Expression Condition { get; } = condition;

        public Block ThenBlock { get; init; } = [];

        public List<ElseIfClause> ElseIfClauses { get; init; } = [];

        public Block ElseBlock { get; init; } = [];

    }
}
