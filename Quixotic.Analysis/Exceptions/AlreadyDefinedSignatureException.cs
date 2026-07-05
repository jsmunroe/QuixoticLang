using Quixotic.Analysis.Semantics;
using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class AlreadyDefinedSignatureException(object name, Span span) : SemanticException($"Function '{name}' has already been defined.", span, Severity.Error);
}
