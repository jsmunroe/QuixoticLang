using Quixotic.Common.Tokens;

namespace Quixotic.Common.Exceptions.Interpret
{
    public class UndefinedIdentifierException(string name, Span span) : InterpreterException($"No identifier named '{name}' has been defined.", span);
}
