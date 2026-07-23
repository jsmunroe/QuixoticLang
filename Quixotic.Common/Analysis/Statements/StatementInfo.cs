using Quixotic.Common.Statements;

namespace Quixotic.Common.Analysis.Statements
{
    public abstract class StatementInfo(QxStatement statement) : AnalysisInfo(statement.Span)
    {
        public QxStatement Statement { get; } = statement;
    }
}
