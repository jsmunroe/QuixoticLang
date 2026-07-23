using Quixotic.Common.Analysis.Expressions;
using Quixotic.Common.Statements;

namespace Quixotic.Common.Analysis.Statements
{
    public class IfStatementInfo(QxStatement statement) : StatementInfo(statement)
    {
        public required ExpressionInfo Condition { get; init; }
        public IReadOnlyList<StatementInfo> IfStatements { get; init; } = [];

        public IReadOnlyList<ElseIfBlockInfo> ElseIfBlocks { get; init; } = [];

        public ElseBlockInfo? ElseBlock { get; init; }

        protected override IEnumerable<AnalysisInfo?> GetChildren()
        {
            return [Condition, .. IfStatements, .. ElseIfBlocks.SelectMany(eib => eib.GetChildren()), .. (ElseBlock?.Statements ?? [])];
        }
    }

    public class ElseIfBlockInfo
    {
        public required ExpressionInfo Condition { get; init; }
        public IReadOnlyList<StatementInfo> Statements { get; init; } = [];

        public IEnumerable<AnalysisInfo> GetChildren()
        {
            return [Condition, .. Statements];
        }
    }

    public class ElseBlockInfo
    {
        public IReadOnlyList<StatementInfo> Statements { get; init; } = [];
    }
}
