using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class ConstructorOutsideOfTypeException(Span span) : SemanticException("A constructor cannot be defined outside of a type definition.", span, Semantics.Severity.Error);
}
