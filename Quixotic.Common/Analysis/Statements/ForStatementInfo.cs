using Quixotic.Common.Analysis.Expressions;
using Quixotic.Common.Statements;

namespace Quixotic.Common.Analysis.Statements
{
    public class ForStatementInfo(QxStatement statement) : StatementInfo(statement)
    {
        public required string IteratorName { get; init; }
        public required ExpressionInfo From { get; init; }
        public required ExpressionInfo To { get; init; }
        public ExpressionInfo? Step { get; init; }

        public required IReadOnlyCollection<StatementInfo> BlockStatements { get; init; } = [];
    }
}
