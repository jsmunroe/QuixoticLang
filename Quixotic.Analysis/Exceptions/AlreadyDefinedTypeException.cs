using Quixotic.Analysis.Semantics;
using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class AlreadyDefinedTypeException(object typeName, Span span) : SemanticException($"A type named '{typeName}' has already been defined.", span, Severity.Error);
}
