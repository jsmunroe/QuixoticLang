using Quixotic.Common.Analysis.Expressions;
using Quixotic.Common.Analysis.Statements;
using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Contracts
{
    public interface ISourceDatabaseEntry
    {
        List<ExpressionInfo> Expressions { get; set; }
        IReadOnlyList<AnalysisInfo> Items { get; }
        Position Position { get; }
        StatementInfo? Statement { get; set; }

        string ToString();
    }
}