using Quixotic.Common.Analysis.Expressions;

namespace Quixotic.Common.Analysis.Statements
{
    public class FunctionCallStatementInfo : StatementInfo
    {
        public required ExpressionInfo Call { get; init; }
    }
}
