using Quixotic.Common.Analysis.Expressions;
using Quixotic.Common.Statements;

namespace Quixotic.Common.Analysis.Statements
{
    public class StatementInfo(QxStatement statement) : AnalysisInfo(statement.Span)
    {
        public QxStatement Statement { get; } = statement;
    }
}
