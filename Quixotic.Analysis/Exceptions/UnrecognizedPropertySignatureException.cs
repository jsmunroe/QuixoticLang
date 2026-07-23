using Quixotic.Analysis.Semantics;
using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class UnrecognizedPropertySignatureException(object targetType, string name, Span span) : SemanticException($"Target '{targetType}' does not have a property '{name}' defined.", span, Severity.Error);
}
