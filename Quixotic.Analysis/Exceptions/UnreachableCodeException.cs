using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class UnreachableCodeException(Span span) : SemanticException($"Unreachable code.", span);
}
