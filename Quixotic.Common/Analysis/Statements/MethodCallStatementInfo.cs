using Quixotic.Common.Analysis.Expressions;
using Quixotic.Common.Statements;

namespace Quixotic.Common.Analysis.Statements
{
    public class MethodCallStatementInfo(QxStatement statement) : StatementInfo(statement)
    {
        public required ExpressionInfo Call { get; init; }
    }
}
