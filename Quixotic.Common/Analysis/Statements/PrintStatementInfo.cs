using Quixotic.Common.Analysis.Expressions;

namespace Quixotic.Common.Analysis.Statements
{
    public class PrintStatementInfo : StatementInfo
    {
        public required ExpressionInfo Expression { get; init; }
    }
}
