using Quixotic.Analysis.Semantics;
using Quixotic.Common.Symbols;
using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class UnrecognizedMethodSignatureException(object targetType, Signature name, Span span) : SemanticException($"Target '{targetType}' does not have a function signature '{name}' defined.", span, Severity.Error);
}
