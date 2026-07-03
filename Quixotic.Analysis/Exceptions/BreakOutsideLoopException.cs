using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class BreakOutsideLoopException(Span span) : SemanticException($"Break statement is not allowed outside of a loop block.", span);
}
