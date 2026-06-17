namespace Quixotic.Parsing.Statements
{
    public class ElseIfClause(Expressions.Expression condition)
    {
        public Expressions.Expression Condition { get; } = condition;

        public Block Block { get; set; } = [];
    }
}
