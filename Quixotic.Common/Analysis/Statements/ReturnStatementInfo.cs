using Quixotic.Common.Analysis.Expressions;

namespace Quixotic.Common.Analysis.Statements
{
    public class ReturnStatementInfo : StatementInfo
    {
        public required ExpressionInfo ReturnValue { get; init; }
    }
}
