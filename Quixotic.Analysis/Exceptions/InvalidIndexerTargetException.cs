using Quixotic.Analysis.Semantics;
using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class InvalidIndexerTargetException(object targetType, Span span) : SemanticException($"Type '{targetType}' does not support indexing.", span, Severity.Error);
}
