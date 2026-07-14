using Quixotic.Common.Contracts;
using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Interpret
{
    public class ExecutionException(string message, Exception innerException) : InterpreterException(message, innerException, GetSpan(innerException))
    {
        private static Span GetSpan(Exception exception)
        {
            return exception is IHasSpan hasSpan ? hasSpan.Span : Span.Empty;
        }
    }
}
