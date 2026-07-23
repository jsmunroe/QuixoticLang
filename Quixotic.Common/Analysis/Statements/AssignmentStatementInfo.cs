using Quixotic.Common.Analysis.Expressions;
using Quixotic.Common.Statements;

namespace Quixotic.Common.Analysis.Statements
{
    public class AssignmentStatementInfo(QxStatement statement) : StatementInfo(statement)
    {
        public required ExpressionInfo Target { get; init; }

        public required ExpressionInfo Value { get; init; }
    }
}
