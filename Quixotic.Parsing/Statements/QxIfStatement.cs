namespace Quixotic.Parsing.Statements
{
    public class QxIfStatement(Expressions.QxExpression condition) : QxStatement
    {
        public Expressions.QxExpression Condition { get; } = condition;

        public Block ThenBlock { get; init; } = [];

        public List<QxElseIfClause> ElseIfClauses { get; init; } = [];

        public Block ElseBlock { get; init; } = [];

    }
}
