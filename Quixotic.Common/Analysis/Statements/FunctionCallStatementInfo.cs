using Quixotic.Common.Analysis.Expressions;
using Quixotic.Common.Statements;

namespace Quixotic.Common.Analysis.Statements
{
    public class FunctionCallStatementInfo(QxStatement statement) : StatementInfo(statement)
    {
        public required ExpressionInfo Call { get; init; }

        protected override IEnumerable<AnalysisInfo> GetChildren()
        {
            return [Call];
        }
    }
}
