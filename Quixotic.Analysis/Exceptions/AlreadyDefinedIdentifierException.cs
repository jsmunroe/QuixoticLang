using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class AlreadyDefinedIdentifierException(string name, Span span) : SemanticException($"The identifier '{name}' has already been defined.", span);
}
