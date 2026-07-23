using Quixotic.Common.Statements;

namespace Quixotic.Common.Analysis.Statements
{
    public class BreakStatementInfo(QxStatement statement) : StatementInfo(statement)
    {
        protected override IEnumerable<AnalysisInfo?> GetChildren()
        {
            return [];
        }
    }
}
