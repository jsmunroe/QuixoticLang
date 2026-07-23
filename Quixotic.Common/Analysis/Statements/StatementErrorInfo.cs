using Quixotic.Common.Statements;

namespace Quixotic.Common.Analysis.Statements
{
    public class StatementErrorInfo(Exception exception, QxStatement statement) : StatementInfo(statement)
    {
        public Exception Exception { get; init; } = exception;
    }
}
