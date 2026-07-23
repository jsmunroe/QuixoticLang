using Quixotic.Common.Analysis.Expressions;

namespace Quixotic.Common.Analysis.Statements
{
    public class MethodCallStatementInfo : StatementInfo
    {
        public required ExpressionInfo Call { get; init; }
    }
}
