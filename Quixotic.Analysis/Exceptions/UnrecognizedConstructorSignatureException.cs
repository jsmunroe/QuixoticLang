using Quixotic.Analysis.Semantics;
using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class UnrecognizedConstructorSignatureException(object targetType, Span span) : SemanticException($"Target '{targetType}' does not have a constructor that matches the given arguments.", span, Severity.Error);
}
