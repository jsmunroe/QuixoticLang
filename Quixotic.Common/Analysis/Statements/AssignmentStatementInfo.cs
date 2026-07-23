using Quixotic.Common.Analysis.Expressions;

namespace Quixotic.Common.Analysis.Statements
{
    public class AssignmentStatementInfo : StatementInfo
    {
        public required ExpressionInfo Target { get; init; }

        public required ExpressionInfo Value { get; init; }
    }
}
