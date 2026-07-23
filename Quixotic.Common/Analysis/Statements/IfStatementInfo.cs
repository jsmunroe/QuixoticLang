using Quixotic.Common.Analysis.Expressions;

namespace Quixotic.Common.Analysis.Statements
{
    public class IfStatementInfo : StatementInfo
    {
        public required ExpressionInfo Condition { get; init; }
        public IReadOnlyList<StatementInfo> IfStatements { get; init; } = [];

        public IReadOnlyList<ElseIfBlockInfo> ElseIfBlocks { get; init; } = [];

        public ElseBlockInfo? ElseBlock { get; init; }
    }

    public class ElseIfBlockInfo
    {
        public required ExpressionInfo Condition { get; init; }
        public IReadOnlyList<StatementInfo> Statements { get; init; } = [];
    }

    public class ElseBlockInfo
    {
        public IReadOnlyList<StatementInfo> Statements { get; init; } = [];
    }
}
