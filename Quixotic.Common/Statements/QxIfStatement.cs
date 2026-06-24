using Quixotic.Common.Expressions;

namespace Quixotic.Common.Statements
{
    public class QxIfStatement(QxExpression condition) : QxStatement
    {
        public QxExpression Condition { get; } = condition;

        public Block ThenBlock { get; init; } = [];

        public List<QxElseIfClause> ElseIfClauses { get; init; } = [];

        public Block ElseBlock { get; init; } = [];
    }
}
