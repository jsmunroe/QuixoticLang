using Quixotic.Analysis.Semantics;
using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class TypeNameCasingException(string name, Span span) : SemanticException($"Name '{name}' does not match casing rules for type names.", span, Severity.Warning);
}
