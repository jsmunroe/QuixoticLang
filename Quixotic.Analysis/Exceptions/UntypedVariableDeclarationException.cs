using Quixotic.Analysis.Semantics;
using Quixotic.Common.Tokens;

namespace Quixotic.Analysis.Exceptions
{
    public class UntypedVariableDeclarationException(string name, Span span) : SemanticException($"The declaration of the variable '{name}' is missing a type specification either explicitly or by assigning a value with a type.", span, Severity.Error);
}
