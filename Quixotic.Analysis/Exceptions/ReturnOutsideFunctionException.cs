using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class ReturnOutsideFunctionException(Span span) : SemanticException($"Return statement is not allowed outside of a function block.", span);
}
