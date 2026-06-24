using Quixotic.Common.Expressions;

namespace Quixotic.Common.Statements
{
    public class QxElseIfClause(QxExpression condition)
    {
        public QxExpression Condition { get; } = condition;

        public Block Block { get; set; } = [];
    }
}
