using Quixotic.Analysis.Semantics;
using Quixotic.Common.Symbols;
using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class UnrecognizedFunctionSignatureException(Signature name, Span span) : SemanticException($"Function signature '{name}' has not been defined.", span, Severity.Error);
}
