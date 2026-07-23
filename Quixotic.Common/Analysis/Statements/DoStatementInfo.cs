using Quixotic.Common.Analysis.Expressions;
using Quixotic.Common.Statements;

namespace Quixotic.Common.Analysis.Statements
{
    public class DoStatementInfo(QxStatement statement) : StatementInfo(statement)
    {
        public required bool IsEntryControlled { get; init; }
        public required bool IsExitControlled { get; init; }
        public required ExpressionInfo Condition { get; init; }


        public required IReadOnlyCollection<StatementInfo> BlockStatements { get; init; } = [];

        protected override IEnumerable<AnalysisInfo> GetChildren()
        {
            return [Condition, .. BlockStatements];
        }
    }
}
