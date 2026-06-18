namespace Quixotic.Parsing.Statements
{
    public class QxElseIfClause(Expressions.QxExpression condition)
    {
        public Expressions.QxExpression Condition { get; } = condition;

        public Block Block { get; set; } = [];
    }
}
