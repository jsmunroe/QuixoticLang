using Quixotic.Analysis.Semantics;
using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class UnrecognizedTypeException(string typeName, Span span) : SemanticException($"The type '{typeName}' is unrecognized and cannot be meaningfully resolved.", span, Severity.Error);
}
