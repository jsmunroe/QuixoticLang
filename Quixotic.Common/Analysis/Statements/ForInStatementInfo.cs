using Quixotic.Common.Analysis.Expressions;
using Quixotic.Common.Statements;

namespace Quixotic.Common.Analysis.Statements
{
    public class ForInStatementInfo(QxStatement statement) : StatementInfo(statement)
    {
        public required string IteratorName { get; init; }

        public required ExpressionInfo Collection { get; init; }

        public required IReadOnlyCollection<StatementInfo> BlockStatements { get; init; } = [];
    }
}
