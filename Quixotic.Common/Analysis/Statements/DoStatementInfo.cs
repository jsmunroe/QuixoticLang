using Quixotic.Common.Analysis.Expressions;

namespace Quixotic.Common.Analysis.Statements
{
    public class DoStatementInfo : StatementInfo
    {
        public required bool IsEntryControlled { get; init; }
        public required bool IsExitControlled { get; init; }
        public required ExpressionInfo Condition { get; init; }

        public required IReadOnlyCollection<StatementInfo> BlockStatements { get; init; } = [];
    }
}
