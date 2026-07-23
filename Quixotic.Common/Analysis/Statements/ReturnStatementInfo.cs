using Quixotic.Common.Analysis.Expressions;
using Quixotic.Common.Statements;

namespace Quixotic.Common.Analysis.Statements
{
    public class ReturnStatementInfo(QxStatement statement) : StatementInfo(statement)
    {
        public required ExpressionInfo ReturnValue { get; init; }
    }
}
