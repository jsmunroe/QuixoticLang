using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class BaseConstructorOnTypeWithoutBaseTypeException(object type, Span span) : SemanticException($"The type '{type}' does not have a base type. The base constructor cannot be invoked.", span, Semantics.Severity.Error);
}
