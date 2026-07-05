using Quixotic.Analysis.Semantics;
using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class IndexTypeException(object type, Span span) : SemanticException($"Type '{type}' is not valid for indexing and array.", span, Severity.Error);
}
