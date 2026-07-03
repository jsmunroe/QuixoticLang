using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class UnrecognizedIdentifierException(string name, Span span) : SemanticException($"The identifier '{name}' has not been defined.", span);
}
