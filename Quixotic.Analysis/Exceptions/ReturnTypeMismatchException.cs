using Quixotic.Analysis.Semantics;
using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class ReturnTypeMismatchException(string functionName, object returnType, Span span) : SemanticException($"The function '{functionName}' does not return '{returnType}'.", span, Severity.Error);
}
