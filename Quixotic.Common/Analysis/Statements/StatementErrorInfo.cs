namespace Quixotic.Common.Analysis.Statements
{
    public class StatementErrorInfo(Exception exception) : StatementInfo
    {
        public Exception Exception { get; init; } = exception;
    }
}
