using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class ContinueOutsideLoopException(Span span) : SemanticException($"Continue statement is not allowed outside of a loop block.", span);
}
