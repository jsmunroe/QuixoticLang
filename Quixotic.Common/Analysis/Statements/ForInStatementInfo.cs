using Quixotic.Common.Analysis.Expressions;

namespace Quixotic.Common.Analysis.Statements
{
    public class ForInStatementInfo : StatementInfo
    {
        public required string IteratorName { get; init; }

        public required ExpressionInfo Collection { get; init; }

        public required IReadOnlyCollection<StatementInfo> BlockStatements { get; init; } = [];
    }
}
