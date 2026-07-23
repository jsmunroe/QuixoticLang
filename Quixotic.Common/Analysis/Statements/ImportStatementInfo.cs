using Quixotic.Common.Namespaces;
using Quixotic.Common.Statements;

namespace Quixotic.Common.Analysis.Statements
{
    public class ImportStatementInfo(QxStatement statement) : StatementInfo(statement)
    {
        public required Namespace Namespace { get; init; }

        protected override IEnumerable<AnalysisInfo> GetChildren()
        {
            return [];
        }
    }
}
