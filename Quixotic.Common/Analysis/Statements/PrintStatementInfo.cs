using Quixotic.Common.Analysis.Expressions;
using Quixotic.Common.Statements;

namespace Quixotic.Common.Analysis.Statements
{
    public class PrintStatementInfo(QxStatement statement) : StatementInfo(statement)
    {
        public required ExpressionInfo Expression { get; init; }
    }
}
