using Quixotic.Analysis.Semantics;
using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class ForInLoopCollectionTypeException(object collectionType, Span span) : SemanticException($"Type '{collectionType}' is not a collection and cannot be iterated.", span, Severity.Error);
}
